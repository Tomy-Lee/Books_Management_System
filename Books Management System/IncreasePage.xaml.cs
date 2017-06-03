using SQLitePCL;
using System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Books_Management_System
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class IncreasePage : Page
    {
        public IncreasePage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }
        public string writer_t;
        /// <summary>
        /// 记录search到的书目的作者
        /// </summary>
        public long c;
        /// <summary>
        /// 记录搜寻到的数目的当前库存
        /// </summary>
        public bool dele_con;
        /// <summary>
        /// 记录书库中是否存在待删除的查询到某数目
        /// </summary>
        public long tt;
        /// <summary>
        /// 记录搜寻到的数目的总库存
        /// </summary>
        /// 
        private ViewModels.bookviewmodel bookmodel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bookmodel = ((ViewModels.bookviewmodel)e.Parameter);

            Frame root = Window.Current.Content as Frame;

            if (root.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }
        /// <summary>
        /// 取消则返回主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cencel_Click(object sender, RoutedEventArgs e)
        {
            Frame root = Window.Current.Content as Frame;
            root.Navigate(typeof(MainPage));
        }

        private void cencel1_Click(object sender, RoutedEventArgs e)
        {
            Frame root = Window.Current.Content as Frame;
            root.Navigate(typeof(MainPage));
        }
        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="title"></param>
        /// <param name="total"></param>
        /// <param name="cu"></param>
        public void update(string title, long total, long cu)
        {
            var db = App.conn;

            try
            {
                using (var statement = db.Prepare("UPDATE book SET totalamount = ?, currentamount = ?  WHERE bookname = (?)"))
                {
                    statement.Bind(1, total);
                    statement.Bind(2, cu);
                    statement.Bind(3, title);
                    statement.Step();
                }
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 插入需增加的书目入库
        /// </summary>
        /// <param name="title"></param>
        /// <param name="details"></param>
        /// <param name="amount"></param>
        /// <param name="c_amount"></param>
        public void insert(string title, string details, int amount, int c_amount)
        {
            var db = App.conn;

            try
            {
                using (var statement = db.Prepare("INSERT INTO book(bookname, writer, totalamount, currentamount) VALUES(?, ?, ?, ?)"))
                {
                    statement.Bind(1, title);
                    statement.Bind(2, details);
                    statement.Bind(3, amount);
                    statement.Bind(4, c_amount);
                    statement.Step();
                }
            }
            catch (Exception ex)
            { }

        }
        /// <summary>
        /// 增加时根据书名搜索书库中书目
        /// </summary>
        /// <param name="info"></param>
        public void search_inf(string info)
        {
            var db = App.conn;
            bool ok = false;
            writer_t = "";
            using (var statement = db.Prepare("SELECT bookname, writer ,totalamount, currentamount FROM book WHERE bookname = (?)"))
            {
                statement.Bind(1, info);
                while (SQLiteResult.ROW == statement.Step())
                {
                    var inf = (Int64)statement[2];
                    c = (Int64)statement[3];
                    writer_t = (string)statement[1];

                    writer_text.Text = writer_t;

                    returnman1.Text = inf.ToString();
                    // returnman1.Text = writer_t;
                    ok = true;
                }
                if (!ok)
                {

                    returnman1.Text = "0";
                }

            }
        }
        /// <summary>
        /// 删除时根据书名搜查到的书目信息
        /// </summary>
        /// <param name="info"></param>
        public void dele_search_inf(string info)
        {
            var db = App.conn;
            bool ok = false;
            using (var statement = db.Prepare("SELECT bookname, totalamount, currentamount FROM book WHERE bookname = (?)"))
            {
                statement.Bind(1, info);
                while (SQLiteResult.ROW == statement.Step())
                {
                    tt = (Int64)statement[1];
                    var inf = (Int64)statement[2];
                    borrowman3.Text = inf.ToString();
                    ok = true;
                }
                if (!ok)
                {
                    var i = new MessageDialog("图书馆没有这本书").ShowAsync();
                }
                dele_con = ok;
            }
        }
        /// <summary>
        /// 从书库中删除书目
        /// </summary>
        /// <param name="name"></param>
        public void delete(string name)
        {
            var db = App.conn;

            try
            {
                using (var statement = db.Prepare("DELETE FROM book WHERE bookname = ?"))
                {
                    statement.Bind(1, name);
                    statement.Step();
                }
            }
            catch (Exception ex)
            { }

        }
        /// <summary>
        /// 增加查询库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkbutton2_Click(object sender, RoutedEventArgs e)
        {
            string info = bookname1.Text.ToString();
            if (info != "")
            {
                search_inf(info);
            }
            else { }
        }
        /// <summary>
        /// 确定增加书目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comfirm_Click(object sender, RoutedEventArgs e)
        {
            string info = bookname1.Text.ToString();
            if (info != "")
            {
                search_inf(info);
            }
            else { }

            string bname = bookname1.Text.ToString();
            string wri = writer_text.Text.ToString();

            if (bname != "" && wri != "")
            {
                int number = 0;
                if (returnmanID1.Text.ToString() != "")
                    int.TryParse(returnmanID1.Text.ToString(), out number);
                else
                {
                    var i = new MessageDialog("请输入您要添加的数目").ShowAsync();
                    return;
                }

                int t = 0;
                if (returnman1.Text.ToString() != "")
                    int.TryParse(returnman1.Text.ToString(), out t);

                if (t == 0)
                {
                    insert(bname, wri, number, number);
                    returnman1.Text = (t + number).ToString();
                    writer_text.Text = "";
                    returnmanID1.Text = "";
                    var i = new MessageDialog("成功").ShowAsync();
                    Frame root = Window.Current.Content as Frame;
                    root.Navigate(typeof(MainPage));

                }
                else
                {
                    if (wri == writer_t)
                    {
                        update(bname, t + number, c + number);
                        returnman1.Text = (t + number).ToString();
                        writer_text.Text = "";
                        returnmanID1.Text = "";
                        var i = new MessageDialog("成功！").ShowAsync();
                        Frame root = Window.Current.Content as Frame;
                        root.Navigate(typeof(MainPage));

                    }
                    else
                    {
                        var i = new MessageDialog("您不能更改作者！").ShowAsync();
                        return;
                    }
                }
            }
            else
            {
                var i = new MessageDialog("书名或作者为空！").ShowAsync();
                return;
            }
        }
        /// <summary>
        /// 删除书目查询库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkbutton1_Click(object sender, RoutedEventArgs e)
        {
            string info = bookname3.Text.ToString();
            if (info != "")
            {
                dele_search_inf(info);
            }
            else { }
        }
        /// <summary>
        /// 确认删除书目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comfirm1_Click(object sender, RoutedEventArgs e)
        {
            string info = bookname3.Text.ToString();
            if (info != "")
            {
                dele_search_inf(info);
            }
            else
            {
                var i = new MessageDialog("书名为空！").ShowAsync();
                return;
            }

            if (dele_con)
            {
                string bname = bookname3.Text.ToString();
                int t = 0;
                if (borrowman3.Text.ToString() != "")
                    int.TryParse(borrowman3.Text.ToString(), out t);

                int number = 0;
                if (borrowmanID1.Text.ToString() != "")
                    int.TryParse(borrowmanID1.Text.ToString(), out number);
                else
                {
                    var i = new MessageDialog("请输入您要删除的数目！").ShowAsync();
                    return;
                }


                if (bname != "")
                {
                    if (t == 0)
                    {
                        var i = new MessageDialog("没有库存！").ShowAsync();
                        borrowmanID1.Text = "";
                    }
                    else
                    {
                        if (number <= t)
                        {
                            if (tt - number > 0)
                            {
                                update(bname, tt - number, t - number);
                                borrowman3.Text = (t - number).ToString();
                                borrowmanID1.Text = "";
                                var i = new MessageDialog("成功！").ShowAsync();
                                Frame root = Window.Current.Content as Frame;
                                root.Navigate(typeof(MainPage));
                            }
                            else if (tt - number == 0)
                            {
                                borrowman3.Text = "";
                                borrowmanID1.Text = "";
                                delete(bname);
                                var i = new MessageDialog("成功！").ShowAsync();
                                Frame root = Window.Current.Content as Frame;
                                root.Navigate(typeof(MainPage));
                            }

                        }
                        else
                        {
                            var i = new MessageDialog("您要删除的数目超过了库存！").ShowAsync();
                        }
                    }
                }
            }
        }
    }
}