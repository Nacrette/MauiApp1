using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class SolarSystemOverviewViewModel : ViewModelBase
{
    private readonly ICelestialBodyService _celestialBodyService;
    private readonly IAuthService _authService;

    public ObservableCollection<CelestialBody> SolarSystemPlanets { get; } = [];
    public ObservableCollection<CelestialBody> Exoplanets { get; } = [];
    public ObservableCollection<CelestialBody> SearchResults { get; } = [];

    private IReadOnlyList<CelestialBody> _allSolarSystem = Array.Empty<CelestialBody>();
    private IReadOnlyList<CelestialBody> _allExoplanets = Array.Empty<CelestialBody>();
    private bool _initialDataLoaded;

    [ObservableProperty]
    private string? searchText;

    [ObservableProperty]
    private bool isSearching;

    [ObservableProperty]
    private bool isLoadingSolarSystem = true;

    [ObservableProperty]
    private bool isLoadingExoplanets = true;

    [ObservableProperty]
    private int selectedTabIndex;

    public SolarSystemOverviewViewModel(ICelestialBodyService celestialBodyService, IAuthService authService)
    {
        _celestialBodyService = celestialBodyService;
        _authService = authService;
    }

    /// <summary>First visit to Home only — avoids refetching when switching Shell tabs.</summary>
    [RelayCommand]
    public async Task EnsureInitialDataAsync()
    {
        if (_initialDataLoaded)
            return;

        IsLoadingSolarSystem = true;
        try
        {
            _allSolarSystem = await _celestialBodyService.GetSolarSystemPlanetsAsync();
            SolarSystemPlanets.Clear();
            foreach (var planet in _allSolarSystem)
                SolarSystemPlanets.Add(planet);
        }
        finally
        {
            IsLoadingSolarSystem = false;
        }

        IsLoadingExoplanets = true;
        try
        {
            _allExoplanets = await _celestialBodyService.GetExoplanetsAsync();
            Exoplanets.Clear();
            foreach (var planet in _allExoplanets)
                Exoplanets.Add(planet);
        }
        finally
        {
            IsLoadingExoplanets = false;
        }

        _initialDataLoaded = true;
    }

    [RelayCommand]
    public async Task RefreshExoplanetsAsync()
    {
        if (IsLoadingExoplanets)
            return;

        IsLoadingExoplanets = true;
        try
        {
            _allExoplanets = await _celestialBodyService.RefreshExoplanetsAsync();
            Exoplanets.Clear();
            foreach (var planet in _allExoplanets)
                Exoplanets.Add(planet);
            ApplySearch();
        }
        finally
        {
            IsLoadingExoplanets = false;
        }
    }

    partial void OnSearchTextChanged(string? value)
    {
        ApplySearch();
    }

    private void ApplySearch()
    {
        var q = (SearchText ?? string.Empty).Trim();
        
        if (string.IsNullOrWhiteSpace(q))
        {
            IsSearching = false;
            SearchResults.Clear();
            return;
        }

        IsSearching = true;
        SearchResults.Clear();

        var results = _allSolarSystem
            .Where(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
            .Concat(_allExoplanets.Where(p => 
                p.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                (p.HostStar?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)))
            .Take(20);

        foreach (var result in results)
            SearchResults.Add(result);
    }

    [RelayCommand]
    private Task SelectCelestialBodyAsync(CelestialBody? body)
    {
        if (body is null)
            return Task.CompletedTask;

        return Shell.Current.GoToAsync("planet-detail", new Dictionary<string, object>
        {
            ["name"] = body.Name
        });
    }
}
