using MauiApp1.Views;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Non-tab routes
            Routing.RegisterRoute("planet-detail", typeof(PlanetDetailPage));
            Routing.RegisterRoute("random-news", typeof(RandomCosmicNewsPage));
            Routing.RegisterRoute("flight-mode", typeof(FlightModePage));
        }
    }
}
