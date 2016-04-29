using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJ.Models
{
    public class User
    {
        public int created_at { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public object id { get; set; }
        public string name { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_bigger { get; set; }
        public string screen_name { get; set; }
        public int statuses_count { get; set; }
    }

    public class Medium
    {
        public int type { get; set; }
        public string thumbnail_url { get; set; }
        public string media_url { get; set; }
        public int thumbnail_width { get; set; }
        public int thumbnail_height { get; set; }
        public double ratio { get; set; }
    }

    public class TweetsApi
    {
        public string id { get; set; }
        public string text { get; set; }
        public User user { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
        public bool has_media { get; set; }
        public List<Medium> media { get; set; }
        public bool isFavorited { get; set; }
        public string security_user_hash { get; set; }
        public int created_at { get; set; }
    }
}
