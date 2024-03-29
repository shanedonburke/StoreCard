﻿#region

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// A window that allows the user to select a new executable with which a
/// file system item will be opened.
/// </summary>
public sealed partial class ChangeExecutableWindow : INotifyPropertyChanged
{
    private readonly SavedFileSystemItem _item;

    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = string.Empty;

    /// <summary>
    /// Whether <see cref="ShowDialog"/> was used to open the window.
    /// </summary>
    private bool _isDialog;

    public ChangeExecutableWindow(SavedFileSystemItem item)
    {
        _item = item;
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// For executables, name of the selected executable.
    /// </summary>
    public string ExecutableName
    {
        get => _executableName;
        set
        {
            _executableName = value;
            OnPropertyChanged(nameof(ExecutableName));
        }
    }

    /// <summary>
    /// For executables, whether the entered path referes to a real file.
    /// </summary>
    public bool DoesExecutableExist
    {
        get => _doesExecutableExist;
        set
        {
            _doesExecutableExist = value;
            OnPropertyChanged(nameof(DoesExecutableExist));
        }
    }

    /// <summary>
    /// For executables, the icon derived from the file.
    /// </summary>
    public ImageSource? ExecutableIcon
    {
        get => _executableIcon;
        set
        {
            _executableIcon = value;
            OnPropertyChanged(nameof(ExecutableIcon));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Show the window as a dialog, remembering that it is a dialog.
    /// </summary>
    /// <returns></returns>
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
            // DialogResult can only be set if this is a dialog
            DialogResult = false;
        }

        Close();
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Sets the executable path of the item to the selected app's executable path,
    /// then saves it.
    /// </summary>
    private void SaveSelectedAppAndClose()
    {
        if (AppSelector.SelectedApp == null)
        {
            Logger.Log("Tried to save app, but no app was selected.");
            DialogResult = false;
        }
        else
        {
            // If we don't know the path to the selected app, use the default
            SetExecPathAndSave(AppSelector.SelectedApp.ExecutablePath
                               ?? SavedFileSystemItem.DefaultExecutable);
        }

        Close();
    }

    /// <summary>
    /// Sets the executable path of the item and saves it.
    /// </summary>
    /// <param name="path">Path to the new executable</param>
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

    private void AppSelector_SaveButtonClick(object sender, RoutedEventArgs e) => SaveSelectedAppAndClose();

    private void AppSelector_CancelButtonClick(object sender, RoutedEventArgs e) => Close();

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        // Set the path box text if a file was selected, triggering the TextChanged handler
        if (new BrowseExecutableCommand().Execute() is { } filePath)
        {
            ExecutablePathBox.Text = filePath;
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

    private void SaveExecutableButton_Click(object sender, RoutedEventArgs e)
    {
        SetExecPathAndSave(ExecutablePathBox.Text);

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
        if (!_isDialog)
        {
            new ShowSearchCommand().Execute();
        }
    }
}
