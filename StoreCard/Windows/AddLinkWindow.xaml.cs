using System;
using System.Windows;
using StoreCard.Commands;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for AddLinkWindow.xaml
/// </summary>
public partial class AddLinkWindow
{
    public AddLinkWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        new ShowSearchCommand().Execute();
    }

    private void LinkSelector_Finished(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
