#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// A window that allows the user to edit a saved file/folder.
/// </summary>
public sealed partial class EditFileWindow : INotifyPropertyChanged
{
    private SavedFileSystemItem _item;

    public EditFileWindow(SavedFileSystemItem item)
    {
        _item = item;

        DataContext = this;
        InitializeComponent();

        PathBox.Text = item.ItemPath;
        NameBox.Text = item.Name;
    }

    /// <summary>
    /// Enable the Save button for the file path if the path has changed and refers to a real file.
    /// </summary>
    public bool ShouldEnableSavePathButton => PathBox.Text != _item.ItemPath && File.Exists(PathBox.Text);

    /// <summary>
    /// Enable the Save button for the name if a name has been entered that's different from the existing one.
    /// </summary>
    public bool ShouldEnableSaveNameButton => NameBox.Text.Trim() != string.Empty && NameBox.Text != _item.Name;

    public string ExecutableName => _item.ExecutableName;

    private bool HasUnsavedChanges => ShouldEnableSavePathButton || ShouldEnableSaveNameButton;

    /// <summary>
    /// Icon for the "Open with" executable.
    /// </summary>
    public ImageSource ExecutableIcon
    {
        get
        {
            string execPath = _item.ExecutablePath;

            if (!File.Exists(execPath))
            {
                execPath = SavedFileSystemItem.DefaultExecutable;
            }

            // Use the icon for the default executable if we can't get a specific one
            Icon? hIcon = System.Drawing.Icon.ExtractAssociatedIcon(execPath) ??
                          System.Drawing.Icon.ExtractAssociatedIcon(SavedFileSystemItem.DefaultExecutable);
            return IconUtils.CreateBitmapSourceFromHIcon(hIcon!);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// For files, browse for a new path.
    /// </summary>
    private void BrowseFile()
    {
        if (new BrowseFileCommand().Execute() is { } filePath)
        {
            PathBox.Text = filePath;
        }
    }

    /// <summary>
    /// For folders, browse for a new path.
    /// </summary>
    private void BrowseFolder()
    {
        if (new BrowseFolderCommand().Execute() is { } folderPath)
        {
            PathBox.Text = folderPath;
        }
    }

    private void SavePath()
    {
        if (!ShouldEnableSavePathButton)
        {
            return;
        }

        string path = PathBox.Text;

        SavedFileSystemItem? updatedItem =
            AppData.UpdateSavedItemById<SavedFileSystemItem>(_item.Id, i => { i.ItemPath = path; });

        if (updatedItem != null)
        {
            _item = updatedItem;
            OnPathChanged();
        }
        else
        {
            Logger.Log("Tried to change item path, but no matching stored item was found.");
        }
    }

    private void SaveName()
    {
        if (!ShouldEnableSaveNameButton)
        {
            return;
        }

        string name = NameBox.Text;

        SavedFileSystemItem? updatedItem =
            AppData.UpdateSavedItemById<SavedFileSystemItem>(_item.Id, i => i.Name = name);

        if (updatedItem != null)
        {
            _item = updatedItem;
            OnNameChanged();
        }
        else
        {
            Logger.Log("Tried to change item name, but no matching stored item was found.");
        }
    }

    /// <summary>
    /// Save all changes.
    /// </summary>
    private void SaveAll()
    {
        SavePath();
        SaveName();
    }

    /// <summary>
    /// Changes to the name may change the status of the Save button.
    /// </summary>
    private void OnNameChanged() => OnPropertyChanged(nameof(ShouldEnableSaveNameButton));

    /// <summary>
    /// Changes to the path may change the status of the Save button.
    /// </summary>
    private void OnPathChanged() => OnPropertyChanged(nameof(ShouldEnableSavePathButton));

    private void ChangeExecutableButton_Click(object sender, RoutedEventArgs e)
    {
        if (!new ChangeExecutableCommand(_item, true).Execute())
        {
            return;
        }

        // The item may have been updated, so get it from the file system
        SavedFileSystemItem? item =
            AppData.FindSavedItemById<SavedFileSystemItem>(AppData.ReadItemsFromFile(), _item.Id);

        if (item == null)
        {
            Logger.Log("Failed to find matching item for edit file window.");
            return;
        }

        _item = item;
        OnPropertyChanged(nameof(ExecutableName));
        OnPropertyChanged(nameof(ExecutableIcon));
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

    private void SavePathButton_Click(object sender, RoutedEventArgs e) => SavePath();

    private void SaveNameButton_Click(object sender, RoutedEventArgs e) => SaveName();

    private void PathBox_TextChanged(object sender, TextChangedEventArgs e) => OnPathChanged();

    private void NameBox_TextChanged(object sender, TextChangedEventArgs e) => OnNameChanged();

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        AppData.DeleteItemAndSave(_item);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    // Handle closing the window when there are unsaved changes.
    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (!HasUnsavedChanges)
        {
            return;
        }

        UnsavedChangesWindow.Result result = new ShowUnsavedChangesAlertCommand().Execute();

        switch (result)
        {
            case UnsavedChangesWindow.Result.SaveAndClose:
                SaveAll();
                break;
            case UnsavedChangesWindow.Result.Cancel:
                e.Cancel = true;
                break;
            case UnsavedChangesWindow.Result.CloseWithoutSaving:
                break;
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Close the window if Escape is pressed
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Window_Closed(object? sender, EventArgs e) => new ShowSearchCommand().Execute();

}
