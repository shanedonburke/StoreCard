#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// The main window that shows the search bar and other controls.
/// </summary>
public sealed partial class MainWindow : INotifyPropertyChanged
{
    private ItemCategory _category = ItemCategory.None;

    private List<SavedItem> _savedItems;

    public MainWindow()
    {
        _savedItems = AppData.ReadItemsFromFile();
        DataContext = this;
        InitializeComponent();

        // Without this, opening the context menu for the first time results
        // in a small delay that looks strange; the button's BG color flashes
        // to the "mouse over" state before the menu opens.
        AddButtonContextMenu.Visibility = Visibility.Collapsed;
        AddButtonContextMenu.IsOpen = true;
        AddButtonContextMenu.IsOpen = false;

        OnPropertyChanged(nameof(NoSavedItems));
        ApplyFilter();
        SelectFirstItem();
    }

    /// <summary>
    /// The currently selected item category. Filters displayed items.
    /// </summary>
    public ItemCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged(nameof(Category));
            ApplyFilter();
        }
    }

    /// <summary>
    /// Whether the user has saved any items (before filtering).
    /// Used to control when the "You haven't added any items" text is shown.
    /// </summary>
    public bool NoSavedItems => _savedItems.Count == 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// A modulo function that always returns a positive number.<br/>
    /// Ex. <c>Nfmod(-1, 3) == 2</c><br/>
    /// Ex. <c>Nfmod(3, 3) == 0</c>
    /// </summary>
    /// <param name="a">First operand of modulo</param>
    /// <param name="b">Second operand of modulo</param>
    /// <returns></returns>
    private static uint Nfmod(float a, float b) => (uint)(a - b * Math.Floor(a / b));

    /// <summary>
    /// Refresh the list of saved items from the file system.
    /// </summary>
    public void RefreshSavedItems()
    {
        _savedItems = AppData.ReadItemsFromFile();
        ApplyFilter();
        OnPropertyChanged(nameof(NoSavedItems));
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Filter the items in the list by the search text and <see cref="Category"/>.
    /// </summary>
    /// <returns>The new list of items</returns>
    private IEnumerable<SavedItem> FilterItems()
    {
        // Filter first by category
        IEnumerable<SavedItem> items = Category switch
        {
            ItemCategory.None => _savedItems,
            ItemCategory.Recent => GetRecentItems(),
            _ => _savedItems.Where(item => item.Category == Category)
        };
        // Show items whose names begin with the search text, then items whose names contain the text elsewhere
        items = items.Where(item => item.Name.ToUpper().StartsWith(SearchBox.Text.ToUpper()));
        // The first condition ensures we don't include items from above
        items = items.Concat(_savedItems.Where(item => !item.Name.ToUpper().StartsWith(SearchBox.Text.ToUpper()) &&
                                                       item.Name.ToUpper().Contains(SearchBox.Text.ToUpper())));
        return items;
    }

    /// <summary>
    /// Get the first 20 items in the order in which they were last opened (recent first).
    /// </summary>
    /// <returns>The list of items</returns>
    private IEnumerable<SavedItem> GetRecentItems()
    {
        var recentItems = _savedItems.ToList();
        recentItems.Sort((a, b) => b.LastOpened.CompareTo(a.LastOpened));
        return recentItems.Take(Range.EndAt(20));
    }

    /// <summary>
    /// Apply the current filter to the items in the list box.
    /// </summary>
    private void ApplyFilter() => ItemListBox.ItemsSource = FilterItems();

    private void OpenSelectedItem()
    {
        (ItemListBox.SelectedItem as SavedItem)?.Open();
        Close();
    }

    private void SelectFirstItem()
    {
        if (ItemListBox.ItemsSource.Any())
        {
            ItemListBox.SelectedIndex = 0;
        }
    }

    private void AddApplication_Click(object sender, RoutedEventArgs e)
    {
        new AddAppCommand().Execute();
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

    private void OpenButton_Click(object sender, RoutedEventArgs e) => OpenSelectedItem();

    /// <summary>
    /// Handle keys pressed while the search box is focused
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (!ItemListBox.ItemsSource.Any())
        {
            return;
        }

        switch (e.Key)
        {
            // Move list selection with arrow keys. This happens already when the list box is focused
            case Key.Up:
                if (ItemListBox.SelectedIndex == -1)
                {
                    ItemListBox.SelectedIndex = ItemListBox.ItemsSource.Count() - 1;
                }
                else
                {
                    ItemListBox.SelectedIndex = (int)Nfmod(ItemListBox.SelectedIndex - 1, ItemListBox.ItemsSource.Count());
                }

                ItemListBox.ScrollIntoView(ItemListBox.SelectedItem);
                break;
            case Key.Down:
                if (ItemListBox.SelectedIndex == -1)
                {
                    SelectFirstItem();
                }
                else
                {
                    ItemListBox.SelectedIndex = (int)Nfmod(ItemListBox.SelectedIndex + 1, ItemListBox.ItemsSource.Count());
                }

                ItemListBox.ScrollIntoView(ItemListBox.SelectedItem);
                break;
            // Open selected item with Enter key
            case Key.Enter:
                OpenSelectedItem();
                e.Handled = true;
                break;
        }
    }

    private void ItemListBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        OpenSelectedItem();
        e.Handled = true;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            // Close window
            case Key.Escape:
                Deactivate();
                break;
            // Switch category with arrow keys
            case Key.Left:
                Category = (ItemCategory)Nfmod((int)Category - 1, Enum.GetNames(typeof(ItemCategory)).Length);
                SelectFirstItem();
                break;
            case Key.Right:
                Category = (ItemCategory)Nfmod((int)Category + 1, Enum.GetNames(typeof(ItemCategory)).Length);
                SelectFirstItem();
                break;
        }
    }

    private void MainWindow_Activated(object? sender, EventArgs e) => SearchBox.Focus();

    private void Window_Deactivated(object sender, EventArgs e) => Deactivate();

    private void Deactivate()
    {
        try
        {
            Close();
        }
        catch (Exception e)
        {
            Logger.LogExceptionMessage("Failed to close main window", e);
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddButtonContextMenu.Visibility = Visibility.Visible;
        AddButtonContextMenu.IsOpen = true;
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e) => OpenSelectedItem();

    private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedIndex == -1)
        {
            return;
        }

        AppData.DeleteItemAndSave((SavedItem)ItemListBox.SelectedItem);
        RefreshSavedItems();
    }

    private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e) => OpenSelectedItem();

    private void EditFileMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedItem is SavedFileSystemItem item)
        {
            new EditFileCommand(item).Execute();
        }
        else
        {
            Logger.Log("Tried to edit the selected item as a file, but the item is not a file or folder.");
        }

        Close();
    }

    private void EditExecutableMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ItemListBox.SelectedItem is SavedExecutable executable)
        {
            new EditExecutableCommand(executable).Execute();
        }
        else
        {
            Logger.Log("Tried to edit the selected item as an executable, but the item is not an executable.");
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
            Logger.Log("Tried to edit the selected item as a link, but the item is not a link.");
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
        ApplyFilter();

        if (ItemListBox.SelectedIndex == -1)
        {
            SelectFirstItem();
        }
    }

    private void AllCategoryButton_Click(object sender, RoutedEventArgs e) => Category = ItemCategory.None;

    private void RecentCategoryButton_Click(object sender, RoutedEventArgs e) => Category = ItemCategory.Recent;

    private void AppsCategoryButton_Click(object sender, RoutedEventArgs e) => Category = ItemCategory.App;

    private void GamesCategoryButton_Click(object sender, RoutedEventArgs e) => Category = ItemCategory.Game;

    private void FilesCategoryButton_Click(object sender, RoutedEventArgs e) => Category = ItemCategory.File;

    private void FoldersCategoryButton_Click(object sender, RoutedEventArgs e) => Category = ItemCategory.Folder;

    private void LinksCategoryButton_Click(object sender, RoutedEventArgs e) => Category = ItemCategory.Link;
}
