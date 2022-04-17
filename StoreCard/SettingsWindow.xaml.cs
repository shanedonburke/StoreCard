using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {

        private readonly UserConfig _config;

        public string HotKeyString => HotKeyUtils.KeyStringFromConfig(_config);

        public SettingsWindow() {
            InitializeComponent();
            _config = StorageUtils.ReadConfigFromFile();
            DataContext = this;
        }

        private void SettingsWindow_Closed(object? sender, EventArgs e)
        {
            StorageUtils.SaveConfigToFile(_config);
            new ShowMainWindowCommand().Execute(null);
            Close();
        }
    }
}
