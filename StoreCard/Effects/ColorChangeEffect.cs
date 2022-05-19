using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Color = System.Windows.Media.Color;

namespace StoreCard.Effects;

public class ColorChangeEffect : ShaderEffect
{
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty("Input", typeof(ColorChangeEffect), 0);

    public static readonly DependencyProperty TargetColorProperty =
        DependencyProperty.Register("TargetColor", typeof(Color), typeof(ColorChangeEffect),
            new UIPropertyMetadata(Colors.White, PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty TargetBrushProperty =
        DependencyProperty.Register("TargetBrush", typeof(SolidColorBrush), typeof(ColorChangeEffect),
            new UIPropertyMetadata(new SolidColorBrush(Colors.White), OnTargetBrushChanged));

    private static readonly PixelShader s_pixelShader = new();

    static ColorChangeEffect()
    {
        s_pixelShader.UriSource = new Uri("pack://application:,,,/Effects/bin/ColorChange.ps");
    }

    public ColorChangeEffect()
    {
        PixelShader = s_pixelShader;
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(TargetColorProperty);
    }

    private static void OnTargetBrushChanged(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        (sender as ColorChangeEffect)!.SetValue(TargetColorProperty, (e.NewValue as SolidColorBrush)!.Color);
    }

    public SolidColorBrush TargetBrush
    {
        get => (SolidColorBrush)GetValue(TargetBrushProperty);
        set => SetValue(TargetColorProperty, value.Color);
    }
}
