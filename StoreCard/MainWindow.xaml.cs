using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Annotations;

namespace StoreCard;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    private ItemCategory _category = ItemCategory.None;

    private List<SavedItem> _savedItems;

    public MainWindow()
    {
        _savedItems = StorageUtils.ReadItemsFromFile();

        InitializeComponent();
        DataContext = this;

        Activate();

        // Without this, opening the context menu for the first time results
        // in a small delay that looks strange; the button's BG color flashes
        // to the "mouse over" state before the menu opens.
        AddButtonContextMenu.Visibility = Visibility.Collapsed;
        AddButtonContextMenu.IsOpen = true;
        AddButtonContextMenu.IsOpen = false;

        SelectFirstItem();

        SearchBox.Focus();
    }

    public IEnumerable<SavedItem> FilteredItems
    {
        get
        {
            var items = Category == ItemCategory.None
                ? _savedItems
                : _savedItems.Where(item => item.Category == Category);
            items = items.Where(item => item.Name.ToUpper().StartsWith(SearchBox.Text.ToUpper()));
            items = items.Concat(_savedItems.Where(item => !item.Name.ToUpper().StartsWith(SearchBox.Text.ToUpper()) &&
                                                           item.Name.ToUpper().Contains(SearchBox.Text.ToUpper())));
            return items;
        }
    }

    public ItemCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged(nameof(Category));
            OnPropertyChanged(nameof(FilteredItems));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RefreshSavedItems()
    {
        _savedItems = StorageUtils.ReadItemsFromFile();
        OnPropertyChanged(nameof(FilteredItems));
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void AddApplication_Click(object sender, RoutedEventArgs e)
    {
        new AddApplicationWindow().Show();
        Close();
    }

    private void AddFile_Click(object sender, RoutedEventArgs e)
    {
        new AddFileWindow().Show();
        Close();
    }

    private void AddLink_Click(object sender, RoutedEventArgs e)
    {
        new AddLinkWindow().Show();
        Close();
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        OpenSelectedItem();
    }

    private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                if (!ItemListBox.Items.Any()) return;
                switch (ItemListBox.SelectedIndex)
                {
                    case 0:
                    case -1:
                        ItemListBox.SelectedIndex = ItemListBox.Items.Count() - 1;
                        break;
                    default:
                        ItemListBox.SelectedIndex = (ItemListBox.SelectedIndex - 1) % ItemListBox.Items.Count();
                        break;
                }

                ItemListBox.ScrollIntoView(ItemListBox.SelectedItem);
                break;
            case Key.Down:
                if (!ItemListBox.Items.Any()) return;
                if (ItemListBox.SelectedIndex == -1)
                {
                    SelectFirstItem();
                }
                else
                {
                    ItemListBox.SelectedIndex = (ItemListBox.SelectedIndex + 1) % ItemListBox.Items.Count();
                }
                ItemListBox.ScrollIntoView(ItemListBox.SelectedItem);
                break;
            case Key.Enter:
                OpenSelectedItem();
                e.Handled = true;
                break;
        }
    }

    private void OpenSelectedItem()
    {
        (ItemListBox.SelectedItem as SavedItem)?.Open();
        Close();
    }

    private void ItemListBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        OpenSelectedItem();
        e.Handled = true;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                Deactivate();
                break;
            case Key.Left:
                Category = (ItemCategory) Nfmod((int) Category - 1, Enum.GetNames(typeof(ItemCategory)).Length);
                SelectFirstItem();
                break;
            case Key.Right:
                Category = (ItemCategory) Nfmod((int) Category + 1, Enum.GetNames(typeof(ItemCategory)).Length);
                SelectFirstItem();
                break;
        }
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        Deactivate();
    }

    private void Deactivate()
    {
        try
        {
            Close();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddButtonContextMenu.Visibility = Visibility.Visible;
        AddButtonContextMenu.IsOpen = true;
    }

    private void ContextMenuOpenItem_Click(object sender, RoutedEventArgs e)
    {
        OpenSelectedItem();
    }

    private void DeleteItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedIndex == -1) return;
        var savedItems = StorageUtils.ReadItemsFromFile();
        savedItems.RemoveAt(ItemListBox.SelectedIndex);
        StorageUtils.SaveItemsToFile(savedItems);
        RefreshSavedItems();
    }

    private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        OpenSelectedItem();
    }

    private static uint Nfmod(float a, float b)
    {
        return (uint) (a - b * Math.Floor(a / b));
    }

    private void ContextMenuEditFileItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedItem is SavedFileSystemItem item)
        {
            new EditFileWindow(item).Show();
            Close();
        }
        else
        {
            Debug.WriteLine("Tried to open edit file window for non-file item.");
        }
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().Show();
        Close();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        OnPropertyChanged(nameof(FilteredItems));
        if (ItemListBox.SelectedIndex == -1)
        {
            SelectFirstItem();
        }
    }

    private void SelectFirstItem()
    {
        if (FilteredItems.Any()) ItemListBox.SelectedIndex = 0;
    }
}