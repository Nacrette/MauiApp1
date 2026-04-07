using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using MauiApp1.Models;

namespace MauiApp1.Services;

public class FavoritesService : IFavoritesService
{
    private readonly ILogger<FavoritesService> _logger;
    private readonly Dictionary<string, List<CelestialBody>> _planetFavorites = new();
    private readonly Dictionary<string, List<ApodFavorite>> _apodFavorites = new();
    private const string PlanetsKey = "favorite_planets";
    private const string ApodsKey = "favorite_apods";
    private bool _isLoaded;

    public FavoritesService(ILogger<FavoritesService> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<CelestialBody> GetFavoritePlanets(string username)
    {
        return _planetFavorites.TryGetValue(username, out var favorites) 
            ? favorites.AsReadOnly() 
            : new List<CelestialBody>().AsReadOnly();
    }

    public IReadOnlyList<ApodFavorite> GetFavoriteApods(string username)
    {
        return _apodFavorites.TryGetValue(username, out var favorites) 
            ? favorites.AsReadOnly() 
            : new List<ApodFavorite>().AsReadOnly();
    }

    public async Task<IReadOnlyList<CelestialBody>> GetFavoritePlanetsAsync(string username)
    {
        await EnsureLoadedAsync();
        return GetFavoritePlanets(username);
    }

    public void AddFavoritePlanet(string username, CelestialBody planet)
    {
        EnsureUserExists(username);
        
        if (!_planetFavorites[username].Any(p => p.Name == planet.Name))
        {
            _planetFavorites[username].Add(planet);
            _ = SaveAllAsync();
        }
    }

    public void RemoveFavoritePlanet(string username, string planetName)
    {
        if (_planetFavorites.TryGetValue(username, out var favorites))
        {
            favorites.RemoveAll(p => p.Name == planetName);
            _ = SaveAllAsync();
        }
    }

    public bool IsPlanetFavorite(string username, string planetName)
    {
        return _planetFavorites.TryGetValue(username, out var favorites) 
            && favorites.Any(p => p.Name == planetName);
    }

    public void AddFavoriteApod(string username, ApodFavorite apod)
    {
        EnsureUserExists(username);
        
        if (!_apodFavorites[username].Any(a => a.Date == apod.Date))
        {
            _apodFavorites[username].Add(apod);
            _ = SaveAllAsync();
        }
    }

    public void RemoveFavoriteApod(string username, string apodDate)
    {
        if (_apodFavorites.TryGetValue(username, out var favorites))
        {
            favorites.RemoveAll(a => a.Date == apodDate);
            _ = SaveAllAsync();
        }
    }

    public bool IsApodFavorite(string username, string apodDate)
    {
        return _apodFavorites.TryGetValue(username, out var favorites) 
            && favorites.Any(a => a.Date == apodDate);
    }

    public async Task SaveAllAsync()
    {
        try
        {
            var planetsJson = JsonSerializer.Serialize(_planetFavorites);
            await SecureStorage.Default.SetAsync(PlanetsKey, planetsJson);

            var apodsJson = JsonSerializer.Serialize(_apodFavorites);
            await SecureStorage.Default.SetAsync(ApodsKey, apodsJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save favorites to storage");
        }
    }

    private async Task EnsureLoadedAsync()
    {
        if (_isLoaded) return;
        
        try
        {
            var planetsJson = await SecureStorage.Default.GetAsync(PlanetsKey);
            if (!string.IsNullOrEmpty(planetsJson))
            {
                var planets = JsonSerializer.Deserialize<Dictionary<string, List<CelestialBody>>>(planetsJson);
                if (planets != null)
                {
                    foreach (var kvp in planets)
                    {
                        _planetFavorites[kvp.Key] = kvp.Value;
                    }
                }
            }

            var apodsJson = await SecureStorage.Default.GetAsync(ApodsKey);
            if (!string.IsNullOrEmpty(apodsJson))
            {
                var apods = JsonSerializer.Deserialize<Dictionary<string, List<ApodFavorite>>>(apodsJson);
                if (apods != null)
                {
                    foreach (var kvp in apods)
                    {
                        _apodFavorites[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load favorites from storage");
        }

        _isLoaded = true;
    }

    private void EnsureUserExists(string username)
    {
        if (!_planetFavorites.ContainsKey(username))
            _planetFavorites[username] = new List<CelestialBody>();
        if (!_apodFavorites.ContainsKey(username))
            _apodFavorites[username] = new List<ApodFavorite>();
    }
}
