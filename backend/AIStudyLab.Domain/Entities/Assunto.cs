using AIStudyLab.Domain.Enums;

namespace AIStudyLab.Domain.Entities;

public class Assunto : AuditableEntity
{
    private Assunto()
    {
    }

    public Assunto(Guid disciplinaId, string nome, string? descricao = null)
    {
        if (disciplinaId == Guid.Empty)
        {
            throw new ArgumentException("A disciplina é obrigatória.", nameof(disciplinaId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        DisciplinaId = disciplinaId;
        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public Guid DisciplinaId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public SubjectDifficulty? Dificuldade { get; private set; }
    public int Ordem { get; private set; } = 1;

    public Disciplina Disciplina { get; private set; } = null!;
    public ICollection<SessaoEstudo> SessoesEstudo { get; } = new List<SessaoEstudo>();
    public ICollection<Revisao> Revisoes { get; } = new List<Revisao>();

    public void Atualizar(string nome, string? descricao, SubjectDifficulty? dificuldade, int ordem)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        if (ordem < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(ordem), "A ordem deve ser maior que zero.");
        }

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Dificuldade = dificuldade;
        Ordem = ordem;
        MarkUpdated();
    }
}
