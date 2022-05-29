#region

using System;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for EditExecutableWindow.xaml
/// </summary>
public sealed partial class EditExecutableWindow
{
    public EditExecutableWindow(SavedExecutable executable)
    {
        DataContext = this;
        InitializeComponent();
        ExecutableSelector.Executable = executable;
    }

    private void ExecutableSelector_Finished(object sender, RoutedEventArgs e) => Close();

    private void Window_Closed(object? sender, EventArgs e) => new ShowSearchCommand().Execute();
}
