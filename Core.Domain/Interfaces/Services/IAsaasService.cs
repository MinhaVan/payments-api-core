using System.Threading.Tasks;
using Core.Domain.ApiModel;

namespace Core.Domain.Interfaces.Services;

public interface IAsaasService
{
    Task<bool> PagamentoHookAsync(PagamentoWebHookAsaasRequest payment);
}