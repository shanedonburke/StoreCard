using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        public IEnumerable<SavedItem> FilteredItems
        {
            get => _savedItems;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _searchText = "";

        private List<SavedItem> _savedItems = new List<SavedItem>();

        public MainWindow()
        {
            InitializeComponent();

            _savedItems = StorageUtils.ReadItemsFromFile();

            DataContext = this;
        }

        public void RefreshSavedItems()
        {
            _savedItems = StorageUtils.ReadItemsFromFile();
            OnPropertyChanged("FilteredItems");
        }

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void AddApplication_Click(object sender, RoutedEventArgs e)
        {
            new AddApplicationWindow().Show();
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddLink_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
