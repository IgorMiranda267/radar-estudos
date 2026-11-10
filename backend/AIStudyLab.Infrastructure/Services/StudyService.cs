using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using AIStudyLab.Domain.Entities;
using AIStudyLab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIStudyLab.Infrastructure.Services;

public sealed class StudyService(
    StudyLabDbContext dbContext,
    IPasswordHasher<Usuario> passwordHasher) : IStudyService
{
    public async Task<UsuarioResponse> CriarUsuarioAsync(
        CriarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Usuarios.AnyAsync(usuario => usuario.Email == email, cancellationToken))
        {
            throw new InvalidOperationException("Já existe uma conta com este e-mail.");
        }

        var usuario = new Usuario(request.Nome, email, "pending");
        var senhaHash = passwordHasher.HashPassword(usuario, request.Senha);
        usuario = new Usuario(request.Nome, email, senhaHash);

        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(usuario);
    }

    public async Task<UsuarioResponse?> AutenticarUsuarioAsync(
        AutenticarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await dbContext.Usuarios
            .SingleOrDefaultAsync(item => item.Email == email, cancellationToken);

        if (usuario is null
            || usuario.Status != Domain.Enums.AccountStatus.Active
            || passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha)
                == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return ToResponse(usuario);
    }

    public async Task<PerfilUsuarioResponse> ObterPerfilAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        var perfil = await dbContext.PerfisUsuario
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.UsuarioId == usuarioId, cancellationToken);

        return perfil is null ? new PerfilUsuarioResponse(null, null, null, null, null, null) : ToResponse(perfil);
    }

    public async Task<PerfilUsuarioResponse> AtualizarPerfilAsync(
        Guid usuarioId,
        AtualizarPerfilUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        if (request.DataNascimento > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ArgumentException("A data de nascimento não pode estar no futuro.", nameof(request));
        }

        var perfil = await dbContext.PerfisUsuario
            .SingleOrDefaultAsync(item => item.UsuarioId == usuarioId, cancellationToken);

        if (perfil is null)
        {
            perfil = new PerfilUsuario(usuarioId);
            dbContext.PerfisUsuario.Add(perfil);
        }

        perfil.Atualizar(
            request.Objetivo,
            request.Nivel,
            request.Cidade,
            request.Instituicao,
            request.Bio,
            request.DataNascimento);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(perfil);
    }

    public async Task<UsuarioResponse?> ObterUsuarioAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        var usuario = await dbContext.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == usuarioId, cancellationToken);

        return usuario is null ? null : ToResponse(usuario);
    }

    public async Task<DashboardResponse> ObterDashboardAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);

        var inicioSemana = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-6);
        var sessoesRecentes = await dbContext.SessoesEstudo
            .AsNoTracking()
            .Where(sessao => sessao.UsuarioId == usuarioId && sessao.Data >= inicioSemana)
            .Select(sessao => new
            {
                sessao.Data,
                sessao.TempoEstudadoMinutos,
                sessao.Questoes,
                sessao.Acertos
            })
            .ToListAsync(cancellationToken);

        var totais = await dbContext.SessoesEstudo
            .Where(sessao => sessao.UsuarioId == usuarioId)
            .GroupBy(_ => 1)
            .Select(grupo => new
            {
                Questoes = grupo.Sum(sessao => sessao.Questoes),
                Acertos = grupo.Sum(sessao => sessao.Acertos),
                Erros = grupo.Sum(sessao => sessao.Erros),
                TempoEstudadoMinutos = grupo.Sum(sessao => sessao.TempoEstudadoMinutos),
                TotalSessoes = grupo.Count()
            })
            .SingleOrDefaultAsync(cancellationToken);

        var dias = Enumerable.Range(0, 7)
            .Select(offset => inicioSemana.AddDays(offset))
            .Select(data =>
            {
                var sessoesDoDia = sessoesRecentes.Where(sessao => sessao.Data == data);
                return new ResumoDiarioResponse(
                    data,
                    sessoesDoDia.Sum(sessao => sessao.TempoEstudadoMinutos),
                    sessoesDoDia.Sum(sessao => sessao.Questoes),
                    sessoesDoDia.Sum(sessao => sessao.Acertos));
            })
            .ToArray();

        return new DashboardResponse(
            await dbContext.Disciplinas.CountAsync(
                disciplina => disciplina.UsuarioId == usuarioId && disciplina.Ativa,
                cancellationToken),
            await dbContext.Assuntos.CountAsync(
                assunto => assunto.Disciplina.UsuarioId == usuarioId,
                cancellationToken),
            totais?.TotalSessoes ?? 0,
            totais?.Questoes ?? 0,
            totais?.Acertos ?? 0,
            totais?.Erros ?? 0,
            totais?.TempoEstudadoMinutos ?? 0,
            await dbContext.Revisoes.CountAsync(
                revisao => revisao.UsuarioId == usuarioId && revisao.Status == Domain.Enums.ReviewStatus.Pending,
                cancellationToken),
            dias);
    }

    public async Task<ConfiguracaoUsuarioResponse> ObterConfiguracaoAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        var configuracao = await dbContext.ConfiguracoesUsuario
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.UsuarioId == usuarioId, cancellationToken);

        return configuracao is null
            ? new ConfiguracaoUsuarioResponse("system", "pt-BR", true, 120, null)
            : ToResponse(configuracao);
    }

    public async Task<ConfiguracaoUsuarioResponse> AtualizarConfiguracaoAsync(
        Guid usuarioId,
        AtualizarConfiguracaoUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        if (request.DisciplinaFocoId is { } disciplinaId
            && !await dbContext.Disciplinas.AnyAsync(
                disciplina => disciplina.Id == disciplinaId
                    && disciplina.UsuarioId == usuarioId
                    && disciplina.Ativa,
                cancellationToken))
        {
            throw new KeyNotFoundException("A disciplina de foco não pertence ao usuário ou está inativa.");
        }

        var configuracao = await dbContext.ConfiguracoesUsuario
            .SingleOrDefaultAsync(item => item.UsuarioId == usuarioId, cancellationToken);

        if (configuracao is null)
        {
            configuracao = new ConfiguracaoUsuario(usuarioId);
            dbContext.ConfiguracoesUsuario.Add(configuracao);
        }

        configuracao.Atualizar(
            request.Tema,
            request.Idioma,
            request.NotificacoesAtivadas,
            request.ObjetivoDiarioMinutos,
            request.DisciplinaFocoId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(configuracao);
    }

    public async Task<IReadOnlyList<DisciplinaResponse>> ListarDisciplinasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        var disciplinas = await dbContext.Disciplinas
            .AsNoTracking()
            .Where(disciplina => disciplina.UsuarioId == usuarioId)
            .OrderBy(disciplina => disciplina.Nome)
            .ToListAsync(cancellationToken);

        return disciplinas.Select(ToResponse).ToArray();
    }

    public async Task<DisciplinaResponse?> ObterDisciplinaAsync(
        Guid usuarioId,
        Guid disciplinaId,
        CancellationToken cancellationToken)
    {
        var disciplina = await dbContext.Disciplinas
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.Id == disciplinaId && item.UsuarioId == usuarioId,
                cancellationToken);

        return disciplina is null ? null : ToResponse(disciplina);
    }

    public async Task<DisciplinaResponse> CriarDisciplinaAsync(
        Guid usuarioId,
        CriarDisciplinaRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        var disciplina = new Disciplina(usuarioId, request.Nome, request.Cor, request.Icone);
        dbContext.Disciplinas.Add(disciplina);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(disciplina);
    }

    public async Task<DisciplinaResponse?> AtualizarDisciplinaAsync(
        Guid usuarioId,
        Guid disciplinaId,
        AtualizarDisciplinaRequest request,
        CancellationToken cancellationToken)
    {
        var disciplina = await dbContext.Disciplinas.SingleOrDefaultAsync(
            item => item.Id == disciplinaId && item.UsuarioId == usuarioId,
            cancellationToken);

        if (disciplina is null)
        {
            return null;
        }

        disciplina.Atualizar(request.Nome, request.Cor, request.Icone);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(disciplina);
    }

    public async Task<bool> DesativarDisciplinaAsync(
        Guid usuarioId,
        Guid disciplinaId,
        CancellationToken cancellationToken)
    {
        var disciplina = await dbContext.Disciplinas.SingleOrDefaultAsync(
            item => item.Id == disciplinaId && item.UsuarioId == usuarioId,
            cancellationToken);

        if (disciplina is null)
        {
            return false;
        }

        disciplina.Desativar();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<AssuntoResponse>> ListarAssuntosAsync(
        Guid usuarioId,
        Guid disciplinaId,
        CancellationToken cancellationToken)
    {
        await EnsureDisciplineExistsAsync(usuarioId, disciplinaId, cancellationToken);
        var assuntos = await dbContext.Assuntos
            .AsNoTracking()
            .Where(assunto => assunto.DisciplinaId == disciplinaId)
            .OrderBy(assunto => assunto.Ordem)
            .ThenBy(assunto => assunto.Nome)
            .ToListAsync(cancellationToken);

        return assuntos.Select(ToResponse).ToArray();
    }

    public async Task<AssuntoResponse?> ObterAssuntoAsync(
        Guid usuarioId,
        Guid disciplinaId,
        Guid assuntoId,
        CancellationToken cancellationToken)
    {
        var assunto = await dbContext.Assuntos
            .AsNoTracking()
            .Where(item => item.Id == assuntoId && item.DisciplinaId == disciplinaId)
            .Where(item => item.Disciplina.UsuarioId == usuarioId)
            .SingleOrDefaultAsync(cancellationToken);

        return assunto is null ? null : ToResponse(assunto);
    }

    public async Task<AssuntoResponse> CriarAssuntoAsync(
        Guid usuarioId,
        Guid disciplinaId,
        CriarAssuntoRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureActiveDisciplineExistsAsync(usuarioId, disciplinaId, cancellationToken);
        var assunto = new Assunto(disciplinaId, request.Nome, request.Descricao);
        assunto.Atualizar(request.Nome, request.Descricao, request.Dificuldade, request.Ordem);
        dbContext.Assuntos.Add(assunto);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(assunto);
    }

    public async Task<AssuntoResponse?> AtualizarAssuntoAsync(
        Guid usuarioId,
        Guid disciplinaId,
        Guid assuntoId,
        AtualizarAssuntoRequest request,
        CancellationToken cancellationToken)
    {
        var assunto = await dbContext.Assuntos
            .Where(item => item.Id == assuntoId && item.DisciplinaId == disciplinaId)
            .Where(item => item.Disciplina.UsuarioId == usuarioId)
            .SingleOrDefaultAsync(cancellationToken);

        if (assunto is null)
        {
            return null;
        }

        assunto.Atualizar(request.Nome, request.Descricao, request.Dificuldade, request.Ordem);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(assunto);
    }

    public async Task<bool> ExcluirAssuntoAsync(
        Guid usuarioId,
        Guid disciplinaId,
        Guid assuntoId,
        CancellationToken cancellationToken)
    {
        var assunto = await dbContext.Assuntos
            .Where(item => item.Id == assuntoId && item.DisciplinaId == disciplinaId)
            .Where(item => item.Disciplina.UsuarioId == usuarioId)
            .SingleOrDefaultAsync(cancellationToken);

        if (assunto is null)
        {
            return false;
        }

        if (await dbContext.SessoesEstudo.AnyAsync(item => item.AssuntoId == assuntoId, cancellationToken)
            || await dbContext.Revisoes.AnyAsync(item => item.AssuntoId == assuntoId, cancellationToken))
        {
            throw new InvalidOperationException("O assunto possui sessões ou revisões vinculadas e não pode ser excluído.");
        }

        dbContext.Assuntos.Remove(assunto);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<SessaoEstudoResponse>> ListarSessoesAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        var sessoes = await dbContext.SessoesEstudo
            .AsNoTracking()
            .Where(sessao => sessao.UsuarioId == usuarioId)
            .OrderByDescending(sessao => sessao.Data)
            .ThenByDescending(sessao => sessao.CreatedAt)
            .ToListAsync(cancellationToken);

        return sessoes.Select(ToResponse).ToArray();
    }

    public async Task<SessaoEstudoResponse?> ObterSessaoAsync(
        Guid usuarioId,
        Guid sessaoId,
        CancellationToken cancellationToken)
    {
        var sessao = await dbContext.SessoesEstudo
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.Id == sessaoId && item.UsuarioId == usuarioId,
                cancellationToken);

        return sessao is null ? null : ToResponse(sessao);
    }

    public async Task<SessaoEstudoResponse> CriarSessaoAsync(
        Guid usuarioId,
        Guid disciplinaId,
        CriarSessaoEstudoRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureActiveDisciplineExistsAsync(usuarioId, disciplinaId, cancellationToken);

        if (request.AssuntoId is { } assuntoId
            && !await dbContext.Assuntos.AnyAsync(
                assunto => assunto.Id == assuntoId
                    && assunto.DisciplinaId == disciplinaId
                    && assunto.Disciplina.UsuarioId == usuarioId,
                cancellationToken))
        {
            throw new KeyNotFoundException("O assunto não pertence à disciplina informada.");
        }

        var sessao = new SessaoEstudo(
            usuarioId,
            disciplinaId,
            request.Data,
            request.TempoEstudadoMinutos,
            request.Questoes,
            request.Acertos,
            request.Erros,
            request.AssuntoId,
            request.Banca,
            request.Observacoes);

        dbContext.SessoesEstudo.Add(sessao);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(sessao);
    }

    public async Task<IReadOnlyList<MetaResponse>> ListarMetasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        var metas = await dbContext.Metas
            .AsNoTracking()
            .Where(meta => meta.UsuarioId == usuarioId)
            .OrderByDescending(meta => meta.Ativa)
            .ThenBy(meta => meta.DataFim)
            .ToListAsync(cancellationToken);

        return metas.Select(ToResponse).ToArray();
    }

    public async Task<MetaResponse> CriarMetaAsync(
        Guid usuarioId,
        CriarMetaRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        var meta = new Meta(
            usuarioId,
            request.Tipo,
            request.Nome,
            request.Quantidade,
            request.Unidade,
            request.DataInicio,
            request.DataFim);

        dbContext.Metas.Add(meta);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(meta);
    }

    public async Task<MetaResponse?> AtualizarMetaAsync(
        Guid usuarioId,
        Guid metaId,
        AtualizarMetaRequest request,
        CancellationToken cancellationToken)
    {
        var meta = await dbContext.Metas.SingleOrDefaultAsync(
            item => item.Id == metaId && item.UsuarioId == usuarioId,
            cancellationToken);

        if (meta is null)
        {
            return null;
        }

        meta.Atualizar(
            request.Tipo,
            request.Nome,
            request.Quantidade,
            request.Unidade,
            request.DataInicio,
            request.DataFim,
            request.Ativa);

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(meta);
    }

    public async Task<bool> ExcluirMetaAsync(
        Guid usuarioId,
        Guid metaId,
        CancellationToken cancellationToken)
    {
        var meta = await dbContext.Metas.SingleOrDefaultAsync(
            item => item.Id == metaId && item.UsuarioId == usuarioId,
            cancellationToken);

        if (meta is null)
        {
            return false;
        }

        dbContext.Metas.Remove(meta);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<RevisaoResponse>> ListarRevisoesAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        var revisoes = await dbContext.Revisoes
            .AsNoTracking()
            .Where(revisao => revisao.UsuarioId == usuarioId)
            .OrderBy(revisao => revisao.DataAgendada)
            .ThenBy(revisao => revisao.CreatedAt)
            .ToListAsync(cancellationToken);

        return revisoes.Select(ToResponse).ToArray();
    }

    public async Task<RevisaoResponse> CriarRevisaoAsync(
        Guid usuarioId,
        Guid assuntoId,
        CriarRevisaoRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(usuarioId, cancellationToken);
        if (!await dbContext.Assuntos.AnyAsync(
                assunto => assunto.Id == assuntoId && assunto.Disciplina.UsuarioId == usuarioId,
                cancellationToken))
        {
            throw new KeyNotFoundException("O assunto não foi encontrado para este usuário.");
        }

        var revisao = new Revisao(usuarioId, assuntoId, request.DataAgendada);
        dbContext.Revisoes.Add(revisao);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(revisao);
    }

    public async Task<RevisaoResponse?> AtualizarRevisaoAsync(
        Guid usuarioId,
        Guid revisaoId,
        AtualizarRevisaoRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Status))
        {
            throw new ArgumentException("O status informado é inválido.", nameof(request));
        }

        var revisao = await dbContext.Revisoes.SingleOrDefaultAsync(
            item => item.Id == revisaoId && item.UsuarioId == usuarioId,
            cancellationToken);

        if (revisao is null)
        {
            return null;
        }

        revisao.Atualizar(request.DataAgendada, request.Status, request.Observacoes);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(revisao);
    }

    public async Task<bool> ExcluirRevisaoAsync(
        Guid usuarioId,
        Guid revisaoId,
        CancellationToken cancellationToken)
    {
        var revisao = await dbContext.Revisoes.SingleOrDefaultAsync(
            item => item.Id == revisaoId && item.UsuarioId == usuarioId,
            cancellationToken);

        if (revisao is null)
        {
            return false;
        }

        dbContext.Revisoes.Remove(revisao);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureUserExistsAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        if (!await dbContext.Usuarios.AnyAsync(usuario => usuario.Id == usuarioId, cancellationToken))
        {
            throw new KeyNotFoundException("O usuário não foi encontrado.");
        }
    }

    private async Task EnsureDisciplineExistsAsync(
        Guid usuarioId,
        Guid disciplinaId,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Disciplinas.AnyAsync(
                disciplina => disciplina.Id == disciplinaId && disciplina.UsuarioId == usuarioId,
                cancellationToken))
        {
            throw new KeyNotFoundException("A disciplina não foi encontrada para este usuário.");
        }
    }

    private async Task EnsureActiveDisciplineExistsAsync(
        Guid usuarioId,
        Guid disciplinaId,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Disciplinas.AnyAsync(
                disciplina => disciplina.Id == disciplinaId
                    && disciplina.UsuarioId == usuarioId
                    && disciplina.Ativa,
                cancellationToken))
        {
            throw new KeyNotFoundException("A disciplina ativa não foi encontrada para este usuário.");
        }
    }

    private static UsuarioResponse ToResponse(Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.Email, usuario.CreatedAt);

    private static DisciplinaResponse ToResponse(Disciplina disciplina) =>
        new(
            disciplina.Id,
            disciplina.Nome,
            disciplina.Cor,
            disciplina.Icone,
            disciplina.Ativa,
            disciplina.CreatedAt,
            disciplina.UpdatedAt);

    private static AssuntoResponse ToResponse(Assunto assunto) =>
        new(
            assunto.Id,
            assunto.DisciplinaId,
            assunto.Nome,
            assunto.Descricao,
            assunto.Dificuldade,
            assunto.Ordem,
            assunto.CreatedAt,
            assunto.UpdatedAt);

    private static SessaoEstudoResponse ToResponse(SessaoEstudo sessao) =>
        new(
            sessao.Id,
            sessao.DisciplinaId,
            sessao.AssuntoId,
            sessao.Data,
            sessao.TempoEstudadoMinutos,
            sessao.Questoes,
            sessao.Acertos,
            sessao.Erros,
            sessao.Banca,
            sessao.Observacoes,
            sessao.CreatedAt);

    private static MetaResponse ToResponse(Meta meta) =>
        new(
            meta.Id,
            meta.Tipo,
            meta.Nome,
            meta.Quantidade,
            meta.Unidade,
            meta.DataInicio,
            meta.DataFim,
            meta.Ativa,
            meta.CreatedAt,
            meta.UpdatedAt);

    private static RevisaoResponse ToResponse(Revisao revisao) =>
        new(
            revisao.Id,
            revisao.AssuntoId,
            revisao.DataAgendada,
            revisao.DataConclusao,
            revisao.Status,
            revisao.Observacoes,
            revisao.CreatedAt,
            revisao.UpdatedAt);

    private static ConfiguracaoUsuarioResponse ToResponse(ConfiguracaoUsuario configuracao) =>
        new(
            configuracao.Tema,
            configuracao.Idioma,
            configuracao.NotificacoesAtivadas,
            configuracao.ObjetivoDiarioMinutos,
            configuracao.DisciplinaFocoId);

    private static PerfilUsuarioResponse ToResponse(PerfilUsuario perfil) =>
        new(
            perfil.Objetivo,
            perfil.Nivel,
            perfil.Cidade,
            perfil.Instituicao,
            perfil.Bio,
            perfil.DataNascimento);
}
