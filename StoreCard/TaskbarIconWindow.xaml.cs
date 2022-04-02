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
    /// Interaction logic for TaskbarIconWindow.xaml
    /// </summary>
    public partial class TaskbarIconWindow : Window
    {
        public TaskbarIconWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private static void TaskbarIcon_Click()
        {

        }
    }
}
