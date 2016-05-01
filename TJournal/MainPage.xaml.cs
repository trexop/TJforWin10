using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TJ.ViewModels;
using TJournal.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TJournal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainPageViewModel();
            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            Object value = localSettings.Values["DefaultPage"]; // Получить настройки стартовой страницы и навигировать

            if (value != null)
            {
                if (value.ToString() == "Editorial")
                {
                    GeneralFrame.Navigate(typeof(News), "News");
                }
                else
                {
                    GeneralFrame.Navigate(typeof(News), value.ToString());
                }
            }
            else
            {
                GeneralFrame.Navigate(typeof(News), "News");
            }

        }
        public MainPageViewModel ViewModel { get; set; }

        public void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            string Page = rb.Name.ToString();
            switch (Page)
            {
                case "News":
                    GeneralFrame.Navigate(typeof(News), "News");
                    DebugTextBlock.Text = "Новости";
                    break;
                case "Articles":
                    GeneralFrame.Navigate(typeof(News), "Articles");
                    DebugTextBlock.Text = "Статьи";
                    break;
                case "Video":
                    GeneralFrame.Navigate(typeof(News), "Video");
                    DebugTextBlock.Text = "Видео";
                    break;
                case "Offtop":
                    GeneralFrame.Navigate(typeof(News), "Offtopic");
                    DebugTextBlock.Text = "Оффтоп";
                    break;
                case "Tweets":
                    GeneralFrame.Navigate(typeof(Tweets));
                    DebugTextBlock.Text = "Твиты";
                    break;
                case "Settings":
                    GeneralFrame.Navigate(typeof(Settings));
                    DebugTextBlock.Text = "Параметры";
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
