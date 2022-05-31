#region

using System;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// Tutorial window that shows on startup until the user presses "Don't show again".
/// </summary>
public sealed partial class TutorialWindow
{
    private readonly UserConfig _config;

    public TutorialWindow()
    {
        _config = AppData.ReadConfigFromFile();
        DataContext = this;
        InitializeComponent();
    }

    /// <summary>
    /// Display string for hot key.
    /// </summary>
    public string HotKeyText => HotKeyUtils.KeyStringFromConfig(_config);

    private void DontShowAgainButton_Click(object sender, RoutedEventArgs e)
    {
        _config.DisableTutorial();
        AppData.SaveConfigToFile(_config);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void Window_Closed(object? sender, EventArgs e) => new ShowSearchCommand().Execute();
}
