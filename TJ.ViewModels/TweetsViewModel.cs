using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TJ.GetData;
using TJ.Models;

namespace TJ.ViewModels
{
    public class TweetsPivotItems
    {
        public string Header { get; set; }
        public bool IsEnabled { get; set; }
        public ObservableCollection<TweetsApi> Content { get; set; }
    }
    public class TweetsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<TweetsPivotItems> PivotItemsWrapper { get; set; }

        public ObservableCollection<TweetsApi> ThreeHoursTweets { get; set; }
        public ObservableCollection<TweetsApi> Tweets { get; set; }
        public ObservableCollection<TweetsApi> WeeklyTweets { get; set; }
        public ObservableCollection<TweetsApi> MonthlyTweets { get; set; }

        public void CreateCollections()
        {
            ThreeHoursTweets = new ObservableCollection<TweetsApi>();
            Tweets = new ObservableCollection<TweetsApi>();
            WeeklyTweets = new ObservableCollection<TweetsApi>();
            MonthlyTweets = new ObservableCollection<TweetsApi>();
        }

        public async void setPivotItems()
        {
            PivotItemsWrapper = new ObservableCollection<TweetsPivotItems>();
            CreateCollections();

            await Facade.PopulateTweetsAsync(ThreeHoursTweets, "3hours");
            await Facade.PopulateTweetsAsync(Tweets, "fresh");
            await Facade.PopulateTweetsAsync(WeeklyTweets, "week");
            await Facade.PopulateTweetsAsync(MonthlyTweets, "month");

            PivotItemsWrapper.Add(new TweetsPivotItems { Header = "За три часа", IsEnabled = false, Content = ThreeHoursTweets });
            PivotItemsWrapper.Add(new TweetsPivotItems { Header = "Свежее", IsEnabled = true, Content = Tweets });
            PivotItemsWrapper.Add(new TweetsPivotItems { Header = "Лучшее за неделю", IsEnabled = false, Content = WeeklyTweets });
            PivotItemsWrapper.Add(new TweetsPivotItems { Header = "Лучшее за месяц", IsEnabled = false, Content = MonthlyTweets });
        }
    }
}
