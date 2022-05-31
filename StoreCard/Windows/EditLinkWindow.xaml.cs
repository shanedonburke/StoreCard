using System;
using System.Windows;
using System.Windows.Input;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;

namespace StoreCard.Windows;

/// <summary>
/// A window that allows the user to edit a saved link.
/// </summary>
public sealed partial class EditLinkWindow
{
    public EditLinkWindow(SavedLink link)
    {
        DataContext = this;
        InitializeComponent();
        // Must set link so the selector knows we're editing an existing item
        LinkSelector.Link = link;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Close the window if Escape is pressed
        if (e.Key == Key.Escape)
        {
            Close();
        }
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
