using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJ.Models
{

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
        public string text { get; set; }
        public string format { get; set; }
        public string introText { get; set; }
        public string cite { get; set; }
        public string size { get; set; }
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
