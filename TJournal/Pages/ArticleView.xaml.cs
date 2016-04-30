using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TJ.GetData;
using TJ.Models;
using TJ.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class ArticleView : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ArticleView()
        {
            this.InitializeComponent();
        }
        public SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            byte R = Convert.ToByte(hexaColor.Substring(1, 2), 16);
            byte G = Convert.ToByte(hexaColor.Substring(3, 2), 16);
            byte B = Convert.ToByte(hexaColor.Substring(5, 2), 16);
            SolidColorBrush scb = new SolidColorBrush(Color.FromArgb(0xFF, R, G, B));
            return scb;
        }

        private ArticleWrapper _ArticleInfo { get; set; }
        public ArticleWrapper ArticleInfo
        {
            get
            {
                return _ArticleInfo;
            }
            set
            {
                _ArticleInfo = value;
                this.OnPropertyChanged("ArticleInfo");
            }
        }

        private Visibility _SponsoredBadgeVisibility { get; set; }
        public Visibility SponsoredBadgeVisibility
        {
            get
            {
                return _SponsoredBadgeVisibility;
            }
            set
            {
                _SponsoredBadgeVisibility = value;
                this.OnPropertyChanged("SponsoredBadgeVisibility");
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            int id = 1000;
            int.TryParse(e.Parameter.ToString(), out id);
            var entry = await Facade.GetArticleEntryJSONasync(id);

            ArticleInfo = entry;

            SponsoredBadgeVisibility = Visibility.Collapsed;
            if (entry.isAdvertising == true) 
            {
                SponsoredBadgeVisibility = Visibility.Visible; // Показывает плашку спонсорского материала, если материал спонсорский
            }

            var content = Facade.ParseEntryJSON(entry.entryJSON);

            foreach (var block in content.data)
            {
                var temp = block;
            }
        }
    }
}
