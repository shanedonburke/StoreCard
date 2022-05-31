#region

using System.Windows;
using StoreCard.Models.Items.Saved;

#endregion

namespace StoreCard.UserControls;

/// <summary>
/// A special button that represents an item category. If the "selected" category
/// (in reference to the main window) is the same as this one, the button is highlighted.
/// Clicking the button triggers an event that selects the represented category.
/// </summary>
public partial class ItemCategoryButton
{
    public static readonly DependencyProperty CategoryNameProperty = DependencyProperty.Register(
        nameof(CategoryName),
        typeof(string),
        typeof(ItemCategoryButton));

    public static readonly DependencyProperty SelectedCategoryProperty = DependencyProperty.Register(
        nameof(SelectedCategory),
        typeof(ItemCategory),
        typeof(ItemCategoryButton));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(ItemCategoryButton));

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        nameof(Click),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ItemCategoryButton));

    public ItemCategoryButton()
    {
        DataContext = this;
        InitializeComponent();
    }

    /// <summary>
    /// Name of the category this button represents.
    /// </summary>
    public string CategoryName
    {
        get => (string)GetValue(CategoryNameProperty);
        set => SetValue(CategoryNameProperty, value);
    }

    /// <summary>
    /// The currently selected item category.
    /// </summary>
    public ItemCategory SelectedCategory
    {
        get => (ItemCategory)GetValue(SelectedCategoryProperty);
        set => SetValue(SelectedCategoryProperty, value);
    }

    /// <summary>
    /// The text to show for the button.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    private void Button_Click(object sender, RoutedEventArgs e) =>
        RaiseEvent(new RoutedEventArgs(ClickEvent) {RoutedEvent = ClickEvent});
}
