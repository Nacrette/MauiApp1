using Microsoft.Maui.Graphics;

namespace MauiApp1.Models;

public enum CelestialBodyType
{
    SolarSystemPlanet,
    Exoplanet
}

public class CelestialBody
{
    public string Name { get; set; } = string.Empty;
    public CelestialBodyType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public double DistanceFromSun { get; set; }
    public double Mass { get; set; }
    public double Radius { get; set; }
    public int? DiscoveryYear { get; set; }
    public string? HostStar { get; set; }
    public string? OrbitalPeriod { get; set; }
    public string? OrbitalRadius { get; set; }
    public Color AccentColor { get; set; } = Colors.Cyan;
    public string Emoji { get; set; } = "\U0001F31F";
    public bool HasRing { get; set; }
    public int Order { get; set; }

    public string DisplayDistance => Type == CelestialBodyType.SolarSystemPlanet
        ? $"{DistanceFromSun:N1} AU"
        : $"{DistanceFromSun:N0} light-years";

    public string DisplayMass => Mass > 0 ? $"{Mass:N2} M\U000020Earth" : "Unknown";

    public string DisplayRadius => Radius > 0 ? $"{Radius:N2} R\U000020Earth" : "Unknown";

    public string Subtitle => Type == CelestialBodyType.SolarSystemPlanet
        ? $"{Description} - {DisplayDistance} from Sun"
        : $"{HostStar} system - Discovered {DiscoveryYear}";
}
