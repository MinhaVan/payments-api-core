using System.Threading.Tasks;
using Core.Domain.ViewModels;

namespace Core.Domain.Interfaces.Services;

public interface IUsuarioService
{
    Task<UsuarioViewModel> Registrar(UsuarioNovoViewModel user);
    Task Atualizar(UsuarioAtualizarViewModel user);
    Task<UsuarioViewModel> ObterPorId(int UserId);
    Task<UsuarioViewModel> ObterDadosDoUsuario();
    Task VincularPermissao(PermissaoViewModel user);
    Task ConfirmarCadastroAsync(int userId);
}