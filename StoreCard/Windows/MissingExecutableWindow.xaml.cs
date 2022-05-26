// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
using StoreCard.Commands;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for MissingExecutableWindow.xaml
/// </summary>
public partial class MissingExecutableWindow
{
    private readonly SavedFileSystemItem _item;

    private bool _shouldShowSearchOnClose = true;

    public MissingExecutableWindow(SavedFileSystemItem item)
    {
        _item = item;
        DataContext = this;
        InitializeComponent();
    }

    public string ExecutableName => _item.ExecutableName;

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (_shouldShowSearchOnClose)
        {
            new ShowSearchCommand().Execute();
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        AppData.DeleteItemAndSave(_item);
        Close();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        new ChangeExecutableCommand(_item, false).Execute();
        Close();
    }
}
