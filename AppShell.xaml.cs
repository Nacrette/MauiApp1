using MauiApp1.Views;

namespace MauiApp1;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("planet-detail", typeof(PlanetDetailPage));
        Routing.RegisterRoute("flight-mode", typeof(FlightModePage));
        Routing.RegisterRoute("login", typeof(LoginPage));
    }
}
