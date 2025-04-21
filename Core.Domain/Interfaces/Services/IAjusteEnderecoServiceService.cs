using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.ViewModels;
using Core.Domain.ViewModels.Rota;

namespace Core.Domain.Interfaces.Services;

public interface IAjusteEnderecoService
{
    Task<List<RotaAjusteEnderecoViewModel>> ObterAjusteEnderecoAsync(int AlunoId, int rotaId);
    Task AdicionarAjusteEnderecoAsync(RotaAdicionarAjusteEnderecoViewModel alterarEnderecoViewModel);
    Task AlterarAjusteEnderecoAsync(RotaAlterarAjusteEnderecoViewModel alterarAjusteEnderecoViewModel);
}