using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TJ.ViewModels;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using TJ.Models;
using Windows.UI;
using static TJournal.MainPage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TJournal.Pages
{

    public sealed partial class News : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

        public News()
        {
            this.InitializeComponent();
            Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] = new SolidColorBrush(Colors.Transparent); // Убираем ебучую тематическую подсветку с нажатой кнопки
            Application.Current.Resources["SystemControlHighlightListAccentMediumBrush"] = new SolidColorBrush(Colors.Transparent);
            ViewModel = new NewsPageViewModel();
        }
        public NewsPageViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Offset = 0;
            ViewModel.Parameter = e.Parameter as Helpers.Payload;
            ViewModel.CreateCollections();
            ViewModel.SetPivotItems(ViewModel.Parameter.parameter, ViewModel.Offset);
        }

        private void RelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private async void OpenInBrowser_Click(object sender, RoutedEventArgs e) // Открыть новость в браузере
        {
            var datacontext = (e.OriginalSource as FrameworkElement).DataContext as NewsApi;
            var link = new Uri(datacontext.url);
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;
            Loading.Visibility = Visibility.Visible;
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedItem = (NewsApi)e.ClickedItem;
            var ArticleId = selectedItem.id;

            if (selectedItem.IsThisANextButton == Visibility.Collapsed)
            {
                Frame.Navigate(typeof(ArticleView), ArticleId);
            } else
            {
                Debug.WriteLine("Хуй");
                var n = 30; // количество загружаемых новостей
                int.TryParse(localSettings.Values["NumberOfOnetimeLoadedItems"].ToString(), out n);
                ViewModel.Offset += n;
                ViewModel.SetPivotItems(ViewModel.Parameter.parameter, ViewModel.Offset);
            }
        }
    }
}
