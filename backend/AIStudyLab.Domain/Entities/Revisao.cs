using AIStudyLab.Domain.Enums;

namespace AIStudyLab.Domain.Entities;

public class Revisao : AuditableEntity
{
    private Revisao()
    {
    }

    public Revisao(Guid usuarioId, Guid assuntoId, DateOnly dataAgendada)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuário é obrigatório.", nameof(usuarioId));
        }

        if (assuntoId == Guid.Empty)
        {
            throw new ArgumentException("O assunto é obrigatório.", nameof(assuntoId));
        }

        UsuarioId = usuarioId;
        AssuntoId = assuntoId;
        DataAgendada = dataAgendada;
        Status = ReviewStatus.Pending;
    }

    public Guid UsuarioId { get; private set; }
    public Guid AssuntoId { get; private set; }
    public DateOnly DataAgendada { get; private set; }
    public DateTimeOffset? DataConclusao { get; private set; }
    public ReviewStatus Status { get; private set; }
    public string? Observacoes { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Assunto Assunto { get; private set; } = null!;

    public void Atualizar(DateOnly dataAgendada, ReviewStatus status, string? observacoes)
    {
        if (status == ReviewStatus.Completed)
        {
            DataConclusao ??= DateTimeOffset.UtcNow;
        }
        else
        {
            DataConclusao = null;
        }

        DataAgendada = dataAgendada;
        Status = status;
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
        MarkUpdated();
    }
}
