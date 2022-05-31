#region

using System.Windows;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// Window shown when closing a window that has unsaved changes, e.g.,
/// the "Edit file" window.
/// </summary>
public sealed partial class UnsavedChangesWindow
{
    /// <summary>
    /// Represents the button pressed by the user.
    /// </summary>
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
