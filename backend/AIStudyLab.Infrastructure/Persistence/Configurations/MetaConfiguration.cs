using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIStudyLab.Infrastructure.Persistence.Configurations;

public sealed class MetaConfiguration : IEntityTypeConfiguration<Meta>
{
    public void Configure(EntityTypeBuilder<Meta> builder)
    {
        builder.ToTable("metas", table =>
        {
            table.HasCheckConstraint("ck_metas_quantidade_positiva", "quantidade > 0");
            table.HasCheckConstraint("ck_metas_intervalo_datas", "data_fim >= data_inicio");
        });
        builder.HasKey(meta => meta.Id);

        builder.Property(meta => meta.Id).HasColumnName("id");
        builder.Property(meta => meta.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(meta => meta.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(meta => meta.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
        builder.Property(meta => meta.Quantidade).HasColumnName("quantidade").IsRequired();
        builder.Property(meta => meta.Unidade).HasColumnName("unidade").HasMaxLength(50).IsRequired();
        builder.Property(meta => meta.DataInicio).HasColumnName("data_inicio").IsRequired();
        builder.Property(meta => meta.DataFim).HasColumnName("data_fim").IsRequired();
        builder.Property(meta => meta.Ativa).HasColumnName("ativa").IsRequired();
        builder.Property(meta => meta.CreatedAt).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(meta => meta.UpdatedAt).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(meta => new { meta.UsuarioId, meta.Ativa, meta.DataInicio })
            .HasDatabaseName("ix_metas_usuario_ativa_inicio");
        builder.HasOne(meta => meta.Usuario)
            .WithMany(usuario => usuario.Metas)
            .HasForeignKey(meta => meta.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
