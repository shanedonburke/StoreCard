using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for AddLinkWindow.xaml
/// </summary>
public partial class AddLinkWindow
{
    public AddLinkWindow() {
        InitializeComponent();
        DataContext = this;
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        new ShowMainWindowCommand().Execute();
    }

    private void LinkSelector_Finished(object sender, RoutedEventArgs e)
    {
        Close();
    }
}