using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using MauiApp1.Models;
using MauiApp1.Services.Database;

namespace MauiApp1.Services;

public class FavoritesService : IFavoritesService
{
    private readonly ILogger<FavoritesService> _logger;
    private readonly AppDatabase _database;
    private readonly Dictionary<string, List<CelestialBody>> _planetFavorites = new();
    private readonly Dictionary<string, List<ApodFavorite>> _apodFavorites = new();
    private bool _isLoaded;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private bool _isMigrated;

    public FavoritesService(ILogger<FavoritesService> logger, AppDatabase database)
    {
        _logger = logger;
        _database = database;
    }

    public Task EnsureLoadedAsync() => LoadFromStorageAsync();

    public IReadOnlyList<CelestialBody> GetFavoritePlanets(string username)
    {
        username = NormalizeUsername(username);
        EnsureLoaded();
        EnsureUserLoaded(username);
        return _planetFavorites.TryGetValue(username, out var favorites) 
            ? favorites.AsReadOnly() 
            : new List<CelestialBody>().AsReadOnly();
    }

    public IReadOnlyList<ApodFavorite> GetFavoriteApods(string username)
    {
        username = NormalizeUsername(username);
        EnsureLoaded();
        EnsureUserLoaded(username);
        return _apodFavorites.TryGetValue(username, out var favorites) 
            ? favorites.AsReadOnly() 
            : new List<ApodFavorite>().AsReadOnly();
    }

    public async Task<IReadOnlyList<CelestialBody>> GetFavoritePlanetsAsync(string username)
    {
        username = NormalizeUsername(username);
        await LoadFromStorageAsync();
        await EnsureUserLoadedAsync(username).ConfigureAwait(false);
        return GetFavoritePlanets(username);
    }

    public void AddFavoritePlanet(string username, CelestialBody planet)
    {
        username = NormalizeUsername(username);
        EnsureLoaded();
        EnsureUserLoaded(username);
        EnsureUserExists(username);
        
        if (!_planetFavorites[username].Any(p => p.Name == planet.Name))
        {
            _planetFavorites[username].Add(planet);
            _ = SaveAllAsync();
        }
    }

    public void RemoveFavoritePlanet(string username, string planetName)
    {
        username = NormalizeUsername(username);
        EnsureLoaded();
        EnsureUserLoaded(username);
        if (_planetFavorites.TryGetValue(username, out var favorites))
        {
            favorites.RemoveAll(p => p.Name == planetName);
            _ = SaveAllAsync();
        }
    }

    public bool IsPlanetFavorite(string username, string planetName)
    {
        username = NormalizeUsername(username);
        EnsureLoaded();
        EnsureUserLoaded(username);
        return _planetFavorites.TryGetValue(username, out var favorites) 
            && favorites.Any(p => p.Name == planetName);
    }

    public void AddFavoriteApod(string username, ApodFavorite apod)
    {
        username = NormalizeUsername(username);
        EnsureLoaded();
        EnsureUserLoaded(username);
        EnsureUserExists(username);
        
        if (!_apodFavorites[username].Any(a => a.Date == apod.Date))
        {
            _apodFavorites[username].Add(apod);
            _ = SaveAllAsync();
        }
    }

    public void RemoveFavoriteApod(string username, string apodDate)
    {
        username = NormalizeUsername(username);
        EnsureLoaded();
        EnsureUserLoaded(username);
        if (_apodFavorites.TryGetValue(username, out var favorites))
        {
            favorites.RemoveAll(a => a.Date == apodDate);
            _ = SaveAllAsync();
        }
    }

    public bool IsApodFavorite(string username, string apodDate)
    {
        username = NormalizeUsername(username);
        EnsureLoaded();
        EnsureUserLoaded(username);
        return _apodFavorites.TryGetValue(username, out var favorites) 
            && favorites.Any(a => a.Date == apodDate);
    }

