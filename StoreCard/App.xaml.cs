using System;
using System.Linq;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Static;
using StoreCard.Utils;
using StoreCard.Windows;

namespace StoreCard
{
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

        private void App_Startup(object sender, StartupEventArgs e)
        {
            Processes.KillOtherStoreCardProcesses();

            new CreateTaskbarIconCommand().Execute();

            if (!Environment.GetCommandLineArgs().Contains(CommandLineOptions.StartMinimized))
            {
                new ShowMainWindowCommand().Execute();
            }
        }
    }
}
