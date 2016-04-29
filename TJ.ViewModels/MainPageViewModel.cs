using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TJ.ViewModels
{

    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; // Вспомогательный, можно перенести в VMHelper
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _Title;
        public string Title // x:Bind to Textblock
        {
            get
            {
                if (_Title == null)
                {
                    _Title = "TJournal";
                }
                return _Title;
            }
        }

        private bool _isPaneOpen; 
        public bool isPaneOpen // x:Bind to SplitViews's "IsPaneOpen" property
        {
            get { return _isPaneOpen; } 
            set
            {
                if (value != this._isPaneOpen)
                {
                    _isPaneOpen = value;
                    this.OnPropertyChanged("isPaneOpen");
                }  
            }
        }

        public void changePaneState() // x:Bind to button
        {
            isPaneOpen = !isPaneOpen;
        }
    }
}