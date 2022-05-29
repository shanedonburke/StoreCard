#region

using System.Windows;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for UnsavedChangesWindow.xaml
/// </summary>
public sealed partial class UnsavedChangesWindow
{
    public enum Result
    {
        CloseWithoutSaving,
        Cancel,
        SaveAndClose
    }

    public new Result DialogResult = Result.Cancel;

    public UnsavedChangesWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    private void CloseWithoutSaving_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = Result.CloseWithoutSaving;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = Result.Cancel;
        Close();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = Result.SaveAndClose;
        Close();
    }
}
