using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1.Helpers;

public static class AppServices
{
    private static IServiceProvider? _root;

    /// <summary>
    /// Set from <see cref="Application"/> constructor so pages resolved before
    /// <c>Application.Handler.MauiContext</c> exists can still resolve services.
    /// </summary>
    public static void Initialize(IServiceProvider root) => _root = root;

    public static T GetRequiredService<T>() where T : notnull
    {
        var services = Current;
        return services.GetRequiredService<T>();
    }

    public static IServiceProvider Current =>
        _root
        ?? Application.Current?.Handler?.MauiContext?.Services
        ?? throw new InvalidOperationException("App services are not available yet.");
}

