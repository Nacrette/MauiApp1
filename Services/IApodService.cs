using MauiApp1.Models;

namespace MauiApp1.Services;

public interface IApodService
{
    Task<ApodModel> GetTodaysApodAsync(CancellationToken cancellationToken = default);
    Task<ApodModel> GetApodByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
}
