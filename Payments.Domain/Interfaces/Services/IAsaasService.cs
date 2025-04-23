using System.Threading.Tasks;
using Payments.Domain.ApiModel;

namespace Payments.Domain.Interfaces.Services;

public interface IAsaasService
{
    Task PublicarNaFilaAsync(PagamentoWebHookAsaasRequest payment);
    Task<bool> PagamentoHookAsync(PagamentoWebHookAsaasRequest payment);
}