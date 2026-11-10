using AIStudyLab.Application.Services;
using AIStudyLab.Domain.Entities;
using AIStudyLab.Infrastructure.Persistence;
using AIStudyLab.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AIStudyLab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        services.AddDbContext<StudyLabDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IStudyService, StudyService>();
        return services;
    }
}
