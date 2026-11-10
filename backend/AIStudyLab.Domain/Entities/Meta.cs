using AIStudyLab.Domain.Enums;

namespace AIStudyLab.Domain.Entities;

public class Meta : AuditableEntity
{
    private Meta()
    {
    }

    public Meta(
        Guid usuarioId,
        GoalType tipo,
        string nome,
        int quantidade,
        string unidade,
        DateOnly dataInicio,
        DateOnly dataFim)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuário é obrigatório.", nameof(usuarioId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(unidade);

        if (quantidade <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade da meta deve ser maior que zero.");
        }

        if (dataFim < dataInicio)
        {
            throw new ArgumentException("A data final não pode anteceder a data inicial.", nameof(dataFim));
        }

        UsuarioId = usuarioId;
        Tipo = tipo;
        Nome = nome.Trim();
        Quantidade = quantidade;
        Unidade = unidade.Trim();
        DataInicio = dataInicio;
        DataFim = dataFim;
        Ativa = true;
    }

    public Guid UsuarioId { get; private set; }
    public GoalType Tipo { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public string Unidade { get; private set; } = string.Empty;
    public DateOnly DataInicio { get; private set; }
    public DateOnly DataFim { get; private set; }
    public bool Ativa { get; private set; }

    public Usuario Usuario { get; private set; } = null!;

    public void Atualizar(
        GoalType tipo,
        string nome,
        int quantidade,
        string unidade,
        DateOnly dataInicio,
        DateOnly dataFim,
        bool ativa)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(unidade);

        if (quantidade <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade da meta deve ser maior que zero.");
        }

        if (dataFim < dataInicio)
        {
            throw new ArgumentException("A data final não pode anteceder a data inicial.", nameof(dataFim));
        }

        Tipo = tipo;
        Nome = nome.Trim();
        Quantidade = quantidade;
        Unidade = unidade.Trim();
        DataInicio = dataInicio;
        DataFim = dataFim;
        Ativa = ativa;
        MarkUpdated();
    }
}
