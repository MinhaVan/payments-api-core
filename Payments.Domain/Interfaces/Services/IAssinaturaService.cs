using System.Threading.Tasks;
using Payments.Domain.ViewModels;
using Payments.Domain.ViewModels.Assinatura;

namespace Payments.Domain.Interfaces.Services;

public interface IAssinaturaService
{
    Task AssinaturaCreditoAsync(CreditoViewModel model);
    Task AssinaturaBoletoPixAsync(PagamentoViewModel model);
    Task AtualizarFormaPagamentoAsync(AtualizarFormaPagamento requisicao);
    Task<AssinaturaViewModel> ObterHistoricoAsync(int AlunoId);
}
