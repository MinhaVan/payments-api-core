namespace Core.Domain.ApiModel;

public class PixResponse
{
    /// <summary>
    /// Imagem do QrCode em base64
    /// </summary>
    public string EncodedImage { get; set; }

    /// <summary>
    /// Copia e Cola do QrCode
    /// </summary>
    public string Payload { get; set; }

    /// <summary>
    /// Data de expiração do QrCode
    /// </summary>
    public string ExpirationDate { get; set; }
}
