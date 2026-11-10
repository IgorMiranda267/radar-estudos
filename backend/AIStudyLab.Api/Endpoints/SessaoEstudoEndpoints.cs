using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIStudyLab.Api.Endpoints;

public static class SessaoEstudoEndpoints
{
    public static RouteGroupBuilder MapSessoesEstudo(this RouteGroupBuilder usuarioGroup)
    {
        var group = usuarioGroup.MapGroup("/sessoes-estudo").WithTags("Sessões de estudo");

        group.MapGet("/", async Task<Ok<IReadOnlyList<SessaoEstudoResponse>>> (
                ClaimsPrincipal principal,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.ListarSessoesAsync(principal.GetRequiredUserId(), cancellationToken)))
            .WithName("ListarSessoesEstudo")
            .WithSummary("Lista as sessões do usuário")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{sessaoId:guid}", async Task<Results<Ok<SessaoEstudoResponse>, NotFound>> (
                ClaimsPrincipal principal,
                Guid sessaoId,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var sessao = await service.ObterSessaoAsync(principal.GetRequiredUserId(), sessaoId, cancellationToken);
                return sessao is null ? TypedResults.NotFound() : TypedResults.Ok(sessao);
            })
            .WithName("ObterSessaoEstudo")
            .WithSummary("Obtém uma sessão de estudo")
            .ProducesProblem(StatusCodes.Status404NotFound);

        usuarioGroup.MapPost("/disciplinas/{disciplinaId:guid}/sessoes-estudo", async Task<Created<SessaoEstudoResponse>> (
                ClaimsPrincipal principal,
                Guid disciplinaId,
                CriarSessaoEstudoRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var usuarioId = principal.GetRequiredUserId();
                var sessao = await service.CriarSessaoAsync(usuarioId, disciplinaId, request, cancellationToken);
                return TypedResults.Created(
                    $"/api/me/sessoes-estudo/{sessao.Id}",
                    sessao);
            })
            .WithName("CriarSessaoEstudo")
            .WithTags("Sessões de estudo")
            .WithSummary("Registra uma sessão em uma disciplina")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return group;
    }
}
