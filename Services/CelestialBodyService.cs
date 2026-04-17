using Microsoft.Maui.Graphics;
using MauiApp1.Models;

namespace MauiApp1.Services;

public class CelestialBodyService : ICelestialBodyService
{
    private readonly IExoplanetService _exoplanetService;
    private List<CelestialBody>? _solarSystemCache;
    private List<CelestialBody>? _exoplanetCache;

    public CelestialBodyService(IExoplanetService exoplanetService)
    {
        _exoplanetService = exoplanetService;
    }

    public async Task<IReadOnlyList<CelestialBody>> GetSolarSystemPlanetsAsync()
    {
        if (_solarSystemCache != null)
            return _solarSystemCache;

        _solarSystemCache = new List<CelestialBody>
        {
            new() { Name = "Mercury", Type = CelestialBodyType.SolarSystemPlanet, Order = 1, Emoji = "\U0001F421", AccentColor = Color.FromArgb("#A0522D"), DistanceFromSun = 0.39, Mass = 0.055, Radius = 0.383, Description = "The smallest planet and closest to the Sun", HasRing = false },
            new() { Name = "Venus", Type = CelestialBodyType.SolarSystemPlanet, Order = 2, Emoji = "\U0001F460", AccentColor = Color.FromArgb("#FFD700"), DistanceFromSun = 0.72, Mass = 0.815, Radius = 0.949, Description = "The hottest planet with a toxic atmosphere", HasRing = false },
            new() { Name = "Earth", Type = CelestialBodyType.SolarSystemPlanet, Order = 3, Emoji = "\U0001F30D", AccentColor = Color.FromArgb("#4169E1"), DistanceFromSun = 1.0, Mass = 1.0, Radius = 1.0, Description = "Our home planet, the only known world with life", HasRing = false },
            new() { Name = "Mars", Type = CelestialBodyType.SolarSystemPlanet, Order = 4, Emoji = "\U0001F680", AccentColor = Color.FromArgb("#FF4500"), DistanceFromSun = 1.52, Mass = 0.107, Radius = 0.532, Description = "The Red Planet with the largest volcano", HasRing = false },
            new() { Name = "Jupiter", Type = CelestialBodyType.SolarSystemPlanet, Order = 5, Emoji = "\U0001F9F1", AccentColor = Color.FromArgb("#DAA520"), DistanceFromSun = 5.2, Mass = 317.8, Radius = 11.21, Description = "The largest planet with the Great Red Spot", HasRing = true },
            new() { Name = "Saturn", Type = CelestialBodyType.SolarSystemPlanet, Order = 6, Emoji = "\U0001F968", AccentColor = Color.FromArgb("#F4C430"), DistanceFromSun = 9.5, Mass = 95.2, Radius = 9.45, Description = "Famous for its stunning ring system", HasRing = true },
            new() { Name = "Uranus", Type = CelestialBodyType.SolarSystemPlanet, Order = 7, Emoji = "\U0001F47C", AccentColor = Color.FromArgb("#40E0D0"), DistanceFromSun = 19.2, Mass = 14.5, Radius = 4.01, Description = "The ice giant that rotates on its side", HasRing = true },
            new() { Name = "Neptune", Type = CelestialBodyType.SolarSystemPlanet, Order = 8, Emoji = "\U0001F30F", AccentColor = Color.FromArgb("#4169E1"), DistanceFromSun = 30.1, Mass = 17.1, Radius = 3.88, Description = "The windiest planet with supersonic storms", HasRing = true }
        };

        return _solarSystemCache;
    }

    public async Task<IReadOnlyList<CelestialBody>> GetExoplanetsAsync()
    {
        if (_exoplanetCache != null)
            return _exoplanetCache;

        _exoplanetCache = await _exoplanetService.GetExoplanetsAsync();
        return _exoplanetCache;
    }

    public async Task<IReadOnlyList<CelestialBody>> RefreshExoplanetsAsync()
    {
        _exoplanetService.InvalidateCache();
        _exoplanetCache = null;
        return await GetExoplanetsAsync();
    }

    public async Task<IReadOnlyList<CelestialBody>> SearchCelestialBodiesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<CelestialBody>();

        query = query.ToLowerInvariant();
        var solarSystem = await GetSolarSystemPlanetsAsync();
        var exoplanets = await GetExoplanetsAsync();

        var results = new List<CelestialBody>();
        results.AddRange(solarSystem.Where(p => p.Name.ToLowerInvariant().Contains(query)));
        results.AddRange(exoplanets.Where(p => p.Name.ToLowerInvariant().Contains(query) || 
                                               (p.HostStar?.ToLowerInvariant().Contains(query) ?? false)));
        return results;
    }

    public async Task<CelestialBody?> GetCelestialBodyByNameAsync(string name)
    {
        var solarSystem = await GetSolarSystemPlanetsAsync();
        var planet = solarSystem.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (planet != null) return planet;

        var exoplanets = await GetExoplanetsAsync();
        return exoplanets.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
