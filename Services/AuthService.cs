using System.Diagnostics;
using MauiApp1.Models;
using MauiApp1.Services.Database;
using Microsoft.Maui.Storage;

namespace MauiApp1.Services;

public class AuthService : IAuthService
{
    private readonly AppDatabase _database;
    private User? _currentUser;
    private bool _isMigrated;

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;

    public AuthService(AppDatabase database)
    {
        _database = database;
    }

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
        Debug.WriteLine($"[AuthService] User logged in: {_currentUser.Username}");
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
        Debug.WriteLine($"[AuthService] User registered: {_currentUser.Username}");
        return true;
    }

    public async Task LogoutAsync()
    {
        if (_currentUser != null)
            await _database.SetUserLoggedOutAsync(_currentUser.Username);

        _currentUser = null;
        Debug.WriteLine("[AuthService] User logged out.");
    }

    public async Task<User?> GetSavedUserAsync()
    {
        await MigrateFromSecureStorageIfNeededAsync();

        try
        {
            _currentUser = await _database.GetLoggedInUserAsync();
            Debug.WriteLine(_currentUser is null
                ? "[AuthService] No saved logged-in user found."
                : $"[AuthService] Loaded saved user: {_currentUser.Username}");
            return _currentUser;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[AuthService] Failed to load saved user: {ex}");
            return null;
        }
    }

    public async Task SaveUserAsync(User user)
    {
        await _database.UpsertUserAsync(user);
    }

    private async Task MigrateFromSecureStorageIfNeededAsync()
    {
        if (_isMigrated)
            return;

        _isMigrated = true;

        try
        {
            const string legacyUserKey = "current_user";
            var json = await SecureStorage.Default.GetAsync(legacyUserKey);
            if (string.IsNullOrWhiteSpace(json))
                return;

            var user = System.Text.Json.JsonSerializer.Deserialize<User>(json);
            if (user is null)
                return;

            user.IsLoggedIn = true;
            await _database.UpsertUserAsync(user);
            SecureStorage.Default.Remove(legacyUserKey);
            Debug.WriteLine("[AuthService] Migrated saved user from SecureStorage to SQLite.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[AuthService] SecureStorage migration skipped: {ex.Message}");
        }
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
