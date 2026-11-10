using AIStudyLab.Application.Contracts;

namespace AIStudyLab.Application.Services;

public interface IStudyService
{
    Task<UsuarioResponse> CriarUsuarioAsync(CriarUsuarioRequest request, CancellationToken cancellationToken);
    Task<UsuarioResponse?> AutenticarUsuarioAsync(
        AutenticarUsuarioRequest request,
        CancellationToken cancellationToken);
    Task<UsuarioResponse?> ObterUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<PerfilUsuarioResponse> ObterPerfilAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<PerfilUsuarioResponse> AtualizarPerfilAsync(
        Guid usuarioId,
        AtualizarPerfilUsuarioRequest request,
        CancellationToken cancellationToken);
    Task<DashboardResponse> ObterDashboardAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<ConfiguracaoUsuarioResponse> ObterConfiguracaoAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<ConfiguracaoUsuarioResponse> AtualizarConfiguracaoAsync(
        Guid usuarioId,
        AtualizarConfiguracaoUsuarioRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DisciplinaResponse>> ListarDisciplinasAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<DisciplinaResponse?> ObterDisciplinaAsync(Guid usuarioId, Guid disciplinaId, CancellationToken cancellationToken);
    Task<DisciplinaResponse> CriarDisciplinaAsync(Guid usuarioId, CriarDisciplinaRequest request, CancellationToken cancellationToken);
    Task<DisciplinaResponse?> AtualizarDisciplinaAsync(Guid usuarioId, Guid disciplinaId, AtualizarDisciplinaRequest request, CancellationToken cancellationToken);
    Task<bool> DesativarDisciplinaAsync(Guid usuarioId, Guid disciplinaId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AssuntoResponse>> ListarAssuntosAsync(Guid usuarioId, Guid disciplinaId, CancellationToken cancellationToken);
    Task<AssuntoResponse?> ObterAssuntoAsync(Guid usuarioId, Guid disciplinaId, Guid assuntoId, CancellationToken cancellationToken);
    Task<AssuntoResponse> CriarAssuntoAsync(Guid usuarioId, Guid disciplinaId, CriarAssuntoRequest request, CancellationToken cancellationToken);
    Task<AssuntoResponse?> AtualizarAssuntoAsync(Guid usuarioId, Guid disciplinaId, Guid assuntoId, AtualizarAssuntoRequest request, CancellationToken cancellationToken);
    Task<bool> ExcluirAssuntoAsync(Guid usuarioId, Guid disciplinaId, Guid assuntoId, CancellationToken cancellationToken);

    Task<IReadOnlyList<SessaoEstudoResponse>> ListarSessoesAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<SessaoEstudoResponse?> ObterSessaoAsync(Guid usuarioId, Guid sessaoId, CancellationToken cancellationToken);
    Task<SessaoEstudoResponse> CriarSessaoAsync(Guid usuarioId, Guid disciplinaId, CriarSessaoEstudoRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<MetaResponse>> ListarMetasAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<MetaResponse> CriarMetaAsync(Guid usuarioId, CriarMetaRequest request, CancellationToken cancellationToken);
    Task<MetaResponse?> AtualizarMetaAsync(Guid usuarioId, Guid metaId, AtualizarMetaRequest request, CancellationToken cancellationToken);
    Task<bool> ExcluirMetaAsync(Guid usuarioId, Guid metaId, CancellationToken cancellationToken);

    Task<IReadOnlyList<RevisaoResponse>> ListarRevisoesAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<RevisaoResponse> CriarRevisaoAsync(Guid usuarioId, Guid assuntoId, CriarRevisaoRequest request, CancellationToken cancellationToken);
    Task<RevisaoResponse?> AtualizarRevisaoAsync(Guid usuarioId, Guid revisaoId, AtualizarRevisaoRequest request, CancellationToken cancellationToken);
    Task<bool> ExcluirRevisaoAsync(Guid usuarioId, Guid revisaoId, CancellationToken cancellationToken);
}
