#region

using System;
using System.Windows;
using System.Windows.Input;
using StoreCard.Commands;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// A window that allows the user to select a new link to save.
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

    // Close the window when the user presses Save/Cancel
    private void LinkSelector_Finished(object sender, RoutedEventArgs e) => Close();
}
