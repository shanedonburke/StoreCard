using System.Windows;

namespace StoreCard.UserControls.Buttons;

/// <summary>
/// Interaction logic for PrimaryButton.xaml
/// </summary>
public partial class PrimaryButton
{
    public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
        nameof(IsEnabled),
        typeof(bool),
        typeof(PrimaryButton),
        new FrameworkPropertyMetadata(true));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(PrimaryButton));

    public new static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        nameof(FontSize),
        typeof(int),
        typeof(PrimaryButton),
        new FrameworkPropertyMetadata(14));

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        nameof(Click),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(PrimaryButton));

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

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public PrimaryButton()
    {
        InitializeComponent();
    }

    private void CustomButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent)
            {RoutedEvent = ClickEvent});
    }
}