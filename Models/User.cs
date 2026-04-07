namespace MauiApp1.Models;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarEmoji { get; set; } = "\U0001F47B";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsLoggedIn { get; set; }

    public string Initials => string.IsNullOrEmpty(DisplayName)
        ? Username.Length > 0 ? Username[0].ToString().ToUpper() : "?"
        : DisplayName.Length > 1 ? DisplayName[..2].ToUpper() : DisplayName.ToUpper();
}
