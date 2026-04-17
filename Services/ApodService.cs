using System.Net.Http.Json;
using System.Diagnostics;
using MauiApp1.Models;

namespace MauiApp1.Services;

public sealed class ApodService : IApodService
{
    private readonly HttpClient _httpClient;
    private ApodModel? _todaysCache;
    private DateOnly? _todaysCacheDate;

    public ApodService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApodModel> GetTodaysApodAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        
        if (_todaysCache != null && _todaysCacheDate == today)
            return _todaysCache;

        try
        {
            // Try today first; if unavailable, walk backwards to find a real APOD.
            for (var candidate = today; candidate >= new DateOnly(1995, 6, 16); candidate = candidate.AddDays(-1))
            {
                var model = await TryFetchApodByDateAsync(candidate, cancellationToken);
                if (model is not null)
                {
                    _todaysCache = model;
                    _todaysCacheDate = today;
                    Debug.WriteLine($"[ApodService] APOD resolved using date: {candidate:yyyy-MM-dd}");
                    return model;
                }

                // Avoid scanning too far for long outages.
                if ((today.DayNumber - candidate.DayNumber) >= 10)
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ApodService] Failed to load today's APOD, fallback used. Error: {ex.Message}");
        }

        return new ApodModel
        {
            Title = "Milky Way Explorer",
            Date = today.ToString("yyyy-MM-dd"),
            Explanation = "Welcome to the Milky Way Explorer! Today's astronomical picture is temporarily unavailable. Please check your internet connection and try again.",
            Url = "",
            HdUrl = "",
            MediaType = "image",
            Copyright = "NASA"
        };
    }

    public async Task<ApodModel> GetApodByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var model = await TryFetchApodByDateAsync(date, cancellationToken);
        if (model is not null)
            return model;

        Debug.WriteLine($"[ApodService] Failed to load APOD by date {date:yyyy-MM-dd}, fallback model used.");
        return new ApodModel
        {
            Title = "Image Unavailable",
            Date = date.ToString("yyyy-MM-dd"),
            Explanation = "The astronomical picture for this date is not available.",
            Url = "",
            HdUrl = "",
            MediaType = "image"
        };
    }

    private async Task<ApodModel?> TryFetchApodByDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var url = $"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={date:yyyy-MM-dd}";

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(15));
            Debug.WriteLine($"[ApodService] Fetching APOD by date: {date:yyyy-MM-dd}");
            using var response = await _httpClient.GetAsync(url, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"[ApodService] APOD request failed for {date:yyyy-MM-dd}, status={(int)response.StatusCode}");
                return null;
            }

            var model = await response.Content.ReadFromJsonAsync<ApodModel>(cancellationToken: cts.Token);
            return model;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ApodService] APOD exception for {date:yyyy-MM-dd}: {ex.Message}");
            return null;
        }
    }
}
