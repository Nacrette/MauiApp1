namespace MauiApp1.Models;

public class ApodFavorite
{
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string HdUrl { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string MediaType { get; set; } = "image";
    public string? Copyright { get; set; }
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}
