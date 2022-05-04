using System;
using System.Linq;
using System.Windows;
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
        private void App_Startup(object sender, StartupEventArgs e)
        {
            Processes.KillOtherStoreCardProcesses();

            new TaskbarIconWindow().Show();

            if (!Environment.GetCommandLineArgs().Contains(CommandLineArgs.StartMinimized))
            {
                new MainWindow().Show();
            }
        }
    }
}
