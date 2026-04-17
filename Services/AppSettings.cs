using Microsoft.Maui.Storage;

namespace MauiApp1.Services;

public static class AppSettings
{
    private const string ReduceMotionKey = "global_reduce_motion";

    public static bool ReduceMotion
    {
        get => Preferences.Default.Get(ReduceMotionKey, false);
        set => Preferences.Default.Set(ReduceMotionKey, value);
    }
}
