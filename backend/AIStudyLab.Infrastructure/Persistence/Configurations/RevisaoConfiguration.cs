using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIStudyLab.Infrastructure.Persistence.Configurations;

public sealed class RevisaoConfiguration : IEntityTypeConfiguration<Revisao>
{
    public void Configure(EntityTypeBuilder<Revisao> builder)
    {
        builder.ToTable("revisoes");
        builder.HasKey(revisao => revisao.Id);

        builder.Property(revisao => revisao.Id).HasColumnName("id");
        builder.Property(revisao => revisao.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(revisao => revisao.AssuntoId).HasColumnName("assunto_id").IsRequired();
        builder.Property(revisao => revisao.DataAgendada).HasColumnName("data_agendada").IsRequired();
        builder.Property(revisao => revisao.DataConclusao).HasColumnName("data_conclusao");
        builder.Property(revisao => revisao.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(revisao => revisao.Observacoes).HasColumnName("observacoes");
        builder.Property(revisao => revisao.CreatedAt).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(revisao => revisao.UpdatedAt).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(revisao => new { revisao.UsuarioId, revisao.DataAgendada, revisao.Status })
            .HasDatabaseName("ix_revisoes_usuario_data_status");
        builder.HasOne(revisao => revisao.Usuario)
            .WithMany(usuario => usuario.Revisoes)
            .HasForeignKey(revisao => revisao.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(revisao => revisao.Assunto)
            .WithMany(assunto => assunto.Revisoes)
            .HasForeignKey(revisao => revisao.AssuntoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
