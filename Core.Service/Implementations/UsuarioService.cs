using System;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Interfaces.Repository;
using Core.Domain.Models;
using Core.Domain.ViewModels;
using Core.Domain.Interfaces.Services;
using System.Security.Cryptography;
using System.Linq;
using Core.Domain.Enums;
using Core.Service.Exceptions;
using System.Collections.Generic;

namespace Core.Service.Implementations;
public class UsuarioService : IUsuarioService
{
    private readonly IAmazonService _amazonService;
    private readonly IMapper _mapper;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IBaseRepository<UsuarioPermissao> _usuarioPermissaoRepository;
    private readonly IBaseRepository<Permissao> _permissaoRepository;
    private readonly IBaseRepository<Motorista> _motoristaRepository;
    private readonly IUserContext _userContext;
    private readonly ITokenService _tokenService;
    public UsuarioService(
        IUsuarioRepository repo,
        IUserContext userContext,
        ITokenService ServiceToken,
        IAmazonService amazonService,
        IBaseRepository<UsuarioPermissao> usuarioPermissaoRepository,
        IBaseRepository<Permissao> permissaoRepository,
        IBaseRepository<Motorista> motoristaRepository,
        IMapper map)
    {
        _amazonService = amazonService;
        _motoristaRepository = motoristaRepository;
        _permissaoRepository = permissaoRepository;
        _usuarioPermissaoRepository = usuarioPermissaoRepository;
        _userContext = userContext;
        _mapper = map;
        _usuarioRepository = repo;
        _tokenService = ServiceToken;
    }

    public async Task<UsuarioViewModel> Registrar(UsuarioNovoViewModel user)
    {
        var model = _mapper.Map<Usuario>(user);
        user.Senha = _tokenService.Base64ToString(user.Senha);

        var usuario = await _usuarioRepository.BuscarUmAsync(x => x.CPF == user.CPF && x.EmpresaId == user.EmpresaId);
        if (usuario != null && usuario.Id > 0)
            throw new BusinessRuleException("Usuário já cadastrado!!");

        var permissaoPadroes = await _permissaoRepository.BuscarAsync(x =>
            x.EmpresaId == user.EmpresaId &&
            user.IsMotorista ? x.PadraoMotorista : x.PadraoResponsavel);

        if (user.IsMotorista)
        {
            model.Perfil = PerfilEnum.Motorista;
        }

        await _usuarioPermissaoRepository.AdicionarAsync(
            permissaoPadroes.Select(x => new UsuarioPermissao
            {
                UsuarioId = usuario.Id,
                PermissaoId = x.Id
            })
        );

        model.Status = StatusEntityEnum.Ativo;
        model.Senha = _usuarioRepository.ComputeHash(user.Senha, new SHA256CryptoServiceProvider());
        model.UsuarioValidado = false;

        await _usuarioRepository.AdicionarAsync(model);
        if (user.IsMotorista)
        {
            var motorista = new Motorista
            {
                UsuarioId = model.Id,
                Vencimento = DateTime.MaxValue,
                TipoCNH = TipoCNHEnum.Nenhum,
                CNH = string.Empty
            };
            await _motoristaRepository.AdicionarAsync(motorista);
        }

        await EnviarEmailConfirmacao(model);

        return _mapper.Map<UsuarioViewModel>(model);
    }

    public async Task Atualizar(UsuarioAtualizarViewModel user)
    {
        var model = await _usuarioRepository.BuscarUmAsync(x => x.Id == user.Id, x => x.Alunos);

        model.CPF = user.CPF;
        model.Email = user.Email;
        model.PrimeiroNome = user.PrimeiroNome;
        model.UltimoNome = user.UltimoNome;
        model.PlanoId = user.PlanoId;
        model.Contato = user.Contato;
        model.EnderecoPrincipalId = user.EnderecoPrincipalId;

        await _usuarioRepository.AtualizarAsync(model);
    }

    public async Task<UsuarioViewModel> ObterPorId(int userId)
    {
        var model = await _usuarioRepository.BuscarAsync(x => x.Id == userId, x => x.Alunos);
        return _mapper.Map<UsuarioViewModel>(model);
    }

