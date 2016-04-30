using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJ.Models
{

    public class ArticleAuthor
    {
        public int id { get; set; }
        public string name { get; set; }
        public string profile_image_url { get; set; }
        public string profile_big_image_url { get; set; }
        public string url { get; set; }
    }

    public class ArticleSize
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class ArticleCover
    {
        public int type { get; set; }
        public object additionalData { get; set; }
        public string thumbnailUrl { get; set; }
        public string url { get; set; }
        public Size size { get; set; }
    }

    public class ArticleLikes
    {
        public int count { get; set; }
        public int summ { get; set; }
        public int isLiked { get; set; }
        public bool isHidden { get; set; }
        public object hash { get; set; }
    }

    public class ArticleWrapper
    {
        public int id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public int date { get; set; }
        public string dateRFC { get; set; }
        public ArticleAuthor author { get; set; }
        public int type { get; set; }
        public string intro { get; set; }
        public ArticleCover cover { get; set; }
        public object externalLink { get; set; }
        public object inspiredByThis { get; set; }
        public string entryJSON { get; set; }
        public bool isReadMore { get; set; }
        public int hits { get; set; }
        public ArticleLikes likes { get; set; }
        public int commentsCount { get; set; }
        public bool isFavorited { get; set; }
        public int userDevice { get; set; }
        public string mobileAppUrl { get; set; }
        public bool isDraft { get; set; }
        public bool isGold { get; set; }
        public bool isVotingActive { get; set; }
        public bool isWide { get; set; }
        public bool isAdvertising { get; set; }
        public bool isCommentsClosed { get; set; }
        public bool isStillUpdating { get; set; }
        public bool isComplexMarkup { get; set; }
        public bool isBigPicture { get; set; }
    }
}
