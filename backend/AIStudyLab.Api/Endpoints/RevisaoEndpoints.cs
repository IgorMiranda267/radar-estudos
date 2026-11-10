using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIStudyLab.Api.Endpoints;

public static class RevisaoEndpoints
{
    public static RouteGroupBuilder MapRevisoes(this RouteGroupBuilder usuarioGroup)
    {
        var group = usuarioGroup.MapGroup("/revisoes").WithTags("Revisões");

        group.MapGet("/", async Task<Ok<IReadOnlyList<RevisaoResponse>>> (
                ClaimsPrincipal principal,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.ListarRevisoesAsync(principal.GetRequiredUserId(), cancellationToken)))
            .WithName("ListarRevisoes")
            .WithSummary("Lista as revisões do usuário")
            .ProducesProblem(StatusCodes.Status404NotFound);

        usuarioGroup.MapPost("/assuntos/{assuntoId:guid}/revisoes", async Task<Created<RevisaoResponse>> (
                ClaimsPrincipal principal,
                Guid assuntoId,
                CriarRevisaoRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var usuarioId = principal.GetRequiredUserId();
                var revisao = await service.CriarRevisaoAsync(usuarioId, assuntoId, request, cancellationToken);
                return TypedResults.Created($"/api/me/revisoes/{revisao.Id}", revisao);
            })
            .WithName("CriarRevisao")
            .WithTags("Revisões")
            .WithSummary("Agenda a revisão de um assunto")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{revisaoId:guid}", async Task<Results<Ok<RevisaoResponse>, NotFound>> (
                ClaimsPrincipal principal,
                Guid revisaoId,
                AtualizarRevisaoRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var revisao = await service.AtualizarRevisaoAsync(principal.GetRequiredUserId(), revisaoId, request, cancellationToken);
                return revisao is null ? TypedResults.NotFound() : TypedResults.Ok(revisao);
            })
            .WithName("AtualizarRevisao")
            .WithSummary("Reagenda ou atualiza uma revisão")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/{revisaoId:guid}", async Task<Results<NoContent, NotFound>> (
                ClaimsPrincipal principal,
                Guid revisaoId,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var excluida = await service.ExcluirRevisaoAsync(principal.GetRequiredUserId(), revisaoId, cancellationToken);
                return excluida ? TypedResults.NoContent() : TypedResults.NotFound();
            })
            .WithName("ExcluirRevisao")
            .WithSummary("Exclui uma revisão")
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
