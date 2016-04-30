using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TJ.Models;

namespace TJ.ViewModels
{
    public class ArticleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ArticleWrapper _ArticleInfo { get; set; }
        public ArticleWrapper ArticleInfo
        {
            get
            {
                return _ArticleInfo;
            }
            set
            {
                _ArticleInfo = value;
                this.OnPropertyChanged("ArticleInfo");
            }
        }

        public void setArticleInfo(ArticleWrapper i)
        {
            ArticleInfo = i;
        }
    }
}
