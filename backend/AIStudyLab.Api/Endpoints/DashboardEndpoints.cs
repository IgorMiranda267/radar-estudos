using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIStudyLab.Api.Endpoints;

public static class DashboardEndpoints
{
    public static RouteGroupBuilder MapDashboard(this RouteGroupBuilder usuarioGroup)
    {
        usuarioGroup.MapGet("/dashboard", async Task<Ok<DashboardResponse>> (
                ClaimsPrincipal principal,
                IStudyService service,
                CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.ObterDashboardAsync(principal.GetRequiredUserId(), cancellationToken)))
            .WithName("ObterDashboard")
            .WithTags("Dashboard")
            .WithSummary("Obtém os indicadores de estudo do usuário");

        return usuarioGroup;
    }
}
