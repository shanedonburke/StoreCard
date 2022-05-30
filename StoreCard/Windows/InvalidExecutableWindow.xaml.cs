#region

using System;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for MissingExecutableWindow.xaml
/// </summary>
public sealed partial class InvalidExecutableWindow
{
    private readonly SavedItem _item;

    private readonly Action _editAction;

    private bool _shouldShowSearchOnClose = true;

    public InvalidExecutableWindow(
        SavedItem item,
        string executableName,
        string windowTitle,
        string explanation,
        Action editAction)
    {
        _item = item;
        ExecutableName = executableName;
        WindowTitle = windowTitle;
        Explanation = explanation;
        _editAction = editAction;
        DataContext = this;
        InitializeComponent();
    }

    public string WindowTitle { get; }

    public string Explanation { get; }

    public string ExecutableName { get; }

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
        _editAction.Invoke();
        Close();
    }
}
