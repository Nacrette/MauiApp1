using System.Net.Http.Json;
using MauiApp1.Models;

namespace MauiApp1.Services;

public sealed class ApodService(HttpClient httpClient) : IApodService
{
    private static readonly DateOnly MinDate = new(1995, 06, 16);

    // NASA APOD docs: https://api.nasa.gov/
    public DateOnly GetRandomDate()
    {
        var max = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var rangeDays = max.DayNumber - MinDate.DayNumber;
        var randomOffset = Random.Shared.Next(0, Math.Max(1, rangeDays + 1));
        return DateOnly.FromDayNumber(MinDate.DayNumber + randomOffset);
    }

    public async Task<ApodModel> GetRandomApodAsync(CancellationToken cancellationToken)
    {
        var date = GetRandomDate();
        var url = $"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={date:yyyy-MM-dd}";

        // Keep this resilient: NASA sometimes returns 4xx/5xx or video media types.
        using var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var model = await response.Content.ReadFromJsonAsync<ApodModel>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return model ?? new ApodModel { Date = date.ToString("yyyy-MM-dd"), Title = "Unknown" };
    }
}

