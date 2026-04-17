using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Microsoft.Maui.Devices;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public partial class ApodViewModel : ViewModelBase
{
    private readonly IApodService _apodService;
    private readonly IFavoritesService _favoritesService;
    private readonly IAuthService _authService;
    private readonly IApodNotificationService _apodNotificationService;

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

    public ApodViewModel(
        IApodService apodService,
        IFavoritesService favoritesService,
        IAuthService authService,
        IApodNotificationService apodNotificationService)
    {
        _apodService = apodService;
        _favoritesService = favoritesService;
        _authService = authService;
        _apodNotificationService = apodNotificationService;
    }

    public Task NotifyApodPageEnteredAsync() => _apodNotificationService.SendWelcomeNotificationOncePerSessionAsync();

    [RelayCommand]
    public async Task LoadTodaysApodAsync()
    {
        if (IsLoading)
        {
            Debug.WriteLine("[ApodViewModel] LoadTodaysApodAsync ignored: already loading.");
            return;
        }

        IsLoading = true;
        IsImageLoading = true;
        ErrorMessage = string.Empty;
        Debug.WriteLine("[ApodViewModel] Loading today's APOD...");

        try
        {
            Apod = await _apodService.GetTodaysApodAsync();
            UpdateFromApod();
            await _favoritesService.EnsureLoadedAsync();
            UpdateFavoriteStatus();
            Debug.WriteLine("[ApodViewModel] Today's APOD loaded.");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load APOD: {ex.Message}";
            Debug.WriteLine($"[ApodViewModel] LoadTodaysApodAsync error: {ex}");
        }
        finally
        {
            IsImageLoading = false;
            IsLoading = false;
        }
    }

    /// <summary>Load a specific APOD by calendar date (e.g. from Favorites navigation).</summary>
    public async Task LoadApodForDateAsync(DateOnly date)
    {
        if (IsLoading)
        {
            Debug.WriteLine("[ApodViewModel] LoadApodForDateAsync ignored: already loading.");
            return;
        }

        IsLoading = true;
        IsImageLoading = true;
        ErrorMessage = string.Empty;
        Debug.WriteLine($"[ApodViewModel] Loading APOD for date: {date:yyyy-MM-dd}");

        try
        {
            Apod = await _apodService.GetApodByDateAsync(date);
            UpdateFromApod();
            await _favoritesService.EnsureLoadedAsync();
            UpdateFavoriteStatus();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load APOD: {ex.Message}";
            Debug.WriteLine($"[ApodViewModel] LoadApodForDateAsync error: {ex}");
        }
        finally
        {
            IsImageLoading = false;
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoadRandomApodAsync()
    {
        if (IsLoading)
        {
            Debug.WriteLine("[ApodViewModel] LoadRandomApodAsync ignored: already loading.");
            return;
        }

        IsLoading = true;
        IsImageLoading = true;
        ErrorMessage = string.Empty;

        var startDate = new DateOnly(1995, 6, 16);
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var totalDays = endDate.DayNumber - startDate.DayNumber + 1;
        var randomDayOffset = Random.Shared.Next(totalDays);
        var randomDate = startDate.AddDays(randomDayOffset);

        Debug.WriteLine($"[ApodViewModel] Loading random APOD: {randomDate:yyyy-MM-dd}");

        try
        {
            Apod = await _apodService.GetApodByDateAsync(randomDate);
            UpdateFromApod();
            await _favoritesService.EnsureLoadedAsync();
            UpdateFavoriteStatus();
            Debug.WriteLine("[ApodViewModel] Random APOD loaded.");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load random APOD: {ex.Message}";
            Debug.WriteLine($"[ApodViewModel] LoadRandomApodAsync error: {ex}");
        }
        finally
        {
            IsImageLoading = false;
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
            TriggerFavoriteHapticFeedback();
        }

        IsFavorite = !IsFavorite;
    }

    private static void TriggerFavoriteHapticFeedback()
    {
        var platform = DeviceInfo.Current.Platform;
        if (platform != DevicePlatform.Android && platform != DevicePlatform.iOS)
        {
            Debug.WriteLine("[ApodViewModel] Haptic feedback skipped (non-mobile platform).");
            return;
        }

        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            Debug.WriteLine("[ApodViewModel] Haptic feedback triggered.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ApodViewModel] Haptic feedback failed: {ex.Message}");
        }
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
