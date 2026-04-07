using MauiApp1.Models;

namespace MauiApp1.Services;

public interface IExoplanetService
{
    Task<List<CelestialBody>> GetExoplanetsAsync();
}
