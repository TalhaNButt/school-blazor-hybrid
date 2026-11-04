using Microsoft.AspNetCore.Components;
using MyHybridApp2.Models;

namespace MyHybridApp2.Services
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(LoginRequest request);
        void NavigateToHome();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly NavigationManager _navigationManager;

        public AuthenticationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            // TODO: Add real authentication logic here
            // For now, we simulate a successful login
            await Task.Delay(500); // Simulate network delay
            
            // Return true for demo purposes
            return true;
        }

        public void NavigateToHome()
        {
            _navigationManager.NavigateTo("/home");
        }
    }
}