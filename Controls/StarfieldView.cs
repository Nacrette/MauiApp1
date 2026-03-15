using Microsoft.Maui.Graphics;

namespace MauiApp1.Controls;

// A lightweight, global starfield overlay. Put it on every page for the "twinkling" effect.
public sealed class StarfieldView : GraphicsView
{
    private readonly StarfieldDrawable _drawable;
    private IDispatcherTimer? _timer;

    public StarfieldView()
    {
        _drawable = new StarfieldDrawable();
        Drawable = _drawable;
        InputTransparent = true;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (Width <= 0 || Height <= 0)
            return;

        _drawable.EnsureStars((float)Width, (float)Height);
        Invalidate();
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(33); // ~30 FPS
        _timer.Tick -= OnTick;
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _timer?.Stop();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _drawable.Tick();
        Invalidate();
    }

    private sealed class StarfieldDrawable : IDrawable
    {
        private const int StarCount = 120;

        private readonly List<Star> _stars = new(StarCount);
        private float _w;
        private float _h;
        private int _seed = 1;

        public void EnsureStars(float width, float height)
        {
            if (width <= 0 || height <= 0)
                return;

            if (_stars.Count == StarCount && Math.Abs(_w - width) < 1 && Math.Abs(_h - height) < 1)
                return;

            _w = width;
            _h = height;
            _stars.Clear();

            var rng = new Random(_seed++);
            for (var i = 0; i < StarCount; i++)
            {
                _stars.Add(new Star
                {
                    X = (float)rng.NextDouble() * _w,
                    Y = (float)rng.NextDouble() * _h,
                    Radius = 0.6f + (float)rng.NextDouble() * 1.8f,
                    Alpha = 0.25f + (float)rng.NextDouble() * 0.6f,
                    Twinkle = 0.008f + (float)rng.NextDouble() * 0.03f,
                    Phase = (float)rng.NextDouble() * (float)Math.PI * 2,
                });
            }
        }

        public void Tick()
        {
            for (var i = 0; i < _stars.Count; i++)
            {
                var s = _stars[i];
                s.Phase += s.Twinkle;
                s.Alpha = 0.15f + 0.75f * (0.5f + 0.5f * (float)Math.Sin(s.Phase));

                _stars[i] = s;
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (_stars.Count == 0)
                EnsureStars(dirtyRect.Width, dirtyRect.Height);

            canvas.SaveState();
            canvas.BlendMode = BlendMode.Screen;

            foreach (var s in _stars)
            {
                var glow = new Color(1f, 1f, 1f, s.Alpha * 0.35f);
                var core = new Color(1f, 1f, 1f, s.Alpha);

                canvas.FillColor = glow;
                canvas.FillCircle(s.X, s.Y, s.Radius * 3f);

                canvas.FillColor = core;
                canvas.FillCircle(s.X, s.Y, s.Radius);
            }

            canvas.RestoreState();
        }

        private struct Star
        {
            public float X;
            public float Y;
            public float Radius;
            public float Alpha;
            public float Twinkle;
            public float Phase;
        }
    }
}

