using System.ComponentModel.DataAnnotations;
using AIStudyLab.Domain.Enums;

namespace AIStudyLab.Application.Contracts;

/// <summary>Dados para cadastrar um assunto em uma disciplina.</summary>
public sealed record CriarAssuntoRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string Nome { get; init; }

    [StringLength(4000)]
    public string? Descricao { get; init; }

    [EnumDataType(typeof(SubjectDifficulty))]
    public SubjectDifficulty? Dificuldade { get; init; }

    [Range(1, int.MaxValue)]
    public int Ordem { get; init; } = 1;
}

/// <summary>Dados para atualizar um assunto.</summary>
public sealed record AtualizarAssuntoRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string Nome { get; init; }

    [StringLength(4000)]
    public string? Descricao { get; init; }

    [EnumDataType(typeof(SubjectDifficulty))]
    public SubjectDifficulty? Dificuldade { get; init; }

    [Range(1, int.MaxValue)]
    public int Ordem { get; init; } = 1;
}

/// <summary>Assunto pertencente a uma disciplina.</summary>
public sealed record AssuntoResponse(
    Guid Id,
    Guid DisciplinaId,
    string Nome,
    string? Descricao,
    SubjectDifficulty? Dificuldade,
    int Ordem,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm);
