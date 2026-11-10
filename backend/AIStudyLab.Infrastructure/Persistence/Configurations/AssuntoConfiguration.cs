using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIStudyLab.Infrastructure.Persistence.Configurations;

public sealed class AssuntoConfiguration : IEntityTypeConfiguration<Assunto>
{
    public void Configure(EntityTypeBuilder<Assunto> builder)
    {
        builder.ToTable("assuntos", table =>
            table.HasCheckConstraint("ck_assuntos_ordem", "ordem >= 1"));
        builder.HasKey(assunto => assunto.Id);
        builder.HasAlternateKey(assunto => new { assunto.Id, assunto.DisciplinaId });

        builder.Property(assunto => assunto.Id).HasColumnName("id");
        builder.Property(assunto => assunto.DisciplinaId).HasColumnName("disciplina_id").IsRequired();
        builder.Property(assunto => assunto.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
        builder.Property(assunto => assunto.Descricao).HasColumnName("descricao");
        builder.Property(assunto => assunto.Dificuldade).HasColumnName("dificuldade").HasConversion<string>().HasMaxLength(20);
        builder.Property(assunto => assunto.Ordem).HasColumnName("ordem").IsRequired();
        builder.Property(assunto => assunto.CreatedAt).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(assunto => assunto.UpdatedAt).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(assunto => new { assunto.DisciplinaId, assunto.Nome })
            .HasDatabaseName("ix_assuntos_disciplina_nome");
        builder.HasOne(assunto => assunto.Disciplina)
            .WithMany(disciplina => disciplina.Assuntos)
            .HasForeignKey(assunto => assunto.DisciplinaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
