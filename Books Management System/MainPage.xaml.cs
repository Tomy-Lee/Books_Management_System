using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Books_Management_System.Models;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using SQLitePCL;
using System.Text;
using System.Collections.ObjectModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Books_Management_System
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private book toShare;
        private book AllItems;
        private ObservableCollection<Models.book> allitems = new ObservableCollection<Models.book>();
        ViewModels.bookviewmodel bookmodel { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.bookmodel = new ViewModels.bookviewmodel();
            this.Getuser();
            this.ReadData();
        }
        private void ReadData() {
            var db = App.conn;
            int i = 0;
            using (var statement = db.Prepare("SELECT bookname, writer, totalamount, currentamount FROM book"))
            {
                StringBuilder result = new StringBuilder();
                SQLiteResult r = statement.Step();
                while (SQLiteResult.ROW == r)
                {
                    for (int num = 0; num < statement.DataCount; num += 4)
                    {
                        string ss1 = statement[2].ToString();
                        string ss2 = statement[3].ToString();
                        int sss1 = int.Parse(ss1);
                        int sss2 = int.Parse(ss2);
                       bookmodel.Add("sss",(string)statement[0], (string)statement[1], sss1, sss2);
                    }
                    r = statement.Step();
                }
                if (SQLiteResult.DONE == r)
                {

                }       
                
            }

        }

        /// <summary>
        /// 添加管理员文本框字符串
        /// </summary>
        private void Getuser()
        {
            var db = App.conn;
            using (var statement = db.Prepare("SELECT username, password, ison FROM user WHERE ison = ?"))
            {
                statement.Bind(1, "on");
                int i = 0;
                if (SQLiteResult.ROW == statement.Step())
                {

                    i = 1;
                }
                if (i == 1)
                {
                    textbox.Text += statement[0].ToString();

                }
            }

        }
        /// <summary>
        /// 分别跳转到三个功能页面的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(IncreasePage));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BorrowPage));
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(QueryPage));
        }
        /// <summary>
        /// 此函数跳转到修改管理员密码界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChangePasswordPage));
        }
        /// <summary>
        /// 点击注销，将把是否登录置零，然后跳转到登陆界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string name = "";
            string pass = "";
            var db = App.conn;
            using (var statement = db.Prepare("SELECT username, password, ison FROM user WHERE ison = ?"))
            {
                statement.Bind(1, "on");
                int i = 0;
                if (SQLiteResult.ROW == statement.Step())
                {
                    i = 1;
                    name = (string)statement[0];
                    pass = (string)statement[1];
                }
                if (i == 1)
                {

                    using (var statement1 = db.Prepare("UPDATE user SET password=?, ison = ? WHERE username = ?"))
                    {
                        statement1.Bind(1, pass);
                        statement1.Bind(2, "off");
                        statement1.Bind(3, name);
                        statement1.Step();
                    }
                }
            }
            textbox.Text = "管理员：";
            Frame.Navigate(typeof(LoadPage));
        }

        /// <summary>
        /// 跳转到项目详细信息页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            bookmodel.SelectedItem = (Models.book)e.ClickedItem;
            Frame.Navigate(typeof(MainToQueryPage), bookmodel);
        }
        /// <summary>
        /// 更新磁贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button222_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(File.ReadAllText("tile.xml", Encoding.UTF8));
            foreach (var item in bookmodel.AllItems)
            {
                var elements = XmlDoc.GetElementsByTagName("text");
                for (int i = 0; i < elements.Length;)
                {
                    var Title = elements[i++] as Windows.Data.Xml.Dom.XmlElement;
                    Title.InnerText = item.bookname;
                    var Detail = elements[i++] as Windows.Data.Xml.Dom.XmlElement;
                    Detail.InnerText = item.writer;
                }
                var tileNotification = new TileNotification(XmlDoc);
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.Update(tileNotification);
            }
        }


    }
}
