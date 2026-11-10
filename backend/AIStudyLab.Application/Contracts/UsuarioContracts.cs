using System.ComponentModel.DataAnnotations;

namespace AIStudyLab.Application.Contracts;

/// <summary>Dados para criar uma conta de desenvolvimento.</summary>
public sealed record CriarUsuarioRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string Nome { get; init; }

    [Required, EmailAddress, StringLength(255)]
    public required string Email { get; init; }

    [Required, MinLength(12), StringLength(256)]
    public required string Senha { get; init; }
}

/// <summary>Credenciais de acesso de uma conta.</summary>
public sealed record AutenticarUsuarioRequest
{
    [Required, EmailAddress, StringLength(255)]
    public required string Email { get; init; }

    [Required, StringLength(256)]
    public required string Senha { get; init; }
}

/// <summary>Dados públicos de um usuário, sem credenciais.</summary>
public sealed record UsuarioResponse(Guid Id, string Nome, string Email, DateTimeOffset CriadoEm);
