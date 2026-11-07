using MyHybridApp2.Shared.Auth;

namespace MyHybridApp2.UI.Services;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task<bool> LogoutAsync();
    bool IsLoggedIn();
    void NavigateToHome();
    void NavigateToLogin();
}
