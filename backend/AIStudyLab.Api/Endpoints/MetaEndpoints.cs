using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIStudyLab.Api.Endpoints;

public static class MetaEndpoints
{
    public static RouteGroupBuilder MapMetas(this RouteGroupBuilder usuarioGroup)
    {
        var group = usuarioGroup.MapGroup("/metas").WithTags("Metas");

        group.MapGet("/", async Task<Ok<IReadOnlyList<MetaResponse>>> (
                ClaimsPrincipal principal,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.ListarMetasAsync(principal.GetRequiredUserId(), cancellationToken)))
            .WithName("ListarMetas")
            .WithSummary("Lista as metas do usuário")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async Task<Created<MetaResponse>> (
                ClaimsPrincipal principal,
                CriarMetaRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var meta = await service.CriarMetaAsync(principal.GetRequiredUserId(), request, cancellationToken);
                return TypedResults.Created($"/api/me/metas/{meta.Id}", meta);
            })
            .WithName("CriarMeta")
            .WithSummary("Cria uma meta")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{metaId:guid}", async Task<Results<Ok<MetaResponse>, NotFound>> (
                ClaimsPrincipal principal,
                Guid metaId,
                AtualizarMetaRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var meta = await service.AtualizarMetaAsync(principal.GetRequiredUserId(), metaId, request, cancellationToken);
                return meta is null ? TypedResults.NotFound() : TypedResults.Ok(meta);
            })
            .WithName("AtualizarMeta")
            .WithSummary("Atualiza uma meta")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/{metaId:guid}", async Task<Results<NoContent, NotFound>> (
                ClaimsPrincipal principal,
                Guid metaId,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var excluida = await service.ExcluirMetaAsync(principal.GetRequiredUserId(), metaId, cancellationToken);
                return excluida ? TypedResults.NoContent() : TypedResults.NotFound();
            })
            .WithName("ExcluirMeta")
            .WithSummary("Exclui uma meta")
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
