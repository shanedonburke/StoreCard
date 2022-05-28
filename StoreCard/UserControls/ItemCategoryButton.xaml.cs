using System.Windows;
using StoreCard.Models.Items.Saved;

namespace StoreCard.UserControls;

/// <summary>
/// Interaction logic for ItemCategoryButton.xaml
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

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public string CategoryName
    {
        get => (string)GetValue(CategoryNameProperty);
        set => SetValue(CategoryNameProperty, value);
    }

    public ItemCategory SelectedCategory
    {
        get => (ItemCategory)GetValue(SelectedCategoryProperty);
        set => SetValue(SelectedCategoryProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent) {RoutedEvent = ClickEvent});
    }
}
