using MauiApp1.Models;

namespace MauiApp1.Services;

public interface IFavoritesService
{
    IReadOnlyList<CelestialBody> GetFavoritePlanets(string username);
    IReadOnlyList<ApodFavorite> GetFavoriteApods(string username);
    Task<IReadOnlyList<CelestialBody>> GetFavoritePlanetsAsync(string username);
    void AddFavoritePlanet(string username, CelestialBody planet);
    void RemoveFavoritePlanet(string username, string planetName);
    bool IsPlanetFavorite(string username, string planetName);
    void AddFavoriteApod(string username, ApodFavorite apod);
    void RemoveFavoriteApod(string username, string apodDate);
    bool IsApodFavorite(string username, string apodDate);
    Task SaveAllAsync();
}
