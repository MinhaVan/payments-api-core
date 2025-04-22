using System;
using Payments.Domain.Enums;

namespace Payments.Domain.ApiModel;

public class PagamentoRequest
{
    public string ExternalIdCliente { get; set; }
    public TipoPagamentoEnum FormaPagamento { get; set; }
    public decimal Valor { get; set; }
    public DateTime Vencimento { get; set; }
    public int DiasAposVencimentoParaCancelamento { get; set; }
    public string ExternalReference { get; set; }
}