using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Domain.ViewModels;
using Core.Domain.Interfaces.Services;

namespace Core.API.Controllers.v1;

[ApiController]
[AllowAnonymous]
[Route("v1/[controller]")]
public class AuthController : BaseController
{
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenViewModel>> Login([FromBody] UsuarioLoginViewModel user)
    {
        var token = await _tokenService.Login(user);
        if (token == null)
        {
            return Default(400, "Acesso negado!", true);
        }

        return Success(token);
    }

    [HttpPost("refreshToken")]
    public async Task<ActionResult<TokenViewModel>> RefreshToken([FromBody] UsuarioLoginViewModel user)
    {
        var token = await _tokenService.RefreshToken(user);
        if (token == null)
        {
            return Default(400, "Acesso negado!", true);
        }

        return Success(token);
    }
}
