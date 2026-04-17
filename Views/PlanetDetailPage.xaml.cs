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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        OnPropertyChanged(nameof(BindingContext));
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
