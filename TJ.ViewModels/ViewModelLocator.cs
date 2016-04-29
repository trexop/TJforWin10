using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJ.ViewModels
{
    class ViewModelLocator
    {
        public ViewModelLocator()
        {
        }

        public MainPageViewModel Main
        {
            get
            {
                return new MainPageViewModel();
            }
        }
        public NewsPageViewModel News
        {
            get
            {
                return new NewsPageViewModel();
            }
        }
        public SettingsViewModel Settings
        {
            get
            {
                return new SettingsViewModel();
            }
        }
    }
}
