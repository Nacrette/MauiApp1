using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public partial class ApodViewModel : ViewModelBase
{
    private readonly IApodService _apodService;
    private readonly IFavoritesService _favoritesService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ApodModel? _apod;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _date = string.Empty;

    [ObservableProperty]
    private string _explanation = string.Empty;

    [ObservableProperty]
    private string _imageUrl = string.Empty;

    [ObservableProperty]
    private string _hdImageUrl = string.Empty;

    [ObservableProperty]
    private string _copyright = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private bool _isImageLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ApodViewModel(IApodService apodService, IFavoritesService favoritesService, IAuthService authService)
    {
        _apodService = apodService;
        _favoritesService = favoritesService;
        _authService = authService;
    }

    [RelayCommand]
    public async Task LoadTodaysApodAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            Apod = await _apodService.GetTodaysApodAsync();
            UpdateFromApod();
            UpdateFavoriteStatus();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load APOD: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ToggleFavorite()
    {
        if (Apod == null || !_authService.IsLoggedIn) return;

        var username = _authService.CurrentUser?.Username ?? string.Empty;
        var apodFavorite = new ApodFavorite
        {
            Title = Apod.Title ?? "Unknown",
            Date = Apod.Date ?? DateTime.Now.ToString("yyyy-MM-dd"),
            Url = Apod.Url ?? string.Empty,
            HdUrl = Apod.HdUrl ?? string.Empty,
            Explanation = Apod.Explanation ?? string.Empty,
            MediaType = Apod.MediaType ?? "image",
            Copyright = Apod.Copyright
        };

        if (IsFavorite)
        {
            _favoritesService.RemoveFavoriteApod(username, apodFavorite.Date);
        }
        else
        {
            _favoritesService.AddFavoriteApod(username, apodFavorite);
        }

        IsFavorite = !IsFavorite;
    }

    private void UpdateFromApod()
    {
        if (Apod == null) return;

        Title = Apod.Title ?? "NASA Astronomy Picture";
        Date = Apod.Date ?? string.Empty;
        Explanation = Apod.Explanation ?? string.Empty;
        ImageUrl = Apod.Url ?? string.Empty;
        HdImageUrl = Apod.HdUrl ?? Apod.Url ?? string.Empty;
        Copyright = Apod.Copyright ?? "NASA";
    }

    private void UpdateFavoriteStatus()
    {
        if (Apod == null || !_authService.IsLoggedIn)
        {
            IsFavorite = false;
            return;
        }

        var username = _authService.CurrentUser?.Username ?? string.Empty;
        IsFavorite = _favoritesService.IsApodFavorite(username, Apod.Date ?? string.Empty);
    }
}
