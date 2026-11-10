namespace AIStudyLab.Domain.Entities;

public class Disciplina : AuditableEntity
{
    private Disciplina()
    {
    }

    public Disciplina(Guid usuarioId, string nome, string cor, string icone)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuário é obrigatório.", nameof(usuarioId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(cor);
        ArgumentException.ThrowIfNullOrWhiteSpace(icone);

        UsuarioId = usuarioId;
        Nome = nome.Trim();
        Cor = cor.Trim();
        Icone = icone.Trim();
        Ativa = true;
    }

    public Guid UsuarioId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Cor { get; private set; } = string.Empty;
    public string Icone { get; private set; } = string.Empty;
    public bool Ativa { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public ICollection<Assunto> Assuntos { get; } = new List<Assunto>();
    public ICollection<SessaoEstudo> SessoesEstudo { get; } = new List<SessaoEstudo>();

    public void Atualizar(string nome, string cor, string icone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(cor);
        ArgumentException.ThrowIfNullOrWhiteSpace(icone);

        Nome = nome.Trim();
        Cor = cor.Trim();
        Icone = icone.Trim();
        MarkUpdated();
    }

    public void Desativar()
    {
        Ativa = false;
        MarkUpdated();
    }
}
