using System;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for EditLinkWindow.xaml
/// </summary>
public partial class EditLinkWindow
{
    public EditLinkWindow(SavedLink link)
    {
        DataContext = this;
        InitializeComponent();
        LinkSelector.Link = link;
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