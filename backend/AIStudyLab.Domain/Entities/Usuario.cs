using AIStudyLab.Domain.Enums;

namespace AIStudyLab.Domain.Entities;

public class Usuario : AuditableEntity
{
    private Usuario()
    {
    }

    public Usuario(string nome, string email, string senhaHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(senhaHash);

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        SenhaHash = senhaHash;
        Status = AccountStatus.Active;
    }

    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTimeOffset? UltimoLoginEm { get; private set; }

    public PerfilUsuario? Perfil { get; private set; }
    public ConfiguracaoUsuario? Configuracoes { get; private set; }
    public ICollection<Disciplina> Disciplinas { get; } = new List<Disciplina>();
    public ICollection<SessaoEstudo> SessoesEstudo { get; } = new List<SessaoEstudo>();
    public ICollection<Meta> Metas { get; } = new List<Meta>();
    public ICollection<Revisao> Revisoes { get; } = new List<Revisao>();
}
