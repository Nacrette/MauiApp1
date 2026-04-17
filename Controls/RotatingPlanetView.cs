using Microsoft.Maui.Graphics;

namespace MauiApp1.Controls;

public sealed class RotatingPlanetView : GraphicsView
{
    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(RotatingPlanetView), Colors.Cyan,
            propertyChanged: (b, _, _) => ((RotatingPlanetView)b)._drawable.SetAccent(((RotatingPlanetView)b).AccentColor));

    public static readonly BindableProperty MotionEnabledProperty =
        BindableProperty.Create(nameof(MotionEnabled), typeof(bool), typeof(RotatingPlanetView), true,
            propertyChanged: OnMotionEnabledChanged);

    private readonly PlanetDrawable _drawable;
    private IDispatcherTimer? _timer;

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public bool MotionEnabled
    {
        get => (bool)GetValue(MotionEnabledProperty);
        set => SetValue(MotionEnabledProperty, value);
    }

    public RotatingPlanetView()
    {
        _drawable = new PlanetDrawable(Colors.Cyan);
        Drawable = _drawable;
        HeightRequest = 260;
        WidthRequest = 260;
        InputTransparent = true;

        Loaded += (_, _) =>
        {
            if (MotionEnabled) Start();
        };
        Unloaded += (_, _) => Stop();
    }

    private static void OnMotionEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (RotatingPlanetView)bindable;
        if (newValue is bool enabled)
        {
            if (enabled)
                view.Start();
            else
                view.Stop();
        }
    }

    private void Start()
    {
        if (_timer == null)
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(33);
            _timer.Tick += OnTick;
        }
        _timer.Start();
    }

    private void Stop()
    {
        _timer?.Stop();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (!MotionEnabled) return;
        _drawable.Step();
        Invalidate();
    }

    private sealed class PlanetDrawable(Color accent) : IDrawable
    {
        private float _angle;
        private Color _accent = accent;

        public void SetAccent(Color c) => _accent = c;
        public void Step() => _angle = (_angle + 0.9f) % 360f;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var size = MathF.Min(dirtyRect.Width, dirtyRect.Height);
            var r = size * 0.36f;
            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;

            canvas.SaveState();
            canvas.BlendMode = BlendMode.Screen;

            canvas.FillColor = _accent.WithAlpha(0.18f);
            canvas.FillCircle(cx, cy, r * 2.15f);

            canvas.RestoreState();

            var sphere = new RadialGradientPaint
            {
                Center = new Point(cx - r * 0.45f, cy - r * 0.45f),
                Radius = r * 1.35f,
                GradientStops =
                [
                    new PaintGradientStop(0f, _accent.WithAlpha(1f)),
                    new PaintGradientStop(1f, _accent.WithAlpha(0.18f)),
                ]
            };

            canvas.SetFillPaint(sphere, dirtyRect);
            canvas.FillCircle(cx, cy, r);

            canvas.SaveState();
            var clip = new PathF();
            clip.AppendCircle(cx, cy, r);
            canvas.ClipPath(clip);
            canvas.Translate(cx, cy);
            canvas.Rotate(_angle);
            canvas.Translate(-cx, -cy);

            for (var i = -3; i <= 3; i++)
            {
                var y = cy + i * r * 0.23f;
                canvas.FillColor = Colors.White.WithAlpha(0.05f + (i % 2 == 0 ? 0.03f : 0.0f));
                canvas.FillRoundedRectangle(cx - r * 1.1f, y - r * 0.08f, r * 2.2f, r * 0.16f, r * 0.08f);
            }

            canvas.RestoreState();

            canvas.StrokeColor = Colors.White.WithAlpha(0.15f);
            canvas.StrokeSize = MathF.Max(1.5f, r * 0.06f);
            canvas.DrawCircle(cx, cy, r);
        }
    }
}
