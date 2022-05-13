using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Utils;
using SystemIcons = StoreCard.Utils.SystemIcons;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for AddFileWindow.xaml
/// </summary>
public partial class AddFileWindow : INotifyPropertyChanged
{
    private bool _doesFileExist;

    private bool _doesFolderExist;

    private ImageSource? _fileIcon;

    private ImageSource? _folderIcon;

    public string FileName => FilePathBox.Text.Split(@"\").Last();

    public string FolderName => FolderPathBox.Text.Split(@"\").Last();

    public bool DoesFileExist
    {
        get => _doesFileExist;
        set
        {
            _doesFileExist = value;
            OnPropertyChanged(nameof(DoesFileExist));
        }
    }

    public bool DoesFolderExist
    {
        get => _doesFolderExist;
        set
        {
            _doesFolderExist = value;
            OnPropertyChanged(nameof(DoesFolderExist));
        }
    }

    public ImageSource? FileIcon
    {
        get => _fileIcon;
        set
        {
            _fileIcon = value;
            OnPropertyChanged(nameof(FileIcon));
        }
    }

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

    public AddFileWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "All Files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            Title = "Select File"
        };

        if (dialog.ShowDialog() == true) FilePathBox.Text = dialog.FileName;
    }

    private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog
        {
            RootFolder = Environment.SpecialFolder.UserProfile
        };
        if (dialog.ShowDialog() == true) FolderPathBox.Text = dialog.SelectedPath;
    }

    private void SaveFileButton_Click(object sender, RoutedEventArgs e)
    {
        var base64Icon = FileIcon != null ? Images.ImageToBase64((BitmapSource) FileIcon) : null;
        var savedItems = AppData.ReadItemsFromFile();
        savedItems.Add(new SavedFile(Guid.NewGuid().ToString(), FileName, base64Icon, FilePathBox.Text, SavedFileSystemItem.DEFAULT_EXECUTABLE));
        AppData.SaveItemsToFile(savedItems);
        Close();
    }

    private void SaveFolderButton_Click(object sender, RoutedEventArgs e)
    {
        var base64Icon = FolderIcon != null ? Images.ImageToBase64((BitmapSource) FolderIcon) : null;
        var savedItems = AppData.ReadItemsFromFile();
        savedItems.Add(new SavedFolder(Guid.NewGuid().ToString(), FolderName, base64Icon, FolderPathBox.Text, SavedFileSystemItem.DEFAULT_EXECUTABLE));
        AppData.SaveItemsToFile(savedItems);
        Close();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        new ShowMainWindowCommand().Execute();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void FilePathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = FilePathBox.Text;
        DoesFileExist = File.Exists(text);
        if (DoesFileExist) {
            FileIcon = SystemIcons.GetFileIconByPath(text);
        }
        OnPropertyChanged(nameof(FileName));
    }

    private void FolderPathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = FolderPathBox.Text;
        DoesFolderExist = Directory.Exists(text);
        if (DoesFolderExist) {
            FolderIcon = SystemIcons.GetFolderIconByPath(text);
        }
        OnPropertyChanged(nameof(FolderName));
    }
}