    public async Task<UsuarioViewModel> ObterDadosDoUsuario()
    {
        var user = await _usuarioRepository.ObterPorIdAsync(_userContext.UserId);
        return _mapper.Map<UsuarioViewModel>(user);
    }

    public async Task VincularPermissao(PermissaoViewModel user)
    {
        var usuario = await _usuarioRepository.BuscarUmAsync(x => x.Id == user.UsuarioId);
        if (usuario is null)
            throw new Exception("Usuário já existente!");

        var permissoesDoUsuario = await _usuarioPermissaoRepository
            .BuscarAsync(x => x.UsuarioId == user.UsuarioId && user.PermissaoId.Contains(x.Id));

        System.Linq.Expressions.Expression<Func<Permissao, bool>> predicate =
            x => x.EmpresaId == _userContext.Empresa &&
                usuario.Perfil == PerfilEnum.Motorista ? x.PadraoMotorista :
                usuario.Perfil == PerfilEnum.Passageiro ? x.PadraoPassageiros :
                usuario.Perfil == PerfilEnum.Responsavel ? x.PadraoResponsavel :
                usuario.Perfil == PerfilEnum.Suporte ? x.PadraoSuporte : false;

        // Logica para pegar as permissoes padrão pra o perfil do usuário
        var permissoesPadroes = await _permissaoRepository.BuscarAsync(predicate);
        var permissoesPadraoQueNaoEstaoNaRequest = permissoesPadroes.Where(x => !user.PermissaoId.Contains(x.Id)).Select(x => x.Id);

        // Remove permissoes
        permissoesDoUsuario.ToList().ForEach(item => _usuarioPermissaoRepository.RemoverAsync(item).GetAwaiter().GetResult());

        // Logica para adicionar as permissoes novamente
        var usuarioPermissao = user.PermissaoId.Concat(permissoesPadraoQueNaoEstaoNaRequest).Select(x => new UsuarioPermissao
        {
            PermissaoId = x,
            UsuarioId = user.UsuarioId
        });

        await _usuarioPermissaoRepository.AdicionarAsync(usuarioPermissao);
    }


    private async Task EnviarEmailConfirmacao(Usuario model)
    {
        var now = DateTime.Now;
        var linkDeConfirmacao = "https://www.cadeavan.com.br/confirmacao.html?token=" + model.Id;
        var titulo = "Confirmação de Cadastro";
        var mensagem = $@"
            <!DOCTYPE html>
            <html lang='pt-br'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Confirmação de Cadastro</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        color: #333;
                        padding: 20px;
                    }}
                    .container {{
                        background-color: #fff;
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                    }}
                    h1 {{
                        color: #007bff;
                    }}
                    p {{
                        font-size: 16px;
                    }}
                    .button {{
                        background-color: #007bff;
                        color: white;
                        padding: 10px 20px;
                        text-decoration: none;
                        border-radius: 5px;
                        font-size: 16px;
                        display: inline-block;
                        margin-top: 20px;
                    }}
                    .footer {{
                        text-align: center;
                        font-size: 14px;
                        color: #777;
                        margin-top: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Bem-vindo ao Cadê a Van!</h1>
                    <p>Olá, {model.ObterNomeInteiro()}!</p>
                    <p>Obrigado por se cadastrar no Cadê a Van. Para concluir seu cadastro, por favor, confirme seu endereço de e-mail clicando no botão abaixo:</p>
                    
                    <p><a href='{linkDeConfirmacao}' class='button'>Confirmar E-mail</a></p>
                    
                    <p>Se você não se cadastrou no nosso site, por favor, ignore este e-mail.</p>
                    
                    <div class='footer'>
                        <p>&copy; {now.Year} Cadê a Van. Todos os direitos reservados.</p>
                    </div>
                </div>
            </body>
            </html>";

        await _amazonService.SendEmail(model.Email, titulo, mensagem);
    }

    public async Task ConfirmarCadastroAsync(int userId)
    {
        var usuario = await _usuarioRepository.BuscarUmAsync(x => x.Id == userId);
        if (usuario.UsuarioValidado == false)
        {
            usuario.UsuarioValidado = true;
            await _usuarioRepository.AtualizarAsync(usuario);
        }
    }
}