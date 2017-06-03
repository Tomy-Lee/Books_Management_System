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
using Windows.UI.Popups;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Books_Management_System
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ChangePasswordPage : Page
    {
        public ChangePasswordPage()
        {
            this.InitializeComponent();
            this.Getuser();
        }

        //定义函数来获取当前要修改密码的用户
        private void Getuser()
        {
            var db = App.conn;
            using (var statement = db.Prepare("SELECT username, password, ison FROM user WHERE ison = ?"))
            {
                //根据用户的在线状态来获取用户
                statement.Bind(1, "on");
                int i = 0;
                if (SQLiteResult.ROW == statement.Step())
                {

                    i = 1;
                }
                if (i == 1)
                {
                    //将用户的名字显示在界面中
                    name.Text = statement[0].ToString();

                }
            }

        }
        //更改密码确定按钮的功能实现
        private void sure_Click(object sender, RoutedEventArgs e)
        {
            //引用数据库
            var db = App.conn;
            string oldpass = oldpassword.Text;
            string newpass = newpassword.Text;
            string sure = makesure.Text;
            //判断用户输入的新密码是否为空
            if (newpass == "")
            {
                var a = new MessageDialog("所写的新密码不能为空！").ShowAsync();
                return;
            }
            if (newpass.Length < 6)
            {
                var a = new MessageDialog("请输入不少于六位的新密码！").ShowAsync();
                return; 
            }
            if(newpass == oldpass)
            {
                var a = new MessageDialog("密码与原来相同！").ShowAsync();
                return;
            }
            //判断用户两次输入的新密码是否一致
            if (newpass != sure)
            {
                var a = new MessageDialog("您两次输入的新密码不相同，请重新输入！").ShowAsync();
                return;
            }
            //根据用户在线状态找到用户
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
                    string pass = statement[1].ToString();
                    //判断用户输入的旧密码是否与用户数据库中的密码匹配
                    if (pass != oldpass)
                    {
                        var b = new MessageDialog("您原来的密码输入错误，请重新输入！").ShowAsync();
                        return;
                    }
                    //在数据库中更新用户的密码吗
                    using (var user = db.Prepare("UPDATE user SET password=?,ison=? WHERE username=?"))
                    {
                        string ss = statement[0].ToString();
                        user.Bind(1, newpass);
                        user.Bind(2, "on");
                        user.Bind(3, ss);
                        user.Step();
                    }
                    var c = new MessageDialog("密码修改成功！").ShowAsync();
                    Frame.Navigate(typeof(MainPage));
                    oldpassword.Text = "";
                    newpassword.Text = "";
                    makesure.Text = "";

                }
            }
        }
        //取消按钮的功能实现
        private void cancle_Click(object sender, RoutedEventArgs e)
        {
            //跳转回到用户管理主页面
            Frame.Navigate(typeof(MainPage));
            oldpassword.Text = "";
            newpassword.Text = "";
            makesure.Text = "";
        }
    }
}