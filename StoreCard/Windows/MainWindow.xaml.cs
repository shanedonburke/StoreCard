using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    private ItemCategory _category = ItemCategory.None;

    private List<SavedItem> _savedItems;

    public MainWindow()
    {
        _savedItems = AppData.ReadItemsFromFile();

        InitializeComponent();
        DataContext = this;

        // Without this, opening the context menu for the first time results
        // in a small delay that looks strange; the button's BG color flashes
        // to the "mouse over" state before the menu opens.
        AddButtonContextMenu.Visibility = Visibility.Collapsed;
        AddButtonContextMenu.IsOpen = true;
        AddButtonContextMenu.IsOpen = false;

        RefreshItems();

        SelectFirstItem();
    }

    public ItemCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged(nameof(Category));
            RefreshItems();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private static uint Nfmod(float a, float b) {
        return (uint)(a - b * Math.Floor(a / b));
    }

    public void RefreshSavedItems()
    {
        _savedItems = AppData.ReadItemsFromFile();
        RefreshItems();
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private IEnumerable<SavedItem> FilterItems()
    {
        var items = Category switch
        {
            ItemCategory.None => _savedItems,
            ItemCategory.Recent => GetRecentItems(),
            _ => _savedItems.Where(item => item.Category == Category)
        };
        items = items.Where(item => item.Name.ToUpper().StartsWith(SearchBox.Text.ToUpper()));
        items = items.Concat(_savedItems.Where(item => !item.Name.ToUpper().StartsWith(SearchBox.Text.ToUpper()) &&
                                                       item.Name.ToUpper().Contains(SearchBox.Text.ToUpper())));
        return items;
    }

    private IEnumerable<SavedItem> GetRecentItems()
    {
        var recentItems = _savedItems.ToList();
        recentItems.Sort((a, b) => b.LastOpened.CompareTo(a.LastOpened));
        return recentItems.Take(Range.EndAt(20));
    }

    private void RefreshItems()
    {
        ItemListBox.ItemsSource = FilterItems();
    }

    private void AddApplication_Click(object sender, RoutedEventArgs e)
    {
        new AddApplicationCommand().Execute();
        Close();
    }

    private void AddFile_Click(object sender, RoutedEventArgs e)
    {
        new AddFileCommand().Execute();
        Close();
    }

    private void AddLink_Click(object sender, RoutedEventArgs e)
    {
        new AddLinkCommand().Execute();
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
                if (!ItemListBox.ItemsSource.Any()) return;
                switch (ItemListBox.SelectedIndex)
                {
                    case 0:
                    case -1:
                        ItemListBox.SelectedIndex = ItemListBox.ItemsSource.Count() - 1;
                        break;
                    default:
                        ItemListBox.SelectedIndex = (ItemListBox.SelectedIndex - 1) % ItemListBox.ItemsSource.Count();
                        break;
                }

                ItemListBox.ScrollIntoView(ItemListBox.SelectedItem);
                break;
            case Key.Down:
                if (!ItemListBox.ItemsSource.Any()) return;
                if (ItemListBox.SelectedIndex == -1)
                {
                    SelectFirstItem();
                }
                else
                {
                    ItemListBox.SelectedIndex = (ItemListBox.SelectedIndex + 1) % ItemListBox.ItemsSource.Count();
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

    private void MainWindow_Activated(object? sender, EventArgs e)
    {
        SearchBox.Focus();
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

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        OpenSelectedItem();
    }

    private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedIndex == -1) return;
        AppData.DeleteItemAndSave((SavedItem) ItemListBox.SelectedItem);
        RefreshSavedItems();
    }

    private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        OpenSelectedItem();
    }

    private void EditFileMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedItem is SavedFileSystemItem item)
        {
            new EditFileCommand(item).Execute();
        }
        else
        {
            Debug.WriteLine("Tried to edit the selected item as a file, but the item is not a file or folder.");
        }
        Close();
    }

    private void EditExecutableMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedItem is SavedExecutable executable) {
            new EditExecutableCommand(executable).Execute();
        } else {
            Debug.WriteLine("Tried to edit the selected item as an executable, but the item is not an executable.");
        }
        Close();
    }

    private void EditLinkMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedItem is SavedLink link)
        {
            new EditLinkCommand(link).Execute();
        }
        else
        {
            Debug.WriteLine("Tried to edit the selected item as a link, but the item is not a link.");
        }
        Close();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        new ShowSettingsCommand().Execute();
        Close();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshItems();
        if (ItemListBox.SelectedIndex == -1)
        {
            SelectFirstItem();
        }
    }

    private void SelectFirstItem()
    {
        if (ItemListBox.ItemsSource.Any()) ItemListBox.SelectedIndex = 0;
    }

    private void AllCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        Category = ItemCategory.None;
    }

    private void RecentCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        Category = ItemCategory.Recent;
    }

    private void AppsCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        Category = ItemCategory.App;
    }

    private void GamesCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        Category = ItemCategory.Game;
    }

    private void FoldersCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        Category = ItemCategory.Folder;
    }

    private void FilesCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        Category = ItemCategory.File;
    }

    private void LinksCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        Category = ItemCategory.Link;
    }
}
