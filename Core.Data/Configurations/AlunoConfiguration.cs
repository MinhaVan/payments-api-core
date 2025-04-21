using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configurations;
public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("Alunos");

        modelBuilder.HasOne(x => x.Responsavel)
            .WithMany(y => y.Alunos)
            .HasForeignKey(x => x.ResponsavelId);

        modelBuilder.HasOne(x => x.Empresa)
            .WithMany(y => y.Alunos)
            .HasForeignKey(x => x.EmpresaId);

        modelBuilder.HasOne(x => x.EnderecoPartida)
            .WithMany(y => y.EnderecosPartidas)
            .HasForeignKey(x => x.EnderecoPartidaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.HasOne(x => x.EnderecoRetorno)
            .WithMany(y => y.EnderecosRetornos)
            .HasForeignKey(x => x.EnderecoRetornoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.HasOne(x => x.EnderecoDestino)
            .WithMany(y => y.EnderecosDestinos)
            .HasForeignKey(x => x.EnderecoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Remover esta linha
        // modelBuilder.Ignore(y => y.EnderecoRetorno);
    }
}
