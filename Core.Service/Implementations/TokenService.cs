using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Domain.Models;
using Core.Service.Configuration;
using Core.Domain.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;
using Core.Domain.ViewModels;
using System.Threading.Tasks;
using Core.Domain.Interfaces.Repository;
using Core.Service.Exceptions;

namespace Core.Service.Implementations;
public class TokenService : ITokenService
{
    private readonly TokenConfigurations _configuration;
    private readonly IUsuarioRepository _usuarioRepository;
    private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

    public TokenService(
        TokenConfigurations tokenConfiguration,
        IUsuarioRepository usuarioRepository)
    {
        _configuration = tokenConfiguration;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<TokenViewModel> Login(UsuarioLoginViewModel user)
    {
        user.Senha = Base64ToString(user.Senha);
        user.Senha = _usuarioRepository.ComputeHash(user.Senha, new SHA256CryptoServiceProvider());

        var userModel = await _usuarioRepository.LoginAsync(user);

        _ = userModel ?? throw new BusinessRuleException("Login ou senha inválido!");

        if (!userModel.UsuarioValidado)
        {
            throw new BusinessRuleException("Usuário não foi validado ainda. Favor acessar o e-mail e confirmar!");
        }

        return await GenerateTokensForUser(userModel);
    }

    public async Task<TokenViewModel> RefreshToken(UsuarioLoginViewModel user)
    {
        var userModel = await _usuarioRepository.BuscarUmAsync(x => x.RefreshToken == user.RefreshToken);
        if (userModel == null || userModel.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new BusinessRuleException("Sessão encerrada. Favor reconectar!");
        }

        return await GenerateTokensForUser(userModel);
    }

    private async Task<TokenViewModel> GenerateTokensForUser(Usuario userModel)
    {
        var now = DateTime.UtcNow;
        var accessToken = GenerateAccessToken(userModel);
        var refreshToken = GenerateRefreshToken();

        userModel.RefreshToken = refreshToken;
        userModel.RefreshTokenExpiryTime = now.AddDays(_configuration.DaysToExpiry);

        await _usuarioRepository.AtualizarAsync(userModel);

        var createDate = now;
        var expirationDate = createDate.AddMinutes(_configuration.Minutes);

        return new TokenViewModel(
            authenticated: true,
            created: createDate.ToString(DATE_FORMAT),
            expiration: expirationDate.ToString(DATE_FORMAT),
            accessToken: accessToken,
            refreshToken: refreshToken
        );
    }

    public string Base64ToString(string base64)
    {
        return base64;

        var valueBytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(valueBytes);
    }

    private string GenerateAccessToken(Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(_configuration.Secret) || _configuration.Secret.Length < 32)
            throw new ArgumentException("A chave secreta deve ter pelo menos 32 caracteres.");

        var claims = new List<Claim>
            {
                new Claim("UserId", usuario.Id.ToString()),
                new Claim("Perfil", usuario.Perfil.ToString()),
                new Claim("Empresa", usuario.EmpresaId.ToString())
            };

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: _configuration.Issuer,
            audience: _configuration.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.Minutes),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
