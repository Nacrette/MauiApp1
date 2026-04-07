using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class FavoritesViewModel : ViewModelBase
{
    private readonly IFavoritesService _favoritesService;
    private readonly IAuthService _authService;

    public ObservableCollection<CelestialBody> FavoritePlanets { get; } = [];
    public ObservableCollection<ApodFavorite> FavoriteApods { get; } = [];

    [ObservableProperty]
    private bool hasPlanetFavorites;

    [ObservableProperty]
    private bool hasApodFavorites;

    [ObservableProperty]
    private bool isLoggedIn;

    public FavoritesViewModel(IFavoritesService favoritesService, IAuthService authService)
    {
        _favoritesService = favoritesService;
        _authService = authService;
    }

    public void LoadFavorites()
    {
        IsLoggedIn = _authService.IsLoggedIn;
        var username = _authService.CurrentUser?.Username ?? string.Empty;

        FavoritePlanets.Clear();
        FavoriteApods.Clear();

        if (!IsLoggedIn || string.IsNullOrEmpty(username))
        {
            HasPlanetFavorites = false;
            HasApodFavorites = false;
            return;
        }

        var planets = _favoritesService.GetFavoritePlanets(username);
        foreach (var planet in planets)
            FavoritePlanets.Add(planet);
        HasPlanetFavorites = FavoritePlanets.Count > 0;

        var apods = _favoritesService.GetFavoriteApods(username);
        foreach (var apod in apods)
            FavoriteApods.Add(apod);
        HasApodFavorites = FavoriteApods.Count > 0;
    }

    [RelayCommand]
    private Task OpenPlanetAsync(CelestialBody? planet)
    {
        if (planet is null)
            return Task.CompletedTask;

        return Shell.Current.GoToAsync("planet-detail", new Dictionary<string, object>
        {
            ["name"] = planet.Name
        });
    }

    [RelayCommand]
    private void RemovePlanetFavorite(CelestialBody? planet)
    {
        if (planet is null || !_authService.IsLoggedIn)
            return;

        var username = _authService.CurrentUser?.Username ?? string.Empty;
        _favoritesService.RemoveFavoritePlanet(username, planet.Name);
        FavoritePlanets.Remove(planet);
        HasPlanetFavorites = FavoritePlanets.Count > 0;
    }

    [RelayCommand]
    private Task ViewApodAsync(ApodFavorite? apod)
    {
        if (apod is null)
            return Task.CompletedTask;

        return Shell.Current.GoToAsync("apod-detail", new Dictionary<string, object>
        {
            ["date"] = apod.Date
        });
    }

    [RelayCommand]
    private void RemoveApodFavorite(ApodFavorite? apod)
    {
        if (apod is null || !_authService.IsLoggedIn)
            return;

        var username = _authService.CurrentUser?.Username ?? string.Empty;
        _favoritesService.RemoveFavoriteApod(username, apod.Date);
        FavoriteApods.Remove(apod);
        HasApodFavorites = FavoriteApods.Count > 0;
    }
}
