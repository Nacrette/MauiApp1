using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class SolarSystemOverviewPage : ContentPage
{
    public SolarSystemOverviewPage()
    {
        InitializeComponent();
        BindingContext = AppServices.GetRequiredService<SolarSystemOverviewViewModel>();
    }
}

