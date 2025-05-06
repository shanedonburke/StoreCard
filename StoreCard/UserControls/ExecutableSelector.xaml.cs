#region

using System;
using System.ComponentModel;
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

#endregion

namespace StoreCard.UserControls;

/// <summary>
/// Control for specifying an executable path and name. A preview is shown as well
/// as Save/Edit/Delete buttons. This may be used to create a new executable or to
/// edit an existing one. To edit an existing executable, set <see cref="Executable"/>
/// through code.
/// </summary>
public partial class ExecutableSelector : INotifyPropertyChanged
{
    public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent(
        nameof(Finished),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ExecutableSelector));

    public bool IsEditingExisting { get; set; } = false;

    private SavedExecutable? _executable;

    private ImageSource? _executableIcon;

    private string _executableName = string.Empty;

    private bool _isExecutableValid;

    public ExecutableSelector()
    {
        DataContext = this;
        InitializeComponent();
    }

    /// <summary>
    /// The executable name entered by the user
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
    /// Whether the executable is valid (such that saving is allowed)
    /// </summary>
    public bool IsExecutableValid
    {
        get => _isExecutableValid;
        set
        {
            _isExecutableValid = value;
            OnPropertyChanged(nameof(IsExecutableValid));
        }
    }

    /// <summary>
    /// The executable's icon, set automatically based on the selected file.
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

    /// <summary>
    /// The Delete button is only shown when editing an existing executable.
    /// </summary>
    public bool ShouldShowDeleteButton => Executable != null;

    /// <summary>
    /// The executable being edited, if there is one.
    /// </summary>
    public SavedExecutable? Executable
    {
        get => _executable;
        set
        {
            _executable = value;
            OnPropertyChanged(nameof(ShouldShowDeleteButton));

            // Set text box values
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

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Event triggered when the user cancels/saves/deletes.
    /// </summary>
    public event RoutedEventHandler Finished
    {
        add => AddHandler(FinishedEvent, value);
        remove => RemoveHandler(FinishedEvent, value);
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = PathBox.Text;

        IsExecutableValid = File.Exists(text) && text.EndsWith(".exe");

        if (!IsExecutableValid)
        {
            return;
        }

        if (!IsEditingExisting)
        {
            // Take file name without '.exe'
            ExecutableName = text.Split(@"\").Last().Split(".")[0];
            NameBox.Text = ExecutableName;
        }

        if (IconUtils.GetFileIconByPath(text) is { } icon)
        {
            ExecutableIcon = icon;
        }
    }

    private void NameBox_TextChanged(object sender, TextChangedEventArgs e) => ExecutableName = NameBox.Text;

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        if (new BrowseExecutableCommand().Execute() is { } fileName)
        {
            PathBox.Text = fileName;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        string? base64Icon = (ExecutableIcon as BitmapSource)?.ToBase64();

        // Instead of updating the item we're editing (if applicable), replace it entirely in the list
        var savedItems = AppData.ReadItemsFromFile().Where(i => i.Id != Executable?.Id).ToList();

        // Use details of executable being edited if available
        savedItems.Add(new SavedExecutable(
            Executable?.Id ?? Guid.NewGuid().ToString(),
            ExecutableName,
            base64Icon,
            PathBox.Text,
            Executable?.LastOpened ?? TimeUtils.UnixTimeMillis));
        AppData.SaveItemsToFile(savedItems);
        Finish();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Finish();

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (Executable != null)
        {
            AppData.DeleteItemAndSave(Executable);
        }
        else
        {
            Logger.Log("Tried to delete executable, but no executable was being edited.");
        }

        Finish();
    }

    private void Finish() => RaiseEvent(new RoutedEventArgs(FinishedEvent));
}
