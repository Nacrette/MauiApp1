using System.Globalization;
using MauiApp1.Helpers;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class ApodPage : ContentPage, IQueryAttributable
{
    private readonly ApodViewModel _viewModel;
    private DateOnly? _pendingApodDate;

    public ApodPage()
    {
        InitializeComponent();
        _viewModel = AppServices.GetRequiredService<ApodViewModel>();
        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("date", out var raw))
            return;

        var s = raw as string ?? raw?.ToString();
        if (string.IsNullOrWhiteSpace(s))
            return;

        if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            _pendingApodDate = d;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.NotifyApodPageEnteredAsync();

        if (_pendingApodDate.HasValue)
        {
            var d = _pendingApodDate.Value;
            _pendingApodDate = null;
            await _viewModel.LoadApodForDateAsync(d);
            return;
        }

        if (_viewModel.Apod is null && !_viewModel.IsLoading)
            await _viewModel.LoadTodaysApodCommand.ExecuteAsync(null);
    }
}
