using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;
public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> modelBuilder)
    {
        modelBuilder.ToTable("endereco");
        modelBuilder.ConfigureBaseEntity();

        modelBuilder.HasOne(x => x.Empresa)
            .WithMany(y => y.Enderecos)
            .HasForeignKey(x => x.EmpresaId);

        modelBuilder.HasOne(x => x.Usuario)
            .WithMany(y => y.Enderecos)
            .HasForeignKey(x => x.UsuarioId);
    }
}