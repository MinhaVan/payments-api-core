namespace Core.Domain.ApiModel;

public class BoletoResponse
{
    /// <summary>
    /// Linha digitável do boleto
    /// </summary>
    public string IdentificationField { get; set; }

    /// <summary>
    /// Número de identificação do boleto
    /// </summary>
    public string NossoNumero { get; set; }

    /// <summary>
    /// Código de barras do boleto
    /// </summary>
    public string BarCode { get; set; }
}
