using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StoreCard.Models.Items.Installed;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.UserControls;

/// <summary>
/// Interaction logic for ApplicationSelector.xaml
/// </summary>
public partial class ApplicationSelector : INotifyPropertyChanged
{
    public static readonly RoutedEvent SaveButtonClickEvent = EventManager.RegisterRoutedEvent(
        nameof(SaveButtonClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ApplicationSelector));

    public static readonly RoutedEvent CancelButtonClickEvent = EventManager.RegisterRoutedEvent(
        nameof(CancelButtonClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ApplicationSelector));

    public ApplicationSelector()
    {
        DataContext = this;
        InitializeComponent();
    }

    public bool ShouldEnableSaveButton => AppListBox.SelectedItem != null;

    public InstalledApplication? SelectedApp => AppListBox.SelectedItem as InstalledApplication;

    public string? SelectedAppName => (AppListBox.SelectedItem as InstalledApplication)?.Name;

    public ImageSource? SelectedAppIcon => (AppListBox.SelectedItem as InstalledApplication)?.BitmapIcon;

    public event RoutedEventHandler SaveButtonClick
    {
        add => AddHandler(SaveButtonClickEvent, value);
        remove => RemoveHandler(SaveButtonClickEvent, value);
    }

    public event RoutedEventHandler CancelButtonClick
    {
        add => AddHandler(CancelButtonClickEvent, value);
        remove => RemoveHandler(CancelButtonClickEvent, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void LoadApps()
    {
        foreach (var app in InstalledAppUtils.GetInstalledApplications())
        {
            AppListBox.AddItem(app);
        }
        AppListBox.FinishAddingItems();
    }

    private void AppListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ShouldEnableSaveButton));
        OnPropertyChanged(nameof(SelectedAppName));
        OnPropertyChanged(nameof(SelectedAppIcon));
    }

    private void AppListBox_ItemActivated(object sender, ItemActivatedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(SaveButtonClickEvent));
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(SaveButtonClickEvent));
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(CancelButtonClickEvent));
    }

    private void ApplicationSelector_Loaded(object sender, RoutedEventArgs e)
    {
        Task.Run(LoadApps);
    }
}
