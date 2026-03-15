using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class SolarSystemOverviewViewModel : ViewModelBase
{
    private readonly IPlanetService _planetService;

    public ObservableCollection<Planet> VisiblePlanets { get; } = [];

    private readonly IReadOnlyList<Planet> _allPlanets;

    [ObservableProperty]
    private string? searchText;

    public SolarSystemOverviewViewModel(IPlanetService planetService)
    {
        _planetService = planetService;
        _allPlanets = _planetService.GetPlanets();
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string? value) => ApplyFilter();

    private void ApplyFilter()
    {
        VisiblePlanets.Clear();

        var q = (SearchText ?? string.Empty).Trim();
        var filtered = string.IsNullOrWhiteSpace(q)
            ? _allPlanets
            : _allPlanets.Where(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var planet in filtered)
            VisiblePlanets.Add(planet);
    }

    [RelayCommand]
    private Task SelectPlanetAsync(Planet? planet)
    {
        if (planet is null)
            return Task.CompletedTask;

        return Shell.Current.GoToAsync("planet-detail", new Dictionary<string, object>
        {
            ["name"] = planet.Name
        });
    }

    [RelayCommand]
    private Task RandomExploreAsync() => Shell.Current.GoToAsync("random-news");
}

