using Core.Domain.Enums;

namespace Core.Domain.ApiModel;

public class PagamentoResponse
{
    public string Id { get; set; }
    public TipoPagamentoEnum TipoPagamento { get; set; }
    public string Imagem { get; set; }
    public string CopiaCola { get; set; }
}