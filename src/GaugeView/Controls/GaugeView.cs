using SkiaSharp.Views.Maui;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace GaugeView.Controls;

/// <summary>
///     Represents a gauge view control that displays a value on a circular gauge.
/// </summary>
public class GaugeView : SKCanvasView
{
    // Define the duration of the animation in milliseconds
    private const int AnimationDuration = 250;

    // Define the sweep angle for each segment of the gauge
    private const float SweepAngle = 67.5f;

    // Define the number of steps per frame for the animation
    private const int StepsPerFrame = AnimationDuration / 16;

    // Define the minimum and maximum values for the gauge
    private const float MinValue = 0f;
    private const float MaxValue = 100f;

    // Current animated value of the gauge needle
    private float _animatedValue;

    // Flag to indicate if an animation is in progress
    private bool _isAnimating;


    /// <summary>
    ///     Initializes a new instance of the <see cref="GaugeView"/> class.
    /// </summary>
    public GaugeView()
    {
        WidthRequest = 320;
        HeightRequest = 320;
    }


    #region Properties

    /// <summary>
    ///     Identifies the Value bindable property.
    /// </summary>
    public static readonly BindableProperty ValueProperty
        = BindableProperty.Create(
            nameof(Value),
            typeof(float),
            typeof(GaugeView),
            0.0f,
            propertyChanged: OnValueChanged);

    /// <summary>
    /// Gets or sets the value displayed by the gauge.
    /// </summary>
    public float Value
    {
        get => (float)GetValue(ValueProperty);
        set => SetValue(ValueProperty, Math.Clamp(value, MinValue, MaxValue));
    }

