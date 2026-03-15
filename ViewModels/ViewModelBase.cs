using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp1.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;
}

