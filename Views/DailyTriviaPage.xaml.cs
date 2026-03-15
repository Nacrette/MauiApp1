using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class DailyTriviaPage : ContentPage
{
    public DailyTriviaPage()
    {
        InitializeComponent();
        BindingContext = AppServices.GetRequiredService<DailyTriviaViewModel>();
    }
}

