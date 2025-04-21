using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Domain.ViewModels;
using Core.Domain.Interfaces.Services;
using Core.Domain.Interfaces.Repository;

namespace Core.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class AlunoController : BaseController
{
    private readonly IAlunoService _alunoService;
    private readonly IUserContext _userContext;
    public AlunoController(IAlunoService alunoService, IUserContext userContext)
    {
        _userContext = userContext;
        _alunoService = alunoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterAlunos()
    {
        var responsavelId = _userContext.UserId;
        return Success(await _alunoService.ObterTodos(responsavelId));
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> ObterAlunosPorFiltro([FromQuery] string filtro, [FromQuery] int? rotaId = 0)
    {
        return Success(await _alunoService.ObterAlunosPorFiltro(rotaId.Value, filtro));
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarAluno(AlunoAdicionarViewModel aluno)
    {
        var responsavelId = _userContext.UserId;
        await _alunoService.AdicionarAsync(responsavelId, aluno);
        return Success();
    }

    [HttpPut]
    public async Task<IActionResult> AtualizarAluno(AlunoViewModel aluno)
    {
        var responsavelId = _userContext.UserId;
        await _alunoService.AtualizarAsync(responsavelId, aluno);
        return Success();
    }

    [HttpDelete("{alunoId}")]
    public async Task<IActionResult> DeletarAluno(int alunoId)
    {
        var responsavelId = _userContext.UserId;
        await _alunoService.DeletarAsync(responsavelId, alunoId);
        return Success();
    }

    [HttpPost("{alunoId}/rota/{rotaId}/vincular")]
    public async Task<IActionResult> VincularRota(int alunoId, int rotaId)
    {
        await _alunoService.VincularRotaAsync(rotaId, alunoId);
        return Success();
    }

    [HttpPut("{alunoId}/rota/{rotaId}/desvincular")]
    public async Task<IActionResult> DesvincularRota(int alunoId, int rotaId)
    {
        await _alunoService.DesvincularRotaAsync(rotaId, alunoId);
        return Success();
    }

}
