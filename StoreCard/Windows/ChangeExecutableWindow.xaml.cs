using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for ChangeExecutableWindow.xaml
/// </summary>
public partial class ChangeExecutableWindow : INotifyPropertyChanged
{
    private readonly SavedFileSystemItem _item;

    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = "";

    public string ExecutableName
    {
        get => _executableName;
        set
        {
            _executableName = value;
            OnPropertyChanged(nameof(ExecutableName));
        }
    }

    public bool DoesExecutableExist
    {
        get => _doesExecutableExist;
        set
        {
            _doesExecutableExist = value;
            OnPropertyChanged(nameof(DoesExecutableExist));
        }
    }

    public ImageSource? ExecutableIcon
    {
        get => _executableIcon;
        set
        {
            _executableIcon = value;
            OnPropertyChanged(nameof(ExecutableIcon));
        }
    }

    public ChangeExecutableWindow(SavedFileSystemItem item)
    {
        _item = item;
        InitializeComponent();
        DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SaveDefaultButton_Click(object sender, RoutedEventArgs e)
    {
        SetExecPathAndSave(SavedFileSystemItem.DefaultExecutable);

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SaveExecutableButton_Click(object sender, RoutedEventArgs e)
    {
        SetExecPathAndSave(ExecutablePathBox.Text);

        Close();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        if (new BrowseExecutableCommand().Execute() is { } filePath)
        {
            ExecutablePathBox.Text = filePath;
        }
    }

    private void SaveSelectedAppAndClose()
    {
        if (AppSelector.SelectedApp == null)
        {
            Debug.WriteLine("Tried to save app, but no app was selected.");
            DialogResult = false;
        }
        else
        {
            SetExecPathAndSave(AppSelector.SelectedApp.ExecutablePath
                               ?? SavedFileSystemItem.DefaultExecutable);
        }

        Close();
    }

    private void SetExecPathAndSave(string path)
    {
        if (AppData.UpdateSavedItemById<SavedFileSystemItem>(_item.Id, i => i.SetExecutablePath(path)) != null)
        {
            DialogResult = true;
        }
    }

    private void ExecutablePathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = ExecutablePathBox.Text;
        DoesExecutableExist = File.Exists(text);
        if (!DoesExecutableExist) return;
        // Take file name without '.exe'
        ExecutableName = text.Split(@"\").Last();

        if (IconUtils.GetFileIconByPath(text) is { } icon)
        {
            ExecutableIcon = icon;
        }
    }

    private void AppSelector_SaveButtonClick(object sender, RoutedEventArgs e)
    {
        SaveSelectedAppAndClose();
    }

    private void AppSelector_CancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
