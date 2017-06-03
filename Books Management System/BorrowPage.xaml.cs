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
using SQLitePCL;
using System.Text;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Books_Management_System
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BorrowPage : Page
    {
        public BorrowPage()
        {
            this.InitializeComponent();
        }

        //更新当前库存，加1或者减1
        public void update(long cur, string bookname0)
        {
            var db = App.conn;

            try
            {
                using (var statement = db.Prepare("UPDATE book SET currentamount = ? WHERE bookname=?"))
                {
                    statement.Bind(1, cur);
                    statement.Bind(2, bookname0);

                    statement.Step();
                }
            }
            catch (Exception ex)
            { }
        }
        //插入借书的数据，借书人ID，借书人，书的名字
        public void insert(Int64 Id, string borrowname, string bookname)
        {
            var db = App.conn;

            try
            {
                using (var statement = db.Prepare("INSERT INTO borrow(id,name, book) VALUES(?, ?, ?)"))
                {
                    statement.Bind(1, Id);
                    statement.Bind(2, borrowname);
                    statement.Bind(3, bookname);

                    statement.Step();
                }
            }
            catch (Exception ex)
            { }

        }

        //删除借书的数据，借书人ID，借书人，书的名字
        public void remove(Int64 Id, string borrowname, string bookname)
        {
            var db = App.conn;

            try
            {
                using (var statement = db.Prepare("DELETE FROM borrow WHERE id = ? AND book = ?"))
                {
                    statement.Bind(1, Id);
                    statement.Bind(2, bookname);
                    statement.Step();
                }
            }
            catch (Exception ex)
            { }
        }

        //归还确定按钮
        private void comfirm_click(object sender, RoutedEventArgs e)
        {
            string a = "";//输入为空时，输出的字符串
            int flag = 0, flag1 = 0;
            long current = 0; //用于储存当前库存
            string info = bookname1.Text.ToString();
            string info1 = returnman1.Text.ToString();
            string info2 = returnmanID1.Text.ToString();
            if (info == "") a += "书名为空!\n";
            if (info1 == "") a += "归还人名为空!\n";
            if (info2 == "") a += "归还人账号为空!";

            //当3个框输入都不为空时
            if (info != "" && info1 != "" && info2 != "")
            {
                
                   long ID = long.Parse(info2);//用于储存借书人的ID
                var d = App.conn;
                //先在borrow数据库中，通过id和book来寻找有没有这条借书记录
                using (var statement = d.Prepare("SELECT id, name, book FROM borrow WHERE book = (?) AND id = ?"))
                {
                    statement.Bind(2, ID);
                    statement.Bind(1, info);
                    //若有
                    while (SQLiteResult.ROW == statement.Step())
                    {
                        flag = 1;
                        //通过bookname，来book数据库中寻找book的资料，并把当前库存取下来
                        var db = App.conn;
                        using (var statement1 = db.Prepare("SELECT bookname, currentamount FROM book WHERE bookname = (?)"))
                        {

                            statement1.Bind(1, info);
                            //能找到这本书
                            while (SQLiteResult.ROW == statement1.Step())
                            {
                                current = (Int64)statement1[1];//把当前库存赋值给current
                                update(current + 1, info);//库存+1
                                remove(ID, info1, info);//移除借书记录
                                var m = new Windows.UI.Popups.MessageDialog("成功").ShowAsync();
                                Frame.Navigate(typeof(MainPage));
                                break;
                            }
                        }
                        break;
                    }
                    //若没有这条借书记录
                    if (flag == 0)
                    {
                        var db = App.conn;
                        //通过bookname，来book数据库中寻找book的资料
                        using (var statement1 = db.Prepare("SELECT bookname FROM book WHERE bookname = (?)"))
                        {

                            statement1.Bind(1, info);
                            //若有这本书，则为用户没有借过这本书
                            while (SQLiteResult.ROW == statement1.Step())
                            {
                                flag1 = 1;
                                var m = new Windows.UI.Popups.MessageDialog("此账号没有借过这本书").ShowAsync();
                                break;
                            }
                            //没有这本书
                            if (flag1 == 0)
                            {
                                var m = new Windows.UI.Popups.MessageDialog("图书馆没有这本书").ShowAsync();
                            }
                        }
                    }
                }
            }
            //若3个框的输入有为空的，则输出a
            else
            {
                var n = new Windows.UI.Popups.MessageDialog(a).ShowAsync();
            }

        }
        //归还取消按钮
        private void cencel_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        //借出确定按钮
        private void comfirm1_click(object sender, RoutedEventArgs e)
        {
            string a = "";//输入为空时，输出的字符串
            int flag = 0, flag1 = 0;
            long current = 0; //用于储存当前库存
            int count = 0;//用于记录借书人借了多少本不同的书
            string info = bookname3.Text.ToString();
            string info1 = borrowman3.Text.ToString();
            string info2 = borrowmanID1.Text.ToString();
            if (info == "") a += "书名为空!\n";
            if (info1 == "") a += "借书人名为空!\n";
            if (info2 == "") a += "借书人账号为空！";

            //当3个框输入都不为空时
            if (info != "" && info1 != "" && info2 != "")
            {
                long ID = long.Parse(info2);//用于储存借书人的ID
                //先在borrow数据库中，通过id来寻找有没有这条借书记录，并且总共有几条记录
                var b = App.conn;
                using (var statement = b.Prepare("SELECT id, name, book FROM borrow WHERE id = ?"))
                {
                    statement.Bind(1, ID);
                    while (SQLiteResult.ROW == statement.Step())
                    {
                        count++;//把记录数存到count中
                    }
                }
                //一个人最多只能同时借5本不同的书
                if (count >= 5)
                {
                    var m = new Windows.UI.Popups.MessageDialog("此账号已经借过五本书，请归还后再借书").ShowAsync();
                }
                else
                {
                    //在borrow数据库中，通过id和book来寻找有没有这条借书记录
                    var d = App.conn;
                    using (var statement = d.Prepare("SELECT id, name, book FROM borrow WHERE book = (?) AND id = ?"))
                    {
                        statement.Bind(2, ID);
                        statement.Bind(1, info);

                        while (SQLiteResult.ROW == statement.Step())
                        {
                            flag = 1;

                            break;
                        }
                        //若没有，则可以借书
                        if (flag == 0)
                        {
                            //通过bookname，来book数据库中寻找book的资料
                            var db = App.conn;
                            using (var statement1 = db.Prepare("SELECT bookname, currentamount FROM book WHERE bookname = (?)"))
                            {
                                statement1.Bind(1, info);
                                //若有，则进行借书操作
                                while (SQLiteResult.ROW == statement1.Step())
                                {
                                    current = (Int64)statement1[1];//储存当前库存
                                    if (current <= 0)
                                    {
                                        flag1 = 2;
                                        break;
                                    }
                                    update(current - 1, info);//当前库存-1
                                    insert(ID, info1, info);//插入一条借书数据
                                    var m = new Windows.UI.Popups.MessageDialog("成功！").ShowAsync();
                                    flag1 = 1;
                                    Frame.Navigate(typeof(MainPage));
                                    break;
                                }
                                //若没有，则显示该书没有
                                if (flag1 == 0)
                                {
                                    var m = new Windows.UI.Popups.MessageDialog("图书馆没有这本书").ShowAsync();
                                }
                                //库存不足
                                if (flag1 == 2)
                                {
                                    var m = new Windows.UI.Popups.MessageDialog("库存不足！").ShowAsync();
                                }
                            }
                        }
                        //若已有该书的借书记录，则不能再借
                        if (flag == 1)
                        {
                            var n = new Windows.UI.Popups.MessageDialog("此账号已经借过这本书").ShowAsync();
                        }

                    }
                }
            }
            //若3个框的输入有为空的，则输出a
            else
            {
                var n = new Windows.UI.Popups.MessageDialog(a).ShowAsync();
            }

        }
        private void cencel1_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
