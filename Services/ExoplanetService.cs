using System.Net.Http.Json;
using System.Diagnostics;
using Microsoft.Maui.Graphics;
using MauiApp1.Models;

namespace MauiApp1.Services;

public class ExoplanetService : IExoplanetService
{
    private readonly HttpClient _httpClient;
    private List<CelestialBody>? _cache;

    public ExoplanetService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void InvalidateCache() => _cache = null;

    public async Task<List<CelestialBody>> GetExoplanetsAsync()
    {
        if (_cache != null)
            return _cache;

        var fallbackExoplanets = GetFallbackExoplanets();
        
        try
        {
            var query =
                "select top 50 pl_name,hostname,sy_dist,pl_bmasse,pl_rade,disc_year,pl_orbper,pl_orbsmax " +
                "from ps " +
                "where default_flag=1 " +
                "and pl_bmasse is not null " +
                "and pl_rade is not null " +
                "and sy_dist is not null " +
                "order by sy_dist asc";

            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://exoplanetarchive.ipac.caltech.edu/TAP/sync?query={encodedQuery}&format=json";
            Debug.WriteLine($"[ExoplanetService] Querying exoplanets: {query}");
            var response = await _httpClient.GetAsync(url);
            Debug.WriteLine($"[ExoplanetService] HTTP status: {(int)response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var planets = System.Text.Json.JsonSerializer.Deserialize<List<ExoplanetRecord>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var validPlanets = planets?
                    .Where(p =>
                        !string.IsNullOrWhiteSpace(p.PlName) &&
                        !string.IsNullOrWhiteSpace(p.Hostname) &&
                        p.SyDist.HasValue &&
                        p.PlBmasse.HasValue &&
                        p.PlRade.HasValue)
                    .ToList() ?? new List<ExoplanetRecord>();

                Debug.WriteLine($"[ExoplanetService] Raw rows: {planets?.Count ?? 0}, valid rows: {validPlanets.Count}");

                if (validPlanets.Count >= 15)
                {
                    _cache = validPlanets.Select((p, index) => new CelestialBody
                    {
                        Name = p.PlName ?? $"Exoplanet {index + 1}",
                        Type = CelestialBodyType.Exoplanet,
                        HostStar = p.Hostname ?? "Unknown",
                        DistanceFromSun = p.SyDist ?? 0,
                        Mass = p.PlBmasse ?? 0,
                        Radius = p.PlRade ?? 0,
                        DiscoveryYear = p.DiscYear,
                        OrbitalPeriod = p.PlOrbper?.ToString("N2") ?? "Unknown",
                        OrbitalRadius = p.PlOrbsmax?.ToString("N3") ?? "Unknown",
                        AccentColor = GetExoplanetColor(index),
                        Emoji = "\U0001F31F",
                        Description = GenerateDescription(p)
                    }).ToList();

                    Debug.WriteLine($"[ExoplanetService] Loaded {_cache.Count} exoplanets from API.");
                    return _cache;
                }

                Debug.WriteLine("[ExoplanetService] Not enough valid exoplanets (<15), using fallback.");
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"[ExoplanetService] API error body: {body}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ExoplanetService] Exception: {ex.Message}");
        }

        _cache = fallbackExoplanets;
        Debug.WriteLine($"[ExoplanetService] Loaded fallback exoplanets: {_cache.Count}");
        return _cache;
    }

