using System.Net.Http.Json;
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

        var url = $"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={today:yyyy-MM-dd}";
        
        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var model = await response.Content.ReadFromJsonAsync<ApodModel>(cancellationToken: cancellationToken);
            
            if (model != null)
            {
                _todaysCache = model;
                _todaysCacheDate = today;
                return model;
            }
        }
        catch
        {
            // Return fallback if API fails
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
        var url = $"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={date:yyyy-MM-dd}";
        
        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var model = await response.Content.ReadFromJsonAsync<ApodModel>(cancellationToken: cancellationToken);
            return model ?? new ApodModel { Title = "Unknown", Date = date.ToString("yyyy-MM-dd") };
        }
        catch
        {
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
    }
}
