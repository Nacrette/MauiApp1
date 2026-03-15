using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp1.ViewModels;

public sealed partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private bool reduceMotion;
}

