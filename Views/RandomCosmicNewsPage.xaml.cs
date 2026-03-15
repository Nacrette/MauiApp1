using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class RandomCosmicNewsPage : ContentPage
{
    private readonly RandomCosmicNewsViewModel _vm;

    public RandomCosmicNewsPage()
    {
        InitializeComponent();
        _vm = AppServices.GetRequiredService<RandomCosmicNewsViewModel>();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.OnNavigatedToAsync();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

