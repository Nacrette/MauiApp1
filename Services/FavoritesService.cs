using System.Collections.ObjectModel;
using MauiApp1.Models;

namespace MauiApp1.Services;

public sealed class FavoritesService : IFavoritesService
{
    private readonly ObservableCollection<Planet> _favorites = [];
    private readonly ReadOnlyObservableCollection<Planet> _readOnly;

    public FavoritesService()
    {
        _readOnly = new ReadOnlyObservableCollection<Planet>(_favorites);
    }

    public ReadOnlyObservableCollection<Planet> Favorites => _readOnly;

    public bool IsFavorite(string planetName) =>
        _favorites.Any(p => string.Equals(p.Name, planetName, StringComparison.OrdinalIgnoreCase));

    public void Add(Planet planet)
    {
        if (IsFavorite(planet.Name))
            return;

        _favorites.Add(planet);
    }

    public void Remove(string planetName)
    {
        var existing = _favorites.FirstOrDefault(p => string.Equals(p.Name, planetName, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
            _favorites.Remove(existing);
    }
}

