using System;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for MissingExecutableWindow.xaml
/// </summary>
public partial class InvalidExecutableWindow
{
    private readonly SavedFileSystemItem _item;

    private bool _shouldShowSearchOnClose = true;

    public InvalidExecutableWindow(SavedFileSystemItem item, string windowTitle, string explanation)
    {
        _item = item;
        WindowTitle = windowTitle;
        Explanation = explanation;
        DataContext = this;
        InitializeComponent();
    }

    public string WindowTitle { get; }

    public string Explanation { get; }

    public string ExecutableName => _item.ExecutableName;

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (_shouldShowSearchOnClose)
        {
            new ShowSearchCommand().Execute();
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        AppData.DeleteItemAndSave(_item);
        Close();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        _shouldShowSearchOnClose = false;
        new ChangeExecutableCommand(_item, false).Execute();
        Close();
    }
}
