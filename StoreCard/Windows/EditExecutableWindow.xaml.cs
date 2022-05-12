using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using StoreCard.Annotations;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for EditExecutableWindow.xaml
/// </summary>
public partial class EditExecutableWindow
{
    private SavedExecutable _executable;

    public EditExecutableWindow(SavedExecutable executable)
    {
        _executable = executable;
        DataContext = this;
        InitializeComponent();
        ExecutableSelector.Executable = executable;
    }

    private void ExecutableSelector_Finished(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        new ShowMainWindowCommand().Execute();
    }
}