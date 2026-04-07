using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class SolarSystemOverviewPage : ContentPage
{
    private readonly SolarSystemOverviewViewModel _viewModel;

    public SolarSystemOverviewPage()
    {
        InitializeComponent();
        _viewModel = AppServices.GetRequiredService<SolarSystemOverviewViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
