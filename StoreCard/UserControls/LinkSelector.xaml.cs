using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Annotations;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.UserControls;

/// <summary>
/// Interaction logic for LinkSelector.xaml
/// </summary>
public partial class LinkSelector : INotifyPropertyChanged
{
    public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent(
        nameof(Finished),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(LinkSelector));

    private SavedLink? _link;

    private string _linkTitle = "";

    private ImageSource? _favicon;

    public LinkSelector()
    {
        DataContext = this;
        InitializeComponent();
    }

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

    public ImageSource LinkIcon => _favicon ?? Icons.LinkIcon;

    public bool ShouldEnableSaveButton => UrlBox.Text != string.Empty && LinkTitle != string.Empty;

    public event RoutedEventHandler Finished
    {
        add => AddHandler(FinishedEvent, value);
        remove => RemoveHandler(FinishedEvent, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void GetWebsiteDetails()
    {
        var title = await Links.GetPageTitle(UrlBox.Text);
        if (title != string.Empty)
        {
            LinkTitle = title;
        }

        _favicon = await Links.GetPageIcon(UrlBox.Text);
        OnPropertyChanged(nameof(LinkIcon));
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var base64Icon = Images.ImageToBase64((BitmapSource) LinkIcon);

        // Instead of updating the link we're editing, create a new list with only the new link
        var savedItems = AppData.ReadItemsFromFile().Where(i => i.Id != Link?.Id).ToList();

        var shouldOpenPrivate = OpenPrivateCheckBox.IsChecked == true;
        savedItems.Add(new SavedLink(Guid.NewGuid().ToString(), LinkTitle, base64Icon, Links.NormalizeUrl(UrlBox.Text),
            Time.UnixTimeMillis, shouldOpenPrivate));
        AppData.SaveItemsToFile(savedItems);
        Finish();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Finish();
    }

    private void UrlBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ShouldEnableSaveButton));
        GetWebsiteDetails();
    }

    private void LinkTitleBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        LinkTitle = LinkTitleBox.Text;
    }

    private void Finish()
    {
        RaiseEvent(new RoutedEventArgs(FinishedEvent));
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (Link != null)
        {
            AppData.DeleteItemAndSave(Link);
        }
        else
        {
            Debug.WriteLine("Tried to delete link, but no link was being edited.");
        }

        Finish();
    }
}