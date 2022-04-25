using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Annotations;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for AddLinkWindow.xaml
    /// </summary>
    public partial class AddLinkWindow : INotifyPropertyChanged
    {
        private string _url = "";

        private string _linkTitle = "";

        private ImageSource? _favicon;

        public string Url {
            get => _url;
            set {
                _url = value;
                OnPropertyChanged(nameof(Url));
                OnPropertyChanged(nameof(ShouldEnableSaveButton));
                GetWebsiteDetails();
            }
        }

        public string LinkTitle
        {
            get => _linkTitle;
            set
            {
                _linkTitle = value;
                OnPropertyChanged(nameof(LinkTitle));
                OnPropertyChanged(nameof(ShouldEnableSaveButton));
            }
        }

        public ImageSource LinkIcon => _favicon ?? Icons.LinkIcon;

        public bool ShouldEnableSaveButton => Url != string.Empty && LinkTitle != string.Empty;

        public AddLinkWindow() {
            InitializeComponent();
            DataContext = this;
        }

        private async void GetWebsiteDetails()
        {
            var title = await HttpUtils.GetWebsiteTitle(Url);
            if (title != string.Empty)
            {
                LinkTitle = title;
            }

            _favicon = await HttpUtils.GetWebsiteIcon(Url);
            OnPropertyChanged(nameof(LinkIcon));
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            new ShowMainWindowCommand().Execute(null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var base64Icon = ImageUtils.ImageToBase64((BitmapSource) LinkIcon);
            var savedItems = StorageUtils.ReadItemsFromFile();
            savedItems.Add(new SavedLink(Guid.NewGuid().ToString(), LinkTitle, base64Icon, HttpUtils.NormalizeUrl(Url)));
            StorageUtils.SaveItemsToFile(savedItems);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StoreCardTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Url = UrlBox.Text;
        }

        private void LinkTitleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LinkTitle = LinkTitleBox.Text;
        }
    }
}
