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

        private string _parameter;
        public string _Parameter
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

        public ObservableCollection<NewsApi> TjArticles { get; set; } // Объявляются массивы статей
        public ObservableCollection<NewsApi> TjArticlesUnadmitted { get; set; }

        public ObservableCollection<NewsApi> TjVideo { get; set; } // Объявляется массивы видео
        public ObservableCollection<NewsApi> TjVideoUnadmitted { get; set; }

        public ObservableCollection<NewsApi> TjOfftop { get; set; }
        public ObservableCollection<NewsApi> TjOfftopUnadmitted { get; set; }

        public void CreateCollections()
        {
            TjNews = new ObservableCollection<NewsApi>(); // Создаётся массив на основе модели
            TjNewsRecent = new ObservableCollection<NewsApi>();
            TjNewsEditorial = new ObservableCollection<NewsApi>();

            TjArticles = new ObservableCollection<NewsApi>();
            TjArticlesUnadmitted = new ObservableCollection<NewsApi>();

            TjVideo = new ObservableCollection<NewsApi>();
            TjVideoUnadmitted = new ObservableCollection<NewsApi>();

            TjOfftop = new ObservableCollection<NewsApi>();
            TjOfftopUnadmitted = new ObservableCollection<NewsApi>();
        }

        public async void SetPivotItems()
        {
            PivotItemsWrapper = new ObservableCollection<PivotItems>();
            var NavigatedPage = _Parameter;
            CreateCollections();
            switch (NavigatedPage) // Настраиваем вкладки для разных страниц
            {
                case "News":
                    await Facade.PopulateLatestNewsAsync(TjNews, "mainpage", 0); // Засовывает в массив кучку элементов, полученных после парсинга json
                    await Facade.PopulateLatestNewsAsync(TjNewsRecent, "recent", 0);
                    await Facade.PopulateLatestNewsAsync(TjNewsEditorial, "editorial", 0);

                    PivotItemsWrapper.Add(new PivotItems { Header = "Главное", IsEnabled = true, Content =  TjNews});
                    PivotItemsWrapper.Add(new PivotItems { Header = "Свежее", IsEnabled = false, Content = TjNewsRecent });
                    PivotItemsWrapper.Add(new PivotItems { Header = "Редакция", IsEnabled = false, Content = TjNewsEditorial }); // Добавляет всё в PivotItems
                    break;
                case "Articles":
                    await Facade.PopulateLatestNewsAsync(TjArticles, "recent", 4);
                    await Facade.PopulateLatestNewsAsync(TjArticlesUnadmitted, "unadmitted", 4);

                    PivotItemsWrapper.Add(new PivotItems { Header = "Свежее", IsEnabled = true, Content = TjArticles });
                    PivotItemsWrapper.Add(new PivotItems { Header = "Непризнанное", IsEnabled = false, Content = TjArticlesUnadmitted });
                    break;
                case "Video":
                    await Facade.PopulateLatestNewsAsync(TjVideo, "recent", 3);
                    await Facade.PopulateLatestNewsAsync(TjVideoUnadmitted, "unadmitted", 3);

                    PivotItemsWrapper.Add(new PivotItems { Header = "Свежее", IsEnabled = true, Content = TjVideo });
                    PivotItemsWrapper.Add(new PivotItems { Header = "Непризнанное", IsEnabled = false, Content = TjVideoUnadmitted });
                    break;
                case "Offtopic":
                    await Facade.PopulateLatestNewsAsync(TjOfftop, "recent", 2);
                    await Facade.PopulateLatestNewsAsync(TjOfftopUnadmitted, "unadmitted", 2);

                    PivotItemsWrapper.Add(new PivotItems { Header = "Свежее", IsEnabled = true, Content = TjOfftop });
                    PivotItemsWrapper.Add(new PivotItems { Header = "Непризнанное", IsEnabled = false, Content = TjOfftopUnadmitted }); // При навигации сюда падает, я в курсе
                    break;
            }
        }
    }
}
