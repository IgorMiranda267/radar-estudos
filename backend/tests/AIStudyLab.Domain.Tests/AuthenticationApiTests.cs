using System.Net;
using System.Net.Http.Json;
using AIStudyLab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AIStudyLab.Domain.Tests;

public sealed class AuthenticationApiTests : IClassFixture<AuthenticationApiFactory>
{
    private readonly AuthenticationApiFactory factory;

    public AuthenticationApiTests(AuthenticationApiFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task StudyRoutes_RequireAuthentication()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/me/perfil");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedRoutes_AreMappedOutsideDevelopmentAndStillRequireAuthentication()
    {
        using var productionFactory = AuthenticationApiFactory.CreateProduction();
        using var client = productionFactory.CreateClient();

        var protectedResponse = await client.GetAsync("/api/me/perfil");
        var openApiResponse = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, openApiResponse.StatusCode);
    }

    [Fact]
    public async Task RegistrationAndLogin_CreateHttpOnlyCookieSession()
    {
        using var registrationClient = factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@example.com";
        var registration = await SendJsonAsync(
            registrationClient,
            HttpMethod.Post,
            "/api/auth/register",
            new { nome = "Estudante", email, senha = "SenhaSegura123!" });

        Assert.True(registration.StatusCode == HttpStatusCode.Created,
            $"Expected account registration to succeed but got {(int)registration.StatusCode}: {await registration.Content.ReadAsStringAsync()}");
        Assert.Contains(registration.Headers.GetValues("Set-Cookie"), value =>
            value.Contains("aistudylab.session=", StringComparison.Ordinal)
            && value.Contains("httponly", StringComparison.OrdinalIgnoreCase)
            && value.Contains("samesite=strict", StringComparison.OrdinalIgnoreCase));

        var currentUser = await registrationClient.GetFromJsonAsync<UserPayload>("/api/auth/me");
        Assert.Equal(email, currentUser?.Email);

        using var loginClient = factory.CreateClient();
        var invalidLogin = await SendJsonAsync(
            loginClient,
            HttpMethod.Post,
            "/api/auth/login",
            new { email, senha = "senha-incorreta" });
        Assert.Equal(HttpStatusCode.Unauthorized, invalidLogin.StatusCode);

        var login = await SendJsonAsync(
            loginClient,
            HttpMethod.Post,
            "/api/auth/login",
            new { email, senha = "SenhaSegura123!" });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal(email, (await loginClient.GetFromJsonAsync<UserPayload>("/api/auth/me"))?.Email);

        using var logout = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
        logout.Headers.Add("Origin", "http://localhost:3000");
        logout.Headers.Add("X-AIStudyLab-Request", "1");
        Assert.Equal(HttpStatusCode.NoContent, (await loginClient.SendAsync(logout)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await loginClient.GetAsync("/api/auth/me")).StatusCode);
    }

    [Fact]
    public async Task StudyData_IsolatedByAuthenticatedCookieAndNeverByClientUserId()
    {
        using var firstUser = factory.CreateClient();
        using var secondUser = factory.CreateClient();
        await RegisterAsync(firstUser);
        await RegisterAsync(secondUser);

        var createResponse = await SendJsonAsync(
            firstUser,
            HttpMethod.Post,
            "/api/me/disciplinas",
            new { nome = "Matemática", cor = "#2563EB", icone = "book-open" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var firstList = await firstUser.GetFromJsonAsync<List<object>>("/api/me/disciplinas");
        var secondList = await secondUser.GetFromJsonAsync<List<object>>("/api/me/disciplinas");
        Assert.Single(firstList!);
        Assert.Empty(secondList!);

        var legacyRoute = await firstUser.GetAsync($"/api/usuarios/{Guid.NewGuid()}/disciplinas");
        Assert.Equal(HttpStatusCode.NotFound, legacyRoute.StatusCode);
    }

    [Fact]
    public async Task UnsafeApiRequests_RejectUntrustedOrigin()
    {
        using var client = factory.CreateClient();
        await RegisterAsync(client);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/me/disciplinas")
        {
            Content = JsonContent.Create(new { nome = "Matemática", cor = "#2563EB", icone = "book" })
        };
        request.Headers.Add("Origin", "https://attacker.example");
        request.Headers.Add("X-AIStudyLab-Request", "1");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Profile_IsValidatedAndBoundToAuthenticatedUser()
    {
        using var firstUser = factory.CreateClient();
        using var secondUser = factory.CreateClient();
        await RegisterAsync(firstUser);
        await RegisterAsync(secondUser);

        var update = await SendJsonAsync(
            firstUser,
            HttpMethod.Put,
            "/api/me/perfil",
            new
            {
                objetivo = "  Concurso público  ",
                nivel = "Intermediário",
                cidade = "São Paulo",
                instituicao = "Instituto",
                bio = "  Estudando para a prova.  ",
                dataNascimento = "1995-04-12"
            });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        var profile = await firstUser.GetFromJsonAsync<ProfilePayload>("/api/me/perfil");
        var otherProfile = await secondUser.GetFromJsonAsync<ProfilePayload>("/api/me/perfil");
        Assert.Equal("Concurso público", profile?.Objetivo);
        Assert.Equal("Estudando para a prova.", profile?.Bio);
        Assert.Null(otherProfile?.Objetivo);

        var invalidDate = await SendJsonAsync(
            firstUser,
            HttpMethod.Put,
            "/api/me/perfil",
            new { dataNascimento = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)).ToString("yyyy-MM-dd") });
        Assert.Equal(HttpStatusCode.BadRequest, invalidDate.StatusCode);

        var tooLong = await SendJsonAsync(
            firstUser,
            HttpMethod.Put,
            "/api/me/perfil",
            new { bio = new string('x', 2001) });
        Assert.Equal(HttpStatusCode.BadRequest, tooLong.StatusCode);
    }

    private static async Task RegisterAsync(HttpClient client)
    {
        var response = await SendJsonAsync(
            client,
            HttpMethod.Post,
            "/api/auth/register",
            new
            {
                nome = "Estudante",
                email = $"{Guid.NewGuid():N}@example.com",
                senha = "SenhaSegura123!"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<HttpResponseMessage> SendJsonAsync(
        HttpClient client,
        HttpMethod method,
        string path,
        object body)
    {
        using var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("X-AIStudyLab-Request", "1");
        return await client.SendAsync(request);
    }

    private sealed record UserPayload(Guid Id, string Nome, string Email, DateTimeOffset CriadoEm);
    private sealed record ProfilePayload(string? Objetivo, string? Bio);
}

public sealed class AuthenticationApiFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"auth-test-{Guid.NewGuid():N}";
    private readonly string environment;

    public AuthenticationApiFactory() : this("Development")
    {
    }

    private AuthenticationApiFactory(string environment)
    {
        this.environment = environment;
    }

    public static AuthenticationApiFactory CreateProduction() => new("Production");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment);
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            "Host=localhost;Database=auth-tests;Username=test;Password=test");
        builder.UseSetting("Cors:AllowedOrigins:0", "http://localhost:3000");
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cors:AllowedOrigins:0"] = "http://localhost:3000"
            }));
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<StudyLabDbContext>>();
            services.RemoveAll<DbContextOptions<StudyLabDbContext>>();
            services.RemoveAll<StudyLabDbContext>();
            services.AddDbContext<StudyLabDbContext>(options => options.UseInMemoryDatabase(databaseName));
        });
    }
}
