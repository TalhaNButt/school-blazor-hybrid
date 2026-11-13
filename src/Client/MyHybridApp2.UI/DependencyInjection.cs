using Microsoft.Extensions.DependencyInjection;
using MyHybridApp2.UI.Services;

namespace MyHybridApp2.UI;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the UI's client services. Each host (MAUI, a future web app) supplies the API base address.
    /// </summary>
    public static IServiceCollection AddMyHybridApp2UI(this IServiceCollection services, Uri apiBaseAddress)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddHttpClient<IPostService, ApiPostService>(client => client.BaseAddress = apiBaseAddress);

        return services;
    }
}
