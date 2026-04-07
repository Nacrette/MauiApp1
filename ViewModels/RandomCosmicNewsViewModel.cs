using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class RandomCosmicNewsViewModel : ViewModelBase
{
    private readonly IApodService _apodService;

    [ObservableProperty]
    private string title = "Loading\u2026";

    [ObservableProperty]
    private string dateText = "";

    [ObservableProperty]
    private string explanation = "";

    [ObservableProperty]
    private ImageSource? heroImage;

    [ObservableProperty]
    private bool isUsingFallback;

    public RandomCosmicNewsViewModel(IApodService apodService)
    {
        _apodService = apodService;
    }

    public async Task OnNavigatedToAsync()
    {
        if (HeroImage is null)
            await ExploreAgainAsync();
    }

    [RelayCommand]
    private async Task ExploreAgainAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = null;
        IsUsingFallback = false;

        try
        {
            var minDate = new DateOnly(1995, 6, 16);
            var maxDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var rangeDays = maxDate.DayNumber - minDate.DayNumber;
            var randomOffset = Random.Shared.Next(0, Math.Max(1, rangeDays + 1));
            var randomDate = DateOnly.FromDayNumber(minDate.DayNumber + randomOffset);

            var apod = await _apodService.GetApodByDateAsync(randomDate);
            Apply(apod);
        }
        catch (Exception ex)
        {
            Title = "Cosmic Signal Lost";
            DateText = DateTime.UtcNow.ToString("yyyy-MM-dd");
            Explanation =
                "No internet connection (or NASA APOD is temporarily unavailable). " +
                "You're seeing a local demo image \u2014 tap Explore Again when you're back online.";

            HeroImage = ImageSource.FromFile("dotnet_bot.png");
            IsUsingFallback = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Apply(ApodModel apod)
    {
        Title = apod.Title ?? "NASA APOD";
        DateText = apod.Date ?? string.Empty;
        Explanation = apod.Explanation ?? string.Empty;

        if (!string.Equals(apod.MediaType, "image", StringComparison.OrdinalIgnoreCase))
        {
            Title = "Video Content";
            Explanation = "This APOD is a video. Please visit NASA's website to view it.";
            HeroImage = ImageSource.FromFile("dotnet_bot.png");
            return;
        }

        var url = apod.HdUrl ?? apod.Url;
        if (string.IsNullOrWhiteSpace(url))
        {
            HeroImage = ImageSource.FromFile("dotnet_bot.png");
            return;
        }

        HeroImage = new UriImageSource
        {
            Uri = new Uri(url),
            CachingEnabled = true,
            CacheValidity = TimeSpan.FromDays(7)
        };
    }
}
