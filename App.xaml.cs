using Microsoft.Extensions.DependencyInjection;
using MauiApp1.Helpers;
using MauiApp1.Services;

namespace MauiApp1;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        AppServices.Initialize(services);
        _services = services;
        InitializeComponent();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        
        var authService = _services.GetRequiredService<IAuthService>();
        var savedUser = await authService.GetSavedUserAsync();
        
        if (savedUser == null)
        {
            await Shell.Current.GoToAsync("login");
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_services.GetRequiredService<AppShell>());
    }
}
