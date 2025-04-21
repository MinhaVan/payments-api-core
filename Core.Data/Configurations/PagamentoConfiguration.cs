using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("pagamentos");
        modelBuilder.HasOne(x => x.Assinatura)
            .WithMany(y => y.Pagamentos)
            .HasForeignKey(x => x.AssinaturaId);
    }
}