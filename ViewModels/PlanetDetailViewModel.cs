using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class PlanetDetailViewModel : ViewModelBase, IQueryAttributable
{
    private readonly ICelestialBodyService _celestialBodyService;
    private readonly IFavoritesService _favoritesService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private CelestialBody? planet;

    public ObservableCollection<KeyValuePair<string, string>> FactCards { get; } = [];

    [ObservableProperty]
    private bool isFavorite;

    [ObservableProperty]
    private bool isLoggedIn;

    public PlanetDetailViewModel(ICelestialBodyService celestialBodyService, IFavoritesService favoritesService, IAuthService authService)
    {
        _celestialBodyService = celestialBodyService;
        _favoritesService = favoritesService;
        _authService = authService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("name", out var raw) || raw is not string name || string.IsNullOrWhiteSpace(name))
            return;

        LoadByName(name);
    }

    private async void LoadByName(string name)
    {
        var p = await _celestialBodyService.GetCelestialBodyByNameAsync(name);
        Planet = p;
        IsLoggedIn = _authService.IsLoggedIn;

        FactCards.Clear();
        if (p != null)
        {
            FactCards.Add(new("Type", p.Type == CelestialBodyType.SolarSystemPlanet ? "Solar System Planet" : "Exoplanet"));
            if (p.DiscoveryYear.HasValue)
                FactCards.Add(new("Discovery Year", p.DiscoveryYear.ToString()!));
            if (!string.IsNullOrEmpty(p.HostStar))
                FactCards.Add(new("Host Star", p.HostStar));
            FactCards.Add(new("Distance", p.DisplayDistance));
            FactCards.Add(new("Mass", p.DisplayMass));
            FactCards.Add(new("Radius", p.DisplayRadius));
            if (!string.IsNullOrEmpty(p.OrbitalPeriod) && p.OrbitalPeriod != "Unknown")
                FactCards.Add(new("Orbital Period", $"{p.OrbitalPeriod} days"));
            FactCards.Add(new("Description", p.Description));
        }

        UpdateFavoriteStatus();
    }

    private void UpdateFavoriteStatus()
    {
        if (Planet == null || !_authService.IsLoggedIn)
        {
            IsFavorite = false;
            return;
        }

        var username = _authService.CurrentUser?.Username ?? string.Empty;
        IsFavorite = _favoritesService.IsPlanetFavorite(username, Planet.Name);
    }

    [RelayCommand]
    private async Task EnterFlightModeAsync()
    {
        if (Planet is null)
            return;

        await Shell.Current.GoToAsync("flight-mode", new Dictionary<string, object>
        {
            ["name"] = Planet.Name,
            ["accent"] = Planet.AccentColor.ToArgbHex()
        });
    }

    [RelayCommand]
    private void ToggleFavorite()
    {
        if (Planet == null || !_authService.IsLoggedIn)
            return;

        var username = _authService.CurrentUser?.Username ?? string.Empty;

        if (IsFavorite)
            _favoritesService.RemoveFavoritePlanet(username, Planet.Name);
        else
            _favoritesService.AddFavoritePlanet(username, Planet);

        IsFavorite = !IsFavorite;
    }
}
