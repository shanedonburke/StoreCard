﻿#region

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.UserControls;

/// <summary>
/// A control that lets the user enter the URL and title of a link.
/// It may be used to create a new link item or to edit an existing one.
/// A preview of the link is shown alongside Save/Cancel/Delete buttons.
/// </summary>
public partial class LinkSelector : INotifyPropertyChanged
{
    public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent(
        nameof(Finished),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(LinkSelector));

    private ImageSource? _favicon;

    private SavedLink? _link;

    private string _linkTitle = string.Empty;

    public LinkSelector()
    {
        DataContext = this;
        InitializeComponent();
    }

    /// <summary>
    /// The link being edited, if one has been provided.
    /// </summary>
    public SavedLink? Link
    {
        get => _link;
        set
        {
            _link = value;

            OnPropertyChanged(nameof(ShouldShowDeleteButton));

            if (value != null)
            {
                UrlBox.Text = value.Url;
                LinkTitleBox.Text = value.Name;
                OpenPrivateCheckBox.IsChecked = value.ShouldOpenPrivate;
            }
            else
            {
                UrlBox.Text = string.Empty;
                LinkTitleBox.Text = string.Empty;
                OpenPrivateCheckBox.IsChecked = false;
            }
        }
    }

    /// <summary>
    /// Only show Delete button if we are editing an existing link.
    /// </summary>
    public bool ShouldShowDeleteButton => Link != null;

    public string LinkTitle
    {
        get => _linkTitle;
        set
        {
            _linkTitle = value;
            LinkTitleBox.Text = _linkTitle;
            OnPropertyChanged(nameof(LinkTitle));
            OnPropertyChanged(nameof(ShouldEnableSaveButton));
        }
    }

    /// <summary>
    /// Favicon obtained using the URL.
    /// </summary>
    public ImageSource? Favicon
    {
        get => _favicon;
        set
        {
            _favicon = value;
            OnPropertyChanged(nameof(Favicon));
        }
    }

    public ImageSource DefaultIcon => Icons.LinkIcon;

    /// <summary>
    /// Enable the Save button if a URL and title have been set.
    /// </summary>
    public bool ShouldEnableSaveButton => UrlBox.Text != string.Empty && LinkTitle != string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Event triggered when the user clicks Save/Cancel/Delete.
    /// </summary>
    public event RoutedEventHandler Finished
    {
        add => AddHandler(FinishedEvent, value);
        remove => RemoveHandler(FinishedEvent, value);
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Gets the favicon and title of a website.
    /// </summary>
    private async void GetWebsiteDetails()
    {
        string title = await LinkUtils.GetPageTitle(UrlBox.Text);

        if (title != string.Empty)
        {
            LinkTitle = title;
        }

        Favicon = await LinkUtils.GetPageIcon(UrlBox.Text);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        string base64Icon = ImageUtils.ImageToBase64(Favicon as BitmapSource ?? (BitmapSource)DefaultIcon);

        // Instead of updating the item we're editing (if applicable), replace it entirely in the list
        var savedItems = AppData.ReadItemsFromFile().Where(i => i.Id != Link?.Id).ToList();

        bool shouldOpenPrivate = OpenPrivateCheckBox.IsChecked == true;

        // Create the new item using the details of the one being edited, if available
        savedItems.Add(new SavedLink(
            Link?.Id ?? Guid.NewGuid().ToString(),
            LinkTitle,
            base64Icon,
            LinkUtils.NormalizeUrl(UrlBox.Text),
            Link?.LastOpened ?? TimeUtils.UnixTimeMillis,
            shouldOpenPrivate));
        AppData.SaveItemsToFile(savedItems);
        Finish();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Finish();

    private void UrlBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ShouldEnableSaveButton));
        GetWebsiteDetails();
    }

    private void LinkTitleBox_TextChanged(object sender, TextChangedEventArgs e) => LinkTitle = LinkTitleBox.Text;

    private void Finish() => RaiseEvent(new RoutedEventArgs(FinishedEvent));

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (Link != null)
        {
            AppData.DeleteItemAndSave(Link);
        }
        else
        {
            Logger.Log("Tried to delete link, but no link was being edited.");
        }

        Finish();
    }
}
