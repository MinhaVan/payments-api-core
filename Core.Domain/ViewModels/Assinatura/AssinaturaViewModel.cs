using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Enums;

namespace Core.Domain.ViewModels.Assinatura;

public class AssinaturaViewModel
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
    public StatusEntityEnum Status { get; set; }
    public string IdExterno { get; set; }
    public DateTime Vencimento { get; set; }
    public decimal Valor { get; set; }
    public int PlanoId { get; set; }
    public int AlunoId { get; set; }
    public int UsuarioId { get; set; }
    public TipoPagamentoEnum TipoPagamento { get; set; }
    /// <summary>
    /// Copia e cola do PIX ou Linha digital para boleto
    /// </summary>
    public string CopiaCola { get; set; }
    /// <summary>
    /// QRCode para PIX ou codigo de barras do BOLETO
    /// </summary>
    public string Imagem { get; set; }
    public string NumeroCartao { get; set; }
    public bool PagamentoEmDia
    {
        get
        {
            if (this.Pagamentos is null || !this.Pagamentos.Any())
                return false;

            var statusPermitidos = new List<PagamentoStatusEnum>
            {
                PagamentoStatusEnum.RECEIVED, PagamentoStatusEnum.CONFIRMED
            };

            return Pagamentos.All(x => statusPermitidos.Contains(x.StatusPagamento));
        }
    }
    //
    public List<PagamentoHistoricoViewModel> Pagamentos { get; set; }
    public PlanoViewModel Plano { get; set; }
}