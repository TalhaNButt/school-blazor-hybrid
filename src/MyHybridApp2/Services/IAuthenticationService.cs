using Microsoft.AspNetCore.Components;
using MyHybridApp2.Models;

namespace MyHybridApp2.Services
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync();
        bool IsLoggedIn();
        void NavigateToHome();
        void NavigateToLogin();
    }

    public class AuthenticationService(NavigationManager navigationManager) : IAuthenticationService
    {
        private bool _isLoggedIn = false;
        private readonly NavigationManager _navigationManager = navigationManager;

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            // TODO: Add real authentication logic here
            // For now, we simulate a successful login
            await Task.Delay(500); // Simulate network delay
            
            // Return true for demo purposes
            _isLoggedIn = true;

            return true;
        }

         public async Task<bool> LogoutAsync()
        {
            // TODO: Add real authentication logic here
            // For now, we simulate a successful logout
            await Task.Delay(500); // Simulate network delay
            
            // Return true for demo purposes
            _isLoggedIn = false;

            return true;
        }

        public bool IsLoggedIn() => _isLoggedIn;
        public void NavigateToHome() => _navigationManager.NavigateTo("/home");
        public void NavigateToLogin() => _navigationManager.NavigateTo("/");
        
    }
}