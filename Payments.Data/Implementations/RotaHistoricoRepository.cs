using System.Linq;
using System.Threading.Tasks;
using Payments.Data.Context;
using Payments.Data.Repositories;
using Payments.Domain.Interfaces.Repositories;
using Payments.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Payments.Data.Implementations;

public class RotaHistoricoRepository : BaseRepository<RotaHistorico>, IRotaHistoricoRepository
{
    private readonly APIContext _context;
    public RotaHistoricoRepository(APIContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RotaHistorico> ObterUltimoTrajetoAsync(int rotaId)
    {
        return await _context.RotaHistoricos
            .Include(x => x.Rota)
            .Include(x => x.Rota.Veiculo)
            .Include(x => x.Rota.AlunoRotas)
            .Where(x => x.RotaId == rotaId)
            .OrderByDescending(x => x.DataFim)
            .FirstOrDefaultAsync();
    }
}