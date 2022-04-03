using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for AddApplicationWindow.xaml
    /// </summary>
    public partial class AddApplicationWindow : Window, INotifyPropertyChanged
    {
        private List<InstalledApplication> InstalledApps;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                OnPropertyChanged("FilteredApps");
            }
        }

        public string ExecutablePath
        {
            get => _executablePath;
            set
            {
                _executablePath = value;
                DoesExecutableExist = File.Exists(value);
                if (DoesExecutableExist)
                {
                    ExecutableName = value.Split(@"\").Last();
                }
                OnPropertyChanged("ExecutablePath");
            }
        }

        public string ExecutableName
        {
            get => _executableName;
            set
            {
                _executableName = value;
                OnPropertyChanged("ExecutableName");
            }
        }

        public bool DoesExecutableExist
        {
            get => _doesExecutableExist;
            set
            {
                _doesExecutableExist = value;
                OnPropertyChanged("DoesExecutableExist");
            }
        }

        public IEnumerable<InstalledApplication> FilteredApps
        {
            get => InstalledApps.Where(app => app.Name.ToUpper().StartsWith(_searchText.ToUpper()));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _searchText = "";

        private string _executablePath = "";

        private string _executableName = "";

        private bool _doesExecutableExist = false;

        public AddApplicationWindow()
        {
            InitializeComponent();

            InstalledApps = GetInstalledApplications();
            InstalledApps.Sort();
            ApplicationListBox.SelectedIndex = 0;


            MainWindow? mainWindow = Application.Current.Windows
                .Cast<Window>()
                .Where(w => w is MainWindow)
                .FirstOrDefault() as MainWindow;
            mainWindow?.Close();

            Activate();

            this.DataContext = this;
        }

        // From https://stackoverflow.com/a/57195200
        public static List<InstalledApplication> GetInstalledApplications()
        {
            var installedApps = new List<InstalledApplication>();
            var appsFolderId = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            ShellObject appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(appsFolderId);

            foreach (var app in (IKnownFolder)appsFolder)
            {
                // The friendly app name
                string name = app.Name;
                // The ParsingName property is the AppUserModelID
                string appUserModelId = app.ParsingName;
                BitmapSource icon = app.Thumbnail.ExtraLargeBitmapSource;

                installedApps.Add(new InstalledApplication(name, appUserModelId, icon));
            }
            return installedApps;
        }

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
            if (ApplicationListBox != null && ApplicationListBox.Items.Count > 0)
            {
                ApplicationListBox.SelectedIndex = 0;
            }
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (ApplicationListBox.Items.Count == 0) return;
                    switch (ApplicationListBox.SelectedIndex)
                    {
                        case 0:
                        case -1:
                            ApplicationListBox.SelectedIndex = ApplicationListBox.Items.Count - 1;
                            break;
                        default:
                            ApplicationListBox.SelectedIndex = (ApplicationListBox.SelectedIndex - 1) % ApplicationListBox.Items.Count;
                            break;
                    }
                    ApplicationListBox.ScrollIntoView(ApplicationListBox.SelectedItem);
                    break;
                case Key.Down:
                    if (ApplicationListBox.Items.Count == 0) return;
                    if (ApplicationListBox.SelectedIndex == -1)
                    {
                        ApplicationListBox.SelectedIndex = 0;
                    }
                    else
                    {
                        ApplicationListBox.SelectedIndex = (ApplicationListBox.SelectedIndex + 1) % ApplicationListBox.Items.Count;
                    }
                    ApplicationListBox.ScrollIntoView(ApplicationListBox.SelectedItem);
                    break;
                case Key.Enter:
                    AddSelectedApplication();
                    e.Handled = true;
                    break;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedApplication();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executables|*.exe|All Files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            openFileDialog.Title = "Select Executable";

            if (openFileDialog.ShowDialog() == true)
            {
                ExecutablePath = openFileDialog.FileName;
            }
        }

        private void ApplicationListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddSelectedApplication();
                e.Handled = true;
            }
        }

        private void AddSelectedApplication()
        {
            InstalledApplication? installedApplication = ApplicationListBox.SelectedItem as InstalledApplication;
            if (installedApplication != null)
            {
                List<SavedItem> savedItems = StorageUtils.ReadItemsFromFile();
                savedItems.Add(new SavedApplication(installedApplication));
                StorageUtils.SaveItemsToFile(savedItems);
                (Application.Current.MainWindow as MainWindow)?.RefreshSavedItems();
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            new ShowMainWindowCommand().Execute(null);
        }
    }
}
