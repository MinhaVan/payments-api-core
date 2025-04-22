using System;
using Payments.Domain.Enums;

namespace Payments.Domain.ViewModels.Assinatura;

public abstract class PagamentoViewModel
{
    public int PlanoId { get; set; }
    public int AlunoId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Valor { get; set; }
    public DateTime? Vencimento { get; set; }
    public abstract TipoPagamentoEnum TipoPagamento { get; }
}