using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;
public class UsuarioPermissaoConfiguration : IEntityTypeConfiguration<UsuarioPermissao>
{

    public void Configure(EntityTypeBuilder<UsuarioPermissao> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.HasKey(x => new { x.UsuarioId, x.PermissaoId });

        modelBuilder.ToTable("usuario_permissao");
        modelBuilder.HasOne(x => x.Usuario)
            .WithMany(y => y.Permissoes)
            .HasForeignKey(x => x.UsuarioId);

        modelBuilder.HasOne(x => x.Permissao)
            .WithMany(y => y.Usuarios)
            .HasForeignKey(x => x.PermissaoId);
    }
}