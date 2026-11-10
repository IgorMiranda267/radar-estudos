namespace AIStudyLab.Domain.Entities;

public class ConfiguracaoUsuario : AuditableEntity
{
    private ConfiguracaoUsuario()
    {
    }

    public ConfiguracaoUsuario(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuário é obrigatório.", nameof(usuarioId));
        }

        UsuarioId = usuarioId;
        Tema = "system";
        Idioma = "pt-BR";
        NotificacoesAtivadas = true;
        ObjetivoDiarioMinutos = 120;
    }

    public Guid UsuarioId { get; private set; }
    public string Tema { get; private set; } = string.Empty;
    public string Idioma { get; private set; } = string.Empty;
    public bool NotificacoesAtivadas { get; private set; }
    public int ObjetivoDiarioMinutos { get; private set; }
    public Guid? DisciplinaFocoId { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Disciplina? DisciplinaFoco { get; private set; }

    public void Atualizar(
        string tema,
        string idioma,
        bool notificacoesAtivadas,
        int objetivoDiarioMinutos,
        Guid? disciplinaFocoId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tema);
        ArgumentException.ThrowIfNullOrWhiteSpace(idioma);

        if (objetivoDiarioMinutos < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(objetivoDiarioMinutos),
                "O objetivo diário não pode ser negativo.");
        }

        Tema = tema.Trim();
        Idioma = idioma.Trim();
        NotificacoesAtivadas = notificacoesAtivadas;
        ObjetivoDiarioMinutos = objetivoDiarioMinutos;
        DisciplinaFocoId = disciplinaFocoId;
        MarkUpdated();
    }
}
