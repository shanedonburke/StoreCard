using System;
using System.IO;
using System.Linq;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata {
            DefaultValue = FindResource(typeof(Window))
        });
    }

    public void SetTheme(string theme)
    {
        if (SetThemeInternal(theme))
        {
            return;
        }

        if (!SetThemeInternal("Mint (Dark)"))
        {
            SetThemeInternal(ThemeFinder.FindThemes()[0]);
        }
    }

    private void App_Startup(object sender, StartupEventArgs e)
    {
        Processes.KillOtherStoreCardProcesses();

        new CreateTaskbarIconCommand().Execute();

        SetTheme(AppData.ReadConfigFromFile().Theme);

        if (!Environment.GetCommandLineArgs().Contains(CommandLineOptions.StartMinimized))
        {
            new ShowMainWindowCommand().Execute();
        }
    }

    private bool SetThemeInternal(string theme)
    {
        try
        {
            Resources.MergedDictionaries[0].Source =
                new Uri($"pack://application:,,,/ResourceDictionaries/Themes/{theme}.xaml");
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
