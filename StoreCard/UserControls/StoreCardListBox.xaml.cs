using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Models.Items;

namespace StoreCard.UserControls;

public enum SecondaryTextVisibility
{
    Never,
    WhenActive,
    Always
}

/// <summary>
/// Interaction logic for StoreCardListBox.xaml
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

    public new static readonly RoutedEvent PreviewKeyDownEvent = EventManager.RegisterRoutedEvent(
        nameof(PreviewKeyDown),
        RoutingStrategy.Bubble,
        typeof(KeyEventHandler),
        typeof(StoreCardListBox));

    public new static readonly RoutedEvent KeyUpEvent = EventManager.RegisterRoutedEvent(
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

    public IEnumerable<IListBoxItem> ItemsSource
    {
        get => CustomListBox.ItemsSource?.Cast<IListBoxItem>() ?? new List<IListBoxItem>();
        set
        {
            CustomListBox.ItemsSource = value;
            if (value.Any()) {
                SelectedIndex = 0;
            }
        }
    }

    public ContextMenu ItemContextMenu
    {
        get => (ContextMenu) GetValue(ItemContextMenuProperty);
        set => SetValue(ItemContextMenuProperty, value);
    }

    public bool ShowActionButton
    {
        get => (bool) GetValue(ShowActionButtonProperty);
        set => SetValue(ShowActionButtonProperty, value);
    }

    public SecondaryTextVisibility ShowSecondaryText
    {
        get => (SecondaryTextVisibility) GetValue(ShowSecondaryTextProperty);
        set => SetValue(ShowSecondaryTextProperty, value);
    }

    public string ActionButtonText
    {
        get => (string) GetValue(ActionButtonTextProperty);
        set => SetValue(ActionButtonTextProperty, value);
    }

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

    public new event KeyEventHandler KeyUp {
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

    public StoreCardListBox() {
        InitializeComponent();
    }

    public void ScrollIntoView(object item)
    {
        CustomListBox.ScrollIntoView(item);
    }

    private void Item_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left) { RoutedEvent = ItemDoubleClickEvent });
        }
    }

    private void CustomListBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key) { RoutedEvent = PreviewKeyDownEvent });
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ActionButtonClickEvent));
    }

    private void CustomListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
    }

    private void CustomListBox_KeyUp(object sender, KeyEventArgs e)
    {
        RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key) { RoutedEvent = KeyUpEvent });
    }
}
