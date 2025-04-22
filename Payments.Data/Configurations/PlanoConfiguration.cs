using Payments.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Data.Configurations;

public class PlanoConfiguration : IEntityTypeConfiguration<Plano>
{
    public void Configure(EntityTypeBuilder<Plano> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("plano");
        modelBuilder.HasMany(x => x.Usuarios)
            .WithOne(y => y.Plano)
            .HasForeignKey(x => x.PlanoId);
        modelBuilder.HasOne(x => x.Empresa)
            .WithMany(y => y.Planos)
            .HasForeignKey(x => x.EmpresaId);
    }
}