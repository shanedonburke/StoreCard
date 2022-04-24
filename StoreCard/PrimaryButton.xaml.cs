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
    /// Interaction logic for PrimaryButton.xaml
    /// </summary>
    public partial class PrimaryButton
    {
        public new DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            nameof(IsEnabled),
            typeof(bool),
            typeof(PrimaryButton));

        public RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            nameof(Click),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(PrimaryButton));

        public new bool IsEnabled
        {
            get => (bool) GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        public PrimaryButton()
        {
            InitializeComponent();
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent)
                {RoutedEvent = ClickEvent});
        }
    }
}