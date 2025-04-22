using System;
using Payments.Domain.Enums;

namespace Payments.Domain.ViewModels.Assinatura;

public class PagamentoHistoricoViewModel
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public StatusEntityEnum Status { get; set; }
    public DateTime? DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public PagamentoStatusEnum StatusPagamento { get; set; }
    public int AssinaturaId { get; set; }
    public string PagamentoIdExternal { get; set; }
    public string AssinaturaExternal { get; set; }
    public string TipoFaturamento { get; set; }
    public string FaturaURL { get; set; }
    public string NumeroFatura { get; set; }
    public decimal Valor { get; set; }
}