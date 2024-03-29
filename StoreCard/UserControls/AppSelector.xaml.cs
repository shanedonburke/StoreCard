﻿#region

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StoreCard.Models.Items.Installed;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.UserControls;

/// <summary>
/// A control that includes a searchable list box with all installed apps,
/// a preview of the selected app, and Save/Cancel buttons.
/// </summary>
public partial class AppSelector : INotifyPropertyChanged
{
    public static readonly RoutedEvent SaveButtonClickEvent = EventManager.RegisterRoutedEvent(
        nameof(SaveButtonClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(AppSelector));

    public static readonly RoutedEvent CancelButtonClickEvent = EventManager.RegisterRoutedEvent(
        nameof(CancelButtonClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(AppSelector));

    public AppSelector()
    {
        DataContext = this;
        InitializeComponent();
    }

    // Enable the Save button when an item has been selected
    public bool ShouldEnableSaveButton => AppListBox.SelectedItem != null;

    public string? SelectedAppName => (AppListBox.SelectedItem as InstalledApp)?.Name;

    public ImageSource? SelectedAppIcon => (AppListBox.SelectedItem as InstalledApp)?.BitmapIcon;

    public InstalledApp? SelectedApp => AppListBox.SelectedItem as InstalledApp;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Event triggered when the Save button is clicked.
    /// </summary>
    public event RoutedEventHandler SaveButtonClick
    {
        add => AddHandler(SaveButtonClickEvent, value);
        remove => RemoveHandler(SaveButtonClickEvent, value);
    }

    /// <summary>
    /// Event triggered when the Cancel button is clicked.
    /// </summary>
    public event RoutedEventHandler CancelButtonClick
    {
        add => AddHandler(CancelButtonClickEvent, value);
        remove => RemoveHandler(CancelButtonClickEvent, value);
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Loads the installed apps into the list box.
    /// </summary>
    private void LoadApps()
    {
        foreach (InstalledApp app in InstalledAppUtils.GetInstalledApps())
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

    private void AppListBox_ItemActivated(object sender, ItemActivatedEventArgs e) =>
        RaiseEvent(new RoutedEventArgs(SaveButtonClickEvent));

    private void SaveButton_Click(object sender, RoutedEventArgs e) =>
        RaiseEvent(new RoutedEventArgs(SaveButtonClickEvent));

    private void CancelButton_Click(object sender, RoutedEventArgs e) =>
        RaiseEvent(new RoutedEventArgs(CancelButtonClickEvent));

    private void ApplicationSelector_Loaded(object sender, RoutedEventArgs e) => Task.Run(LoadApps);
}
