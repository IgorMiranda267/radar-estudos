using System.Threading.RateLimiting;
using System.Text.Json.Serialization;
using AIStudyLab.Api.Endpoints;
using AIStudyLab.Api.Middleware;
using AIStudyLab.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "A connection string 'DefaultConnection' is required. Configure it using ConnectionStrings__DefaultConnection.");

builder.Services.AddPersistence(connectionString);
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? (builder.Environment.IsDevelopment()
        ? ["http://localhost:3000", "http://127.0.0.1:3000"]
        : []);
if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("Configure pelo menos uma origem confiável em Cors:AllowedOrigins.");
}

builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowCredentials()
            .WithHeaders("Accept", "Content-Type", "X-AIStudyLab-Request")
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "aistudylab.session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("authentication", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseRouting();
app.UseCors("Frontend");
app.UseMiddleware<RequestOriginValidationMiddleware>(new HashSet<string>(allowedOrigins, StringComparer.Ordinal));
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => TypedResults.Ok(new { status = "healthy" }))
    .WithName("HealthCheck")
    .WithSummary("Verifica se a API está disponível");

app.MapAutenticacao();
var usuarioAutenticado = app.MapGroup("/api/me").RequireAuthorization();
usuarioAutenticado.MapDisciplinas();
usuarioAutenticado.MapAssuntos();
usuarioAutenticado.MapSessoesEstudo();
usuarioAutenticado.MapMetas();
usuarioAutenticado.MapRevisoes();
usuarioAutenticado.MapDashboard();
usuarioAutenticado.MapConfiguracoes();
usuarioAutenticado.MapPerfil();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();

public partial class Program;
