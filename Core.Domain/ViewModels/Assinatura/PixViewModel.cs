using Core.Domain.Enums;

namespace Core.Domain.ViewModels.Assinatura;

public class PixViewModel : PagamentoViewModel
{
    public override TipoPagamentoEnum TipoPagamento => TipoPagamentoEnum.Pix;
}