using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIStudyLab.Infrastructure.Persistence.Configurations;

public sealed class SessaoEstudoConfiguration : IEntityTypeConfiguration<SessaoEstudo>
{
    public void Configure(EntityTypeBuilder<SessaoEstudo> builder)
    {
        builder.ToTable("sessoes_estudo", table =>
        {
            table.HasCheckConstraint("ck_sessoes_estudo_tempo_nao_negativo", "tempo_estudado_minutos >= 0");
            table.HasCheckConstraint("ck_sessoes_estudo_questoes_nao_negativas", "questoes >= 0 AND acertos >= 0 AND erros >= 0");
            table.HasCheckConstraint("ck_sessoes_estudo_total_questoes", "acertos + erros = questoes");
        });
        builder.HasKey(sessao => sessao.Id);

        builder.Property(sessao => sessao.Id).HasColumnName("id");
        builder.Property(sessao => sessao.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(sessao => sessao.DisciplinaId).HasColumnName("disciplina_id").IsRequired();
        builder.Property(sessao => sessao.AssuntoId).HasColumnName("assunto_id");
        builder.Property(sessao => sessao.Data).HasColumnName("data").IsRequired();
        builder.Property(sessao => sessao.TempoEstudadoMinutos).HasColumnName("tempo_estudado_minutos").IsRequired();
        builder.Property(sessao => sessao.Questoes).HasColumnName("questoes").IsRequired();
        builder.Property(sessao => sessao.Acertos).HasColumnName("acertos").IsRequired();
        builder.Property(sessao => sessao.Erros).HasColumnName("erros").IsRequired();
        builder.Property(sessao => sessao.Banca).HasColumnName("banca").HasMaxLength(100);
        builder.Property(sessao => sessao.Observacoes).HasColumnName("observacoes");
        builder.Property(sessao => sessao.CreatedAt).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(sessao => sessao.UpdatedAt).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(sessao => new { sessao.UsuarioId, sessao.Data })
            .HasDatabaseName("ix_sessoes_estudo_usuario_data");
        builder.HasIndex(sessao => new { sessao.DisciplinaId, sessao.Data })
            .HasDatabaseName("ix_sessoes_estudo_disciplina_data");
        builder.HasOne(sessao => sessao.Usuario)
            .WithMany(usuario => usuario.SessoesEstudo)
            .HasForeignKey(sessao => sessao.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(sessao => sessao.Disciplina)
            .WithMany(disciplina => disciplina.SessoesEstudo)
            .HasForeignKey(sessao => new { sessao.DisciplinaId, sessao.UsuarioId })
            .HasPrincipalKey(disciplina => new { disciplina.Id, disciplina.UsuarioId })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(sessao => sessao.Assunto)
            .WithMany(assunto => assunto.SessoesEstudo)
            .HasForeignKey(sessao => new { sessao.AssuntoId, sessao.DisciplinaId })
            .HasPrincipalKey(assunto => new { assunto.Id, assunto.DisciplinaId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
