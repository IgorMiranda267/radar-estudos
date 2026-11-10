using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIStudyLab.Infrastructure.Persistence.Configurations;

public sealed class DisciplinaConfiguration : IEntityTypeConfiguration<Disciplina>
{
    public void Configure(EntityTypeBuilder<Disciplina> builder)
    {
        builder.ToTable("disciplinas");
        builder.HasKey(disciplina => disciplina.Id);
        builder.HasAlternateKey(disciplina => new { disciplina.Id, disciplina.UsuarioId });

        builder.Property(disciplina => disciplina.Id).HasColumnName("id");
        builder.Property(disciplina => disciplina.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(disciplina => disciplina.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
        builder.Property(disciplina => disciplina.Cor).HasColumnName("cor").HasMaxLength(30).IsRequired();
        builder.Property(disciplina => disciplina.Icone).HasColumnName("icone").HasMaxLength(50).IsRequired();
        builder.Property(disciplina => disciplina.Ativa).HasColumnName("ativa").IsRequired();
        builder.Property(disciplina => disciplina.CreatedAt).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(disciplina => disciplina.UpdatedAt).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(disciplina => new { disciplina.UsuarioId, disciplina.Nome })
            .HasDatabaseName("ix_disciplinas_usuario_nome");
        builder.HasOne(disciplina => disciplina.Usuario)
            .WithMany(usuario => usuario.Disciplinas)
            .HasForeignKey(disciplina => disciplina.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
