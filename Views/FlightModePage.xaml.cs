using Microsoft.Maui.Graphics;

namespace MauiApp1.Views;

public partial class FlightModePage : ContentPage, IQueryAttributable
{
    private readonly HudDrawable _drawable = new();

    private double _startScale = 1;
    private double _scale = 1;
    private double _rotation;

    public FlightModePage()
    {
        InitializeComponent();

        HudView.Drawable = _drawable;

        var pan = new PanGestureRecognizer();
        pan.PanUpdated += (_, e) =>
        {
            if (e.StatusType == GestureStatus.Running)
            {
                _rotation = (_rotation + e.TotalX * 0.05) % 360;
                _drawable.RotationDegrees = (float)_rotation;
                HudView.Invalidate();
            }
        };

        var pinch = new PinchGestureRecognizer();
        pinch.PinchUpdated += (_, e) =>
        {
            if (e.Status == GestureStatus.Started)
                _startScale = _scale;

            if (e.Status == GestureStatus.Running)
            {
                _scale = Math.Clamp(_startScale * e.Scale, 0.6, 2.5);
                _drawable.Scale = (float)_scale;
                HudView.Invalidate();
            }
        };

        HudView.GestureRecognizers.Add(pan);
        HudView.GestureRecognizers.Add(pinch);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("accent", out var raw) && raw is string hex && Color.TryParse(hex, out var c))
            _drawable.Accent = c;

        if (query.TryGetValue("name", out var p) && p is string name && !string.IsNullOrWhiteSpace(name))
            Title = $"{name} Flight Mode";
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private sealed class HudDrawable : IDrawable
    {
        public Color Accent { get; set; } = Colors.Cyan;
        public float RotationDegrees { get; set; }
        public float Scale { get; set; } = 1f;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();
            canvas.BlendMode = BlendMode.Screen;

            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;
            var r = MathF.Min(dirtyRect.Width, dirtyRect.Height) * 0.30f * Scale;

            // HUD ring
            canvas.StrokeColor = Accent.WithAlpha(0.65f);
            canvas.StrokeSize = 3;
            canvas.DrawCircle(cx, cy, r);

            // Rotating crosshair
            canvas.Translate(cx, cy);
            canvas.Rotate(RotationDegrees);
            canvas.Translate(-cx, -cy);

            canvas.StrokeColor = Colors.White.WithAlpha(0.25f);
            canvas.StrokeSize = 2;
            canvas.DrawLine(cx - r * 1.2f, cy, cx + r * 1.2f, cy);
            canvas.DrawLine(cx, cy - r * 1.2f, cx, cy + r * 1.2f);

            canvas.RestoreState();

            // Corner brackets
            canvas.SaveState();
            canvas.StrokeColor = Accent.WithAlpha(0.4f);
            canvas.StrokeSize = 2;
            var pad = 18;
            var len = 26;

            // TL
            canvas.DrawLine(pad, pad, pad + len, pad);
            canvas.DrawLine(pad, pad, pad, pad + len);
            // TR
            canvas.DrawLine(dirtyRect.Width - pad, pad, dirtyRect.Width - pad - len, pad);
            canvas.DrawLine(dirtyRect.Width - pad, pad, dirtyRect.Width - pad, pad + len);
            // BL
            canvas.DrawLine(pad, dirtyRect.Height - pad, pad + len, dirtyRect.Height - pad);
            canvas.DrawLine(pad, dirtyRect.Height - pad, pad, dirtyRect.Height - pad - len);
            // BR
            canvas.DrawLine(dirtyRect.Width - pad, dirtyRect.Height - pad, dirtyRect.Width - pad - len, dirtyRect.Height - pad);
            canvas.DrawLine(dirtyRect.Width - pad, dirtyRect.Height - pad, dirtyRect.Width - pad, dirtyRect.Height - pad - len);

            canvas.RestoreState();
        }
    }
}

