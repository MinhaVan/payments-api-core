using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Models;

public class Assinatura : Entity
{
    public string IdExterno { get; set; }
    public DateTime Vencimento { get; set; }
    public decimal Valor { get; set; }
    public int PlanoId { get; set; }
    public int AlunoId { get; set; }
    public int UsuarioId { get; set; }
    public string NumeroCartao { get; set; }
    public TipoPagamentoEnum TipoPagamento { get; set; }
    /// <summary>
    /// Copia e cola do PIX ou Linha digital para boleto
    /// </summary>
    public string CopiaCola { get; set; }
    /// <summary>
    /// QRCode para PIX ou codigo de barras do BOLETO
    /// </summary>
    public string Imagem { get; set; }
    //
    public Usuario Usuario { get; set; }
    public Plano Plano { get; set; }
    public virtual List<Pagamento> Pagamentos { get; set; }
}