using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.Domain.ViewModels.Rota;

namespace Core.Domain.Interfaces.Services;

public interface IEnderecoService
{
    Task AdicionarAsync(EnderecoAdicionarViewModel enderecoAdicionarViewModel);
    Task AtualizarAsync(EnderecoAtualizarViewModel enderecoAdicionarViewModel);
    Task DeletarAsync(int id);
    Task<EnderecoViewModel> Obter(int id);
    Task<List<EnderecoViewModel>> Obter();
    Task<Marcador> ObterMarcadorAsync(string endereco);
}