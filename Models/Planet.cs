namespace MauiApp1.Models;

public sealed class Planet
{
    public required string Name { get; init; }

    // For now this is a color-driven drawable (no external images required).
    public required Color AccentColor { get; init; }

    public required IReadOnlyDictionary<string, string> Facts { get; init; }
}

