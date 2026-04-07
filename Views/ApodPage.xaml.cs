using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class ApodPage : ContentPage
{
    private readonly ApodViewModel _viewModel;

    public ApodPage()
    {
        InitializeComponent();
        _viewModel = AppServices.GetRequiredService<ApodViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTodaysApodCommand.ExecuteAsync(null);
    }
}
