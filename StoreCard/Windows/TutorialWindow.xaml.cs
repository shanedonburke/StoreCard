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
using StoreCard.Models;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for TutorialWindow.xaml
/// </summary>
public partial class TutorialWindow
{
    private readonly UserConfig _config;

    public TutorialWindow()
    {
        _config = AppData.ReadConfigFromFile();
        DataContext = this;
        InitializeComponent();
    }

    public string HotKeyText => HotKeys.KeyStringFromConfig(_config);

    private void DontShowAgainButton_Click(object sender, RoutedEventArgs e)
    {
        _config.DisableTutorial();
        AppData.SaveConfigToFile(_config);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        new ShowSearchCommand().Execute();
    }
}
