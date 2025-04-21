using System.Security.Cryptography;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.Domain.ViewModels;

namespace Core.Domain.Interfaces.Repository;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario> LoginAsync(UsuarioLoginViewModel user);
    string ComputeHash(string input, SHA256CryptoServiceProvider algorithm);
}