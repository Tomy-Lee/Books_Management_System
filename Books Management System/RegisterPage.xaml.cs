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
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
            string ss = Guid.NewGuid().ToString("N");
            string s = ss.Substring(0, 4);
            yanzheng.Content = s;

        }
        //用户注册按钮的功能实现
        private void register_Click(object sender, RoutedEventArgs e)
        {
            //引用数据库
            var db = App.conn;
            string name = username.Text;
            string pass = password.Text;
            string sure = makesure.Text;
            string ver = verify.Text;
            //判断用户注册时输入的用户名是否为空
            if (name == "")
            {
                var a = new MessageDialog("用户名不能为空！").ShowAsync();
                return;
            }
            if(pass.Length < 6)
            {
                var b = new MessageDialog("密码不能少于六位！").ShowAsync();
                return;
            }
            //判断用户注册时输入的用户密码是否为空
            if (pass == "")
            {
                var b = new MessageDialog("密码不能为空！").ShowAsync();
                return;
            }
            //判断用户注册时两次输入的用户密码是否一致
            if (pass != sure)
            {
                var c = new MessageDialog("您两次输入的密码不一样，请确认！").ShowAsync();
                return;
            }
            //判断用户注册时输入的验证码是否正确
            if (ver != yanzheng.Content.ToString())
            {
                var d = new MessageDialog("您的验证码有误，请重新输入！").ShowAsync();
                verify.Text = "";
                string ss = Guid.NewGuid().ToString("N");
                string s = ss.Substring(0, 4);
                yanzheng.Content = s;
                return;
            }

            int i = 0;
            //判断用户注册时输入的用户名在数据库中是否已经存在
            using (var statement = db.Prepare("SELECT username, password, ison FROM user WHERE username = ?"))
            {
                statement.Bind(1, name);
                if (SQLiteResult.ROW == statement.Step())
                {

                    i = 1;
                }
                if (i == 1)
                {
                    //如果用户输入的用户名已经存在，则弹出提示信息
                    var f = new MessageDialog("该用户名已经被使用，请重新输入！").ShowAsync();
                    return;
                }
                if (i == 0)
                {
                    //当条件无误，在用户数据库中添加新的用户
                    try
                    {
                        using (var user = db.Prepare("INSERT INTO user ( username, password, ison) VALUES (?, ?, ?)"))
                        {
                            user.Bind(1, name);
                            user.Bind(2, pass);
                            user.Bind(3, "off");
                            user.Step();
                        }
                    }
                    catch (Exception ex)
                    { }
                    var m = new MessageDialog("注册成功！").ShowAsync();
                    username.Text = "";
                    password.Text = "";
                    makesure.Text = "";
                    verify.Text = "";
                    //页面跳转回到用户登录界面
                    Frame.Navigate(typeof(LoadPage));
                }
            }


        }
        //取消按钮的功能实现
        private void cancle_Click(object sender, RoutedEventArgs e)
        {
            username.Text = "";
            password.Text = "";
            makesure.Text = "";
            verify.Text = "";
            //页面跳转回到用户登录界面
            Frame.Navigate(typeof(LoadPage));
        }

        private void yanzheng_Click(object sender, RoutedEventArgs e)
        {
            string ss = Guid.NewGuid().ToString("N");
            string s = ss.Substring(0, 4);
            yanzheng.Content = s;
        }
    }
}
