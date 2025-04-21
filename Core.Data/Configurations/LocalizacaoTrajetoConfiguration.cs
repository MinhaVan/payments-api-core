using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;
public class LocalizacaoTrajetoConfiguration : IEntityTypeConfiguration<LocalizacaoTrajeto>
{

    public void Configure(EntityTypeBuilder<LocalizacaoTrajeto> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();

        modelBuilder.ToTable("localizacao_trajeto");
        modelBuilder.HasOne(x => x.RotaHistorico)
            .WithMany(y => y.LocalizacaoTrajeto)
            .HasForeignKey(x => x.RotaHistoricoId);
    }
}