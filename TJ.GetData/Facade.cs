using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TJ.Models;
using Windows.UI.Xaml;

namespace TJ.GetData
{
    public class Facade
    {

        private const string APIVersion = "2.2";

        public static async Task PopulateLatestNewsAsync(ObservableCollection<NewsApi> latestNews, string sorting, int Type, int Count)
        {
            var newsWrapper = await GetNewsDataWrapperAsync(sorting, Type, Count);
            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            foreach (var item in newsWrapper)
            {
                if (item.likes.summ > 0) // Настраиваем цвет рейтинга
                {
                    item.likes.color = "#008542";
                }
                else if (item.likes.summ < 0)
                {
                    item.likes.color = "#DD0000";
                }
                else
                {
                    item.likes.color = "#eee";
                }

                bool temp = false;
                if (localSettings.Values["NewsContentVisible"] != null)
                {
                    Boolean.TryParse(localSettings.Values["NewsContentVisible"].ToString(), out temp);
                }
                if (temp == true)
                {
                    item.ShowNewsDetailsInSidebar = Visibility.Collapsed;
                }
                else
                {
                    item.ShowNewsDetailsInSidebar = Visibility.Visible;
                }

                string[] date = item.dateRFC.Split(' ');
                item.dateRFC = date[1] + ' ' + date[2] + ' ' + date[3] + ' ' + date[4];
                string pattern = "<[^>]*>"; // режем html-теги
                string content = item.intro;
                item.intro = Regex.Replace(content, pattern, "");
                latestNews.Add(item);
            }
        }
        public static async Task PopulateTweetsAsync(ObservableCollection<TweetsApi> tweets)
        {
            var tweetsWrapper = await GetTweetsWrapper();
            foreach (var tweet in tweetsWrapper)
            {
                tweets.Add(tweet);
            }
        }
        private static async Task<List<NewsApi>> GetNewsDataWrapperAsync(string sortMode, int Type, int Count)
        {
            string url = String.Format("https://api.tjournal.ru/{0}/club?sortMode={1}&type={2}&count={3}", APIVersion, sortMode, Type.ToString(), Count.ToString());

            HttpClient http = new HttpClient();
            var response = await http.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<NewsApi>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (List<NewsApi>)serializer.ReadObject(ms);
            return result;
        }
        public static async Task<List<TweetsApi>> GetTweetsWrapper()
        {
            string url = String.Format("https://api.tjournal.ru/{0}/tweets", APIVersion);

            HttpClient http = new HttpClient();
            var response = await http.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<TweetsApi>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (List<TweetsApi>)ser.ReadObject(ms);
            return result;
        }
        public static async Task<RootObject> GetArticleEntryJSONasync(int id)
        {
            string url = String.Format("https://api.tjournal.ru/2.2/club/item?entryId={0}", id);

            HttpClient http = new HttpClient();
            var response = await http.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (RootObject)ser.ReadObject(ms);
            return result;
        }
    }
}
