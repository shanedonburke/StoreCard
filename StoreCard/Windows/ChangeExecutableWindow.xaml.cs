using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StoreCard.Commands;
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

    private string _executableName = string.Empty;

    private bool _isDialog;

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

    public new bool? ShowDialog()
    {
        _isDialog = true;
        return base.ShowDialog();
    }

    private void SaveDefaultButton_Click(object sender, RoutedEventArgs e)
    {
        SetExecPathAndSave(SavedFileSystemItem.DefaultExecutable);
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isDialog)
        {
            DialogResult = false;
        }

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
        SavedFileSystemItem? updatedItem = AppData.UpdateSavedItemById<SavedFileSystemItem>(
            _item.Id,
            i => i.SetExecutablePath(path));

        if (_isDialog)
        {
            DialogResult = updatedItem != null;
        }
    }

    private void ExecutablePathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = ExecutablePathBox.Text;
        DoesExecutableExist = File.Exists(text);

        if (!DoesExecutableExist)
        {
            return;
        }

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

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (!_isDialog)
        {
            new ShowSearchCommand().Execute();
        }
    }
}
