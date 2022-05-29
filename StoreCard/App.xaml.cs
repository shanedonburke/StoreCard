#region

using System;
using System.Linq;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly UserConfig _config = AppData.ReadConfigFromFile();

    public App() => FrameworkElement.StyleProperty.OverrideMetadata(
        typeof(Window),
        new FrameworkPropertyMetadata {DefaultValue = FindResource(typeof(Window))});

    private void App_Startup(object sender, StartupEventArgs e)
    {
        ProcessUtils.KillOtherStoreCardProcesses();
        new CreateTaskbarIconCommand().Execute();
        ThemeUtils.SetTheme(_config.Theme);

        if (!Environment.GetCommandLineArgs().Contains(CommandLineOptions.StartMinimized))
        {
            ShowStartupWindow();
        }
    }

    private void ShowStartupWindow()
    {
        if (_config.ShouldShowTutorial)
        {
            new ShowTutorialCommand().Execute();
        }
        else
        {
            new ShowSearchCommand().Execute();
        }
    }
}
