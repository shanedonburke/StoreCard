using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for MissingItemWindow.xaml
/// </summary>
public sealed partial class MissingItemWindow : INotifyPropertyChanged
{
    private readonly SavedItem _item;

    private readonly Action? _editAction;

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

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool ShouldShowEditButton => _editAction != null;

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

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (_shouldShowSearchOnClose)
        {
            new ShowSearchCommand().Execute();
        }
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
