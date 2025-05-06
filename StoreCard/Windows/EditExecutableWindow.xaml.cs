#region

using System;
using System.Windows;
using System.Windows.Input;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// A window that allows the user to edit an executable-type app item.
/// </summary>
public sealed partial class EditExecutableWindow
{
    public EditExecutableWindow(SavedExecutable executable)
    {
        DataContext = this;
        InitializeComponent();
        // Must set executable to let the selector know we're editing an existing item
        ExecutableSelector.IsEditingExisting = true;
        ExecutableSelector.Executable = executable;
    }

    private void ExecutableSelector_Finished(object sender, RoutedEventArgs e) => Close();

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Close the window if Escape is pressed
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Window_Closed(object? sender, EventArgs e) => new ShowSearchCommand().Execute();
}
