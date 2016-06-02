using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TJ.GetData;
using TJ.Models;
using TJ.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
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

        public static string UnGzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
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

        private string _ImageViewSource { get; set; }
        public string ImageViewSource
        {
            get
            {
                if (_ImageViewSource == null)
                {
                    return "http://i.imgur.com/eEalWsA.jpg";
                }
                else if (_ImageViewSource == "")
                {
                    return "x";
                }
                else
                {
                    return _ImageViewSource;
                }
            }
            set
            {
                _ImageViewSource = value;
                this.OnPropertyChanged("ImageViewSource");
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
                RichTextBlock richtextblock = 
                    new RichTextBlock {
                        MaxWidth = 650,
                        Margin = new Thickness(0,10,0,0),
                        TextWrapping = TextWrapping.Wrap
                    };
                Paragraph paragraph = new Paragraph {
                    Margin = new Thickness(20,0,20,0)
                };

                Run run = new Run();

                switch (block.type)
                {
                    case "image_extended":
                        var imgUri = new Uri(block.data.file.url);

                        Image image = 
                            new Image {
                                Source = new BitmapImage(imgUri),
                                Margin = new Thickness(0,20,0,0),
                                MaxWidth = 650
                            };

                        MainView.Children.Add(image);

                        if (block.data.caption != null) // Если есть, добавить подпись к изображению
                        {
                            TextBlock image_sign = 
                                new TextBlock { Text = block.data.caption,
                                    Foreground = GetColorFromHexa("#757575"),
                                    Margin = new Thickness(20, 0, 0, 0)
                                };
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

                        WebView webview = new WebView {
                            Width = videowidth + 20,
                            Height = videoheight + 20
                        };

                        webview.NavigateToString(html);

                        MainView.Children.Add(webview);
                        break;
                    case "text":
                    case "text_limited":
                        var textblock = block.data.text;
                        var paragraphs = Regex.Split(textblock, "<p>");

                        var pattern = "<i>|<b>|<strong>|<a href=\"";

                        foreach (var x in paragraphs)
                        {
                            var par = Regex.Split(x, "</p>")[0];
                            Run paragraph_text = new Run();
                            Paragraph paragraph_body = new Paragraph {
                                Margin = new Thickness(20, 10, 20, 10), 
                                FontSize = TextFontSize
                            };

                            InlineUIContainer inlineUIContainer = new InlineUIContainer();

                            var testregex = Regex.Split(par, pattern);

                            foreach (var y in testregex)
                            {
                                if (y != "")
                                {
                                    var isitalic = y.IndexOf("</i>") == -1 ? 0 : 1;
                                    var isbold = y.IndexOf("</b>") == -1 ? 0 : 1;
                                    var isstrong = y.IndexOf("</strong>") == -1 ? 0 : 1;
                                    var islink = y.IndexOf("</a>") == -1 ? 0 : 1;

                                    var ibl = isitalic.ToString() + isbold.ToString() + isstrong.ToString() + islink.ToString();

                                    switch (ibl)
                                    {
                                        case "1000":
                                            Debug.WriteLine("This is italic");

                                            var itcaliccontainer = Regex.Split(y, "</i>");
                                            var italicpiece = Regex.Split(y, "</i>")[0];
                                            var afteritalic = "";
                                            try
                                            {
                                                afteritalic = Regex.Split(y, "</i>")[1];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                afteritalic = "";
                                            }

                                            Run italicrun = new Run {
                                                Text = italicpiece,
                                                FontSize = TextFontSize,
                                                FontStyle = FontStyle.Italic
                                            };
                                            Run afteritalicrun = new Run {
                                                Text = afteritalic,
                                                FontSize = TextFontSize
                                            };

                                            paragraph_body.Inlines.Add(italicrun);
                                            paragraph_body.Inlines.Add(afteritalicrun);

                                            if (afteritalic == "")
                                            {
                                                try
                                                {
                                                    richtextblock.Blocks.Add(paragraph_body);
                                                } catch (Exception) { }
                                            }
                                            break;
                                        case "0100":
                                        case "0110":
                                        case "0010":
                                            Debug.WriteLine("This is bold");

                                            var boldcontainer = Regex.Split(y, "</b>|</strong>");
                                            var boldpiece = Regex.Split(y, "</b>|</strong>")[0];
                                            var afterbold = "";
                                            try
                                            {
                                                afterbold = Regex.Split(y, "</b>|</strong>")[1];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                afterbold = "";
                                            }

                                            Run boldrun = new Run {
                                                Text = boldpiece,
                                                FontSize = TextFontSize,
                                                FontWeight = FontWeights.Bold
                                            };
                                            Run afterboldrun = new Run {
                                                Text = afterbold,
                                                FontSize = TextFontSize
                                            };

                                            paragraph_body.Inlines.Add(boldrun);
                                            paragraph_body.Inlines.Add(afterboldrun);

                                            if (afterbold == "")
                                            {
                                                try
                                                {
                                                    richtextblock.Blocks.Add(paragraph_body);
                                                } catch (Exception) { }
                                            }
                                            break;
                                        case "0001":
                                            Debug.WriteLine("This is link");

                                            var linkcontainer = Regex.Split(y, "\" target=\"_blank\">");
                                            var linksource = linkcontainer[0];
                                            var linktext = "Ссылка";
                                            try
                                            {
                                                linktext = Regex.Split(linkcontainer[1], "</a>")[0];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                linksource = "http://github.com";
                                                linktext = "Ссылка";
                                            }


                                            var afterlink = "";

                                            if (linkcontainer.Length > 1)
                                            {
                                                afterlink = Regex.Split(linkcontainer[1], "</a>")[1];
                                            }

                                            Run linkrun = new Run {
                                                Text = linktext,
                                                FontSize = TextFontSize,
                                                Foreground = GetColorFromHexa("#1c69a9")
                                            };
                                            Run afterlinkrun = new Run {
                                                Text = afterlink,
                                                FontSize = TextFontSize
                                            };

                                            Hyperlink hyperlink = new Hyperlink { NavigateUri = new Uri(linksource) };

                                            paragraph_body.Inlines.Add(hyperlink);
                                            hyperlink.Inlines.Add(linkrun);
                                            paragraph_body.Inlines.Add(afterlinkrun);

                                            if(afterlink == "")
                                            {
                                                try
                                                {
                                                    richtextblock.Blocks.Add(paragraph_body);
                                                }
                                                catch (Exception) { }
                                            }
                                            break;
                                        default:
                                            Debug.WriteLine("This is generic text");

                                            paragraph_text.Text = y;
                                            paragraph_body.Inlines.Add(paragraph_text);
                                            richtextblock.Blocks.Add(paragraph_body);
                                            break;
                                    }
                                }
                            }
                        }

                        MainView.Children.Add(richtextblock);
                        break;
                    case "quote_styled":
                        var quote_textblock = block.data.text;
                        InlineUIContainer InlineUI = new InlineUIContainer();
                        var quote_paragraphs = Regex.Split(quote_textblock, "<p>");

                        StackPanel stackpanel = new StackPanel {
                            Background = GetColorFromHexa("#F6F6F6"),
                            Margin = new Thickness(0,20,0,0),
                            MaxWidth = 650
                        };

                        foreach (var par in quote_paragraphs)
                        {
                            if (par.Length > 1) // Легаси пиздец из старых версий, лень переделывать прост))
                            {
                                
                                Paragraph addin = new Paragraph();
                                var abzac = Regex.Split(par.ToString(), "</p>");

                                TextBlock para = new TextBlock {
                                    Text = abzac[0],
                                    FontSize = TextFontSize,
                                    TextWrapping = TextWrapping.Wrap,
                                    Margin = new Thickness(20)
                                };

                                stackpanel.Children.Add(para);
                            } 
                        }

                        if (block.data.cite != null)
                        {
                            var quote_author = block.data.cite;

                            TextBlock author = new TextBlock {
                                Text = quote_author,
                                FontSize = TextFontSize,
                                FontWeight = FontWeights.Bold,
                                Margin = new Thickness(20, 0, 20, 20),
                                TextWrapping = TextWrapping.Wrap
                            };

                            stackpanel.Children.Add(author);
                        }
                        MainView.Children.Add(stackpanel);
                        break;
                    case "tweet":
                        var requesturi = 
                            String.Format("https://publish.twitter.com/oembed?url={0}&omit_script=true&hide_thread=true", block.data.status_url);

                        HttpClient http = new HttpClient();
                        var response = await http.GetAsync(requesturi);
                        var gzJsonMessage = await response.Content.ReadAsByteArrayAsync();
                        var jsonMessage = UnGzip(gzJsonMessage);

                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TweetEmbed));
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));
                        var twiresponse = (TweetEmbed)ser.ReadObject(ms);

                        WebView twiwebview = new WebView {
                            MaxWidth = 550,
                            Height = 220,
                            Margin = new Thickness(0,10,0,10)
                        };
                        
                        twiwebview.NavigateToString("<!DOCTYPE HTML><html><head>" +
                            "<script async src=\"http://platform.twitter.com/widgets.js\" charset=\"utf-8\"></script>" +
                            "</head><body><div id=\"wrapper\">" + twiresponse.html + "</div></body></html>");

                        MainView.Children.Add(twiwebview);
                        break;
                    case "rawhtml":
                        string rawhtml = block.data.raw;
                        var rawhtmlheight = Regex.Split(Regex.Split(block.data.raw, "height=\"")[1], "\" ")[0];

                        WebView rawwebview = new WebView {
                            MaxWidth = 650,
                            Height = int.Parse(rawhtmlheight) + 20
                        };
                        
                        rawwebview.NavigateToString(rawhtml);
                        
                        MainView.Children.Add(rawwebview);
                        break;
                    case "link_embed_undeletable":
                        Hyperlink embed_u_link = new Hyperlink();
                        embed_u_link.NavigateUri = new Uri(block.data.url);
                         
                        run.Text = block.data.url.ToString();

                        embed_u_link.Inlines.Add(run);
                        paragraph.Inlines.Add(embed_u_link);
                        richtextblock.Blocks.Add(paragraph);

                        MainView.Children.Add(richtextblock);
                        break;
                    case "link_embed":
                        break;
                    case "gallery":
                        FlipView gallery_flipview = new FlipView();
                        gallery_flipview.MaxWidth = 650;
                        foreach (var item in block.data.files)
                        {
                            var flip_image_uri = new Uri(item.bigUrl);
                            Image flipview_image = new Image { Source = new BitmapImage(flip_image_uri) };
                            gallery_flipview.Items.Add(flipview_image);
                        }
                        MainView.Children.Add(gallery_flipview);
                        break;
                    default:
                        run.Text = block.data.text.ToString();
                        paragraph.Inlines.Add(run);
                        richtextblock.Blocks.Add(paragraph);

                        MainView.Children.Add(richtextblock);
                        break;
                }
            }
        }

        private RoutedEventHandler followButton_Click(string v)
        {
            return new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
            {
                Button_Click(v);
            });
        }

        private RoutedEventHandler goToTweet_Click(string v)
        {
            return new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
            {
                goToTweet(v);
            });
        }

        private RoutedEventHandler OpenImageFullscreen(string v)
        {
            return new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
            {
                _OpenImageFullscreen(v);
            });
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
            else if (MainScrollView.VerticalOffset < 90)
            {
                Head.Visibility = Visibility.Visible;
            }
        }

        private async void Button_Click(string id)
        {
            var link = new Uri("https://twitter.com/intent/follow?screen_name=" + id);
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        private async void goToTweet(string id)
        {
            var link = new Uri(id);
            await Windows.System.Launcher.LaunchUriAsync(link);
        }

        private void _OpenImageFullscreen(string link)
        {
            ImageViewer.Visibility = Visibility.Visible;
            ImageViewSource = link;
        }

        private void Close_ImageViewer(object sender, RoutedEventArgs e)
        {
            ImageViewer.Visibility = Visibility.Collapsed;
        }

        private async void Ignore_User(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(
                "Вы действительно хотите игнорировать пользователя" + ArticleInfoAuthor.Text + "?", "Подтверждение");
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Да, он заебал уже") { Id = 1, Invoked = new UICommandInvokedHandler(this.CommandInvokedHandler) });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Пожалуй, нет") { Id = 2, Invoked = new UICommandInvokedHandler(this.CommandInvokedHandler) });

            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            var s = command.Id.ToString();
            switch (s)
            {
                case "1":
                    SettingsViewModel.AddUserToBlackList(int.Parse(UserId.Text));
                    break;
                default:
                    break;
            }
        }
    }
}
