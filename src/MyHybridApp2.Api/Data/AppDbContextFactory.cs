using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace MyHybridApp2.Api.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
         var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<AppDbContext>(optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("Default");
       
        var builder = new DbContextOptionsBuilder<AppDbContext>();

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Default' not found. Set it via user secrets: " +
                "dotnet user-secrets set \"ConnectionStrings:Default\" \"Host=localhost;...\"");
        }

        builder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            // npgsqlOptions.EnableRetryOnFailure(maxRetries: 3);
        });

        return new AppDbContext(builder.Options);
    }
}