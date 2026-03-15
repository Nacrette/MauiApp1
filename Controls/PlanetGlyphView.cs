using Microsoft.Maui.Graphics;

namespace MauiApp1.Controls;

// A tiny "planet image" without external assets: a glowing sphere with optional ring.
public sealed class PlanetGlyphView : GraphicsView
{
    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(PlanetGlyphView), Colors.Cyan,
            propertyChanged: (b, _, _) => ((PlanetGlyphView)b).Invalidate());

    public static readonly BindableProperty HasRingProperty =
        BindableProperty.Create(nameof(HasRing), typeof(bool), typeof(PlanetGlyphView), false,
            propertyChanged: (b, _, _) => ((PlanetGlyphView)b).Invalidate());

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public bool HasRing
    {
        get => (bool)GetValue(HasRingProperty);
        set => SetValue(HasRingProperty, value);
    }

    public PlanetGlyphView()
    {
        Drawable = new PlanetGlyphDrawable(this);
        HeightRequest = 56;
        WidthRequest = 56;
        InputTransparent = true;
    }

    private sealed class PlanetGlyphDrawable(PlanetGlyphView view) : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;
            var r = MathF.Min(dirtyRect.Width, dirtyRect.Height) * 0.38f;

            canvas.SaveState();
            canvas.BlendMode = BlendMode.Screen;

            // Outer glow
            canvas.FillColor = view.AccentColor.WithAlpha(0.22f);
            canvas.FillCircle(cx, cy, r * 1.9f);

            // Sphere (simple radial gradient)
            var grad = new RadialGradientPaint
            {
                Center = new Point(cx - r * 0.35f, cy - r * 0.35f),
                Radius = r * 1.35f,
                GradientStops =
                [
                    new PaintGradientStop(0f, view.AccentColor.WithAlpha(1f)),
                    new PaintGradientStop(1f, view.AccentColor.WithAlpha(0.15f)),
                ]
            };
            canvas.SetFillPaint(grad, dirtyRect);
            canvas.FillCircle(cx, cy, r);

            // Highlight
            canvas.FillColor = Colors.White.WithAlpha(0.25f);
            canvas.FillCircle(cx - r * 0.3f, cy - r * 0.35f, r * 0.25f);

            if (view.HasRing)
            {
                canvas.StrokeSize = MathF.Max(1.2f, r * 0.13f);
                canvas.StrokeColor = view.AccentColor.WithAlpha(0.45f);
                canvas.DrawEllipse(cx - r * 1.25f, cy - r * 0.4f, r * 2.5f, r * 0.8f);
            }

            canvas.RestoreState();
        }
    }
}

