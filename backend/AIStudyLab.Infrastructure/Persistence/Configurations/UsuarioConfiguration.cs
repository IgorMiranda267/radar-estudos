using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIStudyLab.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(usuario => usuario.Id);

        builder.Property(usuario => usuario.Id).HasColumnName("id");
        builder.Property(usuario => usuario.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
        builder.Property(usuario => usuario.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(usuario => usuario.SenhaHash).HasColumnName("senha_hash").IsRequired();
        builder.Property(usuario => usuario.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(2048);
        builder.Property(usuario => usuario.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(usuario => usuario.UltimoLoginEm).HasColumnName("ultimo_login_em");
        builder.Property(usuario => usuario.CreatedAt).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(usuario => usuario.UpdatedAt).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(usuario => usuario.Email).IsUnique().HasDatabaseName("ux_usuarios_email");
    }
}
