using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TJ.ViewModels;
using TJ.Models;
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
            Helpers.Payload payload = new Helpers.Payload();
            ViewModel = new MainPageViewModel();
            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            Object value = localSettings.Values["DefaultPage"]; // Получить настройки стартовой страницы и навигировать
            try
            {
                int n;
                int.TryParse(localSettings.Values["NumberOfOnetimeLoadedItems"].ToString(), out n);
            } catch (Exception)
            {
                localSettings.Values["NumberOfOnetimeLoadedItems"] = 30; // Установить количество записей по-умолчанию, если неопределено
            }

            if (value != null)
            {
                payload.parameter = value.ToString();
                GeneralFrame.Navigate(typeof(News), payload);
            }
            else
            {
                payload.parameter = "News";
                GeneralFrame.Navigate(typeof(News), payload);
            }

        }
        public MainPageViewModel ViewModel { get; set; }

        public void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            Helpers.Payload payload = new Helpers.Payload();

            string Page = rb.Name.ToString();
            switch (Page)
            {
                case "News":
                    payload.parameter = "News";
                    GeneralFrame.Navigate(typeof(News), payload);
                    DebugTextBlock.Text = "Новости";
                    break;
                case "Articles":
                    payload.parameter = "Articles";
                    GeneralFrame.Navigate(typeof(News), payload);
                    DebugTextBlock.Text = "Статьи";
                    break;
                case "Video":
                    payload.parameter = "Video";
                    GeneralFrame.Navigate(typeof(News), payload);
                    DebugTextBlock.Text = "Видео";
                    break;
                case "Offtop":
                    payload.parameter = "Offtopic";
                    GeneralFrame.Navigate(typeof(News), payload);
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
                case "LogIn":
                    GeneralFrame.Navigate(typeof(Authorization));
                    DebugTextBlock.Text = "Мне тоже очень жаль";
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Helpers.Payload payload = new Helpers.Payload();
            payload.parameter = "Search";
            payload.query = SearchBox.Text;
            GeneralFrame.Navigate(typeof(News), payload);
        }
    }
}
