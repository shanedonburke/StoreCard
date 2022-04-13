using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for FileOptionsWindow.xaml
    /// </summary>
    public partial class FileOptionsWindow
    {
        public readonly SavedFileSystemItem Item;

        public string ExecutableName => Item.ExecutableName;

        public ImageSource ExecutableIcon
        {
            get
            {
                var execPath = Item.ExecutablePath;
                if (!File.Exists(execPath))
                {
                    execPath = SavedFileSystemItem.DEFAULT_EXECUTABLE;
                }
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(execPath) ?? System.Drawing.Icon.ExtractAssociatedIcon(SavedFileSystemItem.DEFAULT_EXECUTABLE);
                Debug.Assert(icon != null, nameof(icon) + " != null");
                return Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public FileOptionsWindow(SavedFileSystemItem item) {
            Item = item;
            DataContext = this;
            InitializeComponent();
        }

        private void ChangeExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            new ChangeExecutableWindow().ShowDialog();
        }
    }
}
