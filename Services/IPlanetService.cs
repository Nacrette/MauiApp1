using MauiApp1.Models;

namespace MauiApp1.Services;

public interface IPlanetService
{
    IReadOnlyList<Planet> GetPlanets();
    Planet? GetByName(string name);
}

