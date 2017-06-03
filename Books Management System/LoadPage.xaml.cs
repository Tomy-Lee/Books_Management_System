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

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Books_Management_System
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoadPage : Page
    {
        public LoadPage()
        {
            this.InitializeComponent();
            this.InitUser();

        }
        /// <summary>
        /// 重新登陆时清除数据
        /// </summary>
        private void InitUser()
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
        }
        //定义登录按钮的功能
        private void Loadin_Click(object sender, RoutedEventArgs e)
        {
            //引用数据库
            var db = App.conn;
            string name = username.Text;
            string pass = password.Text;
            //判断用户登录时用户名是否为空
            if (name == "")
            {
                var a = new MessageDialog("用户名不能为空！").ShowAsync();
                return;
            }
            //判断用户登录时密码是否为空
            if (pass == "")
            {
                var b = new MessageDialog("密码不能为空！").ShowAsync();
                return;
            }
            int i = 0;
            //在用户数据库中寻找输入的用户名是否存在
            using (var statement = db.Prepare("SELECT username, password, ison FROM user WHERE username = ?"))
            {
                statement.Bind(1, name);
                if (SQLiteResult.ROW == statement.Step())
                {
                    i = 1;
                }
                if (i == 1)
                {
                    //当用户名存在时，判断其输入的密码是否与用户数据库中的密码相匹配
                    int j = 0;
                    using (var statement1 = db.Prepare("SELECT username, password, ison FROM user WHERE password = ?"))
                    {
                        statement1.Bind(1, pass);
                        if (SQLiteResult.ROW == statement1.Step())
                        {
                            j = 1;
                        }
                        if (j == 1)
                        {
                            //当用户以及密码无误，将该用户的状态设为在线on
                            using (var user = db.Prepare("UPDATE user SET password=?,ison=? WHERE username=?"))
                            {
                                user.Bind(1, password.Text);
                                user.Bind(2, "on");
                                user.Bind(3, name);
                                user.Step();
                            }
                            //跳转页面到管理主页面
                            Frame.Navigate(typeof(MainPage));
                            username.Text = "";
                            password.Text = "";
                        }
                        if (j == 0)
                        {
                            //当用户密码与输入的账户不匹配时，弹出错误信息
                            var c = new MessageDialog("密码错误，请重新输入密码！").ShowAsync();
                        }
                    }
                }
                if (i == 0)
                {
                    //当用户输入的用户名不存在时，弹出错误信息
                    var g = new MessageDialog("该用户不存在！").ShowAsync();

                }
            }
        }
        //实现注册按钮的功能
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            username.Text = "";
            password.Text = "";
            //跳转到用户注册界面
            Frame.Navigate(typeof(RegisterPage));
        }

    }

}
