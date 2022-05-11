using System;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Utils;

namespace StoreCard.Windows
{
    /// <summary>
    /// Interaction logic for MissingItemWindow.xaml
    /// </summary>
    public partial class MissingItemWindow
    {
        public MissingItemWindow(SavedItem item, Action editAction) : this(item)
        {
            _editAction = editAction;
        }

        public MissingItemWindow(SavedItem item)
        {
            _item = item;
            DataContext = this;
            InitializeComponent();
        }

        public bool ShowEditButton => _editAction != null;

        private readonly SavedItem _item;

        private readonly Action? _editAction = null;

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AppData.DeleteItemAndSave(_item);
            new ShowMainWindowCommand().Execute(null);
            Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _editAction?.Invoke();
            Close();
        }
    }
}
