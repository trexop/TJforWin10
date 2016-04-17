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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TryHamburger.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tweets : Page
    {
        public ObservableCollection<TweetsApi> TweetsWrapper { get; set; }

        public Tweets()
        {
            this.InitializeComponent();
            TweetsWrapper = new ObservableCollection<TweetsApi>();
        }
        public async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;
            Loading.Visibility = Visibility.Visible;

            await Facade.PopulateTweetsAsync(TweetsWrapper);

            Loading.IsActive = false;
            Loading.Visibility = Visibility.Collapsed;
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

        private void TweetRelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }
    }
}
