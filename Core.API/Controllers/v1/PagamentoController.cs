using System.Threading.Tasks;
using Core.Domain.Interfaces.Services;
using Core.Domain.ViewModels;
using Core.Domain.ViewModels.Assinatura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class PagamentoController : BaseController
{
    private readonly IAssinaturaService _assinaturaService;

    public PagamentoController(IAssinaturaService assinaturaService)
    {
        _assinaturaService = assinaturaService;
    }

    [HttpPost("Assinatura")]
    public async Task<IActionResult> CriarAssinatura([FromBody] CreditoViewModel requisicao)
    {
        await _assinaturaService.AssinaturaCreditoAsync(requisicao);
        return Success();
    }

    [HttpPost("Pix")]
    public async Task<IActionResult> PagamentoPixAsync([FromBody] PixViewModel requisicao)
    {
        await _assinaturaService.AssinaturaBoletoPixAsync(requisicao);
        return Success();
    }

    [HttpPost("Boleto")]
    public async Task<IActionResult> PagamentoBoletoAsync([FromBody] BoletoViewModel requisicao)
    {
        await _assinaturaService.AssinaturaBoletoPixAsync(requisicao);
        return Success();
    }

    [HttpGet("{alunoId}")]
    public async Task<IActionResult> ObterHistoricoAsync([FromRoute] int alunoId)
    {
        return Success(await _assinaturaService.ObterHistoricoAsync(alunoId));
    }

    [HttpPut("atualizar-plano")]
    public async Task<IActionResult> AtualizarFormaPagamento([FromBody] AtualizarFormaPagamento requisicao)
    {
        await _assinaturaService.AtualizarFormaPagamentoAsync(requisicao);
        return Success();
    }
}
