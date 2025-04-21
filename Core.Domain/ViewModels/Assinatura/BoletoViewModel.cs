using Core.Domain.Enums;

namespace Core.Domain.ViewModels.Assinatura;

public class BoletoViewModel : PagamentoViewModel
{
    public override TipoPagamentoEnum TipoPagamento => TipoPagamentoEnum.Boleto;
}