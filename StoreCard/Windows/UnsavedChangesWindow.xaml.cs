using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for UnsavedChangesWindow.xaml
/// </summary>
public partial class UnsavedChangesWindow
{
    public new Result DialogResult = Result.Cancel;

    public UnsavedChangesWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    public enum Result
    {
        CloseWithoutSaving,
        Cancel,
        SaveAndClose
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