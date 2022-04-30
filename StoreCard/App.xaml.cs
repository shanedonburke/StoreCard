using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            new TaskbarIconWindow().Show();

            if (!Environment.GetCommandLineArgs().Contains(CommandLineArgs.StartMinimized))
            {
                new MainWindow().Show();
            }
        }
    }
}
