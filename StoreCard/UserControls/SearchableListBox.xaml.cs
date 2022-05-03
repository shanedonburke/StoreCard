using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Models.Items;
using StoreCard.Properties;

namespace StoreCard.UserControls
{
    /// <summary>
    /// Interaction logic for SearchableListBox.xaml
    /// </summary>
    public partial class SearchableListBox : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<IListBoxItem>),
            typeof(SearchableListBox));

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

        private IEnumerable<IListBoxItem> _filteredItems = new List<IListBoxItem>();

        private bool _areItemsLoaded;

        public IEnumerable<IListBoxItem> Items {
            get => (IEnumerable<IListBoxItem>) GetValue(ItemsProperty);
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetValue(ItemsProperty, value);
                    FilteredItems = FilterItems();
                    AreItemsLoaded = true;
                });
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

        public IEnumerable<IListBoxItem> FilteredItems
        {
            get => _filteredItems;
            set
            {
                _filteredItems = value;
                OnPropertyChanged(nameof(FilteredItems));
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_filteredItems.Any()) CustomListBox.SelectedIndex = 0;
                });
            }
        }

        public IListBoxItem? SelectedItem => CustomListBox.SelectedItem as IListBoxItem;

        public int SelectedIndex
        {
            get => CustomListBox.SelectedIndex;
            set => CustomListBox.SelectedIndex = value;
        }

        public SearchableListBox() {
            InitializeComponent();
        }

        public delegate void ItemActivatedEventHandler(object sender, ItemActivatedEventArgs e);

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

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<IListBoxItem> FilterItems() {
            var items = Items
                .Where(DoesItemNameStartWithSearchText);
            items = items.Concat(Items.Where(item => !DoesItemNameStartWithSearchText(item) &&
                                                     DoesItemNameContainSearchText(item)));
            return items;
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilteredItems = FilterItems();
        }

        private bool DoesItemNameStartWithSearchText(IListBoxItem item)
        {
            return item.Name.ToUpper().StartsWith(SearchBox.Text.ToUpper());
        }

        private bool DoesItemNameContainSearchText(IListBoxItem item)
        {
            return item.Name.ToUpper().Contains(SearchBox.Text.ToUpper());
        }

        private void ActivateSelectedItem() {
            if (CustomListBox.SelectedIndex != -1) {
                RaiseEvent(new ItemActivatedEventArgs((IListBoxItem)CustomListBox.SelectedItem) { RoutedEvent = ItemActivatedEvent });
            }
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Up:
                    if (!CustomListBox.Items.Any()) return;
                    switch (CustomListBox.SelectedIndex) {
                        case 0:
                        case -1:
                            CustomListBox.SelectedIndex = CustomListBox.Items.Count() - 1;
                            break;
                        default:
                            CustomListBox.SelectedIndex =
                                (CustomListBox.SelectedIndex - 1) % CustomListBox.Items.Count();
                            break;
                    }

                    CustomListBox.ScrollIntoView(CustomListBox.SelectedItem);
                    break;
                case Key.Down:
                    if (!CustomListBox.Items.Any()) return;
                    if (CustomListBox.SelectedIndex == -1)
                        CustomListBox.SelectedIndex = 0;
                    else
                        CustomListBox.SelectedIndex =
                            (CustomListBox.SelectedIndex + 1) % CustomListBox.Items.Count();
                    CustomListBox.ScrollIntoView(CustomListBox.SelectedItem);
                    break;
            }
        }

        private void CustomListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            HandleKeyUpEvent(e);
        }

        private void CustomListBox_KeyUp(object sender, KeyEventArgs e)
        {
            HandleKeyUpEvent(e);
        }

        private void HandleKeyUpEvent(KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            ActivateSelectedItem();
            e.Handled = true;
        }
    }

    public class ItemActivatedEventArgs : RoutedEventArgs
    {
        public ItemActivatedEventArgs(IListBoxItem item)
        {
            Item = item;
        }

        public IListBoxItem Item;
    }
}
