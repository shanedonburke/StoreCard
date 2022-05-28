using System;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for TutorialWindow.xaml
/// </summary>
public partial class TutorialWindow
{
    private readonly UserConfig _config;

    public TutorialWindow()
    {
        _config = AppData.ReadConfigFromFile();
        DataContext = this;
        InitializeComponent();
    }

    public string HotKeyText => HotKeyUtils.KeyStringFromConfig(_config);

    private void DontShowAgainButton_Click(object sender, RoutedEventArgs e)
    {
        _config.DisableTutorial();
        AppData.SaveConfigToFile(_config);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        new ShowSearchCommand().Execute();
    }
}
