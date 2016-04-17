using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

namespace TryHamburger.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArticleView : Page
    {
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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            var parameter = e.Parameter.ToString();
            var entryId = 1000;
            int.TryParse(parameter, out entryId);
            var response = await Facade.GetArticleEntryJSONasync(entryId);
            
            if (response.entryJSON != null)
            {
                var pattern = "},{";
                var entryBody = Regex.Replace(response.entryJSON, "\\\\|<br />", "");
                string[] blocks = Regex.Split(entryBody, pattern);
                ArticleHeader.Text = response.title;

                foreach (var x in blocks)
                {
                    RichTextBlock richtextblock = new RichTextBlock();
                    Paragraph paragraph = new Paragraph();
                    richtextblock.MaxWidth = 650;
                    richtextblock.TextWrapping = TextWrapping.Wrap;
                    Run run = new Run();

                    if (x.IndexOf("image_extended") > 0) // если картинка
                    {
                        var urls = Regex.Split(x.ToString(), "url");
                        foreach (var url in urls)
                        {
                            if (url.IndexOf("bigUrl") > 0)
                            {
                                var stri = Regex.Replace(url, "\"", "");
                                var links = Regex.Split(stri, ",");
                                var link = links[0].Substring(1);

                                var imgUri = new Uri(link);
                                Image image = new Image();
                                image.Source = new BitmapImage(imgUri);
                                image.Margin = new Thickness(50,20,50,0);
                                image.MaxWidth = 650;
                                
                                MainView.Children.Add(image);
                            }
                        }
                    }
                    else if (x.IndexOf("video_extended") > 0)
                    {
                        var urls = Regex.Split(x.ToString(), "\"remote_id\":\"");
                        var id = Regex.Split(urls[1], "\",\"")[0];
                        var videowidth = MainView.ActualWidth - 50;
                        var videoheight = MainView.ActualWidth*480/800-30;
                        if (videowidth>650) // На что я трачу свою жизнь...
                        {
                            videowidth = 650;
                            videoheight = 390;
                        }
                        string html = @"<iframe width=" + videowidth +
                            @" height=" + videoheight + 
                            @" src=""http://www.youtube.com/embed/" + 
                            id + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>";

                        WebView webview = new WebView();
                        webview.Width = videowidth+20;
                        webview.Height = videoheight+20;
                        webview.NavigateToString(html);
                        MainView.Children.Add(webview);
                    }
                    else if (x.IndexOf("\"text\",\"data\":{\"text\":")>0)
                    {
                        var str = Regex.Split(x.ToString(), "data\":{\"text\":")[1];
                        var block = Regex.Split(str, ",\"format\":")[0].TrimStart('"').TrimEnd('"');
                        var paragraphs = Regex.Split(block, "<p>"); // Открыл в себе талант обфускатора.
                        foreach(var par in paragraphs)
                        {
                            Run para = new Run();
                            Paragraph addin = new Paragraph();
                            addin.Margin = new Thickness(20,10,20,10);
                            InlineUIContainer InlineUi = new InlineUIContainer();
                            var abzac = Regex.Split(par.ToString(), "</p>");
                            if (abzac[0].IndexOf("<a href=\"") > 0)
                            {
                                var linkbody = Regex.Split(abzac[0], "</a>");
                                foreach (var link in linkbody)
                                {
                                    if(link.IndexOf("<a href=\"") >0)
                                    {
                                        var aferr = Regex.Split(link, "<a href=\"");
                                        var linkcontainer = Regex.Split(aferr[1], "\" target=\"_blank\">");
                                        var linksource = linkcontainer[0];
                                        Run linktext = new Run();
                                        Hyperlink hyperlink = new Hyperlink();
                                        hyperlink.NavigateUri = new Uri(linksource);
                                        Run newRun = new Run();
                                        newRun.Text = aferr[0];
                                        linktext.Text = linkcontainer[1];
                                        addin.Inlines.Add(newRun);
                                        addin.Inlines.Add(hyperlink);
                                        hyperlink.Inlines.Add(linktext);
                                    }
                                    else
                                    {
                                        para.Text = link;
                                        addin.Inlines.Add(para);
                                    }
                                }
                                richtextblock.Blocks.Add(addin);
                            } else
                            {
                                para.Text = abzac[0];
                                addin.Inlines.Add(para);
                                richtextblock.Blocks.Add(addin);
                            }
                        }
                    }
                    else if (x.IndexOf("\"quote_styled\",\"data\":{\"text\":") > 0)
                    {
                        var str = Regex.Split(x.ToString(), "data\":{\"text\":")[1];
                        var block = str.TrimStart('"').TrimEnd('"');
                        InlineUIContainer InlineUi = new InlineUIContainer();
                        var paragraphs = Regex.Split(block, "<p>");
                        foreach (var par in paragraphs)
                        {
                            if (par.Length > 1) // Сколько же во мне аутизма, господи
                            {
                                TextBlock para = new TextBlock();
                                Paragraph addin = new Paragraph();
                                var abzac = Regex.Split(par.ToString(), "</p>");

                                para.Text = abzac[0];
                                para.TextWrapping = TextWrapping.Wrap;
                                para.Margin = new Thickness(20, 20, 20, 20);

                                StackPanel stackpanel = new StackPanel();
                                stackpanel.Background = GetColorFromHexa("#F6F6F6");
                                stackpanel.MaxWidth = 650;
                                stackpanel.Children.Add(para);


                                if (block.IndexOf("\"format\":\"html\",\"cite\":")>0)
                                {
                                    TextBlock author = new TextBlock();
                                    var quote_string = Regex.Split(block, "\"format\":\"html\",\"cite\":")[1].TrimStart('"');
                                    var quote_author = "– " + Regex.Split(quote_string, "\",\"size\"")[0];
                                    author.Text = quote_author;
                                    author.FontWeight = FontWeights.Bold;
                                    author.Margin = new Thickness(20, 0, 20, 20);
                                    stackpanel.Children.Add(author);
                                }                               

                                MainView.Children.Add(stackpanel);
                            }                 
                        }
                    }
                    else if(x.IndexOf("heading_styled") > 0)
                    {                      
                        var titleTextWrapper = Regex.Split(x, "\",\"format\":\"html\",\"heading-styles");
                        var titleTextBox = titleTextWrapper[1];
                        var titleText = titleTextWrapper[0];
                        var Text = Regex.Split(titleText, "\"data\":{\"text\":\"");
                        run.Text = Text[1];

                        if (titleTextBox.IndexOf("h1") > 0)
                        {                         
                            run.FontSize = 24;
                        }
                        else if (titleTextBox.IndexOf("h2")>0)
                        {
                            run.FontSize = 20;
                        }
                        else if (titleTextBox.IndexOf("h3")>0)
                        {
                            run.FontSize = 18;
                        }
                        run.FontWeight = FontWeights.Bold;
                        paragraph.Inlines.Add(run);
                    }
                    else
                    {
                        run.Text = x.ToString(); // Это для удобства отладки. Потом надо будет заменить на WebView
                        paragraph.Inlines.Add(run);
                    }

                    richtextblock.Blocks.Add(paragraph);
                    MainView.Children.Add(richtextblock);
                }
            }
        }
    }
}
