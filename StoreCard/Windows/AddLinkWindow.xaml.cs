using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Windows
{
    /// <summary>
    /// Interaction logic for AddLinkWindow.xaml
    /// </summary>
    public partial class AddLinkWindow : INotifyPropertyChanged
    {
        private string _linkTitle = "";

        private ImageSource? _favicon;

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

        public bool ShouldEnableSaveButton => UrlBox.Text != string.Empty && LinkTitle != string.Empty;

        public AddLinkWindow() {
            InitializeComponent();
            DataContext = this;
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

        private void Window_Closed(object? sender, EventArgs e)
        {
            new ShowMainWindowCommand().Execute();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var base64Icon = Images.ImageToBase64((BitmapSource) LinkIcon);
            var savedItems = AppData.ReadItemsFromFile();
            savedItems.Add(new SavedLink(Guid.NewGuid().ToString(), LinkTitle, base64Icon, Links.NormalizeUrl(UrlBox.Text), Time.UnixTimeMillis));
            AppData.SaveItemsToFile(savedItems);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StoreCardTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ShouldEnableSaveButton));
            GetWebsiteDetails();
        }

        private void LinkTitleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LinkTitle = LinkTitleBox.Text;
        }
    }
}
