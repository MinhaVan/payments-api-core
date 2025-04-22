using System;

namespace Payments.Domain.ApiModel;

public class AssinaturaRequest
{
    public string IdExterno { get; set; }
    public DateTime Vencimento { get; set; }
    public decimal Valor { get; set; }
    public int PlanoId { get; set; }
    public int UsuarioId { get; set; }
    public CartaoCreditoRequest CartaoCredito { get; set; }
    public InformacoesTitularCartaoRequest InformacoesTitularCartao { get; set; }
}

public class CartaoCreditoRequest
{
    public string NomeTitular { get; set; }
    public string Numero { get; set; }
    public string MesVencimento { get; set; }
    public string AnoVencimento { get; set; }
    public string Cvv { get; set; }
}

public class InformacoesTitularCartaoRequest
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
