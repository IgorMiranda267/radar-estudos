namespace AIStudyLab.Domain.Entities;

public class SessaoEstudo : AuditableEntity
{
    private SessaoEstudo()
    {
    }

    public SessaoEstudo(
        Guid usuarioId,
        Guid disciplinaId,
        DateOnly data,
        int tempoEstudadoMinutos,
        int questoes,
        int acertos,
        int erros,
        Guid? assuntoId = null,
        string? banca = null,
        string? observacoes = null)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuário é obrigatório.", nameof(usuarioId));
        }

        if (disciplinaId == Guid.Empty)
        {
            throw new ArgumentException("A disciplina é obrigatória.", nameof(disciplinaId));
        }

        if (assuntoId == Guid.Empty)
        {
            throw new ArgumentException("O assunto informado é inválido.", nameof(assuntoId));
        }

        if (tempoEstudadoMinutos < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tempoEstudadoMinutos), "O tempo estudado não pode ser negativo.");
        }

        if (questoes < 0 || acertos < 0 || erros < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(questoes), "As quantidades de questões, acertos e erros não podem ser negativas.");
        }

        if ((long)acertos + erros != questoes)
        {
            throw new ArgumentException("A quantidade de questões deve ser igual à soma de acertos e erros.");
        }

        UsuarioId = usuarioId;
        DisciplinaId = disciplinaId;
        AssuntoId = assuntoId;
        Data = data;
        TempoEstudadoMinutos = tempoEstudadoMinutos;
        Questoes = questoes;
        Acertos = acertos;
        Erros = erros;
        Banca = string.IsNullOrWhiteSpace(banca) ? null : banca.Trim();
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
    }

    public Guid UsuarioId { get; private set; }
    public Guid DisciplinaId { get; private set; }
    public Guid? AssuntoId { get; private set; }
    public DateOnly Data { get; private set; }
    public int TempoEstudadoMinutos { get; private set; }
    public int Questoes { get; private set; }
    public int Acertos { get; private set; }
    public int Erros { get; private set; }
    public string? Banca { get; private set; }
    public string? Observacoes { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Disciplina Disciplina { get; private set; } = null!;
    public Assunto? Assunto { get; private set; }
}
