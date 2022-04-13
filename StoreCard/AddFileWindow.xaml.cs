﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for AddFileWindow.xaml
    /// </summary>
    public partial class AddFileWindow : INotifyPropertyChanged
    {
        private string _filePath = "";

        private string _folderPath = "";

        private bool _doesFileExist;

        private bool _doesFolderExist;

        private ImageSource? _fileIcon;

        private ImageSource? _folderIcon;

        // To get folder icons; from https://stackoverflow.com/a/23666202
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        // Constant flags for SHGetFileInfo 
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // Large icon

        // Import SHGetFileInfo function
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi,
            uint cbSizeFileInfo, uint uFlags);

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;

                DoesFileExist = File.Exists(value);
                if (DoesFileExist)
                {
                    FileIcon = GetFileIconByPath(value);
                }

                OnPropertyChanged("FilePath");
                OnPropertyChanged("FileName");
            }
        }

        public string FolderPath
        {
            get => _folderPath;
            set
            {
                _folderPath = value;

                DoesFolderExist = Directory.Exists(value);
                if (DoesFolderExist)
                {
                    FolderIcon = GetFolderIconByPath(value);
                }

                OnPropertyChanged("FolderPath");
                OnPropertyChanged("FolderName");
            }
        }

        public string FileName => FilePath.Split(@"\").Last();

        public string FolderName => FolderPath.Split(@"\").Last();

        public bool DoesFileExist
        {
            get => _doesFileExist;
            set
            {
                _doesFileExist = value;
                OnPropertyChanged("DoesFileExist");
            }
        }

        public bool DoesFolderExist
        {
            get => _doesFolderExist;
            set
            {
                _doesFolderExist = value;
                OnPropertyChanged("DoesFolderExist");
            }
        }

        public ImageSource? FileIcon
        {
            get => _fileIcon;
            set
            {
                _fileIcon = value;
                OnPropertyChanged("FileIcon");
            }
        }

        public ImageSource? FolderIcon
        {
            get => _folderIcon;
            set
            {
                _folderIcon = value;
                OnPropertyChanged("FolderIcon");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AddFileWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Title = "Select File"
            };

            if (dialog.ShowDialog() == true) FilePath = dialog.FileName;
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.UserProfile
            };
            if (dialog.ShowDialog() == true) FolderPath = dialog.SelectedPath;
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            var base64Icon = FileIcon != null ? ImageUtils.ImageToBase64((BitmapSource) FileIcon) : null;
            var savedItems = StorageUtils.ReadItemsFromFile();
            savedItems.Add(new SavedFile(Guid.NewGuid().ToString(), FileName, base64Icon, FilePath, SavedFileSystemItem.DEFAULT_EXECUTABLE));
            StorageUtils.SaveItemsToFile(savedItems);
            Close();
        }

        private void SaveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var base64Icon = FolderIcon != null ? ImageUtils.ImageToBase64((BitmapSource) FolderIcon) : null;
            var savedItems = StorageUtils.ReadItemsFromFile();
            savedItems.Add(new SavedFolder(Guid.NewGuid().ToString(), FolderName, base64Icon, FolderPath, SavedFileSystemItem.DEFAULT_EXECUTABLE));
            StorageUtils.SaveItemsToFile(savedItems);
            Close();
        }

        private static ImageSource? GetFileIconByPath(string path)
        {
            var icon = System.Drawing.Icon.ExtractAssociatedIcon(path);
            if (icon == null) return null;
            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private static ImageSource GetFolderIconByPath(string path)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            // Call function with the path to the folder you want the icon for
            SHGetFileInfo(
                path,
                0,
                ref shinfo,
                (uint) Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_LARGEICON);

            using Icon i = System.Drawing.Icon.FromHandle(shinfo.hIcon);

            // Convert icon to a bitmap source
            return Imaging.CreateBitmapSourceFromHIcon(
                i.Handle,
                new Int32Rect(0, 0, i.Width, i.Height),
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            new ShowMainWindowCommand().Execute(null);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}