    /// <summary>
    /// Identifies the TextColor bindable property.
    /// </summary>
    public static readonly BindableProperty TextColorProperty
        = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(GaugeView),
            Colors.Black);

    /// <summary>
    /// Gets or sets the color of the text displayed on the gauge.
    /// </summary>
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Identifies the NeedleColor bindable property.
    /// </summary>
    public static readonly BindableProperty NeedleColorProperty
        = BindableProperty.Create(
            nameof(NeedleColor),
            typeof(Color),
            typeof(GaugeView),
            Colors.Black);

    /// <summary>
    /// Gets or sets the color of the gauge needle.
    /// </summary>
    public Color NeedleColor
    {
        get => (Color)GetValue(NeedleColorProperty);
        set => SetValue(NeedleColorProperty, value);
    }

    /// <summary>
    /// Identifies the NeedleScrewColor bindable property.
    /// </summary>
    public static readonly BindableProperty NeedleScrewColorProperty
        = BindableProperty.Create(
            nameof(NeedleScrewColor),
            typeof(Color),
            typeof(GaugeView),
            Colors.DarkGray);

    /// <summary>
    /// Gets or sets the color of the needle screw.
    /// </summary>
    public Color NeedleScrewColor
    {
        get => (Color)GetValue(NeedleScrewColorProperty);
        set => SetValue(NeedleScrewColorProperty, value);
    }

    /// <summary>
    /// Identifies the Unit bindable property.
    /// </summary>
    public static readonly BindableProperty UnitProperty
        = BindableProperty.Create(
            nameof(Unit),
            typeof(string),
            typeof(GaugeView),
            string.Empty);

    /// <summary>
    /// Gets or sets the unit of measurement displayed on the gauge.
    /// </summary>
    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    /// <summary>
    /// Identifies the ValueFontSize bindable property.
    /// </summary>
    public static readonly BindableProperty ValueFontSizeProperty
        = BindableProperty.Create(
            nameof(ValueFontSize),
            typeof(float),
            typeof(GaugeView),
            33.0f);

    /// <summary>
    /// Gets or sets the font size of the value text displayed on the gauge.
    /// </summary>
    public float ValueFontSize
    {
        get => (float)GetValue(ValueFontSizeProperty);
        set => SetValue(ValueFontSizeProperty, value);
    }

    #endregion


    /// <summary>
    ///     Called when the surface needs to be painted.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnPaintSurface(
        SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        SKCanvas canvas = e.Surface.Canvas;
        canvas.Clear();

        float width = e.Info.Width;
        float height = e.Info.Height;
        float size = Math.Min(width, height);

        float centerX = width / 2;
        float centerY = height / 2;

        float scale = size / 210f;

        canvas.Translate(centerX, centerY);
        canvas.Scale(scale);

        DrawBackground(canvas, size);
        DrawGauge(canvas);
        DrawNeedle(canvas, _animatedValue);
        DrawNeedleScrew(canvas);
        DrawValueText(canvas);
    }

    /// <summary>
    ///     Draws the background of the gauge.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="size">The size of the canvas.</param>
    private static void DrawBackground(
        SKCanvas canvas,
        float size)
    {
        canvas.DrawRect(new SKRect(-size / 2, -size / 2, size / 2, size / 2),
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Transparent,
            });
    }

    /// <summary>
    ///     Draws the gauge on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    private static void DrawGauge(
        SKCanvas canvas)
    {
        SKRect rect = new(-100, -100, 100, 100);
        rect.Inflate(-10, -10);

        DrawArc(canvas, rect, 135, SweepAngle, SKColors.DarkGray);
        DrawArc(canvas, rect, 202.5f, SweepAngle, SKColors.LightGray);
        DrawArc(canvas, rect, 270, SweepAngle, SKColors.DarkGray);
        DrawArc(canvas, rect, 337.5f, SweepAngle, SKColors.LightGray);
    }

    /// <summary>
    ///     Draws an arc on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="rect">The rectangle bounding the arc.</param>
    /// <param name="startAngle">The starting angle of the arc.</param>
    /// <param name="sweepAngle">The sweep angle of the arc.</param>
    /// <param name="color">The color of the arc.</param>
    private static void DrawArc(
        SKCanvas canvas,
        SKRect rect,
        float startAngle,
        float sweepAngle,
        SKColor color)
    {
        using SKPath path = new();

        path.AddArc(rect, startAngle, sweepAngle);

        canvas.DrawPath(path, new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = 10
        });
    }

    /// <summary>
    ///     Draws the needle of the gauge.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="value">The value represented by the needle.</param>
    private void DrawNeedle(
        SKCanvas canvas,
        float value)
    {
        float angle = -135f + value / 100 * 270f;
        canvas.Save();
        canvas.RotateDegrees(angle);

        SKPaint paint = new()
        {
            IsAntialias = true,
            Color = NeedleColor.ToSKColor()
        };

        SKPath needlePath = new();
        needlePath.MoveTo(0, -76);
        needlePath.LineTo(-6, 0);
        needlePath.LineTo(6, 0);
        needlePath.Close();

        canvas.DrawPath(needlePath, paint);
        canvas.Restore();
    }


    /// <summary>
    ///     Draws the screw at the center of the needle.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    private void DrawNeedleScrew(
        SKCanvas canvas)
    {
        canvas.DrawCircle(0, 0, 10, new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = NeedleScrewColor.ToSKColor()
        });
    }

    /// <summary>
    ///     Draws the value text and unit text on the gauge.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    private void DrawValueText(
        SKCanvas canvas)
    {
        SKPaint textPaint = new()
        {
            IsAntialias = true,
            Color = TextColor.ToSKColor(),
            TextSize = 12f
        };

        DrawUnitText(canvas, Unit, 95, textPaint);

        textPaint.TextSize = ValueFontSize;
        DrawUnitText(canvas, _animatedValue.ToString("F2"), 85, textPaint);
    }

    /// <summary>
    ///     Draws a unit text on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="y">The y-coordinate of the text.</param>
    /// <param name="paint">The paint to use for drawing the text.</param>
    private static void DrawUnitText(
        SKCanvas canvas,
        string text,
        float y,
        SKPaint paint)
    {
        SKRect textBounds = new();
        paint.MeasureText(text, ref textBounds);
        canvas.DrawText(text, -textBounds.MidX, y - textBounds.Height, paint);
    }

    /// <summary>
    ///     Called when the value property changes.
    /// </summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    private static async void OnValueChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        if (bindable is GaugeView gaugeView)
        {
            await gaugeView.AnimateNeedleAsync((float)newValue);
        }
    }

    /// <summary>
    ///     Animates the needle to a new value.
    /// </summary>
    /// <param name="toValue">The new value to animate to.</param>
    private async Task AnimateNeedleAsync(
        float toValue)
    {
        if (_isAnimating)
        {
            return;
        }

        _isAnimating = true;

        float stepSize = (toValue - _animatedValue) / StepsPerFrame;
        for (int i = 0; i < StepsPerFrame; i++)
        {
            _animatedValue += stepSize;
            InvalidateSurface();
            await Task.Delay(16);
        }

        _animatedValue = toValue;
        InvalidateSurface();
        _isAnimating = false;
    }
}
