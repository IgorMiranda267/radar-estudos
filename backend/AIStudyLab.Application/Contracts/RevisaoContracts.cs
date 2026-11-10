using System.ComponentModel.DataAnnotations;
using AIStudyLab.Domain.Enums;

namespace AIStudyLab.Application.Contracts;

/// <summary>Dados para agendar uma revisão de assunto.</summary>
public sealed record CriarRevisaoRequest
{
    [Required]
    public required DateOnly DataAgendada { get; init; }

    [StringLength(4000)]
    public string? Observacoes { get; init; }
}

/// <summary>Dados para reagendar ou alterar o estado de uma revisão.</summary>
public sealed record AtualizarRevisaoRequest
{
    [Required]
    public required DateOnly DataAgendada { get; init; }

    [EnumDataType(typeof(ReviewStatus))]
    public ReviewStatus Status { get; init; }

    [StringLength(4000)]
    public string? Observacoes { get; init; }
}

/// <summary>Revisão agendada de um assunto.</summary>
public sealed record RevisaoResponse(
    Guid Id,
    Guid AssuntoId,
    DateOnly DataAgendada,
    DateTimeOffset? DataConclusao,
    ReviewStatus Status,
    string? Observacoes,
    DateTimeOffset CriadaEm,
    DateTimeOffset AtualizadaEm);
