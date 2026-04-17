namespace MauiApp1.Services;

public interface IApodNotificationService
{
    Task SendWelcomeNotificationOncePerSessionAsync();
}
