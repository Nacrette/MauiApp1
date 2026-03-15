using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Helpers;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class PlanetDetailViewModel : ViewModelBase, IQueryAttributable
{
    private readonly IPlanetService _planetService;
    private readonly IFavoritesService _favorites;

    [ObservableProperty] private Planet? planet;
    public ObservableCollection<KeyValuePair<string, string>> FactCards { get; } = [];

    [ObservableProperty] private bool isFavorite;

    public PlanetDetailViewModel(IPlanetService planetService, IFavoritesService favorites)
    {
        _planetService = planetService;
        _favorites = favorites;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("name", out var raw) || raw is not string name || string.IsNullOrWhiteSpace(name))
            return;

        LoadByName(name);
    }

    private void LoadByName(string name)
    {
        var p = _planetService.GetByName(name);
        Planet = p;

        FactCards.Clear();
        if (p?.Facts is not null)
        {
            foreach (var kv in p.Facts)
                FactCards.Add(kv);
        }

        IsFavorite = p is not null && _favorites.IsFavorite(p.Name);
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
        if (Planet is null)
            return;

        if (_favorites.IsFavorite(Planet.Name))
            _favorites.Remove(Planet.Name);
        else
            _favorites.Add(Planet);

        IsFavorite = _favorites.IsFavorite(Planet.Name);
    }
}

