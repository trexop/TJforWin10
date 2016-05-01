using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJ.Models
{

    public class Rating
    {
        public double karma { get; set; }
        public int karmaCount { get; set; }
    }

    public class SocialAccount
    {
        public string socialId { get; set; }
        public int socialType { get; set; }
        public string socialClass { get; set; }
        public string screenName { get; set; }
        public string url { get; set; }
    }

    public class ProfileApi
    {
        public int id { get; set; }
        public string name { get; set; }
        public string profile_image_url { get; set; }
        public string profile_big_image_url { get; set; }
        public string profile_background_url { get; set; }
        public int date { get; set; }
        public bool is_club_member { get; set; }
        public Rating rating { get; set; }
        public List<SocialAccount> socialAccounts { get; set; }
        public bool is_online { get; set; }
        // custom properties
        public string carmaColor { get; set; }
        public string isClubMemberColor { get; set; }
    }

}
