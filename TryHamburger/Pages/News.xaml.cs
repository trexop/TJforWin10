using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TryHamburger.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TryHamburger.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class News : Page
    {
        public ObservableCollection<DataWrapper> TjNews { get; set; }
        public ObservableCollection<DataWrapper> TjNewsRecent { get; set; }
        public ObservableCollection<DataWrapper> TjNewsEditorial { get; set; }

        public News()
        {
            this.InitializeComponent();
            TjNews = new ObservableCollection<DataWrapper>();
            TjNewsRecent = new ObservableCollection<DataWrapper>();
            TjNewsEditorial = new ObservableCollection<DataWrapper>();
        }

        public async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;
            Loading.Visibility = Visibility.Visible;

            await Facade.PopulateLatestNewsAsync(TjNews, "mainpage", 0);
            await Facade.PopulateLatestNewsAsync(TjNewsRecent, "recent", 0);
            await Facade.PopulateLatestNewsAsync(TjNewsEditorial, "editorial", 0);

            Loading.IsActive = false;
            Loading.Visibility = Visibility.Collapsed;
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            var selectedItem = (DataWrapper)e.ClickedItem;
            var DebugValue = selectedItem.id;
            if (ArticleView.Visibility == Visibility.Collapsed)
            {
                Frame.Navigate(typeof(ArticleView), DebugValue);
            } else
            {
                ArticleView.Navigate(typeof(ArticleView), DebugValue);
            }
        }

        private async void OpenInBrowser_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = (e.OriginalSource as FrameworkElement).DataContext as DataWrapper;
            var link = new Uri(datacontext.url);
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        private void RelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }
    }
}
