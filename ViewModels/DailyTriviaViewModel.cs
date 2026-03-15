using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp1.ViewModels;

public sealed partial class DailyTriviaViewModel : ViewModelBase
{
    [ObservableProperty]
    private string trivia = "Daily Trivia: Saturn could float in water (if you had a bathtub big enough).";
}

