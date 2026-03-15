using System.Collections.ObjectModel;
using MauiApp1.Models;

namespace MauiApp1.Services;

public interface IFavoritesService
{
    ReadOnlyObservableCollection<Planet> Favorites { get; }
    bool IsFavorite(string planetName);
    void Add(Planet planet);
    void Remove(string planetName);
}

