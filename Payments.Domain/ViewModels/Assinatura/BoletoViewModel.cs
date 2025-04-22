using Payments.Domain.Enums;

namespace Payments.Domain.ViewModels.Assinatura;

public class BoletoViewModel : PagamentoViewModel
{
    public override TipoPagamentoEnum TipoPagamento => TipoPagamentoEnum.Boleto;
}