using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TJ.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace TJ.ViewModels
{
    public class NotificationBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // SetField (Name, value); // where there is a data member
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] String property = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            RaisePropertyChanged(property);
            return true;
        }

        // SetField(()=> somewhere.Name = value; somewhere.Name, value) // Advanced case where you rely on another property
        protected bool SetProperty<T>(T currentValue, T newValue, Action DoSet, [CallerMemberName] String property = null)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue)) return false;
            DoSet.Invoke();
            RaisePropertyChanged(property);
            return true;
        }

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(property)); }
        }
    }

    public class NotificationBase<T> : NotificationBase where T : class, new()
    {
        protected T This;

        public static implicit operator T(NotificationBase<T> thing) { return thing.This; }

        public NotificationBase(T thing = null)
        {
            This = (thing == null) ? new T() : thing;
        }
    }

    public class BindingHelper : UserControl
    {
        

        public static List<TweetLinks> GetText(DependencyObject obj)
        {
            return (List<TweetLinks>)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, List<TweetLinks> value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(List<TweetLinks>), typeof(BindingHelper), new PropertyMetadata(String.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            var control = sender as RichTextBlock;
            if (control != null)
            {
                control.Blocks.Clear();
                var paragraph = new Paragraph();

                List<TweetLinks> value = e.NewValue as List<TweetLinks>;
                foreach (var item in value)
                {
                    double TextFontSize = 18; // Настройка размера шрифта
                    if (localSettings.Values["FontSize"] != null)
                    {
                        TextFontSize = double.Parse(localSettings.Values["FontSize"].ToString()) + 2;
                    }

                    Hyperlink hyperlink = new Hyperlink();
                    hyperlink.NavigateUri = new Uri(item.short_link);
                    paragraph.Inlines.Add(new Run { Text = item.text, FontSize = TextFontSize});
                    hyperlink.Inlines.Add(new Run { Text = item.hr_link, FontSize = TextFontSize });
                    paragraph.Inlines.Add(hyperlink);  
                }
                control.Blocks.Add(paragraph);
            }
        }
    }
}
