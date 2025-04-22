using System;

namespace Payments.Domain.Models;

public class Pagamento : Entity
{
    public DateTime? DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public int StatusPagamento { get; set; }
    public int AssinaturaId { get; set; }
    public string PagamentoIdExternal { get; set; }
    public string AssinaturaExternal { get; set; }
    public string TipoFaturamento { get; set; }
    public string FaturaURL { get; set; }
    public string NumeroFatura { get; set; }
    public decimal Valor { get; set; }
    //
    public virtual Assinatura Assinatura { get; set; }
}