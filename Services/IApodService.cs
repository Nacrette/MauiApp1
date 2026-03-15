using MauiApp1.Models;

namespace MauiApp1.Services;

public interface IApodService
{
    Task<ApodModel> GetRandomApodAsync(CancellationToken cancellationToken);
    DateOnly GetRandomDate();
}

