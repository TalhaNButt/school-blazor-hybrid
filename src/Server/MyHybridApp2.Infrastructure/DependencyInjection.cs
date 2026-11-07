using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Infrastructure.Persistence;

namespace MyHybridApp2.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' not found. Set it via user secrets: " +
                "dotnet user-secrets set \"ConnectionStrings:Default\" \"Host=localhost;...\" --project src/Server/MyHybridApp2.Api");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");

        return services;
    }
}
