namespace Payments.Domain.Models;

public class Token
{
    public Token(bool autenticado, string criado, string expiracao, string accessToken, string refreshToken)
    {
        Autenticado = autenticado;
        Criado = criado;
        Expiracao = expiracao;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public bool Autenticado { get; set; }
    public string Criado { get; set; }
    public string Expiracao { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}