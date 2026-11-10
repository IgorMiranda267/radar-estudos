using System.ComponentModel.DataAnnotations;

namespace AIStudyLab.Application.Contracts;

/// <summary>Dados para registrar uma sessão de estudo.</summary>
public sealed record CriarSessaoEstudoRequest
{
    [Required]
    public required DateOnly Data { get; init; }

    [Range(0, int.MaxValue)]
    public int TempoEstudadoMinutos { get; init; }

    [Range(0, int.MaxValue)]
    public int Questoes { get; init; }

    [Range(0, int.MaxValue)]
    public int Acertos { get; init; }

    [Range(0, int.MaxValue)]
    public int Erros { get; init; }

    public Guid? AssuntoId { get; init; }

    [StringLength(100)]
    public string? Banca { get; init; }

    [StringLength(4000)]
    public string? Observacoes { get; init; }
}

/// <summary>Sessão registrada por um usuário.</summary>
public sealed record SessaoEstudoResponse(
    Guid Id,
    Guid DisciplinaId,
    Guid? AssuntoId,
    DateOnly Data,
    int TempoEstudadoMinutos,
    int Questoes,
    int Acertos,
    int Erros,
    string? Banca,
    string? Observacoes,
    DateTimeOffset CriadaEm);
