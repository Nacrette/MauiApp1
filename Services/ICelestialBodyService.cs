using MauiApp1.Models;

namespace MauiApp1.Services;

public interface ICelestialBodyService
{
    Task<IReadOnlyList<CelestialBody>> GetSolarSystemPlanetsAsync();
    Task<IReadOnlyList<CelestialBody>> GetExoplanetsAsync();
    Task<IReadOnlyList<CelestialBody>> SearchCelestialBodiesAsync(string query);
    Task<CelestialBody?> GetCelestialBodyByNameAsync(string name);
}
