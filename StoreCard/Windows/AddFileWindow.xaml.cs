#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// A window that allows the user to select a new file or folder to save.
/// </summary>
public sealed partial class AddFileWindow : INotifyPropertyChanged
{
    private bool _doesFileExist;

    private bool _doesFolderExist;

    private ImageSource? _fileIcon;

    private ImageSource? _folderIcon;

    public AddFileWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// For files, display name of the selected file.
    /// </summary>
    public string FileName => FilePathBox.Text.Split(@"\").Last();

    /// <summary>
    /// For folders, display name of the selected folder.
    /// </summary>
    public string FolderName => FolderPathBox.Text.Split(@"\").Last();

    /// <summary>
    /// For files, whether the selected file exists.
    /// </summary>
    public bool DoesFileExist
    {
        get => _doesFileExist;
        set
        {
            _doesFileExist = value;
            OnPropertyChanged(nameof(DoesFileExist));
        }
    }

    /// <summary>
    /// For folders, whether the selected folder exists.
    /// </summary>
    public bool DoesFolderExist
    {
        get => _doesFolderExist;
        set
        {
            _doesFolderExist = value;
            OnPropertyChanged(nameof(DoesFolderExist));
        }
    }

    /// <summary>
    /// For files, icon derived from the selected file.
    /// </summary>
    public ImageSource? FileIcon
    {
        get => _fileIcon;
        set
        {
            _fileIcon = value;
            OnPropertyChanged(nameof(FileIcon));
        }
    }

    /// <summary>
    /// For folders, icon derived from the selected folder.
    /// </summary>
    public ImageSource? FolderIcon
    {
        get => _folderIcon;
        set
        {
            _folderIcon = value;
            OnPropertyChanged(nameof(FolderIcon));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (new BrowseFileCommand().Execute() is { } filePath)
        {
            // Set the path to the selected file, triggering the TextChanged handler
            FilePathBox.Text = filePath;
        }
    }

    private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
    {
        if (new BrowseFolderCommand().Execute() is { } folderPath)
        {
            // Set the path to the selected folder, triggering the TextChanged handler
            FolderPathBox.Text = folderPath;
        }
    }

    private void SaveFileButton_Click(object sender, RoutedEventArgs e)
    {
        string? base64Icon = (FileIcon as BitmapSource)?.ToBase64();

        List<SavedItem> savedItems = AppData.ReadItemsFromFile();

        savedItems.Add(new SavedFile(
            Guid.NewGuid().ToString(),
            FileName,
            base64Icon,
            FilePathBox.Text,
            SavedFileSystemItem.DefaultExecutable,
            TimeUtils.UnixTimeMillis));

        AppData.SaveItemsToFile(savedItems);
        Close();
    }

    private void SaveFolderButton_Click(object sender, RoutedEventArgs e)
    {
        string? base64Icon = (FolderIcon as BitmapSource)?.ToBase64();

        List<SavedItem> savedItems = AppData.ReadItemsFromFile();

        savedItems.Add(new SavedFolder(
            Guid.NewGuid().ToString(),
            FolderName,
            base64Icon,
            FolderPathBox.Text,
            SavedFileSystemItem.DefaultExecutable,
            TimeUtils.UnixTimeMillis));

        AppData.SaveItemsToFile(savedItems);
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

    private void Window_Closed(object? sender, EventArgs e) => new ShowSearchCommand().Execute();

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private void FilePathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = FilePathBox.Text;
        DoesFileExist = File.Exists(text);

        if (DoesFileExist)
        {
            FileIcon = IconUtils.GetFileIconByPath(text);
        }

        OnPropertyChanged(nameof(FileName));
    }

    private void FolderPathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = FolderPathBox.Text;
        DoesFolderExist = Directory.Exists(text);

        if (DoesFolderExist)
        {
            FolderIcon = IconUtils.GetFolderIconByPath(text);
        }

        OnPropertyChanged(nameof(FolderName));
    }
}
