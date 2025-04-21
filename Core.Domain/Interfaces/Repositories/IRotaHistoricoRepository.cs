using System.Threading.Tasks;
using Core.Domain.Interfaces.Repository;
using Core.Domain.Models;

namespace Core.Domain.Interfaces.Repositories;
public interface IRotaHistoricoRepository : IBaseRepository<RotaHistorico>
{
    Task<RotaHistorico> ObterUltimoTrajetoAsync(int rotaId);
}