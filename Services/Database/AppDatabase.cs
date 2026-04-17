using System.Diagnostics;
using MauiApp1.Helpers;
using MauiApp1.Models;
using Microsoft.Maui.Storage;
using SQLite;

namespace MauiApp1.Services.Database;

public sealed class AppDatabase
{
    private static readonly string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, "planetexplorer.db");
    private readonly SQLiteAsyncConnection _database = new(DatabasePath);
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _isInitialized;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        await _initLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_isInitialized)
                return;

            Debug.WriteLine($"[AppDatabase] Initializing SQLite database at: {DatabasePath}");
            await _database.CreateTableAsync<UserEntity>().ConfigureAwait(false);
            await _database.CreateTableAsync<PlanetFavoriteEntity>().ConfigureAwait(false);
            await _database.CreateTableAsync<ApodFavoriteEntity>().ConfigureAwait(false);
            _isInitialized = true;
            Debug.WriteLine("[AppDatabase] Database initialized and tables ready.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[AppDatabase] Initialization failed: {ex}");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task UpsertUserAsync(User user)
    {
        await InitializeAsync().ConfigureAwait(false);

        var normalizedUsername = user.Username.Trim().ToLowerInvariant();
        await _database.ExecuteAsync("UPDATE Users SET IsLoggedIn = 0").ConfigureAwait(false);
        await _database.InsertOrReplaceAsync(new UserEntity
        {
            Username = normalizedUsername,
            DisplayName = user.DisplayName,
            AvatarEmoji = user.AvatarEmoji,
            CreatedAtUtc = user.CreatedAt,
            IsLoggedIn = user.IsLoggedIn
        }).ConfigureAwait(false);

        Debug.WriteLine($"[AppDatabase] Upserted user: {normalizedUsername}");
    }

    public async Task<User?> GetLoggedInUserAsync()
    {
        await InitializeAsync().ConfigureAwait(false);

        var entity = await _database.Table<UserEntity>()
            .Where(x => x.IsLoggedIn)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return entity is null ? null : MapToUser(entity);
    }

    public async Task SetUserLoggedOutAsync(string username)
    {
        await InitializeAsync().ConfigureAwait(false);

        var normalized = username.Trim().ToLowerInvariant();
        await _database.ExecuteAsync("UPDATE Users SET IsLoggedIn = 0 WHERE Username = ?", normalized).ConfigureAwait(false);
        Debug.WriteLine($"[AppDatabase] Marked user as logged out: {normalized}");
    }

    public async Task<IReadOnlyList<CelestialBody>> GetPlanetFavoritesAsync(string username)
    {
        await InitializeAsync().ConfigureAwait(false);

        var normalized = NormalizeUsername(username);
        var rows = await _database.Table<PlanetFavoriteEntity>()
            .Where(x => x.Username == normalized)
            .ToListAsync()
            .ConfigureAwait(false);

        return rows.Select(MapToPlanet).ToList();
    }

    public async Task AddOrUpdatePlanetFavoriteAsync(string username, CelestialBody planet)
    {
        await InitializeAsync().ConfigureAwait(false);

        var normalized = NormalizeUsername(username);
        var entity = MapToPlanetEntity(normalized, planet);
        await _database.InsertOrReplaceAsync(entity).ConfigureAwait(false);
        Debug.WriteLine($"[AppDatabase] Saved planet favorite: {normalized}/{planet.Name}");
    }

    public async Task RemovePlanetFavoriteAsync(string username, string planetName)
    {
        await InitializeAsync().ConfigureAwait(false);

        var normalized = NormalizeUsername(username);
        var id = BuildPlanetFavoriteId(normalized, planetName);
        await _database.DeleteAsync<PlanetFavoriteEntity>(id).ConfigureAwait(false);
        Debug.WriteLine($"[AppDatabase] Removed planet favorite: {normalized}/{planetName}");
    }

    public async Task<IReadOnlyList<ApodFavorite>> GetApodFavoritesAsync(string username)
    {
        await InitializeAsync().ConfigureAwait(false);

        var normalized = NormalizeUsername(username);
        var rows = await _database.Table<ApodFavoriteEntity>()
            .Where(x => x.Username == normalized)
            .ToListAsync()
            .ConfigureAwait(false);

        return rows.Select(MapToApod).ToList();
    }

    public async Task AddOrUpdateApodFavoriteAsync(string username, ApodFavorite apod)
    {
        await InitializeAsync().ConfigureAwait(false);

        var normalized = NormalizeUsername(username);
        var entity = MapToApodEntity(normalized, apod);
        await _database.InsertOrReplaceAsync(entity).ConfigureAwait(false);
        Debug.WriteLine($"[AppDatabase] Saved APOD favorite: {normalized}/{apod.Date}");
    }

    public async Task RemoveApodFavoriteAsync(string username, string apodDate)
    {
        await InitializeAsync().ConfigureAwait(false);

        var normalized = NormalizeUsername(username);
        var id = BuildApodFavoriteId(normalized, apodDate);
        await _database.DeleteAsync<ApodFavoriteEntity>(id).ConfigureAwait(false);
        Debug.WriteLine($"[AppDatabase] Removed APOD favorite: {normalized}/{apodDate}");
    }

    private static string NormalizeUsername(string username) => username.Trim().ToLowerInvariant();

    private static string BuildPlanetFavoriteId(string username, string planetName) =>
        $"{username}|{planetName.Trim()}";

    private static string BuildApodFavoriteId(string username, string apodDate) =>
        $"{username}|{apodDate.Trim()}";

    private static User MapToUser(UserEntity entity) => new()
    {
        Username = entity.Username,
        DisplayName = entity.DisplayName,
        AvatarEmoji = entity.AvatarEmoji,
        CreatedAt = entity.CreatedAtUtc,
        IsLoggedIn = entity.IsLoggedIn
    };

    private static PlanetFavoriteEntity MapToPlanetEntity(string username, CelestialBody planet) => new()
    {
        Id = BuildPlanetFavoriteId(username, planet.Name),
        Username = username,
        Name = planet.Name,
        Type = planet.Type,
        Description = planet.Description,
        DistanceFromSun = planet.DistanceFromSun,
        Mass = planet.Mass,
        Radius = planet.Radius,
        DiscoveryYear = planet.DiscoveryYear,
        HostStar = planet.HostStar,
        OrbitalPeriod = planet.OrbitalPeriod,
        OrbitalRadius = planet.OrbitalRadius,
        AccentColorHex = planet.AccentColor.ToArgbHex(),
        Emoji = planet.Emoji,
        HasRing = planet.HasRing,
        Order = planet.Order
    };

    private static CelestialBody MapToPlanet(PlanetFavoriteEntity entity) => new()
    {
        Name = entity.Name,
        Type = entity.Type,
        Description = entity.Description,
        DistanceFromSun = entity.DistanceFromSun,
        Mass = entity.Mass,
        Radius = entity.Radius,
        DiscoveryYear = entity.DiscoveryYear,
        HostStar = entity.HostStar,
        OrbitalPeriod = entity.OrbitalPeriod,
        OrbitalRadius = entity.OrbitalRadius,
        AccentColor = ParseColorOrDefault(entity.AccentColorHex),
        Emoji = entity.Emoji,
        HasRing = entity.HasRing,
        Order = entity.Order
    };

    private static ApodFavoriteEntity MapToApodEntity(string username, ApodFavorite apod) => new()
    {
        Id = BuildApodFavoriteId(username, apod.Date),
        Username = username,
        Title = apod.Title,
        Date = apod.Date,
        Url = apod.Url,
        HdUrl = apod.HdUrl,
        Explanation = apod.Explanation,
        MediaType = apod.MediaType,
        Copyright = apod.Copyright,
        SavedAtUtc = apod.SavedAt
    };

    private static ApodFavorite MapToApod(ApodFavoriteEntity entity) => new()
    {
        Title = entity.Title,
        Date = entity.Date,
        Url = entity.Url,
        HdUrl = entity.HdUrl,
        Explanation = entity.Explanation,
        MediaType = entity.MediaType,
        Copyright = entity.Copyright,
        SavedAt = entity.SavedAtUtc
    };

    private static Color ParseColorOrDefault(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            return Colors.Cyan;

        try
        {
            return Color.FromArgb(hex);
        }
        catch
        {
            return Colors.Cyan;
        }
    }

    [Table("Users")]
    public sealed class UserEntity
    {
        [PrimaryKey]
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarEmoji { get; set; } = "\U0001F47B";
        public DateTime CreatedAtUtc { get; set; }
        public bool IsLoggedIn { get; set; }
    }

    [Table("PlanetFavorites")]
    public sealed class PlanetFavoriteEntity
    {
        [PrimaryKey]
        public string Id { get; set; } = string.Empty;
        [Indexed]
        public string Username { get; set; } = string.Empty;
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
        public string AccentColorHex { get; set; } = "#FF00FFFF";
        public string Emoji { get; set; } = "\U0001F31F";
        public bool HasRing { get; set; }
        public int Order { get; set; }
    }

    [Table("ApodFavorites")]
    public sealed class ApodFavoriteEntity
    {
        [PrimaryKey]
        public string Id { get; set; } = string.Empty;
        [Indexed]
        public string Username { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string HdUrl { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string MediaType { get; set; } = "image";
        public string? Copyright { get; set; }
        public DateTime SavedAtUtc { get; set; }
    }
}
