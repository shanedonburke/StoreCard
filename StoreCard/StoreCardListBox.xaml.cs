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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for StoreCardListBox.xaml
    /// </summary>
    public partial class StoreCardListBox
    {
        public static readonly RoutedEvent ItemDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "ItemDoubleClick",
            RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler),
            typeof(StoreCardListBox));


        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<object>),
            typeof(StoreCardListBox));

        public IEnumerable<object> Items
        {
            get => (IEnumerable<object>) GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public event MouseButtonEventHandler ItemDoubleClick
        {
            add => AddHandler(ItemDoubleClickEvent, value);
            remove => RemoveHandler(ItemDoubleClickEvent, value);
        }

        public int SelectedIndex
        {
            get => ItemListBox.SelectedIndex;
            set => ItemListBox.SelectedIndex = value;
        }

        public object SelectedItem
        {
            get => ItemListBox.SelectedItem;
            set => ItemListBox.SelectedItem = value;
        }

        public StoreCardListBox() {
            InitializeComponent();
        }

        public event RoutedEventHandler PreviewKeyDown
        {
            add => ItemListBox.AddHandler(PreviewKeyDownEvent, value);
            remove => ItemListBox.AddHandler(PreviewKeyDownEvent, value);
        }

        public void ScrollIntoView(object item)
        {
            ItemListBox.ScrollIntoView(item);
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left) { RoutedEvent = ItemDoubleClickEvent });
            }
        }
    }
}
