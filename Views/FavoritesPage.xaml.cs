using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class FavoritesPage : ContentPage
{
    private readonly FavoritesViewModel _viewModel;

    public FavoritesPage()
    {
        InitializeComponent();
        _viewModel = AppServices.GetRequiredService<FavoritesViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadFavoritesAsync();
    }
}
