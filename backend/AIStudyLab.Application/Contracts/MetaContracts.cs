using System.ComponentModel.DataAnnotations;
using AIStudyLab.Domain.Enums;

namespace AIStudyLab.Application.Contracts;

/// <summary>Dados para criar uma meta de estudo.</summary>
public sealed record CriarMetaRequest
{
    [EnumDataType(typeof(GoalType))]
    public GoalType Tipo { get; init; }

    [Required, StringLength(150, MinimumLength = 1)]
    public required string Nome { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantidade { get; init; }

    [Required, StringLength(50, MinimumLength = 1)]
    public required string Unidade { get; init; }

    [Required]
    public required DateOnly DataInicio { get; init; }

    [Required]
    public required DateOnly DataFim { get; init; }
}

/// <summary>Dados para atualizar uma meta.</summary>
public sealed record AtualizarMetaRequest
{
    [EnumDataType(typeof(GoalType))]
    public GoalType Tipo { get; init; }

    [Required, StringLength(150, MinimumLength = 1)]
    public required string Nome { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantidade { get; init; }

    [Required, StringLength(50, MinimumLength = 1)]
    public required string Unidade { get; init; }

    [Required]
    public required DateOnly DataInicio { get; init; }

    [Required]
    public required DateOnly DataFim { get; init; }

    public bool Ativa { get; init; }
}

/// <summary>Meta definida por um usuário.</summary>
public sealed record MetaResponse(
    Guid Id,
    GoalType Tipo,
    string Nome,
    int Quantidade,
    string Unidade,
    DateOnly DataInicio,
    DateOnly DataFim,
    bool Ativa,
    DateTimeOffset CriadaEm,
    DateTimeOffset AtualizadaEm);
