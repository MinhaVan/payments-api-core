using Payments.Domain.Interfaces.Repository;
using Payments.Domain.ViewModels.Assinatura;
using Payments.Domain.Interfaces.Services;
using Payments.Domain.Interfaces.APIs;
using Payments.Application.Exceptions;
using Payments.Domain.ViewModels;
using Payments.Domain.ApiModel;
using Payments.Domain.Models;
using Payments.Domain.Utils;
using Payments.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using AutoMapper;
using System;

namespace Payments.Application.Implementations;

public class AssinaturaService : IAssinaturaService
{
    private readonly ILogger<AssinaturaService> _logger;
    private readonly IBaseRepository<Plano> _planoRepository;
    private readonly IAuthApi _authApi;
    private readonly IBaseRepository<Assinatura> _assinaturaRepository;
    private readonly IApiAsaas _apiAsaas;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public AssinaturaService(
        IMapper mapper,
        ILogger<AssinaturaService> logger,
        IUserContext userContext,
        IBaseRepository<Assinatura> assinaturaRepository,
        IAuthApi authApi,
        IBaseRepository<Plano> planoRepository,
        IApiAsaas apiAsaas)
    {
        _userContext = userContext;
        _mapper = mapper;
        _apiAsaas = apiAsaas;
        _logger = logger;
        _authApi = authApi;
        _assinaturaRepository = assinaturaRepository;
        _planoRepository = planoRepository;
    }

    public async Task AtualizarFormaPagamentoAsync(AtualizarFormaPagamento requisicao)
    {
        var usuarioId = _userContext.UserId;
        var empresaId = _userContext.Empresa;

        _logger.LogInformation($"[{usuarioId}] Obtendo assinatura.");
        var assinaturaDb = await _assinaturaRepository.BuscarUmAsync(
            x => x.UsuarioId == usuarioId && x.Status == StatusEntityEnum.Ativo,
            x => x.Pagamentos);

        _logger.LogInformation($"[{usuarioId}] Validando se existe algum pagamento pendente.");
        if (assinaturaDb != null)
        {
            switch (assinaturaDb.TipoPagamento)
            {
                case TipoPagamentoEnum.Boleto:
                case TipoPagamentoEnum.Pix:
                    if (assinaturaDb.Pagamentos.Any(x => x.StatusPagamento != (int)PagamentoStatusEnum.CONFIRMED && x.StatusPagamento != (int)PagamentoStatusEnum.RECEIVED))
                    {
                        throw new BusinessRuleException("Existem boletos pendentes de pagamento. Por favor, realiza o pagamento deles primeiro!");
                    }
                    break;
                case TipoPagamentoEnum.Credito:
                    break;
                default:
                    throw new BusinessRuleException("Tipo de pagamento inválido!");
            }
        }

        switch (requisicao.NovoTipoPagamento)
        {
            case TipoPagamentoEnum.Pix:
                await AssinaturaBoletoPixAsync(requisicao.Pix);
                break;

            case TipoPagamentoEnum.Boleto:
                await AssinaturaBoletoPixAsync(requisicao.Boleto);
                break;

            case TipoPagamentoEnum.Credito:
                await AssinaturaCreditoAsync(requisicao.Credito);
                break;
            default:
                throw new BusinessRuleException("Tipo de pagamento selecionado inválido!");
        }
    }

    public async Task<AssinaturaViewModel> ObterHistoricoAsync(int AlunoId)
    {
        var usuarioId = _userContext.UserId;
        var empresaId = _userContext.Empresa;

        var usuarioResponse = await _authApi.ObterUsuarioAsync(usuarioId);
        if (usuarioResponse == null || usuarioResponse.Data == null)
        {
            throw new BusinessRuleException(usuarioResponse.Mensagem);
        }

        var usuario = usuarioResponse.Data;
        var assinatura = await _assinaturaRepository.BuscarUmAsync(
            x => x.UsuarioId == usuarioId && usuario.EmpresaId == empresaId && x.AlunoId == AlunoId && x.Status == StatusEntityEnum.Ativo,
            x => x.Pagamentos, x => x.Plano);

        var response = _mapper.Map<AssinaturaViewModel>(assinatura);
        return response;
    }

