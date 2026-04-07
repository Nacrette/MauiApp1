using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage()
    {
        InitializeComponent();
        _viewModel = AppServices.GetRequiredService<SettingsViewModel>();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadUser();
    }

    private async void OnGoToLoginClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("login");
    }
}
