using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for EditFileWindow.xaml
/// </summary>
public sealed partial class EditFileWindow : INotifyPropertyChanged
{
    public EditFileWindow(SavedFileSystemItem item)
    {
        _item = item;

        DataContext = this;
        InitializeComponent();

        PathBox.Text = item.ItemPath;
        NameBox.Text = item.Name;
        OnPropertyChanged(nameof(IsPathValid));
        OnPropertyChanged(nameof(ShouldEnableSavePathButton));
    }

    private SavedFileSystemItem _item;

    public bool IsPathValid => _item.Exists();

    public bool ShouldEnableSavePathButton => IsPathValid && PathBox.Text != _item.ItemPath;

    public bool ShouldEnableSaveNameButton => NameBox.Text.Trim() != "" && NameBox.Text != _item.Name;

    public string ExecutableName => _item.ExecutableName;

    private bool HasUnsavedChanges => ShouldEnableSavePathButton || ShouldEnableSaveNameButton;

    public ImageSource ExecutableIcon
    {
        get
        {
            string execPath = _item.ExecutablePath;
            if (!File.Exists(execPath))
            {
                execPath = SavedFileSystemItem.DefaultExecutable;
            }

            Icon? hIcon = System.Drawing.Icon.ExtractAssociatedIcon(execPath) ??
                          System.Drawing.Icon.ExtractAssociatedIcon(SavedFileSystemItem.DefaultExecutable);
            return IconUtils.CreateBitmapSourceFromHIcon(hIcon!);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ChangeExecutableButton_Click(object sender, RoutedEventArgs e)
    {
        if (new ChangeExecutableWindow(_item).ShowDialog() != true) return;

        var item = AppData.FindSavedItemById<SavedFileSystemItem>(AppData.ReadItemsFromFile(), _item.Id);
        if (item == null)
        {
            Debug.WriteLine("Failed to find matching item for edit file window.");
            return;
        }

        _item = item;
        OnPropertyChanged(nameof(ExecutableName));
        OnPropertyChanged(nameof(ExecutableIcon));
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void BrowseFile()
    {
        if (new BrowseFileCommand().Execute() is { } filePath)
        {
            PathBox.Text = filePath;
        }
    }

    private void BrowseFolder()
    {
        if (new BrowseFolderCommand().Execute() is { } folderPath)
        {
            PathBox.Text = folderPath;
        }
    }

    private void OnNameChanged()
    {
        OnPropertyChanged(nameof(ShouldEnableSaveNameButton));
    }

    private void OnPathChanged()
    {
        OnPropertyChanged(nameof(ShouldEnableSavePathButton));
    }

    private void SavePath()
    {
        var path = PathBox.Text;

        var updatedItem = AppData.UpdateSavedItemById<SavedFileSystemItem>(_item.Id, i =>
        {
            i.ItemPath = path;
        });

        if (updatedItem != null)
        {
            _item = updatedItem;
            OnPathChanged();
        }
        else
        {
            Debug.WriteLine("Tried to change item path, but no matching stored item was found.");
        }
    }

    private void SaveName()
    {
        var name = NameBox.Text;

        var updatedItem = AppData.UpdateSavedItemById<SavedFileSystemItem>(_item.Id, i => i.Name = name);

        if (updatedItem != null)
        {
            _item = updatedItem;
            OnNameChanged();
        }
        else
        {
            Debug.WriteLine("Tried to change item name, but no matching stored item was found.");
        }
    }

    private void SaveAll()
    {
        SavePath();
        SaveName();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        new ShowSearchCommand().Execute();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        switch (_item.SpecificCategory)
        {
            case SpecificItemCategory.File:
                BrowseFile();
                break;
            case SpecificItemCategory.Folder:
                BrowseFolder();
                break;
        }
    }

    private void SavePathButton_Click(object sender, RoutedEventArgs e)
    {
        SavePath();
    }

    private void SaveNameButton_Click(object sender, RoutedEventArgs e)
    {
        SaveName();
    }

    private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        OnPathChanged();
    }

    private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        OnNameChanged();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        AppData.DeleteItemAndSave(_item);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (!HasUnsavedChanges)
        {
            return;
        }

        var unsavedChangesWindow = new UnsavedChangesWindow();
        unsavedChangesWindow.ShowDialog();

        switch (unsavedChangesWindow.DialogResult)
        {
            case UnsavedChangesWindow.Result.SaveAndClose:
                SaveAll();
                break;
            case UnsavedChangesWindow.Result.CloseWithoutSaving: 
                break;
            case UnsavedChangesWindow.Result.Cancel:
                e.Cancel = true;
                break;
        }
    }
}
