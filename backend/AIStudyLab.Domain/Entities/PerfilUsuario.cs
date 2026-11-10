namespace AIStudyLab.Domain.Entities;

public class PerfilUsuario : AuditableEntity
{
    private PerfilUsuario()
    {
    }

    public PerfilUsuario(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuário é obrigatório.", nameof(usuarioId));
        }

        UsuarioId = usuarioId;
    }

    public Guid UsuarioId { get; private set; }
    public string? Objetivo { get; private set; }
    public string? Nivel { get; private set; }
    public string? Cidade { get; private set; }
    public string? Instituicao { get; private set; }
    public string? Bio { get; private set; }
    public DateOnly? DataNascimento { get; private set; }

    public Usuario Usuario { get; private set; } = null!;

    public void Atualizar(
        string? objetivo,
        string? nivel,
        string? cidade,
        string? instituicao,
        string? bio,
        DateOnly? dataNascimento)
    {
        Objetivo = NormalizarOpcional(objetivo);
        Nivel = NormalizarOpcional(nivel);
        Cidade = NormalizarOpcional(cidade);
        Instituicao = NormalizarOpcional(instituicao);
        Bio = NormalizarOpcional(bio);
        DataNascimento = dataNascimento;
    }

    private static string? NormalizarOpcional(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
