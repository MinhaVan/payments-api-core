using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;

public class RotaConfiguration : IEntityTypeConfiguration<Rota>
{
    public void Configure(EntityTypeBuilder<Rota> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("rota");
        modelBuilder.HasOne(x => x.Veiculo)
            .WithMany(y => y.Rotas)
            .HasForeignKey(x => x.VeiculoId);
    }
}