    private static List<CelestialBody> GetFallbackExoplanets() => new()
    {
        new() { Name = "Proxima Centauri b", Type = CelestialBodyType.Exoplanet, HostStar = "Proxima Centauri", DistanceFromSun = 4.24, Mass = 1.27, Radius = 1.1, DiscoveryYear = 2016, AccentColor = Color.FromArgb("#FF6B6B"), Emoji = "\U0001F31F", Description = "Closest known exoplanet to Earth, within the habitable zone" },
        new() { Name = "TRAPPIST-1e", Type = CelestialBodyType.Exoplanet, HostStar = "TRAPPIST-1", DistanceFromSun = 39.5, Mass = 0.77, Radius = 0.91, DiscoveryYear = 2017, AccentColor = Color.FromArgb("#4ECDC4"), Emoji = "\U0001F31F", Description = "Rocky planet in the habitable zone of TRAPPIST-1" },
        new() { Name = "Kepler-452b", Type = CelestialBodyType.Exoplanet, HostStar = "Kepler-452", DistanceFromSun = 1400, Mass = 5.0, Radius = 1.6, DiscoveryYear = 2015, AccentColor = Color.FromArgb("#45B7D1"), Emoji = "\U0001F31F", Description = "Earth's cousin, the first Earth-sized planet in a habitable zone" },
        new() { Name = "TOI-700 d", Type = CelestialBodyType.Exoplanet, HostStar = "TOI-700", DistanceFromSun = 101.4, Mass = 1.7, Radius = 1.2, DiscoveryYear = 2020, AccentColor = Color.FromArgb("#96CEB4"), Emoji = "\U0001F31F", Description = "First Earth-sized planet in the habitable zone discovered by TESS" },
        new() { Name = "LHS 1140 b", Type = CelestialBodyType.Exoplanet, HostStar = "LHS 1140", DistanceFromSun = 40.7, Mass = 6.6, Radius = 1.4, DiscoveryYear = 2017, AccentColor = Color.FromArgb("#DDA0DD"), Emoji = "\U0001F31F", Description = "Super-Earth possibly harboring a thick atmosphere" },
        new() { Name = "HD 219134 b", Type = CelestialBodyType.Exoplanet, HostStar = "HD 219134", DistanceFromSun = 21.3, Mass = 4.5, Radius = 1.6, DiscoveryYear = 2015, AccentColor = Color.FromArgb("#FFD93D"), Emoji = "\U0001F31F", Description = "Closest known rocky exoplanet that could transit its star" },
        new() { Name = "WASP-12b", Type = CelestialBodyType.Exoplanet, HostStar = "WASP-12", DistanceFromSun = 870, Mass = 445, Radius = 19, DiscoveryYear = 2008, AccentColor = Color.FromArgb("#FF8C00"), Emoji = "\U0001F31F", Description = "A hot Jupiter being devoured by its host star" },
        new() { Name = "55 Cancri e", Type = CelestialBodyType.Exoplanet, HostStar = "55 Cancri", DistanceFromSun = 41, Mass = 8.0, Radius = 2.0, DiscoveryYear = 2004, AccentColor = Color.FromArgb("#FF69B4"), Emoji = "\U0001F31F", Description = "A lava world with surface temperatures over 2000C" },
        new() { Name = "K2-18b", Type = CelestialBodyType.Exoplanet, HostStar = "K2-18", DistanceFromSun = 124, Mass = 8.6, Radius = 2.6, DiscoveryYear = 2015, AccentColor = Color.FromArgb("#87CEEB"), Emoji = "\U0001F31F", Description = "First exoplanet with water vapor detected in atmosphere" },
        new() { Name = "Gliese 581 c", Type = CelestialBodyType.Exoplanet, HostStar = "Gliese 581", DistanceFromSun = 20.3, Mass = 5.5, Radius = 1.5, DiscoveryYear = 2007, AccentColor = Color.FromArgb("#98FB98"), Emoji = "\U0001F31F", Description = "Early candidate for a potentially habitable exoplanet" }
    };

    private static string GenerateDescription(ExoplanetRecord p)
    {
        if (p.PlBmasse > 300) return "A gas giant, similar to Jupiter";
        if (p.PlBmasse > 10) return "A Neptune-like ice giant";
        if (p.PlBmasse > 2) return "A super-Earth, larger than our planet";
        if (p.PlBmasse > 0.5) return "A rocky planet, possibly Earth-like";
        return "A small terrestrial exoplanet";
    }

    private static Color GetExoplanetColor(int index)
    {
        var colors = new[]
        {
            Color.FromArgb("#FF6B6B"), Color.FromArgb("#4ECDC4"), Color.FromArgb("#45B7D1"),
            Color.FromArgb("#96CEB4"), Color.FromArgb("#DDA0DD"), Color.FromArgb("#FFD93D"),
            Color.FromArgb("#FF8C00"), Color.FromArgb("#FF69B4"), Color.FromArgb("#87CEEB"),
            Color.FromArgb("#98FB98"), Color.FromArgb("#DDA0DD"), Color.FromArgb("#F0E68C")
        };
        return colors[index % colors.Length];
    }

    private class ExoplanetRecord
    {
        public string? PlName { get; set; }
        public string? Hostname { get; set; }
        public double? SyDist { get; set; }
        public double? PlBmasse { get; set; }
        public double? PlRade { get; set; }
        public int? DiscYear { get; set; }
        public double? PlOrbper { get; set; }
        public double? PlOrbsmax { get; set; }
    }
}
