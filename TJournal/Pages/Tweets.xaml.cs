using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using TJ.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TJ.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TJournal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tweets : Page
    {
        public Tweets()
        {
            this.InitializeComponent();
            ViewModel = new TweetsViewModel();
            ViewModel.setPivotItems();
        }
        public TweetsViewModel ViewModel { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;
            Loading.Visibility = Visibility.Visible;
        }

        private async void replyTo_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = (e.OriginalSource as FrameworkElement).DataContext as TweetsApi;
            var link = new Uri("https://twitter.com/intent/tweet?in_reply_to=" + datacontext.id + "&related=tjournal");
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        private async void retweet_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = (e.OriginalSource as FrameworkElement).DataContext as TweetsApi;
            var link = new Uri("https://twitter.com/intent/retweet?tweet_id=" + datacontext.id + "&related=tjournal");
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        private async void favorite_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = (e.OriginalSource as FrameworkElement).DataContext as TweetsApi;
            var link = new Uri("https://twitter.com/intent/favorite?tweet_id=" + datacontext.id + "&related=tjournal");
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        private void RelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void previewList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
