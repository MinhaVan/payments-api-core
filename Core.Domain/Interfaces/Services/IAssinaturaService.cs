using System.Threading.Tasks;
using Core.Domain.ViewModels;
using Core.Domain.ViewModels.Assinatura;

namespace Core.Domain.Interfaces.Services;
public interface IAssinaturaService
{
    Task AssinaturaCreditoAsync(CreditoViewModel model);
    Task AssinaturaBoletoPixAsync(PagamentoViewModel model);
    Task AtualizarFormaPagamentoAsync(AtualizarFormaPagamento requisicao);
    Task<AssinaturaViewModel> ObterHistoricoAsync(int AlunoId);
}
