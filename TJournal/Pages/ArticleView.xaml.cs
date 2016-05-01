using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using TJ.GetData;
using TJ.Models;
using TJ.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TJournal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArticleView : Page, INotifyPropertyChanged
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

        public ArticleView()
        {
            this.InitializeComponent();
        }
        public SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            byte R = Convert.ToByte(hexaColor.Substring(1, 2), 16);
            byte G = Convert.ToByte(hexaColor.Substring(3, 2), 16);
            byte B = Convert.ToByte(hexaColor.Substring(5, 2), 16);
            SolidColorBrush scb = new SolidColorBrush(Color.FromArgb(0xFF, R, G, B));
            return scb;
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

        private Visibility _SponsoredBadgeVisibility { get; set; }
        public Visibility SponsoredBadgeVisibility
        {
            get
            {
                return _SponsoredBadgeVisibility;
            }
            set
            {
                _SponsoredBadgeVisibility = value;
                this.OnPropertyChanged("SponsoredBadgeVisibility");
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            int id = 1000;
            int.TryParse(e.Parameter.ToString(), out id);
            var entry = await Facade.GetArticleEntryJSONasync(id);

            double TextFontSize = 18; // Настройка размера шрифта
            if (localSettings.Values["FontSize"] != null)
            {
                TextFontSize = double.Parse(localSettings.Values["FontSize"].ToString());
            }

            ArticleInfo = entry;
            
            SponsoredBadgeVisibility = Visibility.Collapsed;
            if (entry.isAdvertising == true) 
            {
                SponsoredBadgeVisibility = Visibility.Visible; // Показывает плашку спонсорского материала, если материал спонсорский
            }

            var content = Facade.ParseEntryJSON(entry.entryJSON);

            foreach (var block in content.data)
            {
                RichTextBlock richtextblock = new RichTextBlock();
                Paragraph paragraph = new Paragraph();
                richtextblock.MaxWidth = 650;
                richtextblock.Margin = new Thickness(0,-10,0,0);
                richtextblock.TextWrapping = TextWrapping.Wrap;
                Run run = new Run();

                switch (block.type)
                {
                    case "image_extended":
                        var link = block.data.file.url;
                        var imgUri = new Uri(link);
                        Image image = new Image();

                        image.Source = new BitmapImage(imgUri);
                        image.Margin = new Thickness(0,20,0,0);
                        image.MaxWidth = 650;

                        MainView.Children.Add(image);

                        if (block.data.caption != null) // Если есть, добавить подпись к изображению
                        {
                            TextBlock image_sign = new TextBlock();
                            image_sign.Text = block.data.caption;
                            image_sign.Foreground = GetColorFromHexa("#757575");
                            image_sign.Margin = new Thickness(20,0,0,0);
                            MainView.Children.Add(image_sign);
                        }
                        break;
                    case "video_extended":
                        var videoid = block.data.remote_id;
                        var videowidth = MainView.ActualWidth;
                        var videoheight = MainView.ActualWidth * 480 / 800 - 10;

                        if (videowidth > 650)
                        {
                            videowidth = 650;
                            videoheight = 390;
                        }

                        string html = @"<iframe width=" + videowidth +
                            @" height=" + videoheight + @" src=""http://www.youtube.com/embed/" +
                            videoid + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>";

                        WebView webview = new WebView();
                        webview.Width = videowidth + 20;
                        webview.Height = videoheight + 20;
                        webview.NavigateToString(html);

                        MainView.Children.Add(webview);
                        break;
                    case "text":
                        var textblock = block.data.text;
                        var paragraphs = Regex.Split(textblock, "<p>");

                        foreach (var x in paragraphs)
                        {
                            var par = Regex.Split(x, "</p>")[0];
                            Run paragraph_text = new Run();
                            Paragraph paragraph_body = new Paragraph();
                            paragraph_body.Margin = new Thickness(20,10,20,10);
                            paragraph_text.FontSize = TextFontSize;
                            InlineUIContainer inlineUIContainer = new InlineUIContainer();

                            if (par.IndexOf("<a href=\"") > 0) // Обработка инлайновых ссылок
                            {
                                var linkbody = Regex.Split(par, "</a>");
                                foreach (var url in linkbody)
                                {
                                    if (url.IndexOf("<a href=\"") > 0)
                                    {
                                        var aferr = Regex.Split(url, "<a href=\"");
                                        var linkcontainer = Regex.Split(aferr[1], "\" target=\"_blank\">");
                                        var linksource = linkcontainer[0];
                                        Run linktext = new Run();
                                        Hyperlink hyperlink = new Hyperlink();
                                        hyperlink.NavigateUri = new Uri(linksource);
                                        Run newRun = new Run();
                                        newRun.Text = aferr[0];
                                        newRun.FontSize = TextFontSize;
                                        linktext.Text = linkcontainer[1];
                                        linktext.FontSize = TextFontSize;
                                        linktext.Foreground = GetColorFromHexa("#1c69a9"); // Теперь они будут синенькие

                                        paragraph_body.Inlines.Add(newRun);
                                        paragraph_body.Inlines.Add(hyperlink);
                                        hyperlink.Inlines.Add(linktext);
                                    }
                                    else
                                    {
                                        paragraph_text.Text = url;
                                        paragraph_body.Inlines.Add(paragraph_text);
                                    }
                                }
                                richtextblock.Blocks.Add(paragraph_body);
                            }
                            else
                            {
                                paragraph_text.Text = par;
                                paragraph_body.Inlines.Add(paragraph_text);
                                richtextblock.Blocks.Add(paragraph_body);
                            }
                        }
                        MainView.Children.Add(richtextblock);
                        break;
                    case "quote_styled":
                        var quote_textblock = block.data.text;
                        InlineUIContainer InlineUI = new InlineUIContainer();
                        var quote_paragraphs = Regex.Split(quote_textblock, "<p>");

                        StackPanel stackpanel = new StackPanel();
                        stackpanel.Background = GetColorFromHexa("#F6F6F6");
                        stackpanel.Margin = new Thickness(0,20,0,0);
                        stackpanel.MaxWidth = 650;

                        foreach (var par in quote_paragraphs)
                        {
                            if (par.Length > 1) // Легаси пиздец из старых версий, лень переделывать прост))
                            {
                                TextBlock para = new TextBlock();
                                Paragraph addin = new Paragraph();
                                var abzac = Regex.Split(par.ToString(), "</p>");

                                para.Text = abzac[0];
                                para.FontSize = TextFontSize;
                                para.TextWrapping = TextWrapping.Wrap;
                                para.Margin = new Thickness(20, 20, 20, 20);

                                stackpanel.Children.Add(para);
                            } 
                        }

                        if (block.data.cite != null)
                        {
                            TextBlock author = new TextBlock();
                            var quote_author = block.data.cite;
                            
                            author.Text = quote_author;
                            author.FontSize = TextFontSize;
                            author.FontWeight = FontWeights.Bold;
                            author.Margin = new Thickness(20, 0, 20, 20);
                            author.TextWrapping = TextWrapping.Wrap;

                            stackpanel.Children.Add(author);
                        }
                        MainView.Children.Add(stackpanel);
                        break;
                }
            }
        }

        private void Author_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Profile), ArticleInfo.author.id);
        }

        private void MainScrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (MainScrollView.VerticalOffset > 200)
            {
                Head.Visibility = Visibility.Collapsed;
            }
            else if (MainScrollView.VerticalOffset < 150)
            {
                Head.Visibility = Visibility.Visible;
            }
        }
    }
}