    public async Task AssinaturaBoletoPixAsync(PagamentoViewModel model)
    {
        model.UsuarioId = _userContext.UserId;
        model.Vencimento = model.Vencimento.HasValue ? model.Vencimento : DateTime.UtcNow;
        _logger.LogInformation($"[{model.UsuarioId}] Iniciando lógica para criar um pagamento no Asaas - Payload: {JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");

        _logger.LogInformation($"[{model.UsuarioId}] Obtendo usuário.");
        var usuario = await ValidarUsuarioAsync(model.UsuarioId);

        _logger.LogInformation($"[{model.UsuarioId}] Validando plano.");
        var plano = await ValidarPlanoAsync(model.PlanoId, model.Valor);

        _logger.LogInformation($"[{model.UsuarioId}] Obtendo dados do cliente no Asaas.");
        var clienteId = await ObterOuCriarClienteAsync(usuario);

        _logger.LogInformation($"[{model.UsuarioId}] Removendo assinaturas anteriores.");
        await RemoverAssinaturasAtivasExistentesAsync(clienteId);

        _logger.LogInformation($"[{model.UsuarioId}] Desativando assinaturas no banco de dados.");
        await DesativarAssinaturasLocaisAsync(model.UsuarioId);

        _logger.LogInformation($"[{model.UsuarioId}] Criando nova assinatura.");
        var assinatura = await CriarNovaAssinaturaAsync(plano.Id, model.UsuarioId, model.Valor, model.AlunoId, model.TipoPagamento, vencimento: model.Vencimento);

        _logger.LogInformation($"[{model.UsuarioId}] Criando novo cobrança e gerando QRCode/Linha digitável no Asaas.");
        var pagamentoResponse = await GerarPagamentoAsync(model, assinatura, plano, clienteId);

        _logger.LogInformation($"[{model.UsuarioId}] Atualizando assinatura localmente.");
        assinatura.IdExterno = pagamentoResponse.Id;
        assinatura.CopiaCola = pagamentoResponse.CopiaCola;
        assinatura.Imagem = pagamentoResponse.Imagem;
        await _assinaturaRepository.AtualizarAsync(assinatura);

        _logger.LogInformation($"[{model.UsuarioId}] Atualizando plano do usuário.");
        await AtualizarPlanoUsuarioAsync(usuario, plano.Id);

        _logger.LogInformation($"[{model.UsuarioId}] Pagamento criado com sucesso - Assinatura: {JsonConvert.SerializeObject(assinatura, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
    }

    public async Task AssinaturaCreditoAsync(CreditoViewModel model)
    {
        model.UsuarioId = _userContext.UserId;
        model.Vencimento = model.Vencimento.HasValue ? model.Vencimento : DateTime.UtcNow;
        _logger.LogInformation($"[{model.UsuarioId}] Criando assinatura - Payload: {JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");

        _logger.LogInformation($"[{model.UsuarioId}] Obtendo usuário.");
        var usuario = await ValidarUsuarioAsync(model.UsuarioId);

        _logger.LogInformation($"[{model.UsuarioId}] Validando plano.");
        var plano = await ValidarPlanoAsync(model.PlanoId, model.Valor);

        _logger.LogInformation($"[{model.UsuarioId}] Obtendo dados do cliente no Asaas.");
        var clienteId = await ObterOuCriarClienteAsync(usuario);

        _logger.LogInformation($"[{model.UsuarioId}] Removendo assinaturas anteriores.");
        await RemoverAssinaturasAtivasExistentesAsync(clienteId);

        _logger.LogInformation($"[{model.UsuarioId}] Desativando assinaturas no banco de dados.");
        await DesativarAssinaturasLocaisAsync(model.UsuarioId);

        _logger.LogInformation($"[{model.UsuarioId}] Criando nova assinatura recorrente no cartão.");
        var assinatura = await CriarNovaAssinaturaAsync(
            plano.Id, model.UsuarioId, model.Valor, model.AlunoId,
            model.TipoPagamento, numeroCartao: model.CartaoCredito.Numero);

        var novaAssinaturaResponse = await _apiAsaas.CriarAssinaturaAsync(
            _mapper.Map<AssinaturaRequest>(model), assinatura.Id, clienteId, plano.Nome);

        _logger.LogInformation($"[{model.UsuarioId}] Atualizando assinatura localmente.");
        assinatura.IdExterno = novaAssinaturaResponse.Id;
        await _assinaturaRepository.AtualizarAsync(assinatura);

        _logger.LogInformation($"[{model.UsuarioId}] Atualizando plano do usuário.");
        await AtualizarPlanoUsuarioAsync(usuario, plano.Id);

        _logger.LogInformation($"[{model.UsuarioId}] Assinatura criada com sucesso - Assinatura: {JsonConvert.SerializeObject(assinatura, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
    }

    #region Private Methos
    private async Task<UsuarioViewModel> ValidarUsuarioAsync(int usuarioId)
    {
        var usuario = await _authApi.ObterUsuarioAsync(usuarioId);
        if (usuario is null || !usuario.Sucesso)
        {
            _logger.LogError($"[{usuarioId}] Usuário não existe ou está inativo! - {usuario.Mensagem}");
            throw new ArgumentException("Usuário não existe ou está inativo!");
        }
        return usuario.Data;
    }

    private async Task<Plano> ValidarPlanoAsync(int planoId, decimal valor)
    {
        var plano = await _planoRepository.BuscarUmAsync(x => x.Id == planoId && x.Status == StatusEntityEnum.Ativo);
        if (plano is null)
        {
            _logger.LogError($"[{planoId}] Plano não existe ou está inativo!");
            throw new ArgumentException("Plano não existe ou está inativo!");
        }

        if (plano.Valor != valor)
        {
            _logger.LogError($"[{planoId}] Valor enviado é diferente do plano!");
            throw new ArgumentException("Valor enviado é diferente do plano!");
        }
        return plano;
    }

    private async Task<string> ObterOuCriarClienteAsync(UsuarioViewModel usuario)
    {
        if (string.IsNullOrEmpty(usuario.ClientIdPaymentGateway))
        {
            ClienteResponse clienteResponse;
            var clienteResponsePeloCPF = await _apiAsaas.ObterClienteAsync(cpfCnpj: usuario.CPF);
            if (clienteResponsePeloCPF is not null && clienteResponsePeloCPF.Data.Any())
            {
                clienteResponse = clienteResponsePeloCPF.Data.First();
            }
            else
            {
                clienteResponse = await _apiAsaas.CriarClienteAsync(
                    new ClienteRequest
                    {
                        Nome = $"{usuario.PrimeiroNome} {usuario.UltimoNome}",
                        CpfCnpj = usuario.CPF,
                        Email = usuario.Email,
                        MobilePhone = usuario.Contato.ApenasNumeros()
                    }
                );
            }

            usuario.ClientIdPaymentGateway = clienteResponse.Id;
            var requestUsuarioAtualizar = _mapper.Map<UsuarioAtualizarViewModel>(usuario);
            await _authApi.AtualizarAsync(requestUsuarioAtualizar);
        }
        return usuario.ClientIdPaymentGateway;
    }

    private async Task RemoverAssinaturasAtivasExistentesAsync(string clienteId)
    {
        var response = await _apiAsaas.ListarAssinaturasAsync(clienteId);
        var assinaturasAtivas = response.Data.Where(x => x.Status == "ACTIVE");
        foreach (var assinatura in assinaturasAtivas)
        {
            await _apiAsaas.ExcluirAssinaturaAsync(assinatura.Id);
        }
    }

    private async Task DesativarAssinaturasLocaisAsync(int usuarioId)
    {
        var assinaturas = await _assinaturaRepository.BuscarAsync(x =>
            x.UsuarioId == usuarioId && x.Status == StatusEntityEnum.Ativo);

        foreach (var item in assinaturas)
        {
            item.Status = StatusEntityEnum.Deletado;
            await _assinaturaRepository.AtualizarAsync(item);
        }
    }

    private async Task<PagamentoResponse> GerarPagamentoAsync(PagamentoViewModel model, Assinatura assinatura, Plano plano, string clienteId)
    {
        var pagamento = new PagamentoRequest
        {
            DiasAposVencimentoParaCancelamento = 2,
            ExternalIdCliente = clienteId,
            ExternalReference = model.UsuarioId.ToString(),
            FormaPagamento = model.TipoPagamento,
            Valor = model.Valor,
            Vencimento = assinatura.Vencimento
        };

        var response = await _apiAsaas.PagamentoAsync(pagamento, assinatura.Id, clienteId, plano.Nome);
        var retorno = new PagamentoResponse
        {
            TipoPagamento = model.TipoPagamento,
            Id = response.Id
        };

        if (model.TipoPagamento == TipoPagamentoEnum.Boleto)
        {
            var boleto = await _apiAsaas.ObterLinhaDigitavelBoleto(response.Id);
            retorno.CopiaCola = boleto.IdentificationField;
            retorno.Imagem = boleto.BarCode;
        }
        else
        {
            var pix = await _apiAsaas.ObterQRCodePix(response.Id);
            retorno.CopiaCola = pix.Payload;
            retorno.Imagem = pix.EncodedImage; ;
        }

        return retorno;
    }

    private async Task<Assinatura> CriarNovaAssinaturaAsync(int planoId, int usuarioId, decimal valor, int AlunoId, TipoPagamentoEnum tipoPagamento, DateTime? vencimento = null, string? numeroCartao = null)
    {
        if (vencimento is null)
            vencimento = DateTime.UtcNow;

        var assinatura = new Assinatura
        {
            PlanoId = planoId,
            Status = StatusEntityEnum.Ativo,
            UsuarioId = usuarioId,
            AlunoId = AlunoId,
            Valor = valor,
            TipoPagamento = tipoPagamento,
            Vencimento = vencimento.Value,
            NumeroCartao = string.IsNullOrEmpty(numeroCartao) ? string.Empty : numeroCartao.Substring(numeroCartao.Length - 4)
        };
        await _assinaturaRepository.AdicionarAsync(assinatura);
        return assinatura;
    }

    private async Task AtualizarPlanoUsuarioAsync(UsuarioViewModel usuario, int planoId)
    {
        var requestUsuarioAtualizar = _mapper.Map<UsuarioAtualizarViewModel>(usuario);
        requestUsuarioAtualizar.PlanoId = planoId;

        await _authApi.AtualizarAsync(requestUsuarioAtualizar);
    }
    #endregion
}
