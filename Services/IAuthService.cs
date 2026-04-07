using MauiApp1.Models;

namespace MauiApp1.Services;

public interface IAuthService
{
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    Task<bool> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(string username, string password, string displayName);
    Task LogoutAsync();
    Task<User?> GetSavedUserAsync();
    Task SaveUserAsync(User user);
}
