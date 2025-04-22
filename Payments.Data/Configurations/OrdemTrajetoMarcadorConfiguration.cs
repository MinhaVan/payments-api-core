using Payments.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Data.Configurations;

public class OrdemTrajetoMarcadorConfiguration : IEntityTypeConfiguration<OrdemTrajetoMarcador>
{
    public void Configure(EntityTypeBuilder<OrdemTrajetoMarcador> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("ordem_trajeto_marcador");
        modelBuilder.HasOne(x => x.OrdemTrajeto)
            .WithMany(y => y.Marcadores)
            .HasForeignKey(x => x.OrdemTrajetoId);
    }
}