using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIStudyLab.Api.Endpoints;

public static class CurrentUserEndpoints
{
    public static RouteGroupBuilder MapPerfil(this RouteGroupBuilder currentUserGroup)
    {
        currentUserGroup.MapGet("/perfil", async Task<Ok<PerfilUsuarioResponse>> (
                ClaimsPrincipal principal,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.ObterPerfilAsync(principal.GetRequiredUserId(), cancellationToken)))
            .WithName("ObterMeuPerfil")
            .WithTags("Perfil")
            .WithSummary("Obtém o perfil do usuário autenticado");

        currentUserGroup.MapPut("/perfil", async Task<Ok<PerfilUsuarioResponse>> (
                ClaimsPrincipal principal,
                AtualizarPerfilUsuarioRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.AtualizarPerfilAsync(
                    principal.GetRequiredUserId(),
                    request,
                    cancellationToken)))
            .WithName("AtualizarMeuPerfil")
            .WithTags("Perfil")
            .WithSummary("Atualiza o perfil do usuário autenticado")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return currentUserGroup;
    }

    public static Guid GetRequiredUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("A sessão não contém um identificador de usuário válido.");
    }
}
