using System.Threading.Tasks;
using Payments.Domain.ApiModel;

namespace Payments.Domain.Interfaces.APIs;

public interface IApiAsaas
{
    Task<PaginadaResponse<ClienteResponse>> ObterClienteAsync(
        int? offset = null,
        int? limit = null,
        string nome = null,
        string email = null,
        string cpfCnpj = null,
        string nomeGrupo = null,
        string referenciaExterna = null);
    Task<ClienteResponse> CriarClienteAsync(ClienteRequest clienteRequest);
    Task<BoletoResponse> ObterLinhaDigitavelBoleto(string cobrancaId);
    Task<PixResponse> ObterQRCodePix(string cobrancaId);
    Task<bool> ExcluirAssinaturaAsync(string assinaturaId);
    Task<PaginadaResponse<AssinaturaResponse>> ListarAssinaturasAsync(string externalId);
    Task<AssinaturaResponse> CriarAssinaturaAsync(AssinaturaRequest assinatura, int assinaturaId, string clienteId, string nomePlano);
    Task<AssinaturaResponse> PagamentoAsync(PagamentoRequest pagamento, int assinaturaId, string clienteId, string nomePlano);
}
