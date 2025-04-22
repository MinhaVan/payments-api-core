using System.Threading.Tasks;
using Payments.Domain.ViewModels;

namespace Payments.Domain.Interfaces.Services;

public interface IMotoristaService
{
    Task VincularAsync(MotoristaVincularViewModel request);
    Task DesvincularAsync(MotoristaVincularViewModel request);
    Task<MotoristaViewModel> AdicionarAsync(MotoristaNovoViewModel usuarioAdicionarViewModel);
    Task AtualizarAsync(MotoristaAtualizarViewModel usuarioAdicionarViewModel);
    Task DeletarAsync(int id);
    Task<UsuarioViewModel> Obter(int id);
    Task<PaginadoViewModel<UsuarioViewModel>> Obter(int pagina, int tamanho);
}