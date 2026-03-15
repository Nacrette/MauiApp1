using MauiApp1.Models;

namespace MauiApp1.Services;

public sealed class PlanetService : IPlanetService
{
    private readonly IReadOnlyList<Planet> _planets =
    [
        new Planet
        {
            Name = "Mercury",
            AccentColor = Color.FromArgb("#AEB6BF"),
            Facts = new Dictionary<string, string>
            {
                ["Type"] = "Terrestrial",
                ["Gravity"] = "3.7 m/s²",
                ["Diameter"] = "4,879 km",
                ["Avg Temp"] = "167°C",
                ["Rotation Period"] = "58.6 Earth days",
                ["Orbital Period"] = "88 Earth days",
            }
        },
        new Planet
        {
            Name = "Venus",
            AccentColor = Color.FromArgb("#E8C07D"),
            Facts = new Dictionary<string, string>
            {
                ["Type"] = "Terrestrial",
                ["Gravity"] = "8.87 m/s²",
                ["Diameter"] = "12,104 km",
                ["Avg Temp"] = "464°C",
                ["Rotation Period"] = "243 Earth days (retrograde)",
                ["Orbital Period"] = "225 Earth days",
            }
        },
        new Planet
        {
            Name = "Earth",
            AccentColor = Color.FromArgb("#4FC3F7"),
            Facts = new Dictionary<string, string>
            {
                ["Type"] = "Terrestrial",
                ["Gravity"] = "9.81 m/s²",
                ["Diameter"] = "12,742 km",
                ["Avg Temp"] = "15°C",
                ["Rotation Period"] = "23.93 hours",
                ["Orbital Period"] = "365.25 days",
            }
        },
        new Planet
        {
            Name = "Mars",
            AccentColor = Color.FromArgb("#FF7043"),
            Facts = new Dictionary<string, string>
            {
                ["Type"] = "Terrestrial",
                ["Gravity"] = "3.71 m/s²",
                ["Diameter"] = "6,779 km",
                ["Avg Temp"] = "-63°C",
                ["Rotation Period"] = "24.6 hours",
                ["Orbital Period"] = "687 Earth days",
            }
        },
        new Planet
        {
            Name = "Jupiter",
            AccentColor = Color.FromArgb("#FFCC80"),
            Facts = new Dictionary<string, string>
            {
                ["Type"] = "Gas Giant",
                ["Gravity"] = "24.79 m/s²",
                ["Diameter"] = "139,820 km",
                ["Avg Temp"] = "-108°C",
                ["Rotation Period"] = "9.9 hours",
                ["Orbital Period"] = "11.86 Earth years",
            }
        },
        new Planet
        {
            Name = "Saturn",
            AccentColor = Color.FromArgb("#FFD180"),
            Facts = new Dictionary<string, string>
            {
                ["Type"] = "Gas Giant",
                ["Gravity"] = "10.44 m/s²",
                ["Diameter"] = "116,460 km",
                ["Avg Temp"] = "-139°C",
                ["Rotation Period"] = "10.7 hours",
                ["Orbital Period"] = "29.45 Earth years",
            }
        },
        new Planet
        {
            Name = "Uranus",
            AccentColor = Color.FromArgb("#80DEEA"),
            Facts = new Dictionary<string, string>
            {
                ["Type"] = "Ice Giant",
                ["Gravity"] = "8.69 m/s²",
                ["Diameter"] = "50,724 km",
                ["Avg Temp"] = "-195°C",
                ["Rotation Period"] = "17.2 hours (retrograde)",
                ["Orbital Period"] = "84 Earth years",
            }
        },
        new Planet
        {
            Name = "Neptune",
            AccentColor = Color.FromArgb("#7986CB"),
            Facts = new Dictionary<string, string>
            {
                ["Type"] = "Ice Giant",
                ["Gravity"] = "11.15 m/s²",
                ["Diameter"] = "49,244 km",
                ["Avg Temp"] = "-201°C",
                ["Rotation Period"] = "16.1 hours",
                ["Orbital Period"] = "164.8 Earth years",
            }
        }
    ];

    public IReadOnlyList<Planet> GetPlanets() => _planets;

    public Planet? GetByName(string name) =>
        _planets.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
}

