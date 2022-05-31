#region

using System;
using System.Windows;
using System.Windows.Input;
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

    /// <summary>
    /// Whether the main window should be shown when this window closes.
    /// Will be <c>false</c> when we choose to edit the item (opening another window).
    /// </summary>
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

    /// <summary>
    /// Short explanation of the reason for the window.
    /// Will be interpolated into the phrase "but that executable {explanation}".
    /// </summary>
    public string Explanation { get; }

    /// <summary>
    /// Display name of the executable.
    /// </summary>
    public string ExecutableName { get; }

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

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Close the window if Escape is pressed
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (_shouldShowSearchOnClose)
        {
            new ShowSearchCommand().Execute();
        }
    }
}
