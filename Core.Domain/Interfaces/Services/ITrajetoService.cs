using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.ViewModels;
using Core.Domain.ViewModels.Rota;

namespace Core.Domain.Interfaces.Services;

public interface ITrajetoService
{
    Task AtualizarStatusAlunoTrajetoAsync(int alunoId, int rotaId, bool alunoEntrouNaVan);
    Task FinalizarTrajetoAsync(int rotaId);
    Task IniciarTrajetoAsync(int rotaId);
    Task<RotaHistoricoViewModel> ObterTrajetoEmAndamentoAsync(int rotaId);
    Task SalvarOrdemDoTrajetoAsync(int rotaId, List<Marcador> marcadoresOrdenados);
    Task<List<Marcador>> ObterTodosMarcadoresParaRotasAsync(int rotaId);
    Task<List<Marcador>> ObterDestinoAsync(int rotaId, bool validarRotaOnline = true);
    Task<RotaHistoricoViewModel> RelatorioUltimoTrajetoAsync(int rotaId);
    Task<RotaViewModel> RotaOnlineParaMotoristaAsync();
}