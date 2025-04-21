using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;

public class AssinaturaConfiguration : IEntityTypeConfiguration<Assinatura>
{
    public void Configure(EntityTypeBuilder<Assinatura> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("assinaturas");
        modelBuilder.HasOne(x => x.Usuario)
            .WithMany(y => y.Assinaturas)
            .HasForeignKey(x => x.UsuarioId);

        modelBuilder.HasOne(x => x.Plano)
            .WithMany(y => y.Assinaturas)
            .HasForeignKey(x => x.PlanoId);

    }
}