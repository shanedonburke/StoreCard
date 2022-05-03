using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            SystemUtils.KillOtherStoreCardProcesses();

            new TaskbarIconWindow().Show();

            if (!Environment.GetCommandLineArgs().Contains(CommandLineArgs.StartMinimized))
            {
                new MainWindow().Show();
            }
        }
    }
}
