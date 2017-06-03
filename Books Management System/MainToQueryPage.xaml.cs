using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Books_Management_System
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainToQueryPage : Page
    {
        public MainToQueryPage()
        {
            this.InitializeComponent();
        }
        ViewModels.bookviewmodel ViewModel { get; set; }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        { 
            ViewModel = ((ViewModels.bookviewmodel)e.Parameter);
            if (ViewModel.SelectedItem != null)
            {
                bookname1.Text = ViewModel.SelectedItem.bookname;
                this.Getbook();
            }
        }
        /// <summary>
        /// 获取借书的信息，但是感觉会有问题，注意一下！！！
        /// </summary>
        private void Getbook()
        {
            string ss = bookname1.Text.ToString();
          //  var Maaaa11 = new MessageDialog(ss).ShowAsync(); 测试
            var db = App.conn;
            int i = 0;
            using (var statement = db.Prepare("SELECT bookname, writer, totalamount, currentamount FROM book WHERE bookname = ?"))
            {
                statement.Bind(1, ss);
                if (SQLiteResult.ROW == statement.Step())
                {
                    i = 1;
                }
                if(i == 1)
                {
                    bookname1.Text = statement[0].ToString();
                    total.Text = statement[2].ToString();
                   // var Maaaa = new MessageDialog(statement[1].ToString()).ShowAsync(); 测试
                    current.Text = statement[3].ToString();
                }
            }
            try {
                using (var statement1 = db.Prepare("SELECT id, name, book FROM borrow WHERE book = ?"))
                {
                    /*statement1.Bind(1, ss);
                    while (SQLiteResult.ROW == statement1.Step())
                    {
                        borrower.Text += statement1[0].ToString() + " ";
                        borrower.Text += statement1[1].ToString() + " ";

                    }*/
                    statement1.Bind(1, ss);
                    StringBuilder result = new StringBuilder();
                    SQLiteResult r = statement1.Step();
                    while (SQLiteResult.ROW == r)
                    {
                        for (int num = 0; num < statement1.DataCount; num += 3)
                        {
                            borrower.Text += statement1[0].ToString() + "\n";
                            borrower1.Text += statement1[1].ToString() + "\n";
                        }
                        r = statement1.Step();
                    }
                    if (SQLiteResult.DONE == r)
                    {
                    }
                }
            }
            catch (Exception e1)
            {
                var ma = new MessageDialog(e1.ToString()).ShowAsync();
            }
            
        }
        /// <summary>
        /// 点击返回跳转到主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cencel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveTodoItem();
            Frame.Navigate(typeof(MainPage));
        }

    }
   
}
