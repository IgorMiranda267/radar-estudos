using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIStudyLab.Api.Endpoints;

public static class AssuntoEndpoints
{
    public static RouteGroupBuilder MapAssuntos(this RouteGroupBuilder usuarioGroup)
    {
        var group = usuarioGroup.MapGroup("/disciplinas/{disciplinaId:guid}/assuntos")
            .WithTags("Assuntos");

        group.MapGet("/", async Task<Ok<IReadOnlyList<AssuntoResponse>>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.ListarAssuntosAsync(principal.GetRequiredUserId(), disciplinaId, cancellationToken)))
            .WithName("ListarAssuntos")
            .WithSummary("Lista os assuntos de uma disciplina")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async Task<Created<AssuntoResponse>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                CriarAssuntoRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var usuarioId = principal.GetRequiredUserId();
                var assunto = await service.CriarAssuntoAsync(usuarioId, disciplinaId, request, cancellationToken);
                return TypedResults.Created(
                    $"/api/me/disciplinas/{disciplinaId}/assuntos/{assunto.Id}",
                    assunto);
            })
            .WithName("CriarAssunto")
            .WithSummary("Cadastra um assunto em uma disciplina")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{assuntoId:guid}", async Task<Results<Ok<AssuntoResponse>, NotFound>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                Guid assuntoId,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var assunto = await service.ObterAssuntoAsync(principal.GetRequiredUserId(), disciplinaId, assuntoId, cancellationToken);
                return assunto is null ? TypedResults.NotFound() : TypedResults.Ok(assunto);
            })
            .WithName("ObterAssunto")
            .WithSummary("Obtém um assunto")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{assuntoId:guid}", async Task<Results<Ok<AssuntoResponse>, NotFound>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                Guid assuntoId,
                AtualizarAssuntoRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var assunto = await service.AtualizarAssuntoAsync(
                    principal.GetRequiredUserId(),
                    disciplinaId,
                    assuntoId,
                    request,
                    cancellationToken);
                return assunto is null ? TypedResults.NotFound() : TypedResults.Ok(assunto);
            })
            .WithName("AtualizarAssunto")
            .WithSummary("Atualiza um assunto")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{assuntoId:guid}", async Task<Results<NoContent, NotFound>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                Guid assuntoId,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var excluido = await service.ExcluirAssuntoAsync(
                    principal.GetRequiredUserId(),
                    disciplinaId,
                    assuntoId,
                    cancellationToken);
                return excluido ? TypedResults.NoContent() : TypedResults.NotFound();
            })
            .WithName("ExcluirAssunto")
            .WithSummary("Exclui um assunto que não possua sessões ou revisões")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}
