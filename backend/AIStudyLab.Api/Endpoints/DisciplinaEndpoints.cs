using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIStudyLab.Api.Endpoints;

public static class DisciplinaEndpoints
{
    public static RouteGroupBuilder MapDisciplinas(this RouteGroupBuilder usuarioGroup)
    {
        var group = usuarioGroup.MapGroup("/disciplinas").WithTags("Disciplinas");

        group.MapGet("/", async Task<Ok<IReadOnlyList<DisciplinaResponse>>> (
                ClaimsPrincipal principal,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.ListarDisciplinasAsync(principal.GetRequiredUserId(), cancellationToken)))
            .WithName("ListarDisciplinas")
            .WithSummary("Lista as disciplinas do usuário")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async Task<Created<DisciplinaResponse>> (
                ClaimsPrincipal principal,
                CriarDisciplinaRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var usuarioId = principal.GetRequiredUserId();
                var disciplina = await service.CriarDisciplinaAsync(usuarioId, request, cancellationToken);
                return TypedResults.Created(
                    $"/api/me/disciplinas/{disciplina.Id}",
                    disciplina);
            })
            .WithName("CriarDisciplina")
            .WithSummary("Cadastra uma disciplina")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{disciplinaId:guid}", async Task<Results<Ok<DisciplinaResponse>, NotFound>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var disciplina = await service.ObterDisciplinaAsync(principal.GetRequiredUserId(), disciplinaId, cancellationToken);
                return disciplina is null ? TypedResults.NotFound() : TypedResults.Ok(disciplina);
            })
            .WithName("ObterDisciplina")
            .WithSummary("Obtém uma disciplina")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{disciplinaId:guid}", async Task<Results<Ok<DisciplinaResponse>, NotFound>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                AtualizarDisciplinaRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var disciplina = await service.AtualizarDisciplinaAsync(
                    principal.GetRequiredUserId(),
                    disciplinaId,
                    request,
                    cancellationToken);
                return disciplina is null ? TypedResults.NotFound() : TypedResults.Ok(disciplina);
            })
            .WithName("AtualizarDisciplina")
            .WithSummary("Atualiza uma disciplina")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{disciplinaId:guid}", async Task<Results<NoContent, NotFound>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var desativada = await service.DesativarDisciplinaAsync(principal.GetRequiredUserId(), disciplinaId, cancellationToken);
                return desativada ? TypedResults.NoContent() : TypedResults.NotFound();
            })
            .WithName("DesativarDisciplina")
            .WithSummary("Desativa uma disciplina sem remover seu histórico")
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
