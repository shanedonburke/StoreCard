﻿using System;
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
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.UserControls;

/// <summary>
/// Interaction logic for ExecutableSelector.xaml
/// </summary>
public partial class ExecutableSelector : INotifyPropertyChanged
{
    public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent(
        nameof(Finished),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ExecutableSelector));

    private SavedExecutable? _executable;

    private bool _isExecutableValid;

    private ImageSource? _executableIcon;

    private string _executableName = "";

    public ExecutableSelector()
    {
        DataContext = this;
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event RoutedEventHandler Finished
    {
        add => AddHandler(FinishedEvent, value);
        remove => RemoveHandler(FinishedEvent, value);
    }

    public SavedExecutable? Executable
    {
        get => _executable;
        set
        {
            _executable = value;
            OnPropertyChanged(nameof(ShouldShowDeleteButton));
            if (value != null)
            {
                PathBox.Text = value.Path;
                NameBox.Text = value.Name;
            }
            else
            {
                PathBox.Text = string.Empty;
                NameBox.Text = string.Empty;
            }
        }
    }

    public string ExecutableName
    {
        get => _executableName;
        set
        {
            _executableName = value;
            OnPropertyChanged(nameof(ExecutableName));
        }
    }

    public bool IsExecutableValid
    {
        get => _isExecutableValid;
        set
        {
            _isExecutableValid = value;
            OnPropertyChanged(nameof(IsExecutableValid));
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

    public bool ShouldShowDeleteButton => Executable != null;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = PathBox.Text;
        
        IsExecutableValid = File.Exists(text) && text.EndsWith(".exe");
        if (!IsExecutableValid) return;
        
        // Take file name without '.exe'
        ExecutableName = text.Split(@"\").Last().Split(".")[0];
        NameBox.Text = ExecutableName;

        var icon = System.Drawing.Icon.ExtractAssociatedIcon(text);
        if (icon != null)
            ExecutableIcon = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
    }

    private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ExecutableName = NameBox.Text;
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Executables|*.exe|All Files (*.*)|*.*",
            InitialDirectory = Environment.ExpandEnvironmentVariables("%ProgramW6432%"),
            Title = "Select Executable"
        };

        if (openFileDialog.ShowDialog() == true) PathBox.Text = openFileDialog.FileName;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var base64Icon = ExecutableIcon != null ? Images.ImageToBase64((BitmapSource) ExecutableIcon) : null;
        // Don't include the executable we are editing, if there is one
        var savedItems = AppData.ReadItemsFromFile().Where(i => i.Id != Executable?.Id).ToList();
        savedItems.Add(new SavedExecutable(Guid.NewGuid().ToString(), ExecutableName, base64Icon, PathBox.Text));
        AppData.SaveItemsToFile(savedItems);
        Finish();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Finish();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (Executable != null)
        {
            AppData.DeleteItemAndSave(Executable);
        }
        else
        {
            Debug.WriteLine("Tried to delete executable, but no executable was being edited.");
        }

        Finish();
    }

    private void Finish()
    {
        RaiseEvent(new RoutedEventArgs(FinishedEvent));
    }
}