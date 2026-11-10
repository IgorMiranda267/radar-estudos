using System.ComponentModel.DataAnnotations;

namespace AIStudyLab.Application.Contracts;

/// <summary>Dados complementares do perfil do usuário.</summary>
public sealed record PerfilUsuarioResponse(
    string? Objetivo,
    string? Nivel,
    string? Cidade,
    string? Instituicao,
    string? Bio,
    DateOnly? DataNascimento);

/// <summary>Dados para atualizar o perfil do usuário.</summary>
public sealed record AtualizarPerfilUsuarioRequest
{
    [StringLength(300)]
    public string? Objetivo { get; init; }

    [StringLength(50)]
    public string? Nivel { get; init; }

    [StringLength(120)]
    public string? Cidade { get; init; }

    [StringLength(200)]
    public string? Instituicao { get; init; }

    [StringLength(2000)]
    public string? Bio { get; init; }

    public DateOnly? DataNascimento { get; init; }
}
