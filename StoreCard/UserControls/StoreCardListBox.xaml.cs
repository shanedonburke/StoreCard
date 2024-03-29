﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Models;
using StoreCard.Models.Items;
using StoreCard.Utils;

#endregion

namespace StoreCard.UserControls;

public enum SecondaryTextVisibility
{
    WhenActive,
    Always
}

/// <summary>
/// A custom list box that supports item context menus and an action button
/// shown when an item is selected.
/// </summary>
public partial class StoreCardListBox
{
    public static readonly DependencyProperty ShowActionButtonProperty = DependencyProperty.Register(
        nameof(ShowActionButton),
        typeof(bool),
        typeof(StoreCardListBox),
        new FrameworkPropertyMetadata(false));

    public static readonly DependencyProperty ShowSecondaryTextProperty = DependencyProperty.Register(
        nameof(ShowSecondaryText),
        typeof(SecondaryTextVisibility),
        typeof(StoreCardListBox),
        new FrameworkPropertyMetadata(SecondaryTextVisibility.WhenActive));

    public static readonly DependencyProperty ActionButtonTextProperty = DependencyProperty.Register(
        nameof(ActionButtonText),
        typeof(string),
        typeof(StoreCardListBox),
        new FrameworkPropertyMetadata("Open"));

    public static readonly DependencyProperty ItemContextMenuProperty = DependencyProperty.Register(
        nameof(ItemContextMenu),
        typeof(ContextMenu),
        typeof(StoreCardListBox));

    public static readonly RoutedEvent ItemDoubleClickEvent = EventManager.RegisterRoutedEvent(
        nameof(ItemDoubleClick),
        RoutingStrategy.Bubble,
        typeof(MouseButtonEventHandler),
        typeof(StoreCardListBox));

    public static new readonly RoutedEvent PreviewKeyDownEvent = EventManager.RegisterRoutedEvent(
        nameof(PreviewKeyDown),
        RoutingStrategy.Bubble,
        typeof(KeyEventHandler),
        typeof(StoreCardListBox));

    public static new readonly RoutedEvent KeyUpEvent = EventManager.RegisterRoutedEvent(
        nameof(KeyUpEvent),
        RoutingStrategy.Bubble,
        typeof(KeyEventHandler),
        typeof(StoreCardListBox));

    public static readonly RoutedEvent ActionButtonClickEvent = EventManager.RegisterRoutedEvent(
        nameof(ActionButtonClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(StoreCardListBox));

    public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(SelectionChanged),
        RoutingStrategy.Bubble,
        typeof(SelectionChangedEventHandler),
        typeof(StoreCardListBox));

    private readonly UserConfig _userConfig;

    public StoreCardListBox()
    {
        _userConfig = AppData.ReadConfigFromFile();
        DataContext = this;
        InitializeComponent();
    }

    /// <summary>
    /// Items source for the list box.
    /// </summary>
    public IEnumerable<IListBoxItem> ItemsSource
    {
        get => CustomListBox.ItemsSource?.Cast<IListBoxItem>() ?? new List<IListBoxItem>();
        set
        {
            CustomListBox.ItemsSource = value;

            // Select first item automatically
            if (value.Any())
            {
                SelectedIndex = 0;
            }
        }
    }

    /// <summary>
    /// The context menu for items.
    /// </summary>
    public ContextMenu ItemContextMenu
    {
        get => (ContextMenu)GetValue(ItemContextMenuProperty);
        set => SetValue(ItemContextMenuProperty, value);
    }

    /// <summary>
    /// Whether the action button should be shown when an item is selected.
    /// </summary>
    public bool ShowActionButton
    {
        get => (bool)GetValue(ShowActionButtonProperty);
        set => SetValue(ShowActionButtonProperty, value);
    }


    /// <summary>
    /// When secondary text (e.g., the platform name for games) should be shown.
    /// </summary>
    public SecondaryTextVisibility ShowSecondaryText
    {
        get => (SecondaryTextVisibility)GetValue(ShowSecondaryTextProperty);
        set => SetValue(ShowSecondaryTextProperty, value);
    }

    /// <summary>
    /// Text to show for the action button.
    /// </summary>
    public string ActionButtonText
    {
        get => (string)GetValue(ActionButtonTextProperty);
        set => SetValue(ActionButtonTextProperty, value);
    }

    /// <summary>
    /// Index of the selected item.
    /// </summary>
    public int SelectedIndex
    {
        get => CustomListBox.SelectedIndex;
        set => CustomListBox.SelectedIndex = value;
    }

    public object SelectedItem
    {
        get => CustomListBox.SelectedItem;
        set => CustomListBox.SelectedItem = value;
    }

    /// <summary>
    /// Whether prefix icons should be shown (defined by user config).
    /// </summary>
    public bool ShouldShowPrefixIcons => _userConfig.ShouldShowPrefixIcons;

    public event MouseButtonEventHandler ItemDoubleClick
    {
        add => AddHandler(ItemDoubleClickEvent, value);
        remove => RemoveHandler(ItemDoubleClickEvent, value);
    }

    public new event KeyEventHandler PreviewKeyDown
    {
        add => AddHandler(PreviewKeyDownEvent, value);
        remove => RemoveHandler(PreviewKeyDownEvent, value);
    }

    public new event KeyEventHandler KeyUp
    {
        add => AddHandler(KeyUpEvent, value);
        remove => RemoveHandler(KeyUpEvent, value);
    }

    public event RoutedEventHandler ActionButtonClick
    {
        add => AddHandler(ActionButtonClickEvent, value);
        remove => RemoveHandler(ActionButtonClickEvent, value);
    }

    public event SelectionChangedEventHandler SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    /// <summary>
    /// Scroll to show the given item.
    /// </summary>
    /// <param name="item">Item to show</param>
    public void ScrollIntoView(object item) => CustomListBox.ScrollIntoView(item);

    private void Item_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Raise ItemDoubleClickEvent if this was a double click
        if (e.ClickCount == 2)
        {
            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
            {
                RoutedEvent = ItemDoubleClickEvent
            });
        }
    }

    private void CustomListBox_PreviewKeyDown(object sender, KeyEventArgs e) =>
        RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key)
        {
            RoutedEvent = PreviewKeyDownEvent
        });

    private void ActionButton_Click(object sender, RoutedEventArgs e) =>
        RaiseEvent(new RoutedEventArgs(ActionButtonClickEvent));

    private void CustomListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));

    private void CustomListBox_KeyUp(object sender, KeyEventArgs e) =>
        RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key)
        {
            RoutedEvent = KeyUpEvent
        });
}
