using System.Windows;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Utils;

namespace StoreCard.Windows
{
    /// <summary>
    /// Interaction logic for MissingFileWindow.xaml
    /// </summary>
    public partial class MissingFileWindow
    {
        public MissingFileWindow(SavedFileSystemItem item)
        {
            _item = item;
            DataContext = this;
            InitializeComponent();
        }

        private readonly SavedFileSystemItem _item;

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AppData.DeleteItemAndSave(_item);
            new ShowMainWindowCommand().Execute(null);
            Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            new EditFileWindow(_item).Show();
            Close();
        }
    }
}
