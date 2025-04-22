using Payments.Domain.Enums;
using Payments.Domain.ViewModels.Assinatura;

namespace Payments.Domain.ViewModels;

public class CreditoViewModel : PagamentoViewModel
{
    public override TipoPagamentoEnum TipoPagamento => TipoPagamentoEnum.Credito;
    public int EmpresaId { get; set; }
    public CartaoCreditoViewModel CartaoCredito { get; set; }
    public InformacoesTitularCartaoViewModel InformacoesTitularCartao { get; set; }
}

public class CartaoCreditoViewModel
{
    public string NomeTitular { get; set; }
    public string Numero { get; set; }
    public string MesVencimento { get; set; }
    public string AnoVencimento { get; set; }
    public string Cvv { get; set; }
}

public class InformacoesTitularCartaoViewModel
{
    public string Nome { get; set; }
    public string CpfCnpj { get; set; }
    public string Telefone { get; set; }
    public string Celular { get; set; }
    public string Email { get; set; }
    public string Cep { get; set; }
    public string NumeroEndereco { get; set; }
    public string Complemento { get; set; }
}