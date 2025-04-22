using System.Threading.Tasks;
using Payments.Domain.Interfaces.Repository;
using Payments.Domain.Models;

namespace Payments.Domain.Interfaces.Repositories;

public interface IRotaHistoricoRepository : IBaseRepository<RotaHistorico>
{
    Task<RotaHistorico> ObterUltimoTrajetoAsync(int rotaId);
}