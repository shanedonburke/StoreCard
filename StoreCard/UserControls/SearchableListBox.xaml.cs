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
/// Interaction logic for SearchableListBox.xaml
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

    private readonly ObservableCollection<IListBoxItem> _filteredItems = new();

    private bool _areItemsLoaded;

    private List<IListBoxItem> _items = new();

    public SearchableListBox()
    {
        InitializeComponent();
        CustomListBox.ItemsSource = _filteredItems;
    }

    public IEnumerable<IListBoxItem> ItemsSource
    {
        get => CustomListBox.ItemsSource;
        set
        {
            _items = value.ToList();
            _items.Sort();
            FilterItems();
        }
    }

    public bool AreItemsLoaded
    {
        get => _areItemsLoaded;
        set
        {
            _areItemsLoaded = value;
            OnPropertyChanged(nameof(AreItemsLoaded));
        }
    }

    public IListBoxItem? SelectedItem => CustomListBox.SelectedItem as IListBoxItem;

    public int SelectedIndex
    {
        get => CustomListBox.SelectedIndex;
        set => CustomListBox.SelectedIndex = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event ItemActivatedEventHandler ItemActivated
    {
        add => AddHandler(ItemActivatedEvent, value);
        remove => RemoveHandler(ItemActivatedEvent, value);
    }

    public event SelectionChangedEventHandler SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    public void AddItem(IListBoxItem item) =>
        Application.Current.Dispatcher.Invoke(() =>
        {
            _filteredItems.Add(item);
            _items.Add(item);
            _items.Sort();

            if (_items.Any())
            {
                SelectedIndex = 0;
            }
        });

    public void FinishAddingItems()
    {
        FilterItems();
        AreItemsLoaded = true;
    }

    public void FilterItems()
    {
        IEnumerable<IListBoxItem> items = _items
            .Where(DoesItemNameStartWithSearchText);
        items = items.Concat(ItemsSource.Where(item =>
            !DoesItemNameStartWithSearchText(item) && DoesItemNameContainSearchText(item)));

        Application.Current.Dispatcher.Invoke(() =>
        {
            _filteredItems.Clear();

            foreach (IListBoxItem item in items)
            {
                _filteredItems.Add(item);
            }

            if (_filteredItems.Any())
            {
                SelectedIndex = 0;
            }
        });
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => FilterItems();

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
        switch (e.Key)
        {
            case Key.Up:
                if (!CustomListBox.ItemsSource.Any())
                {
                    return;
                }

                CustomListBox.SelectedIndex = CustomListBox.SelectedIndex switch
                {
                    0 => CustomListBox.ItemsSource.Count() - 1,
                    -1 => CustomListBox.ItemsSource.Count() - 1,
                    _ => (CustomListBox.SelectedIndex - 1) % CustomListBox.ItemsSource.Count()
                };

                CustomListBox.ScrollIntoView(CustomListBox.SelectedItem);
                break;
            case Key.Down:
                if (!CustomListBox.ItemsSource.Any())
                {
                    return;
                }

                if (CustomListBox.SelectedIndex == -1)
                {
                    CustomListBox.SelectedIndex = 0;
                }
                else
                {
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

public sealed class ItemActivatedEventArgs : RoutedEventArgs
{
    public IListBoxItem Item;

    public ItemActivatedEventArgs(IListBoxItem item) => Item = item;
}
