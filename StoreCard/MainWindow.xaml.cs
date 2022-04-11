﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreCard;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    private ItemCategory _category = ItemCategory.None;

    private List<SavedItem> _savedItems;

    private string _searchText = "";

    public MainWindow()
    {
        InitializeComponent();

        _savedItems = StorageUtils.ReadItemsFromFile();


        // Without this, opening the context menu for the first time results
        // in a small delay that looks strange; the button's BG color flashes
        // to the "mouse over" state before the menu opens.
        AddButtonContextMenu.Visibility = Visibility.Collapsed;
        AddButtonContextMenu.IsOpen = true;
        AddButtonContextMenu.IsOpen = false;

        if (_savedItems.Count > 0) ItemListBox.SelectedIndex = 0;

        Activate();
        SearchBox.Focus();

        if (!Application.Current.Windows
                .Cast<Window>().Any(w => w is TaskbarIconWindow))
            new TaskbarIconWindow().Show();

        DataContext = this;
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged("SearchText");
            OnPropertyChanged("FilteredItems");
        }
    }

    public IEnumerable<SavedItem> FilteredItems
    {
        get
        {
            var items = Category == ItemCategory.None
                ? _savedItems
                : _savedItems.Where(item => item.Category == Category);
            items = items.Where(item => item.Name.ToUpper().StartsWith(_searchText.ToUpper()));
            items = items.Concat(_savedItems.Where(item =>
            {
                return !item.Name.ToUpper().StartsWith(_searchText.ToUpper()) &&
                       item.Name.ToUpper().Contains(_searchText.ToUpper());
            }));
            return items;
        }
    }

    public ItemCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged("Category");
            OnPropertyChanged("FilteredItems");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RefreshSavedItems()
    {
        _savedItems = StorageUtils.ReadItemsFromFile();
        OnPropertyChanged("FilteredItems");
    }

    private void OnPropertyChanged(string name)
    {
        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        if (ItemListBox != null && ItemListBox.Items.Count > 0) ItemListBox.SelectedIndex = 0;
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
                if (ItemListBox.Items.Count == 0) return;
                switch (ItemListBox.SelectedIndex)
                {
                    case 0:
                    case -1:
                        ItemListBox.SelectedIndex = ItemListBox.Items.Count - 1;
                        break;
                    default:
                        ItemListBox.SelectedIndex = (ItemListBox.SelectedIndex - 1) % ItemListBox.Items.Count;
                        break;
                }

                ItemListBox.ScrollIntoView(ItemListBox.SelectedItem);
                break;
            case Key.Down:
                if (ItemListBox.Items.Count == 0) return;
                if (ItemListBox.SelectedIndex == -1)
                    ItemListBox.SelectedIndex = 0;
                else
                    ItemListBox.SelectedIndex = (ItemListBox.SelectedIndex + 1) % ItemListBox.Items.Count;
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
        if (e.Key == Key.Enter)
        {
            OpenSelectedItem();
            e.Handled = true;
        }
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
                break;
            case Key.Right:
                Category = (ItemCategory) Nfmod((int) Category + 1, Enum.GetNames(typeof(ItemCategory)).Length);
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

    private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) OpenSelectedItem();
    }

    private static uint Nfmod(float a, float b)
    {
        return (uint) (a - b * Math.Floor(a / b));
    }
}