using Core.Domain.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Interfaces.Services;

public interface IPlanoService
{
    Task Adicionar(PlanoAdicionarViewModel planoVM);
    Task Atualizar(PlanoViewModel planoVM);
    Task Deletar(int planoId);
    Task<List<PlanoViewModel>> Obter(int empresaId);
}