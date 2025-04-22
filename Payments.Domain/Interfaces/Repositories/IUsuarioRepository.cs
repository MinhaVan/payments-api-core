using System.Security.Cryptography;
using System.Threading.Tasks;
using Payments.Domain.Models;
using Payments.Domain.ViewModels;

namespace Payments.Domain.Interfaces.Repository;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario> LoginAsync(UsuarioLoginViewModel user);
    string ComputeHash(string input, SHA256CryptoServiceProvider algorithm);
}