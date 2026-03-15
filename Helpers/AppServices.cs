using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1.Helpers;

public static class AppServices
{
    public static T GetRequiredService<T>() where T : notnull
    {
        var services = Current;
        return services.GetRequiredService<T>();
    }

    public static IServiceProvider Current =>
        Application.Current?.Handler?.MauiContext?.Services
        ?? throw new InvalidOperationException("App services are not available yet.");
}

