﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TJ.ViewModels
{
    public class ComboBoxItem
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsSelected { get; set; } 
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;


        public async void Button_Click(object sender, RoutedEventArgs e)
        {
            var link = new Uri("mailto:admin@kyubey.ru");
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        public async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var link = new Uri("https://github.com/trexop/TJforWin10");
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        public List<ComboBoxItem> ComboBoxItems { get; set; }

        private ComboBoxItem _DefaultPageSelectedItem = new ComboBoxItem();
        public ComboBoxItem DefaultPageSelectedItem
        {
            get
            {
                return _DefaultPageSelectedItem;
            }
            set
            {
                _DefaultPageSelectedItem = value; OnPropertyChanged("DefaultPageSelectedItem");
            }
        }
        public void PopulateComboboxItems()
        {
            ComboBoxItems = new List<ComboBoxItem>();
            var DefaultPageValue = localSettings.Values["DefaultPage"].ToString();

            ComboBoxItems.Add(new ComboBoxItem { Name = "News", Content = "Новости/Главное"});
            ComboBoxItems.Add(new ComboBoxItem { Name = "Editorial", Content = "Новости/Редакция" });
            ComboBoxItems.Add(new ComboBoxItem { Name = "Articles", Content = "Статьи" });
            ComboBoxItems.Add(new ComboBoxItem { Name = "Video", Content = "Видео" });
            ComboBoxItems.Add(new ComboBoxItem { Name = "Offtopic", Content = "Оффтоп" });
            ComboBoxItems.Add(new ComboBoxItem { Name = "Tweets", Content = "Твиты" });

            foreach (var Item in ComboBoxItems)
            {
                if (Item.Name == DefaultPageValue)
                {
                    DefaultPageSelectedItem = Item;
                }
            }
        }

        public void DefaultPage_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = (ComboBox)sender;
            var item = (ComboBoxItem)combo.SelectedItem;
            localSettings.Values["DefaultPage"] = item.Name.ToString();
        }
    }
}