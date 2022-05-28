using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
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

        if (IconUtils.GetFileIconByPath(text) is { } icon)
        {
            ExecutableIcon = icon;
        }
    }

    private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ExecutableName = NameBox.Text;
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        if (new BrowseExecutableCommand().Execute() is { } fileName)
        {
            PathBox.Text = fileName;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var base64Icon = ExecutableIcon != null ? ImageUtils.ImageToBase64((BitmapSource) ExecutableIcon) : null;

        // Instead of updating the item we're editing, create a new list with only the new item
        var savedItems = AppData.ReadItemsFromFile().Where(i => i.Id != Executable?.Id).ToList();

        savedItems.Add(new SavedExecutable(Guid.NewGuid().ToString(), ExecutableName, base64Icon, PathBox.Text,
            TimeUtils.UnixTimeMillis));
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
