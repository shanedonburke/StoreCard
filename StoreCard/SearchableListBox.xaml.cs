using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoreCard.Annotations;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for SearchableListBox.xaml
    /// </summary>
    public partial class SearchableListBox : INotifyPropertyChanged
    {
        public DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<IListBoxItem>),
            typeof(SearchableListBox));

        private IEnumerable<IListBoxItem> _filteredItems = new List<IListBoxItem>();

        public SearchableListBox() {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public IEnumerable<object> Items
        {
            get => (IEnumerable<object>) GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
