using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Payments.Domain.ApiModel;
using Payments.Domain.Interfaces.APIs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Payments.Data.APIs;

public class ApiAsaas : IApiAsaas
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiAsaas> _logger;

    public ApiAsaas(IHttpClientFactory httpClientFactory, ILogger<ApiAsaas> logger)
    {
        _httpClient = httpClientFactory.CreateClient("api-asaas");
        _logger = logger;
    }

    public async Task<PixResponse> ObterQRCodePix(string cobrancaId)
    {
        _logger.LogInformation($"Enviando requisição para gerar obter QRCode do PIX no Asaas - Dados: {cobrancaId}");
        var response = await _httpClient.GetAsync($"/api/v3/payments/{cobrancaId}/pixQrCode");

        if (response.IsSuccessStatusCode)
        {
            var pixResponsd = await response.Content.ReadFromJsonAsync<PixResponse>();
            _logger.LogInformation($"Resposta da requisição para obter QRCode do PIX no Asaas - Dados: {JsonConvert.SerializeObject(pixResponsd, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            return pixResponsd;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao obter QRCode do PIX no Asaas - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao obter QRCode do PIX!");
        }
    }

    public async Task<BoletoResponse> ObterLinhaDigitavelBoleto(string cobrancaId)
    {
        _logger.LogInformation($"Enviando requisição para gerar obter linha digitável do BOLETO no Asaas - Dados: {cobrancaId}");
        var response = await _httpClient.GetAsync($"/api/v3/payments/{cobrancaId}/identificationField");

        if (response.IsSuccessStatusCode)
        {
            var boletoResponse = await response.Content.ReadFromJsonAsync<BoletoResponse>();
            _logger.LogInformation($"Resposta da requisição para obter linha digitável do BOLETO no Asaas - Dados: {JsonConvert.SerializeObject(boletoResponse, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            return boletoResponse;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao obter linha digitável do BOLETO no Asaas - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao obter linha digitável do boleto!");
        }
    }

    /// <summary>
    /// Pagamento via PIX ou Boleto
    /// </summary>
    /// <param name="pagamento">Modelo recebido do front com as informações</param>
    /// <param name="assinaturaId">ID da assinatura no banco de dados</param>
    /// <param name="clienteId">ID do cliente no Asaas</param>
    /// <param name="nomePlano">Nome do plano no banco de dados</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<AssinaturaResponse> PagamentoAsync(PagamentoRequest pagamento, int assinaturaId, string clienteId, string nomePlano)
    {
        var request = new
        {
            customer = clienteId,
            billingType = pagamento.FormaPagamento == Domain.Enums.TipoPagamentoEnum.Boleto ? "BOLETO" : "PIX",
            value = pagamento.Valor,
            dueDate = pagamento.Vencimento.ToString("yyyy-MM-dd"),
            description = $"Pagamento da assinatura {assinaturaId} para o cliente {clienteId} no plano {nomePlano}.",
            daysAfterDueDateToRegistrationCancellation = pagamento.DiasAposVencimentoParaCancelamento,
            externalReference = assinaturaId,
        };

        _logger.LogInformation($"Enviando requisição para gerar BOLETO/PIX no Asaas - Dados: {JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
        var response = await _httpClient.PostAsJsonAsync("/api/v3/payments", request);

        if (response.IsSuccessStatusCode)
        {
            var novaAssinaturaResponse = await response.Content.ReadFromJsonAsync<AssinaturaResponse>();
            _logger.LogInformation($"Resposta da requisição para gerar BOLETO/PIX no Asaas - Dados: {JsonConvert.SerializeObject(novaAssinaturaResponse, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            return novaAssinaturaResponse;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao gerar BOLETO/PIX no Asaas - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao realizar a assinatura!");
        }
    }

    public async Task<bool> ExcluirAssinaturaAsync(string assinaturaId)
    {
        var response = await _httpClient.DeleteAsync($"/api/v3/subscriptions/{assinaturaId}");
        if (!response.IsSuccessStatusCode)
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            throw new Exception("Erro ao excluir a cobrança: " + mensagemErro);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<PaginadaResponse<AssinaturaResponse>> ListarAssinaturasAsync(string externalId)
    {
        var response = await _httpClient.GetAsync($"/api/v3/subscriptions?customer={externalId}");
        if (!response.IsSuccessStatusCode)
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            throw new Exception("Erro ao listar assinaturas: " + mensagemErro);
        }

        return await response.Content.ReadFromJsonAsync<PaginadaResponse<AssinaturaResponse>>();
    }

    public async Task<PaginadaResponse<ClienteResponse>> ObterClienteAsync(
        int? offset = null,
        int? limit = null,
        string nome = null,
        string email = null,
        string cpfCnpj = null,
        string nomeGrupo = null,
        string referenciaExterna = null)
    {
        var parametrosConsulta = new List<string>();

        if (offset.HasValue) parametrosConsulta.Add($"offset={offset.Value}");
        if (limit.HasValue) parametrosConsulta.Add($"limit={limit.Value}");
        if (!string.IsNullOrEmpty(nome)) parametrosConsulta.Add($"name={Uri.EscapeDataString(nome)}");
        if (!string.IsNullOrEmpty(email)) parametrosConsulta.Add($"email={Uri.EscapeDataString(email)}");
        if (!string.IsNullOrEmpty(cpfCnpj)) parametrosConsulta.Add($"cpfCnpj={cpfCnpj}");
        if (!string.IsNullOrEmpty(nomeGrupo)) parametrosConsulta.Add($"groupName={Uri.EscapeDataString(nomeGrupo)}");
        if (!string.IsNullOrEmpty(referenciaExterna)) parametrosConsulta.Add($"externalReference={Uri.EscapeDataString(referenciaExterna)}");

        var url = "v3/customers";
        if (parametrosConsulta.Count > 0)
        {
            url += "?" + string.Join("&", parametrosConsulta);
        }

        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var respostaJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PaginadaResponse<ClienteResponse>>(respostaJson);
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao obter clientes: {mensagemErro}");
        }
    }

    public async Task<ClienteResponse> NovoClienteAsync(ClienteRequest clienteRequest)
    {
        _logger.LogInformation($"Adicionando novo cliente com CPF {clienteRequest.CpfCnpj} - Dados: {JsonConvert.SerializeObject(clienteRequest, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
        var response = await _httpClient.PostAsJsonAsync("/api/v3/customers", clienteRequest);
        _logger.LogInformation($"Resposta da criação do novo cliente: {JsonConvert.SerializeObject(response, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ClienteResponse>(responseBody);
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao criar cliente {clienteRequest.Nome} - Mensagem: {mensagemErro}");
            throw new Exception($"Erro ao criar cliente {clienteRequest.Nome}");
        }
    }

    public async Task<AssinaturaResponse> CriarAssinaturaAsync(AssinaturaRequest assinatura, int assinaturaId, string clienteId, string nomePlano)
    {
        var request = new
        {
            customer = clienteId,
            externalReference = assinaturaId.ToString(),
            billingType = "CREDIT_CARD",
            nextDueDate = assinatura.Vencimento.ToString("yyyy-MM-dd"),
            value = assinatura.Valor,
            cycle = "MONTHLY",
            description = $"Assinatura Plano '{nomePlano}'",
            creditCard = new
            {
                holderName = assinatura.CartaoCredito.NomeTitular,
                number = assinatura.CartaoCredito.Numero,
                expiryMonth = assinatura.CartaoCredito.MesVencimento,
                expiryYear = assinatura.CartaoCredito.AnoVencimento,
                ccv = assinatura.CartaoCredito.Cvv
            },
            creditCardHolderInfo = new
            {
                name = assinatura.CartaoCredito.NomeTitular, // assinatura.InformacoesTitularCartao.Nome,
                email = assinatura.InformacoesTitularCartao.Email,
                cpfCnpj = assinatura.InformacoesTitularCartao.CpfCnpj,
                postalCode = assinatura.InformacoesTitularCartao.Cep,
                addressNumber = assinatura.InformacoesTitularCartao.NumeroEndereco,
                addressComplement = assinatura.InformacoesTitularCartao.Complemento,
                phone = assinatura.InformacoesTitularCartao.Celular, //assinatura.InformacoesTitularCartao.Telefone,
                mobilePhone = assinatura.InformacoesTitularCartao.Celular
            }
        };

        _logger.LogInformation($"Enviando requisição para nova assinatura no Asaas - Dados: {JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
        var response = await _httpClient.PostAsJsonAsync("/api/v3/subscriptions", request);

        if (response.IsSuccessStatusCode)
        {
            var novaAssinaturaResponse = await response.Content.ReadFromJsonAsync<AssinaturaResponse>();
            _logger.LogInformation($"Resposta da assinatura no Asaas - Dados: {JsonConvert.SerializeObject(novaAssinaturaResponse, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            return novaAssinaturaResponse;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao realizar assinatura no Asaas - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao realizar a assinatura!");
        }
    }

    public async Task<ClienteResponse> CriarClienteAsync(ClienteRequest clienteRequest)
    {
        _logger.LogInformation($"Adicionando novo cliente com CPF {clienteRequest.CpfCnpj} - Data: {JsonConvert.SerializeObject(clienteRequest, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
        var response = await _httpClient.PostAsJsonAsync("/api/v3/customers", clienteRequest);
        _logger.LogInformation($"Resposta do novo cliente {JsonConvert.SerializeObject(response, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var customerResponse = JsonConvert.DeserializeObject<ClienteResponse>(responseBody);
            return customerResponse;
        }
        else
        {
            _logger.LogError($"Erro ao criar cliente {clienteRequest.Nome} - Mensagem: {await response.Content.ReadAsStringAsync()}");
            throw new Exception($"Erro ao criar cliente {clienteRequest.Nome}");
        }
    }
}