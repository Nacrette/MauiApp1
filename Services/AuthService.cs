using System.Text.Json;
using Microsoft.Maui.Storage;
using MauiApp1.Models;

namespace MauiApp1.Services;

public class AuthService : IAuthService
{
    private User? _currentUser;
    private const string UserKey = "current_user";

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;

    public async Task<bool> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        await Task.Delay(300);

        _currentUser = new User
        {
            Username = username.ToLowerInvariant().Trim(),
            DisplayName = username.Trim(),
            AvatarEmoji = GetRandomAvatarEmoji(),
            IsLoggedIn = true,
            CreatedAt = DateTime.UtcNow
        };

        await SaveUserAsync(_currentUser);
        return true;
    }

    public async Task<bool> RegisterAsync(string username, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 4)
            return false;

        await Task.Delay(300);

        _currentUser = new User
        {
            Username = username.ToLowerInvariant().Trim(),
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? username.Trim() : displayName.Trim(),
            AvatarEmoji = GetRandomAvatarEmoji(),
            IsLoggedIn = true,
            CreatedAt = DateTime.UtcNow
        };

        await SaveUserAsync(_currentUser);
        return true;
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        try
        {
            SecureStorage.Default.Remove(UserKey);
        }
        catch
        {
            // Ignore
        }
        return Task.CompletedTask;
    }

    public async Task<User?> GetSavedUserAsync()
    {
        try
        {
            var json = await SecureStorage.Default.GetAsync(UserKey);
            if (!string.IsNullOrEmpty(json))
            {
                _currentUser = JsonSerializer.Deserialize<User>(json);
                return _currentUser;
            }
        }
        catch
        {
            try { SecureStorage.Default.Remove(UserKey); } catch { }
        }
        return null;
    }

    public Task SaveUserAsync(User user)
    {
        var json = JsonSerializer.Serialize(user);
        return SecureStorage.Default.SetAsync(UserKey, json);
    }

    private static string GetRandomAvatarEmoji()
    {
        var emojis = new[]
        {
            "\U0001F47B", "\U0001F916", "\U0001F468", "\U0001F469",
            "\U0001F47D", "\U0001F47E", "\U0001F916", "\U0001F47F",
            "\U0001F43A", "\U0001F98B", "\U0001F981", "\U0001F430"
        };
        return emojis[Random.Shared.Next(emojis.Length)];
    }
}
