﻿using System.Windows;
using System.Windows.Controls;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for SecondaryButton.xaml
    /// </summary>
    public partial class SecondaryButton
    {
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            nameof(IsEnabled),
            typeof(bool),
            typeof(SecondaryButton),
            new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(SecondaryButton));

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            nameof(Click),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SecondaryButton));

        public new bool IsEnabled {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public string Text {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public event RoutedEventHandler Click {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        public SecondaryButton() {
            InitializeComponent();
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e) {
            RaiseEvent(new RoutedEventArgs(ClickEvent) { RoutedEvent = ClickEvent });
        }
    }
}