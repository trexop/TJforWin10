using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TJ.GetData;
using TJ.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TJournal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Profile : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Profile()
        {
            this.InitializeComponent();
        }

        private ProfileApi _ProfileData { get; set; }
        public ProfileApi ProfileData
        {
            get
            {
                return _ProfileData;
            }
            set
            {
                _ProfileData = value;
                this.OnPropertyChanged("ProfileData");
            }
        }

        private Visibility _IsOnline { get; set; }
        public Visibility IsOnline
        {
            get
            {
                return _IsOnline;
            }
            set
            {
                _IsOnline = value;
                this.OnPropertyChanged("IsOnline");
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ProfileData = await Facade.GetProfileJSON(int.Parse(e.Parameter.ToString()));

            IsOnline = Visibility.Collapsed;
            if (ProfileData.is_online == true)
            {
                IsOnline = Visibility.Visible;
            }
        }
    }
}
