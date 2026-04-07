using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MauiApp1.Services;
using MauiApp1.ViewModels;
using MauiApp1.Views;

namespace MauiApp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IFavoritesService, FavoritesService>();
        builder.Services.AddHttpClient<IExoplanetService, ExoplanetService>();
        builder.Services.AddSingleton<ICelestialBodyService, CelestialBodyService>();
        builder.Services.AddHttpClient<IApodService, ApodService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddSingleton<SolarSystemOverviewViewModel>();
        builder.Services.AddTransient<PlanetDetailViewModel>();
        builder.Services.AddSingleton<FavoritesViewModel>();
        builder.Services.AddSingleton<ApodViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<DailyTriviaViewModel>();
        builder.Services.AddSingleton<RandomCosmicNewsViewModel>();

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<SolarSystemOverviewPage>();
        builder.Services.AddTransient<PlanetDetailPage>();
        builder.Services.AddSingleton<FavoritesPage>();
        builder.Services.AddSingleton<ApodPage>();
        builder.Services.AddSingleton<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
