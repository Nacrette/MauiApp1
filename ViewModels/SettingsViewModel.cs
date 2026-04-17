using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels;

public sealed partial class SettingsViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private bool reduceMotion;

    [ObservableProperty]
    private User? currentUser;

    [ObservableProperty]
    private bool isLoggedIn;

    public SettingsViewModel(IAuthService authService)
    {
        _authService = authService;
        LoadUser();
    }

    public void LoadUser()
    {
        CurrentUser = _authService.CurrentUser;
        IsLoggedIn = _authService.IsLoggedIn;
        ReduceMotion = AppSettings.ReduceMotion;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        CurrentUser = null;
        IsLoggedIn = false;
        await Shell.Current.GoToAsync("//login");
    }

    partial void OnReduceMotionChanged(bool value)
    {
        AppSettings.ReduceMotion = value;
    }
}
