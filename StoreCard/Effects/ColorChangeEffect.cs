#region

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using StoreCard.Utils;

#endregion

namespace StoreCard.Effects;

/// <summary>
/// Applies a solid color to an image. For the right color to be displayed,
/// the source image must be white.
/// </summary>
public sealed class ColorChangeEffect : ShaderEffect
{
    /// <summary>
    /// The source image.
    /// </summary>
    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
        "Input",
        typeof(ColorChangeEffect),
        0);

    /// <summary>
    /// A brush representing the target color. This class could be simplified by exposing just the
    /// target color as a dependency property, but the theme files only include brushes for each color.
    /// Thus, a duplication (color + brush of that color) would need to be included in each theme.
    /// Converting a brush to its color in XAML is not trivial, so it was easier to do the conversion
    /// within this class.
    /// </summary>
    public static readonly DependencyProperty TargetBrushProperty = DependencyProperty.Register(
        nameof(TargetBrush),
        typeof(SolidColorBrush),
        typeof(ColorChangeEffect),
        new UIPropertyMetadata(new SolidColorBrush(Colors.White), OnTargetBrushChanged));

    /// <summary>
    /// The target color. Brushes cannot be passed into the shader, so this property is set
    /// privately through <see cref="TargetBrush"/>.
    /// </summary>
    private static readonly DependencyProperty s_targetColorProperty = DependencyProperty.Register(
        "s_targetColor",
        typeof(Color),
        typeof(ColorChangeEffect),
        new UIPropertyMetadata(Colors.White, PixelShaderConstantCallback(0)));

    private static readonly PixelShader s_pixelShader = new();

    /// <summary>
    /// Obtain the compiled shader. The shader is compiled in a pre-build event using
    /// "CompileShaders.bat" in this directory.
    /// </summary>
    static ColorChangeEffect() =>
        s_pixelShader.UriSource = UriUtils.BuildPackUri("Effects/bin/ColorChange.ps");

    public ColorChangeEffect()
    {
        PixelShader = s_pixelShader;
        // Pass values into the shader. Brushes cannot be passed in, but colors can.
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(s_targetColorProperty);
    }

    /// <summary>
    /// The target brush property, accessible through code.
    /// The getter returns the value of the brush property.
    /// The setter sets both the target brush property and the target color property.
    /// </summary>
    public SolidColorBrush TargetBrush
    {
        get => (SolidColorBrush)GetValue(TargetBrushProperty);
        set
        {
            SetValue(TargetBrushProperty, value);
            SetValue(s_targetColorProperty, value.Color);
        }
    }

    /// <summary>
    /// Triggered when the <see cref="TargetBrush"/> property is changed through XAML.
    /// Because the target color property is private and cannot be set through XAML, this
    /// handler is used to set that property's value using the new brush's color.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnTargetBrushChanged(
        DependencyObject sender,
        DependencyPropertyChangedEventArgs e) =>
        ((ColorChangeEffect)sender).SetValue(
            s_targetColorProperty,
            ((SolidColorBrush)e.NewValue).Color);
}
