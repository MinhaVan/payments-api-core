using Microsoft.EntityFrameworkCore;
using Payments.Domain.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Threading;
using Payments.Data.Configurations;

namespace Payments.Data.Context;

public class APIContext : DbContext
{
    public APIContext(DbContextOptions<APIContext> options) : base(options)
    { }

    public override int SaveChanges()
    {
        AtualizarDatas();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarDatas();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlanoConfiguration());
        modelBuilder.ApplyConfiguration(new PagamentoConfiguration());
        modelBuilder.ApplyConfiguration(new AssinaturaConfiguration());

        base.OnModelCreating(modelBuilder);
    }
    private void AtualizarDatas()
    {
        var now = DateTime.UtcNow;
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((Entity)entry.Entity).DataCriacao = now;
                ((Entity)entry.Entity).Status = Domain.Enums.StatusEntityEnum.Ativo;
            }
            ((Entity)entry.Entity).DataAlteracao = now;
        }
    }

    public DbSet<Plano> Planos { get; set; }
    public DbSet<Assinatura> Assinaturas { get; set; }
    public DbSet<Pagamento> Pagamentos { get; set; }
}