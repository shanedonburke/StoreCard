#region

using System;
using System.Windows;
using System.Windows.Input;
using StoreCard.Commands;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for AddLinkWindow.xaml
/// </summary>
public sealed partial class AddLinkWindow
{
    public AddLinkWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Close the window if Escape is pressed
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Window_Closed(object? sender, EventArgs e) => new ShowSearchCommand().Execute();

    private void LinkSelector_Finished(object sender, RoutedEventArgs e) => Close();
}
