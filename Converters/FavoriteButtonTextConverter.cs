using System.Globalization;

namespace MauiApp1.Converters;

public sealed class FavoriteButtonTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isFav = value is bool b && b;
        return isFav ? "★ Favorited" : "Add to Favorites";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

