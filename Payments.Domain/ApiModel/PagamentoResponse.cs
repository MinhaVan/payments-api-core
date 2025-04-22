using Payments.Domain.Enums;

namespace Payments.Domain.ApiModel;

public class PagamentoResponse
{
    public string Id { get; set; }
    public TipoPagamentoEnum TipoPagamento { get; set; }
    public string Imagem { get; set; }
    public string CopiaCola { get; set; }
}