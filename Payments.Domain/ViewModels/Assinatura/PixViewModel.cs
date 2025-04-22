using Payments.Domain.Enums;

namespace Payments.Domain.ViewModels.Assinatura;

public class PixViewModel : PagamentoViewModel
{
    public override TipoPagamentoEnum TipoPagamento => TipoPagamentoEnum.Pix;
}