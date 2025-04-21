using System.Threading.Tasks;
using Core.Domain.Enums;
using Core.Domain.Interfaces.Services;
using Core.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class PlanoController : BaseController
{
    private readonly IPlanoService _planoService;
    public PlanoController(IPlanoService planoService)
    {
        _planoService = planoService;
    }

    [AllowAnonymous]
    [HttpGet("empresa/{empresaId}")]
    public async Task<IActionResult> Obter(int empresaId)
    {
        var response = await _planoService.Obter(empresaId);
        return Success(response);
    }

    [HttpPost]
    // [AuthorizeRoles(PerfilEnum.Administrador)]
    public async Task<IActionResult> Adicionar(PlanoAdicionarViewModel request)
    {
        await _planoService.Adicionar(request);
        return Success();
    }

    [HttpPut]
    // [AuthorizeRoles(PerfilEnum.Administrador)]
    public async Task<IActionResult> Atualizar(PlanoViewModel request)
    {
        await _planoService.Atualizar(request);
        return Success();
    }

    [HttpDelete("{planoId}")]
    // [AuthorizeRoles(PerfilEnum.Administrador)]
    public async Task<IActionResult> Deletar(int planoId)
    {
        await _planoService.Deletar(planoId);
        return Success();
    }
}