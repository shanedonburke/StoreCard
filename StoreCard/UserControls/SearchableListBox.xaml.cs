#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Models.Items;
using StoreCard.Properties;

#endregion

namespace StoreCard.UserControls;

/// <summary>
/// An enhanced list box that includes a search bar. Items may be added through code.
/// </summary>
public partial class SearchableListBox : INotifyPropertyChanged
{
    public delegate void ItemActivatedEventHandler(object sender, ItemActivatedEventArgs e);

    public static readonly RoutedEvent ItemActivatedEvent = EventManager.RegisterRoutedEvent(
        nameof(ItemActivated),
        RoutingStrategy.Bubble,
        typeof(ItemActivatedEventHandler),
        typeof(SearchableListBox));

    public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(SelectionChanged),
        RoutingStrategy.Bubble,
        typeof(SelectionChangedEventHandler),
        typeof(SearchableListBox));

    /// <summary>
    /// Items filtered through search. An <see cref="ObservableCollection{IListBoxItem}"/> is used to ensure
    /// that the list box updates safely when the items within it change.
    /// </summary>
    private readonly ObservableCollection<IListBoxItem> _filteredItems = new();

    private bool _areItemsLoaded;

    /// <summary>
    /// Unfiltered items.
    /// </summary>
    private readonly List<IListBoxItem> _items = new();

    public SearchableListBox()
    {
        InitializeComponent();
        CustomListBox.ItemsSource = _filteredItems;
    }

    /// <summary>
    /// The items source of the list box.
    /// </summary>
    public IEnumerable<IListBoxItem> ItemsSource => CustomListBox.ItemsSource;

    /// <summary>
    /// Whether the client has finished adding items. Determines
    /// whether the spinner is shown.
    /// </summary>
    public bool AreItemsLoaded
    {
        get => _areItemsLoaded;
        set
        {
            _areItemsLoaded = value;
            OnPropertyChanged(nameof(AreItemsLoaded));
        }
    }

    /// <summary>
    /// The currently selected item.
    /// </summary>
    public IListBoxItem? SelectedItem => CustomListBox.SelectedItem as IListBoxItem;

    /// <summary>
    /// The index of the currently selected item.
    /// </summary>
    public int SelectedIndex
    {
        get => CustomListBox.SelectedIndex;
        set => CustomListBox.SelectedIndex = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Event triggered when the user presses "Enter" to activate the selected item.
    /// </summary>
    public event ItemActivatedEventHandler ItemActivated
    {
        add => AddHandler(ItemActivatedEvent, value);
        remove => RemoveHandler(ItemActivatedEvent, value);
    }

    /// <summary>
    /// Event triggered when the selected item changes.
    /// </summary>
    public event SelectionChangedEventHandler SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    /// <summary>
    /// Adds an item to the list box.
    /// </summary>
    /// <param name="item">The item to add</param>
    public void AddItem(IListBoxItem item) =>
        // Interacting with the UI must occur on the main thread
        Application.Current.Dispatcher.Invoke(() =>
        {
            // Don't sort the items yet; sorting the
            // items every time we add one is expensive.
            // Item loading tends to be quick, so this isn't too noticeable.
            _filteredItems.Add(item);
            _items.Add(item);

            // Automatically select first item, so an item is always selected
            if (_items.Any())
            {
                SelectedIndex = 0;
            }
        });

    /// <summary>
    /// Indicate that item loading is finished. This should always be called
    /// to remove the spinner and sort the added items.
    /// </summary>
    public void FinishAddingItems()
    {
        SortAndFilterItems();
        AreItemsLoaded = true;
    }

    /// <summary>
    /// Sort and filter items using the search text, updating the list box.
    /// </summary>
    public void SortAndFilterItems()
    {
        _items.Sort();

        // Items whose name starts with the search text, e.g., "Firefox" with "Fi".
        IEnumerable<IListBoxItem> items = _items
            .Where(DoesItemNameStartWithSearchText);
        // After those, show items with the search text somewhere else, e.g., "Notepad" for "tep".
        // The first condition ensures that we don't get duplicates of the items found above.
        items = items.Concat(ItemsSource.Where(item =>
            !DoesItemNameStartWithSearchText(item) && DoesItemNameContainSearchText(item)));

        // Interacting with UI must occur on the main thread
        Application.Current.Dispatcher.Invoke(() =>
        {
            // ObservableCollection doesn't have a Sort() method. There are ways to rearrange items
            // to be sorted, but clearing the list and readding the items in order was found
            // to be more faster and more reliable.
            _filteredItems.Clear();

            foreach (IListBoxItem item in items)
            {
                _filteredItems.Add(item);
            }

            // Automatically select first item, so an item is always selected
            if (_filteredItems.Any())
            {
                SelectedIndex = 0;
            }
        });
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => SortAndFilterItems();

    private bool DoesItemNameStartWithSearchText(IListBoxItem item) =>
        item.Name.ToUpper().StartsWith(SearchBox.Text.ToUpper());

    private bool DoesItemNameContainSearchText(IListBoxItem item) =>
        item.Name.ToUpper().Contains(SearchBox.Text.ToUpper());

    private void ActivateSelectedItem()
    {
        if (CustomListBox.SelectedIndex != -1)
        {
            RaiseEvent(new ItemActivatedEventArgs((IListBoxItem)CustomListBox.SelectedItem)
            {
                RoutedEvent = ItemActivatedEvent
            });
        }
    }

    private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (!CustomListBox.ItemsSource.Any())
        {
            return;
        }

        // Move the list box selection using the arrow keys when the search box
        // is focused. This happens already when the list box is focused.
        switch (e.Key)
        {
            case Key.Up:
                CustomListBox.SelectedIndex = CustomListBox.SelectedIndex switch
                {
                    // Pressing Up while on the first item goes to the last
                    -1 or 0 => CustomListBox.ItemsSource.Count() - 1,
                    _ => (CustomListBox.SelectedIndex - 1) % CustomListBox.ItemsSource.Count()
                };

                CustomListBox.ScrollIntoView(CustomListBox.SelectedItem);
                break;
            case Key.Down:
                if (CustomListBox.SelectedIndex == -1)
                {
                    CustomListBox.SelectedIndex = 0;
                }
                else
                {
                    // Pressing down while on the last item goes to the first
                    CustomListBox.SelectedIndex = (CustomListBox.SelectedIndex + 1) %
                                                  CustomListBox.ItemsSource.Count();
                }

                CustomListBox.ScrollIntoView(CustomListBox.SelectedItem);
                break;
        }
    }

    private void CustomListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));

    private void SearchBox_KeyUp(object sender, KeyEventArgs e) => HandleKeyUpEvent(e);

    private void CustomListBox_KeyUp(object sender, KeyEventArgs e) => HandleKeyUpEvent(e);

    /// <summary>
    /// Activates the selected item when the user presses "Enter".
    /// </summary>
    /// <param name="e"></param>
    private void HandleKeyUpEvent(KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        ActivateSelectedItem();
        e.Handled = true;
    }
}

/// <summary>
/// Arguments for the <c>ItemActivated</c> event.
/// </summary>
public sealed class ItemActivatedEventArgs : RoutedEventArgs
{
    public IListBoxItem Item;

    public ItemActivatedEventArgs(IListBoxItem item) => Item = item;
}
