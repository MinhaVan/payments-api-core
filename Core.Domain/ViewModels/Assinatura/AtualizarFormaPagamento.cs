using Core.Domain.Enums;

namespace Core.Domain.ViewModels.Assinatura;

public class AtualizarFormaPagamento
{
    public TipoPagamentoEnum NovoTipoPagamento { get; set; }
    public BoletoViewModel Boleto { get; set; }
    public PixViewModel Pix { get; set; }
    public CreditoViewModel Credito { get; set; }
}