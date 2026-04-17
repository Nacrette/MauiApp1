using System.Diagnostics;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;

namespace MauiApp1.Services;

public sealed class ApodNotificationService : IApodNotificationService
{
    private static bool _hasSentThisSession;
    private static readonly object _lock = new();

    public Task SendWelcomeNotificationOncePerSessionAsync()
    {
        lock (_lock)
        {
            if (_hasSentThisSession)
            {
                Debug.WriteLine("[ApodNotificationService] APOD welcome notification skipped (already sent this session).");
                return Task.CompletedTask;
            }

            _hasSentThisSession = true;
        }

        try
        {
            var request = new NotificationRequest
            {
                NotificationId = 1001,
                Title = "Planet Explorer",
                Description = "Welcome to Today's APOD - Astronomy Picture of the Day",
                ReturningData = "apod_welcome"
            };

            LocalNotificationCenter.Current.Show(request);
            Debug.WriteLine("[ApodNotificationService] Local notification sent.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ApodNotificationService] Failed to send local notification: {ex}");
        }

        return Task.CompletedTask;
    }
}
