using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TJ.Models;
using Windows.Storage;
using Windows.UI.Xaml;

namespace TJ.GetData
{
    public class BlackListedAccount
    {
        public int id { get; set; }
    }

    public class Facade
    {
        private const string APIVersion = "2.3";
        public static ObservableCollection<BlackListedAccount> NewsBlackList { get; set; }

        public static async Task PopulateLatestNewsAsync(ObservableCollection<NewsApi> latestNews, string sorting, int Type, int Count, int offset)
        {
            var newsWrapper = await GetNewsDataWrapperAsync(sorting, Type, Count, offset);
            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            foreach (var item in newsWrapper)
            {
                if (item.likes.summ > 0) // Настраиваем цвет рейтинга
                { item.likes.color = "#008542"; }
                else if (item.likes.summ < 0)
                { item.likes.color = "#DD0000"; }
                else
                { item.likes.color = "#eee"; }

                bool temp = false;

                if (localSettings.Values["NewsContentVisible"] != null)
                { Boolean.TryParse(localSettings.Values["NewsContentVisible"].ToString(), out temp); }
                if (temp == true)
                { item.ShowNewsDetailsInSidebar = Visibility.Collapsed; }
                else
                { item.ShowNewsDetailsInSidebar = Visibility.Visible; }

                item.ShowGenericInfo = Visibility.Visible;
                item.IsThisANextButton = Visibility.Collapsed;
                   
                if (item.cover == null)
                {
                    item.cover = new Cover();
                    item.cover.url = "x";
                }
                else
                {
                    if (item.cover.url == null)
                    {
                        item.cover = new Cover();
                        item.cover.url = "x";
                    }
                }

                string[] date = item.dateRFC.Split(' ');
                item.dateRFC = date[1] + ' ' + date[2] + ' ' + date[3] + ' ' + date[4];
                string pattern = "<[^>]*>"; // режем html-теги
                string content = item.intro;
                item.intro = Regex.Replace(content, pattern, "");

                if (NewsBlackList.Any(p => p.id == item.author.id))
                {
                    Debug.WriteLine("В чёрном списке");
                } else
                {
                    latestNews.Add(item);
                }
            }

            var NextButton = new NewsApi();
            int n = 30;
            int.TryParse(localSettings.Values["NumberOfOnetimeLoadedItems"].ToString(), out n); // количество загружаемых новостей
            NextButton.IsThisANextButton = Visibility.Visible;
            NextButton.ShowGenericInfo = Visibility.Collapsed;
            NextButton.intro = String.Format("Загрузить следующие {0} записей", n);
            latestNews.Add(NextButton);
        }

        public static async Task PopulateTweetsAsync(ObservableCollection<TweetsApi> tweets, string interval)
        {

            var tweetsWrapper = await GetTweetsWrapper(interval);
            foreach (var tweet in tweetsWrapper)
            {
                tweet.inlineLinks = new List<TweetLinks>();
                string[] el_links = { "http://twitter.com", "http://twitter.com", "" };

                var test1 = tweet.text.IndexOf("[[");
                var test2 = tweet.text.IndexOf("\\[\\[");

                if (tweet.text.IndexOf("[[") > -1)
                {
                    var te = tweet.text;
                    var urls = Regex.Split(te, "\\[\\[");

                    for (var i = 1; i < urls.Length ; i++)
                    {
                        var str = urls[i];
                        var strings = Regex.Split(urls[i], "\\]\\]"); // Убираем ненужные символы, разбиваем на массив
                        foreach (var el in strings)
                        {
                            if (el.IndexOf("||")>0)
                            {
                                el_links = Regex.Split(el, "\\|\\|");
                                tweet.inlineLinks.Add(new TweetLinks { text = urls[0], short_link = el_links[0], long_link = el_links[1], hr_link = el_links[2] });
                            }
                            else
                            {
                                tweet.inlineLinks.Add(new TweetLinks { text = urls[0], short_link = "http://twitter.com", long_link = "http://twitter.com", hr_link = "" });
                            }
                        }
                    }

                    tweet.text = urls[0];
                    
                }
                else
                {
                    tweet.inlineLinks.Add(new TweetLinks { text = tweet.text, short_link = "http://twitter.com", long_link = "http://twitter.com", hr_link = "" });
                }

                tweets.Add(tweet);
            }
        }

        private static async Task<List<NewsApi>> GetNewsDataWrapperAsync(string sortMode, int Type, int Count, int offset)
        {
            string url =
                String.Format("https://api.tjournal.ru/{0}/club?sortMode={1}&type={2}&count={3}&offset={4}",
                APIVersion, sortMode, Type.ToString(), Count.ToString(), offset.ToString());

            HttpClient http = new HttpClient();
            var response = await http.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<NewsApi>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (List<NewsApi>)serializer.ReadObject(ms);
            return result;
        }

        public static async Task<List<TweetsApi>> GetTweetsWrapper(string interval)
        {
            string url = String.Format("https://api.tjournal.ru/{0}/tweets?interval={1}", APIVersion, interval);

            HttpClient http = new HttpClient();
            var response = await http.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<TweetsApi>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (List<TweetsApi>)ser.ReadObject(ms);
            return result;
        }

        public static async Task<ArticleWrapper> GetArticleEntryJSONasync(int id)
        {
            string url = String.Format("https://api.tjournal.ru/2.2/club/item?entryId={0}", id);

            HttpClient http = new HttpClient();
            var response = await http.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ArticleWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (ArticleWrapper)ser.ReadObject(ms);
            return result;
        }

        public static ArticleContent ParseEntryJSON(string str)
        {
            string json = str.Replace("\\\\|<br />", "");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ArticleContent));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var result = (ArticleContent)ser.ReadObject(ms);
            return result;
        }

        public static async Task<ProfileApi> GetProfileJSON(int id)
        {
            string url = String.Format("https://api.tjournal.ru/2.2/account/info?userId={0}", id);

            HttpClient http = new HttpClient();
            var response = await http.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ProfileApi));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (ProfileApi)ser.ReadObject(ms);

            if (result.rating.karma < 0) {
                result.carmaColor = "#DD0000"; } // Красный
            else { result.carmaColor = "#008542"; } // Зелёный

            if (result.is_club_member == true) {
                result.isClubMemberColor = "#008542"; } // Зелёный
            else { result.isClubMemberColor = "#DD0000"; }

            return result;
        }

        public static async Task<ObservableCollection<BlackListedAccount>> GetBlackListCollection()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync("blacklist.json");
                using (var stream = await file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string json = await reader.ReadToEndAsync();
                    if (json != "")
                    {
                        var collection = JsonConvert.DeserializeObject<ObservableCollection<BlackListedAccount>>(json);
                        return collection;
                    }
                    else
                    {
                        var collection = new ObservableCollection<BlackListedAccount>();
                        return collection;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                var collection = new ObservableCollection<BlackListedAccount>();
                return collection;
            }
        }

        public static async void PopulateBlackListedAccounts(ObservableCollection<BlackListedAccount> coll)
        {
            var collection = await GetBlackListCollection();
            foreach (var item in collection)
            {
                coll.Add(item);
            }
        }
    }
}
