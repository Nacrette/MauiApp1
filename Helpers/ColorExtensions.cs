namespace MauiApp1.Helpers;

public static class ColorExtensions
{
    public static string ToArgbHex(this Color color)
    {
        var a = (byte)Math.Round(color.Alpha * 255);
        var r = (byte)Math.Round(color.Red * 255);
        var g = (byte)Math.Round(color.Green * 255);
        var b = (byte)Math.Round(color.Blue * 255);
        return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
    }
}

