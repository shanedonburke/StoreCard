using System.Windows;
using System.Windows.Media;

namespace StoreCard.UserControls;

/// <summary>
/// Interaction logic for StoreCardButton.xaml
/// </summary>
public partial class StoreCardButton
{
    public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
        nameof(IsEnabled),
        typeof(bool),
        typeof(StoreCardButton),
        new FrameworkPropertyMetadata(true));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(StoreCardButton));

    public new static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        nameof(FontSize),
        typeof(int),
        typeof(StoreCardButton),
        new FrameworkPropertyMetadata(14));

    public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register(
        nameof(BackgroundBrush),
        typeof(Brush),
        typeof(StoreCardButton));

    public static readonly DependencyProperty HoverBackgroundBrushProperty = DependencyProperty.Register(
        nameof(HoverBackgroundBrush),
        typeof(Brush),
        typeof(StoreCardButton));

    public static readonly DependencyProperty PressedBackgroundBrushProperty = DependencyProperty.Register(
        nameof(PressedBackgroundBrush),
        typeof(Brush),
        typeof(StoreCardButton));

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        nameof(Click),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(StoreCardButton));

    public new bool IsEnabled
    {
        get => (bool) GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public new int FontSize
    {
        get => (int) GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }


    public Brush BackgroundBrush
    {
        get => (Brush)GetValue(BackgroundBrushProperty);
        set => SetValue(BackgroundBrushProperty, value);
    }

    public Brush HoverBackgroundBrush
    {
        get => (Brush) GetValue(HoverBackgroundBrushProperty);
        set => SetValue(HoverBackgroundBrushProperty, value);
    }

    public Brush PressedBackgroundBrush
    {
        get => (Brush) GetValue(PressedBackgroundBrushProperty);
        set => SetValue(PressedBackgroundBrushProperty, value);
    }

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public StoreCardButton()
    {
        InitializeComponent();
    }

    private void CustomButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent) {RoutedEvent = ClickEvent});
    }
}