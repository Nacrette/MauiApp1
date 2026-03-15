using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class PlanetDetailPage : ContentPage
{
    public PlanetDetailPage()
    {
        InitializeComponent();
        BindingContext = AppServices.GetRequiredService<PlanetDetailViewModel>();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

