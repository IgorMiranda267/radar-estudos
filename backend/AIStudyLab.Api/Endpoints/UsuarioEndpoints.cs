using System.Security.Claims;
using AIStudyLab.Application.Contracts;
using AIStudyLab.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;

namespace AIStudyLab.Api.Endpoints;

public static class UsuarioEndpoints
{
    public static RouteGroupBuilder MapAutenticacao(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth").WithTags("Autenticação");

        group.MapPost("/register", async Task<Created<UsuarioResponse>> (
                CriarUsuarioRequest request,
                IStudyService service,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var usuario = await service.CriarUsuarioAsync(request, cancellationToken);
                await SignInAsync(httpContext, usuario);
                return TypedResults.Created("/api/auth/me", usuario);
            })
            .WithName("RegistrarConta")
            .WithSummary("Registra uma conta e inicia uma sessão autenticada")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .RequireRateLimiting("authentication");

        group.MapPost("/login", async Task<Results<Ok<UsuarioResponse>, UnauthorizedHttpResult>> (
                AutenticarUsuarioRequest request,
                IStudyService service,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var usuario = await service.AutenticarUsuarioAsync(request, cancellationToken);
                if (usuario is null)
                {
                    return TypedResults.Unauthorized();
                }

                await SignInAsync(httpContext, usuario);
                return TypedResults.Ok(usuario);
            })
            .WithName("Entrar")
            .WithSummary("Autentica uma conta")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("authentication");

        group.MapGet("/me", async Task<Results<Ok<UsuarioResponse>, NotFound>> (
                ClaimsPrincipal principal,
                IStudyService service,
                CancellationToken cancellationToken) =>
            {
                var usuario = await service.ObterUsuarioAsync(principal.GetRequiredUserId(), cancellationToken);
                return usuario is null ? TypedResults.NotFound() : TypedResults.Ok(usuario);
            })
            .WithName("ObterMinhaConta")
            .WithSummary("Obtém os dados da conta autenticada")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/logout", async Task<NoContent> (HttpContext httpContext) =>
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return TypedResults.NoContent();
            })
            .WithName("Sair")
            .WithSummary("Encerra a sessão autenticada")
            .RequireAuthorization();

        return group;
    }

    private static async Task SignInAsync(HttpContext httpContext, UsuarioResponse usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme));
        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });
    }
}
