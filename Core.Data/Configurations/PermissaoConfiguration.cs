using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;
public class PermissaoConfiguration : IEntityTypeConfiguration<Permissao>
{

    public void Configure(EntityTypeBuilder<Permissao> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();

        modelBuilder.ToTable("permissoes");
    }
}