    public async Task SaveAllAsync()
    {
        try
        {
            foreach (var (username, planets) in _planetFavorites)
            {
                foreach (var planet in planets)
                    await _database.AddOrUpdatePlanetFavoriteAsync(username, planet).ConfigureAwait(false);
            }

            foreach (var (username, apods) in _apodFavorites)
            {
                foreach (var apod in apods)
                    await _database.AddOrUpdateApodFavoriteAsync(username, apod).ConfigureAwait(false);
            }

            Debug.WriteLine("[FavoritesService] SaveAllAsync completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save favorites to storage");
        }
    }

    private async Task LoadFromStorageAsync()
    {
        if (_isLoaded)
            return;

        await _loadLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_isLoaded)
                return;

            await _database.InitializeAsync().ConfigureAwait(false);
            await MigrateFromSecureStorageIfNeededAsync().ConfigureAwait(false);

            _planetFavorites.Clear();
            _apodFavorites.Clear();

            var userNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var savedUser = await _database.GetLoggedInUserAsync().ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(savedUser?.Username))
                userNames.Add(savedUser.Username);

            foreach (var username in userNames)
            {
                _planetFavorites[username] = (await _database.GetPlanetFavoritesAsync(username).ConfigureAwait(false)).ToList();
                _apodFavorites[username] = (await _database.GetApodFavoritesAsync(username).ConfigureAwait(false)).ToList();
            }

            _isLoaded = true;
            Debug.WriteLine("[FavoritesService] Favorites loaded from SQLite.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load favorites from SQLite");
        }
        finally
        {
            _loadLock.Release();
        }
    }

    private void EnsureUserExists(string username)
    {
        if (!_planetFavorites.ContainsKey(username))
            _planetFavorites[username] = [];
        if (!_apodFavorites.ContainsKey(username))
            _apodFavorites[username] = [];
    }

    private void EnsureLoaded()
    {
        if (_isLoaded) return;
        // Must not capture UI sync context — blocking .GetResult() on the main thread would deadlock.
        LoadFromStorageAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private void EnsureUserLoaded(string username)
    {
        if (_planetFavorites.ContainsKey(username) && _apodFavorites.ContainsKey(username))
            return;

        EnsureUserLoadedAsync(username).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private async Task EnsureUserLoadedAsync(string username)
    {
        if (_planetFavorites.ContainsKey(username) && _apodFavorites.ContainsKey(username))
            return;

        _planetFavorites[username] = (await _database.GetPlanetFavoritesAsync(username).ConfigureAwait(false)).ToList();
        _apodFavorites[username] = (await _database.GetApodFavoritesAsync(username).ConfigureAwait(false)).ToList();
    }

    private static string NormalizeUsername(string username) => username.Trim().ToLowerInvariant();

    private async Task MigrateFromSecureStorageIfNeededAsync()
    {
        if (_isMigrated)
            return;

        _isMigrated = true;

        const string legacyPlanetsKey = "favorite_planets";
        const string legacyApodsKey = "favorite_apods";

        try
        {
            var planetsJson = await SecureStorage.Default.GetAsync(legacyPlanetsKey).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(planetsJson))
            {
                var legacyPlanets =
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<CelestialBody>>>(planetsJson);
                if (legacyPlanets != null)
                {
                    foreach (var (username, planets) in legacyPlanets)
                    {
                        foreach (var planet in planets)
                            await _database.AddOrUpdatePlanetFavoriteAsync(username, planet).ConfigureAwait(false);
                    }
                }
            }

            var apodsJson = await SecureStorage.Default.GetAsync(legacyApodsKey).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(apodsJson))
            {
                var legacyApods =
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<ApodFavorite>>>(apodsJson);
                if (legacyApods != null)
                {
                    foreach (var (username, apods) in legacyApods)
                    {
                        foreach (var apod in apods)
                            await _database.AddOrUpdateApodFavoriteAsync(username, apod).ConfigureAwait(false);
                    }
                }
            }

            SecureStorage.Default.Remove(legacyPlanetsKey);
            SecureStorage.Default.Remove(legacyApodsKey);
            Debug.WriteLine("[FavoritesService] Legacy favorites migration completed.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Legacy favorites migration skipped.");
        }
    }
}
