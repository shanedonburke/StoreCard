#region

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// A window to be shown when the user tries to open an item that doesn't exist
/// (e.g., a file that's been deleted). Includes Edit and Delete buttons.
/// </summary>
public sealed partial class MissingItemWindow : INotifyPropertyChanged
{
    private readonly SavedItem _item;

    /// <summary>
    /// Action to be invoked when the Edit button is pressed. If <c>null</c>,
    /// the edit button won't be shown.
    /// </summary>
    private readonly Action? _editAction;

    /// <summary>
    /// Whether the main window should be shown when this one closes.
    /// Will be <c>false</c> if the Edit button is pressed.
    /// </summary>
    private bool _shouldShowSearchOnClose = true;

    public MissingItemWindow(SavedItem item, Action editAction) : this(item)
    {
        _editAction = editAction;
        OnPropertyChanged(nameof(ShouldShowEditButton));
    }

    public MissingItemWindow(SavedItem item)
    {
        _item = item;
        DataContext = this;
        InitializeComponent();
    }

    /// <summary>
    /// Show the Edit button if there's a registered "edit" action.
    /// </summary>
    public bool ShouldShowEditButton => _editAction != null;

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        AppData.DeleteItemAndSave(_item);
        Close();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        _shouldShowSearchOnClose = false;
        _editAction?.Invoke();
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
