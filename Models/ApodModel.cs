using System.Text.Json.Serialization;

namespace MauiApp1.Models;

public sealed class ApodModel
{
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("hdurl")]
    public string? HdUrl { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }
}

