using MauiApp1.Views;

namespace MauiApp1;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("planet-detail", typeof(PlanetDetailPage));
        Routing.RegisterRoute("apod-detail", typeof(ApodPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
    }
}
