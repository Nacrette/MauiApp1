using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class LoginPage : ContentPage
{
    private LoginViewModel? _viewModel;

    public LoginPage()
    {
        InitializeComponent();
        _viewModel = AppServices.GetRequiredService<LoginViewModel>();
        BindingContext = _viewModel;
        UpdateUI();
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LoginViewModel.IsRegistering))
                UpdateUI();
        };
    }

    private void UpdateUI()
    {
        if (_viewModel == null) return;

        DisplayNameEntry.IsVisible = _viewModel.IsRegistering;
        LoginButton.IsVisible = !_viewModel.IsRegistering;
        RegisterButton.IsVisible = _viewModel.IsRegistering;

        AuthLabel.Text = _viewModel.IsRegistering 
            ? "Create your account to start exploring" 
            : "Sign in to continue your journey";

        ToggleButton.Text = _viewModel.IsRegistering 
            ? "Already have an account? Login" 
            : "New here? Create an account";
    }
}
