using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TryHamburger.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Foundation.Metadata;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TryHamburger
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //PC customization



            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    // Color Customization for later
                    titleBar.ButtonBackgroundColor = Colors.White;
                    //titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.BackgroundColor = Colors.White;
                    //titleBar.ForegroundColor = Windows.UI.Colors.White;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SideBar.IsPaneOpen = !SideBar.IsPaneOpen;
        }

        public void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            string Page = rb.Name.ToString();
            switch (Page)
                {
                    case "News":
                        GeneralFrame.Navigate(typeof(News));
                        DebugTextBlock.Text = "Новости";
                        break;
                    case "Articles":
                        GeneralFrame.Navigate(typeof(Articles));
                        DebugTextBlock.Text = "Статьи";
                        break;
                    case "Video":
                        GeneralFrame.Navigate(typeof(Video));
                        DebugTextBlock.Text = "Видео";
                        break;
                    case "Offtop":
                        GeneralFrame.Navigate(typeof(offtop));
                        DebugTextBlock.Text = "Оффтоп";
                        break;
                    case "Tweets":
                        GeneralFrame.Navigate(typeof(Tweets));
                        DebugTextBlock.Text = "Твиты";
                        break;
                }
            SideBar.IsPaneOpen = false;
            }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (GeneralFrame.CanGoBack)
            {
                GeneralFrame.GoBack();
            }
        }
    }
}
