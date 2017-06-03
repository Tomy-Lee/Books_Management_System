using System;
using System.IO;
using System.Net.Http;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Newtonsoft.Json;
using System.Xml;
using Windows.Data.Xml.Dom;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Books_Management_System
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class QueryPage : Page
    {
        public QueryPage()
        {
            this.InitializeComponent();
        }
        ViewModels.bookviewmodel ViewModel { get; set; }
       // Models.book ShareItem;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
            
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }
        /// <summary>
        /// 共享事件的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dp = args.Request.Data;
            var deferral = args.Request.GetDeferral();
            dp.Properties.Title = booknamee.Text + writertext.Text;
            dp.Properties.Description = infotext.Text;
            dp.SetText("done" + booknamee.Text);
            deferral.Complete();
        }
        /// <summary>
        /// 共享按钮点击后检测是否有可供共享的内容，如果可以则共享书名、作者和简介
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void share_Click_1(object sender, RoutedEventArgs e)
        {
            if (infotext.Text != "")
            {
                // ShareItem = (Models.book)((MenuFlyoutItem)sender).DataContext;
                DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += DataTransferManager_DataRequested;
                DataTransferManager.ShowShareUI();
            }
            else
            {
                var msgdialog22 = new MessageDialog("没有可供共享的内容").ShowAsync();
            }
        }
        void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
        
            request.Data.Properties.Title = booknamee.Text + writertext.Text;
            request.Data.Properties.Description = infotext.Text;
            request.Data.SetText(booknamee.Text + writertext.Text);
            request.Data.SetText(infotext.Text);
            var Deferral = args.Request.GetDeferral();
            Deferral.Complete();

        }
        /// <summary>
        /// ISBN查询图书相关信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void query_Click(object sender, RoutedEventArgs e)
        {
            infotext.Text = "";
            writertext.Text = "";
            booknamee.Text = "";
            var str2 = isbn.Text.ToString();
            if (str2.Length != 13)
            {
                var msgdialog2 = new MessageDialog("您输入的编号格式不正确，请重新输入13位ISBN编码").ShowAsync();
                isbn.Text = "";
            }
            else
            {
                queryxml(isbn.Text);
            }
        }
        /// <summary>
        /// 网络请求，解析xml格式传回的数据，书名、作者和简介
        /// </summary>
        /// <param name="testa"></param>
        async void queryxml(string testa)
        {
            try
            {
                string url = "http://api.douban.com/book/subject/isbn/" + testa;
                HttpClient client = new HttpClient();
                string result = await client.GetStringAsync(url);
                Windows.Data.Xml.Dom.XmlDocument document = new Windows.Data.Xml.Dom.XmlDocument();
                document.LoadXml(result);

                Windows.Data.Xml.Dom.XmlNodeList list = document.GetElementsByTagName("summary");
                IXmlNode node1 = list.Item(0);
                infotext.Text = node1.InnerText;

                list = document.GetElementsByTagName("title");
                node1 = list.Item(0);
                booknamee.Text = "《" + node1.InnerText +"》";
                list = document.GetElementsByTagName("name");
                node1 = list.Item(0);
                writertext.Text = node1.InnerText;

            }
            catch (HttpRequestException)
            {
                var msgdialog1 = new MessageDialog("号码输入有误，请查证后输入").ShowAsync();
            }
            catch (Exception e2)
            {
                 var msgdialog2 = new MessageDialog("您输入的号码不存在，请重新输入").ShowAsync();
            }
        
    }
        /// <summary>
        /// 点击返回跳转到主页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cencel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
