using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TJ.GetData;
using TJ.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace TJ.ViewModels
{
    public class PivotItems
    {
        public string Header { get; set; }
        public bool IsEnabled { get; set; }
        public ObservableCollection<NewsApi> Content { get; set; }
    }

    public class NewsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

        private int _Offset;
        public int Offset
        {
            get
            {
                return _Offset;
            }
            set
            {
                _Offset = value;
            }
        }

        private Helpers.Payload _Parameter;
        public Helpers.Payload Parameter
        {
            get
            {
                return _Parameter;
            }
            set
            {
                _Parameter = value;
            }
        }

        private string _rating_color;
        public string rating_color
        {
            get
            {
                return _rating_color;
            }
            set
            {
                _rating_color = value;
            }
        }

        public ObservableCollection<PivotItems> PivotItemsWrapper { get; set; } // Объявляется список вкладок

        public ObservableCollection<NewsApi> TjNews { get; set; } // Объявляются массивы новостей
        public ObservableCollection<NewsApi> TjNewsRecent { get; set; }
        public ObservableCollection<NewsApi> TjNewsEditorial { get; set; }
        public ObservableCollection<NewsApi> TjNewsWeek { get; set; }

        public ObservableCollection<NewsApi> TjArticles { get; set; } // Объявляются массивы статей
        public ObservableCollection<NewsApi> TjArticlesUnadmitted { get; set; }

        public ObservableCollection<NewsApi> TjVideo { get; set; } // Объявляется массивы видео
        public ObservableCollection<NewsApi> TjVideoUnadmitted { get; set; }

        public ObservableCollection<NewsApi> TjOfftop { get; set; }
        public ObservableCollection<NewsApi> TjOfftopUnadmitted { get; set; }

        public ObservableCollection<NewsApi> TjSearch { get; set; }

        public void CreateCollections()
        {
            TjNews = new ObservableCollection<NewsApi>(); // Создаётся массив на основе модели
            TjNewsRecent = new ObservableCollection<NewsApi>();
            TjNewsEditorial = new ObservableCollection<NewsApi>();
            TjNewsWeek = new ObservableCollection<NewsApi>();

            TjArticles = new ObservableCollection<NewsApi>();
            TjArticlesUnadmitted = new ObservableCollection<NewsApi>();

            TjVideo = new ObservableCollection<NewsApi>();
            TjVideoUnadmitted = new ObservableCollection<NewsApi>();

            TjOfftop = new ObservableCollection<NewsApi>();
            TjOfftopUnadmitted = new ObservableCollection<NewsApi>();

            TjSearch = new ObservableCollection<NewsApi>();    
        }

        public async void SetPivotItems(string page, int offset)
        {
            PivotItemsWrapper = new ObservableCollection<PivotItems>();
            int n = 30;
            int.TryParse(localSettings.Values["NumberOfOnetimeLoadedItems"].ToString(), out n); // количество загружаемых новостей
            var NavigatedPage = page;
            Facade.NewsBlackList = new ObservableCollection<GetData.BlackListedAccount>();
            Facade.PopulateBlackListedAccounts(Facade.NewsBlackList);

            try
            {
                switch (NavigatedPage) // Настраиваем вкладки для разных страниц
                {
                    case "News":
                        await Facade.PopulateLatestNewsAsync(TjNews, "mainpage", 0, n, offset, "news", ""); // Засовывает в массив кучку элементов, полученных после парсинга json
                        await Facade.PopulateLatestNewsAsync(TjNewsRecent, "recent", 0, n, offset, "news", "");
                        await Facade.PopulateLatestNewsAsync(TjNewsWeek, "week", 0, n, offset, "news", "");
                        await Facade.PopulateLatestNewsAsync(TjNewsEditorial, "editorial", 0, n, offset, "news", "");

                        PivotItemsWrapper.Add(new PivotItems { Header = "Главное", IsEnabled = true, Content = TjNews });
                        PivotItemsWrapper.Add(new PivotItems { Header = "Свежее", IsEnabled = false, Content = TjNewsRecent });
                        PivotItemsWrapper.Add(new PivotItems { Header = "За неделю", IsEnabled = false, Content = TjNewsWeek });
                        PivotItemsWrapper.Add(new PivotItems { Header = "Редакция", IsEnabled = false, Content = TjNewsEditorial }); // Добавляет всё в PivotItems
                        break;
                    case "Articles":
                        await Facade.PopulateLatestNewsAsync(TjArticles, "recent", 4, n, offset, "news", "");
                        await Facade.PopulateLatestNewsAsync(TjArticlesUnadmitted, "unadmitted", 4, n, offset, "news", "");

                        PivotItemsWrapper.Add(new PivotItems { Header = "Свежее", IsEnabled = true, Content = TjArticles });
                        PivotItemsWrapper.Add(new PivotItems { Header = "Непризнанное", IsEnabled = false, Content = TjArticlesUnadmitted });
                        break;
                    case "Video":
                        await Facade.PopulateLatestNewsAsync(TjVideo, "recent", 3, n, offset, "news", "");
                        await Facade.PopulateLatestNewsAsync(TjVideoUnadmitted, "unadmitted", 3, n, offset, "news", "");

                        PivotItemsWrapper.Add(new PivotItems { Header = "Свежее", IsEnabled = true, Content = TjVideo });
                        PivotItemsWrapper.Add(new PivotItems { Header = "Непризнанное", IsEnabled = false, Content = TjVideoUnadmitted });
                        break;
                    case "Offtopic":
                        await Facade.PopulateLatestNewsAsync(TjOfftop, "mainpage", 2, n, offset, "news", "");
                        await Facade.PopulateLatestNewsAsync(TjOfftopUnadmitted, "unadmitted", 2, n, offset, "news", "");

                        PivotItemsWrapper.Add(new PivotItems { Header = "Свежее", IsEnabled = true, Content = TjOfftop });
                        PivotItemsWrapper.Add(new PivotItems { Header = "Непризнанное", IsEnabled = false, Content = TjOfftopUnadmitted }); // При навигации сюда падает, я в курсе
                        break;
                    case "Search":
                        await Facade.PopulateLatestNewsAsync(TjSearch, "mainpage", 1, 30, 0, "search", Parameter.query);

                        PivotItemsWrapper.Add(new PivotItems { Header = "Результаты поиска", IsEnabled = true, Content = TjSearch });
                        break;
                }
            }
            catch (Exception)
            {
                ErrorWhileLoadingData();
            }
        }

        public async void ErrorWhileLoadingData ()
        {
            var dialog = new Windows.UI.Popups.MessageDialog("Что-то пошло не так. Во время загрузки данных произошла ошибка, сервер не ответил. Но вы держитесь там", ":'(");
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Всего доброго") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Хорошего настроения") { Id = 2 });
            await dialog.ShowAsync();
        }
    }
}
