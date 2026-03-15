using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MauiApp1.Services;
using MauiApp1.ViewModels;
using MauiApp1.Views;

namespace MauiApp1
{
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
                    // fonts.AddFont("Orbitron-Regular.ttf", "Orbitron");
                });

            // Services
            builder.Services.AddSingleton<IPlanetService, PlanetService>();
            builder.Services.AddSingleton<IFavoritesService, FavoritesService>();
            builder.Services.AddHttpClient<IApodService, ApodService>();

            // ViewModels
            builder.Services.AddSingleton<SolarSystemOverviewViewModel>();
            builder.Services.AddTransient<RandomCosmicNewsViewModel>();
            builder.Services.AddTransient<PlanetDetailViewModel>();
            builder.Services.AddSingleton<FavoritesViewModel>();
            builder.Services.AddSingleton<DailyTriviaViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();

            // Views (Pages)
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<SolarSystemOverviewPage>();
            builder.Services.AddTransient<RandomCosmicNewsPage>();
            builder.Services.AddTransient<PlanetDetailPage>();
            builder.Services.AddSingleton<FavoritesPage>();
            builder.Services.AddSingleton<DailyTriviaPage>();
            builder.Services.AddSingleton<SettingsPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
