using Microsoft.AspNetCore.Components;
using MyHybridApp2.Shared.Auth;

namespace MyHybridApp2.UI.Services;

public sealed class AuthenticationService(NavigationManager navigationManager) : IAuthenticationService
{
    private bool _isLoggedIn;

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        // TODO: Replace with a real token-based login against the API.
        await Task.Delay(500);
        _isLoggedIn = true;
        return true;
    }

    public async Task<bool> LogoutAsync()
    {
        // TODO: Replace with a real logout (clear stored tokens).
        await Task.Delay(500);
        _isLoggedIn = false;
        return true;
    }

    public bool IsLoggedIn() => _isLoggedIn;

    public void NavigateToHome() => navigationManager.NavigateTo("/home");

    public void NavigateToLogin() => navigationManager.NavigateTo("/");
}
