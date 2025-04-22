using System.Threading.Tasks;
using Payments.Domain.ApiModel;

namespace Payments.Domain.Interfaces.Services;

public interface IAsaasService
{
    Task<bool> PagamentoHookAsync(PagamentoWebHookAsaasRequest payment);
}