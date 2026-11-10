using System.ComponentModel.DataAnnotations;

namespace AIStudyLab.Application.Contracts;

/// <summary>Dados para cadastrar uma disciplina.</summary>
public sealed record CriarDisciplinaRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string Nome { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public required string Cor { get; init; }

    [Required, StringLength(50, MinimumLength = 1)]
    public required string Icone { get; init; }
}

/// <summary>Dados para atualizar uma disciplina.</summary>
public sealed record AtualizarDisciplinaRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string Nome { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public required string Cor { get; init; }

    [Required, StringLength(50, MinimumLength = 1)]
    public required string Icone { get; init; }
}

/// <summary>Disciplina cadastrada para um usuário.</summary>
public sealed record DisciplinaResponse(
    Guid Id,
    string Nome,
    string Cor,
    string Icone,
    bool Ativa,
    DateTimeOffset CriadaEm,
    DateTimeOffset AtualizadaEm);
