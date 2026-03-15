using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class FavoritesViewModel : ViewModelBase
{
    private readonly IFavoritesService _favorites;

    public ReadOnlyObservableCollection<Planet> Favorites => _favorites.Favorites;

    public FavoritesViewModel(IFavoritesService favorites)
    {
        _favorites = favorites;
    }

    [RelayCommand]
    private Task OpenPlanetAsync(Planet? planet)
    {
        if (planet is null)
            return Task.CompletedTask;

        return Shell.Current.GoToAsync("planet-detail", new Dictionary<string, object>
        {
            ["name"] = planet.Name
        });
    }

    [RelayCommand]
    private void RemoveFavorite(Planet? planet)
    {
        if (planet is null)
            return;

        _favorites.Remove(planet.Name);
        OnPropertyChanged(nameof(Favorites));
    }
}

