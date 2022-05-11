using System;
using System.ComponentModel;
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
    public ExecutableSelector()
    {
        DataContext = this;
        InitializeComponent();
    }

    public ExecutableSelector(SavedExecutable executable) : this()
    {
        _executable = executable;
        PathBox.Text = executable.Path;
        NameBox.Text = executable.Name;
    }

    public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent(
        nameof(Finished),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ExecutableSelector));

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

    private readonly SavedExecutable? _executable;

    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public event RoutedEventHandler Finished
    {
        add => AddHandler(FinishedEvent, value);
        remove => RemoveHandler(FinishedEvent, value);
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = PathBox.Text;
        DoesExecutableExist = File.Exists(text);
        if (!DoesExecutableExist) return;
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
        var savedItems = AppData.ReadItemsFromFile().Where(i => i.Id != _executable?.Id).ToList();
        savedItems.Add(new SavedExecutable(Guid.NewGuid().ToString(), ExecutableName, base64Icon, PathBox.Text));
        AppData.SaveItemsToFile(savedItems);
        RaiseEvent(new RoutedEventArgs(FinishedEvent));
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(FinishedEvent));
    }
}