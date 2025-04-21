using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;

public class RotaHistoricoConfiguration : IEntityTypeConfiguration<RotaHistorico>
{
    public void Configure(EntityTypeBuilder<RotaHistorico> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("rota_historico");
        modelBuilder.HasOne(x => x.Rota)
            .WithMany(y => y.Historicos)
            .HasForeignKey(x => x.RotaId);
    }
}