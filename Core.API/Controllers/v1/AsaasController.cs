using System.Threading.Tasks;
using Core.API.Filters;
using Core.Domain.ApiModel;
using Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
public class AsaasController : BaseController
{
    private readonly IAsaasService _asaasService;
    public AsaasController(IAsaasService pasaaservice)
    {
        _asaasService = pasaaservice;
    }

    [HttpPost("pagamento")]
    [ValidateAsaasAccessToken]
    public async Task<IActionResult> PagamentoHook([FromBody] PagamentoWebHookAsaasRequest payment)
    {
        var response = await _asaasService.PagamentoHookAsync(payment);
        return response ? Ok() : BadRequest();
    }
}

