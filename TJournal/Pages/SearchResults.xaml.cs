using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    public sealed partial class SearchResults : Page
    {
        public SearchResults()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<NewsApi> SearchItems{ get; set; }
        private string _parameter { get; set; }
        public string parameter
        {
            get
            {
                return _parameter;
            }
            set
            {
                _parameter = value;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            parameter = e.Parameter.ToString();
            getData(parameter);
        }

        public async void getData(string p)
        {
            SearchItems = new ObservableCollection<NewsApi>();
            await Facade.PopulateLatestNewsAsync(SearchItems, "mainpage", 1, 30, 0, "search", p);
        }
    }
}
