﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using StoreCard.Annotations;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for AddLinkWindow.xaml
    /// </summary>
    public partial class AddLinkWindow : INotifyPropertyChanged
    {
        private string _url = "";

        public string Url {
            get => _url;
            set {
                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }

        public AddLinkWindow() {
            InitializeComponent();
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
    }
}
