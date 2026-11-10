using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIStudyLab.Infrastructure.Persistence.Configurations;

public sealed class ConfiguracaoUsuarioConfiguration : IEntityTypeConfiguration<ConfiguracaoUsuario>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoUsuario> builder)
    {
        builder.ToTable("configuracoes_usuario", table =>
            table.HasCheckConstraint("ck_configuracoes_usuario_objetivo_diario", "objetivo_diario_minutos >= 0"));
        builder.HasKey(configuracao => configuracao.Id);

        builder.Property(configuracao => configuracao.Id).HasColumnName("id");
        builder.Property(configuracao => configuracao.UsuarioId).HasColumnName("usuario_id");
        builder.Property(configuracao => configuracao.Tema).HasColumnName("tema").HasMaxLength(30).IsRequired();
        builder.Property(configuracao => configuracao.Idioma).HasColumnName("idioma").HasMaxLength(20).IsRequired();
        builder.Property(configuracao => configuracao.NotificacoesAtivadas).HasColumnName("notificacoes_ativadas").IsRequired();
        builder.Property(configuracao => configuracao.ObjetivoDiarioMinutos).HasColumnName("objetivo_diario_minutos").IsRequired();
        builder.Property(configuracao => configuracao.DisciplinaFocoId).HasColumnName("disciplina_foco_id");
        builder.Property(configuracao => configuracao.CreatedAt).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(configuracao => configuracao.UpdatedAt).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(configuracao => configuracao.UsuarioId).IsUnique().HasDatabaseName("ux_configuracoes_usuario_usuario_id");
        builder.HasOne(configuracao => configuracao.Usuario)
            .WithOne(usuario => usuario.Configuracoes)
            .HasForeignKey<ConfiguracaoUsuario>(configuracao => configuracao.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(configuracao => configuracao.DisciplinaFoco)
            .WithMany()
            .HasForeignKey(configuracao => new { configuracao.DisciplinaFocoId, configuracao.UsuarioId })
            .HasPrincipalKey(disciplina => new { FocusDisciplineId = disciplina.Id, disciplina.UsuarioId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
