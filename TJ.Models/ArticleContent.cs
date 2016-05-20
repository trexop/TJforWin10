using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJ.Models
{
    public class tweetUser
    {
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string screen_name { get; set; }
        public string name { get; set; }
    }

    public class File
    {
        public string url { get; set; }
        public string bigUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class ContentData
    {
        public bool background { get; set; }
        public bool border { get; set; }
        public File file { get; set; }
        public string caption { get; set; }
        public bool cover { get; set; }
        public string source { get; set; }
        public string remote_id { get; set; }
        public string text { get; set; }
        public string format { get; set; }
        public string introText { get; set; }
        public string thumbnailUrl { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public string cite { get; set; }
        public string size { get; set; }

        // tweets related methods
        public bool media { get; set; }
        public bool conversation { get; set; }
        public tweetUser user { get; set; }
        public long id { get; set; }
        public string created_at { get; set; }
        public string status_url { get; set; }
    }

    public class Datum
    {
        public string type { get; set; }
        public ContentData data { get; set; }
    }

    public class ArticleContent
    {
        public List<Datum> data { get; set; }
    }

}
