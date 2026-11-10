using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIStudyLab.Infrastructure.Persistence.Configurations;

public sealed class PerfilUsuarioConfiguration : IEntityTypeConfiguration<PerfilUsuario>
{
    public void Configure(EntityTypeBuilder<PerfilUsuario> builder)
    {
        builder.ToTable("perfis_usuario");
        builder.HasKey(perfil => perfil.Id);

        builder.Property(perfil => perfil.Id).HasColumnName("id");
        builder.Property(perfil => perfil.UsuarioId).HasColumnName("usuario_id");
        builder.Property(perfil => perfil.Objetivo).HasColumnName("objetivo");
        builder.Property(perfil => perfil.Nivel).HasColumnName("nivel").HasMaxLength(50);
        builder.Property(perfil => perfil.Cidade).HasColumnName("cidade").HasMaxLength(120);
        builder.Property(perfil => perfil.Instituicao).HasColumnName("instituicao").HasMaxLength(200);
        builder.Property(perfil => perfil.Bio).HasColumnName("bio");
        builder.Property(perfil => perfil.DataNascimento).HasColumnName("data_nascimento");
        builder.Property(perfil => perfil.CreatedAt).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(perfil => perfil.UpdatedAt).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(perfil => perfil.UsuarioId).IsUnique().HasDatabaseName("ux_perfis_usuario_usuario_id");
        builder.HasOne(perfil => perfil.Usuario)
            .WithOne(usuario => usuario.Perfil)
            .HasForeignKey<PerfilUsuario>(perfil => perfil.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
