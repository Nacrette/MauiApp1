using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class RandomCosmicNewsViewModel : ViewModelBase
{
    private readonly IApodService _apodService;

    [ObservableProperty] private string title = "Loading…";
    [ObservableProperty] private string dateText = "";
    [ObservableProperty] private string explanation = "";
    [ObservableProperty] private ImageSource? heroImage;

    [ObservableProperty] private bool isUsingFallback;

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
            var apod = await _apodService.GetRandomApodAsync(CancellationToken.None);
            Apply(apod);
        }
        catch (Exception ex)
        {
            // Offline / NASA error: fall back to a bundled image so the page still looks great.
            Title = "Cosmic Signal Lost";
            DateText = DateTime.UtcNow.ToString("yyyy-MM-dd");
            Explanation =
                "No internet connection (or NASA APOD is temporarily unavailable). " +
                "You’re seeing a local demo image — tap Explore Again when you’re back online.";

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
        DateText = apod.Date ?? _apodService.GetRandomDate().ToString("yyyy-MM-dd");
        Explanation = apod.Explanation ?? "";

        if (!string.Equals(apod.MediaType, "image", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("APOD was not an image for this date.");

        var url = apod.HdUrl ?? apod.Url;
        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException("APOD image url missing.");

        HeroImage = new UriImageSource
        {
            Uri = new Uri(url),
            CachingEnabled = true,
            CacheValidity = TimeSpan.FromDays(7)
        };
    }
}

