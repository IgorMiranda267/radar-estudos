using AIStudyLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIStudyLab.Infrastructure.Persistence;

public sealed class StudyLabDbContext(DbContextOptions<StudyLabDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilUsuario> PerfisUsuario => Set<PerfilUsuario>();
    public DbSet<ConfiguracaoUsuario> ConfiguracoesUsuario => Set<ConfiguracaoUsuario>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<Assunto> Assuntos => Set<Assunto>();
    public DbSet<SessaoEstudo> SessoesEstudo => Set<SessaoEstudo>();
    public DbSet<Meta> Metas => Set<Meta>();
    public DbSet<Revisao> Revisoes => Set<Revisao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudyLabDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateAuditTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkUpdated();
            }
        }
    }
}
