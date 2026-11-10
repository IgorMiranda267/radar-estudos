using System.ComponentModel.DataAnnotations;

namespace AIStudyLab.Application.Contracts;

/// <summary>Preferências de estudo e interface do usuário.</summary>
public sealed record ConfiguracaoUsuarioResponse(
    string Tema,
    string Idioma,
    bool NotificacoesAtivadas,
    int ObjetivoDiarioMinutos,
    Guid? DisciplinaFocoId);

/// <summary>Dados para salvar as preferências de usuário.</summary>
public sealed record AtualizarConfiguracaoUsuarioRequest
{
    [Required, StringLength(30, MinimumLength = 1)]
    public required string Tema { get; init; }

    [Required, StringLength(20, MinimumLength = 2)]
    public required string Idioma { get; init; }

    public bool NotificacoesAtivadas { get; init; }

    [Range(0, 1440)]
    public int ObjetivoDiarioMinutos { get; init; }

    public Guid? DisciplinaFocoId { get; init; }
}
