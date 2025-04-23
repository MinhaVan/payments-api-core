using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Filters;
using Payments.Domain.ApiModel;
using Payments.Domain.Interfaces.Services;

namespace Payments.API.Controllers.v1;

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
    public async Task<IActionResult> PagamentoHookAsync([FromBody] PagamentoWebHookAsaasRequest payment)
    {
        await _asaasService.PublicarNaFilaAsync(payment);
        return Ok();
    }

    [HttpPost("pagamento")]
    [Subscriber("queue.asaas.pagamento.v1")]
    public async Task<IActionResult> ProcessarPagamentoAsaasAsync(PagamentoWebHookAsaasRequest payment)
    {
        var response = await _asaasService.PagamentoHookAsync(payment);
        return response ? Ok() : BadRequest();
    }
}

