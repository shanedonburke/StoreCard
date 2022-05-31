#region

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Properties;

#endregion

namespace StoreCard.UserControls;

/// <summary>
/// A custom text box that supports placeholder text.
/// </summary>
public partial class StoreCardTextBox : INotifyPropertyChanged
{
    public static readonly DependencyProperty ActivePlaceholderProperty = DependencyProperty.Register(
        nameof(ActivePlaceholder),
        typeof(string),
        typeof(StoreCardTextBox),
        new FrameworkPropertyMetadata(""));

    public static readonly DependencyProperty InactivePlaceholderProperty = DependencyProperty.Register(
        nameof(InactivePlaceholder),
        typeof(string),
        typeof(StoreCardTextBox),
        new FrameworkPropertyMetadata(""));

    public static new readonly RoutedEvent PreviewKeyDownEvent = EventManager.RegisterRoutedEvent(
        nameof(PreviewKeyDown),
        RoutingStrategy.Bubble,
        typeof(KeyEventHandler),
        typeof(StoreCardTextBox));

    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(TextChangedEventHandler),
        typeof(StoreCardTextBox));

    private string _text = string.Empty;

    public StoreCardTextBox()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// The user-entered text.
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged(nameof(Text));
        }
    }

    /// <summary>
    /// The placeholder text shown when the text box is focused, e.g., "Start typing to search...".
    /// </summary>
    public string ActivePlaceholder
    {
        get => (string)GetValue(ActivePlaceholderProperty);
        set => SetValue(ActivePlaceholderProperty, value);
    }

    /// <summary>
    /// The placeholder text shown when the text box is not focused, e.g., "Click to search...".
    /// </summary>
    public string InactivePlaceholder
    {
        get => (string)GetValue(InactivePlaceholderProperty);
        set => SetValue(InactivePlaceholderProperty, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public new event KeyEventHandler PreviewKeyDown
    {
        add => AddHandler(PreviewKeyDownEvent, value);
        remove => RemoveHandler(PreviewKeyDownEvent, value);
    }

    public new event KeyEventHandler KeyUp
    {
        add => AddHandler(KeyUpEvent, value);
        remove => RemoveHandler(KeyUpEvent, value);
    }

    public event TextChangedEventHandler TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    public new bool Focus() => CustomTextBox.Focus();

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void CustomTextBox_PreviewKeyDown(object sender, KeyEventArgs e) =>
        RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key)
        {
            RoutedEvent = PreviewKeyDownEvent
        });

    private void CustomTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
        RaiseEvent(new TextChangedEventArgs(TextChangedEvent, e.UndoAction));
}
