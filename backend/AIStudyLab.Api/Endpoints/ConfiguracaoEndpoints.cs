using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIStudyLab.Api.Endpoints;

public static class ConfiguracaoEndpoints
{
    public static RouteGroupBuilder MapConfiguracoes(this RouteGroupBuilder usuarioGroup)
    {
        usuarioGroup.MapGet("/configuracoes", async Task<Ok<ConfiguracaoUsuarioResponse>> (
                ClaimsPrincipal principal,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.ObterConfiguracaoAsync(principal.GetRequiredUserId(), cancellationToken)))
            .WithName("ObterConfiguracaoUsuario")
            .WithTags("Configurações")
            .WithSummary("Obtém as preferências do usuário")
            .ProducesProblem(StatusCodes.Status404NotFound);

        usuarioGroup.MapPut("/configuracoes", async Task<Ok<ConfiguracaoUsuarioResponse>> (
                ClaimsPrincipal principal,
                AtualizarConfiguracaoUsuarioRequest request,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.AtualizarConfiguracaoAsync(principal.GetRequiredUserId(), request, cancellationToken)))
            .WithName("AtualizarConfiguracaoUsuario")
            .WithTags("Configurações")
            .WithSummary("Salva as preferências do usuário")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return usuarioGroup;
    }
}
