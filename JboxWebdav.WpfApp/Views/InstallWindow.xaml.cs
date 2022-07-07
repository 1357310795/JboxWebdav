using JboxWebdav.WpfApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JboxWebdav.WpfApp.Views
{
    /// <summary>
    /// InstallWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InstallWindow : Window, INotifyPropertyChanged
    {
        public InstallWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        private string info;
        public string Info
        {
            get { return info; }
            set
            {
                info = value;
                this.RaisePropertyChanged("Info");
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(Go);
            t.Start();
        }

        private void Fail()
        {
            this.Dispatcher.Invoke(() => {
                buttonclose.Visibility = Visibility.Visible;
                textstate.Text = "安装失败";
                progressbar1.IsIndeterminate = false;
                icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Close;
            });
        }

        private void Success(string str)
        {
            this.Dispatcher.Invoke(() => {
                buttonclose.Visibility = Visibility.Visible;
                textstate.Text = str;
                buttonclose.Content = "启动程序";
                progressbar1.IsIndeterminate = false;
                icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Check;
                IniHelper.SetKeyValue("WpfApp", "firstrun", "false", IniHelper.inipath);
            });
        }

        private void Go()
        {
            var basepath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            try
            {
                Info = "安装WinFsp";
                ProcessStartInfo psi = new ProcessStartInfo("msiexec");
                psi.Arguments = "/i " + basepath + @"Data\winfsp-1.11.22176.msi";
                psi.WorkingDirectory = basepath + @"Data\";
                Process p = new Process();
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                Fail();
                return;
            }

            try
            {
                Info = "配置Rclone";
                ProcessStartInfo psi = new ProcessStartInfo(basepath + @"Data\rclone-v1.58.1-windows-amd64\rclone.exe");
                psi.Arguments = "config create jbox webdav url=http://127.0.0.1:65472/ vendor=other --non-interactive";
                Process p = new Process();
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                Fail();
                return;
            }

            Info = "任务全部完成";
            Success("安装完成");
        }

        private void buttonclose_Click(object sender, RoutedEventArgs e)
        {
            if (textstate.Text == "安装完成")
            {
                var w1 = new LoginWindow();
                App.Current.MainWindow = w1;
                w1.Show();
                this.Close();
            }
            else
                Application.Current.Shutdown();
        }

        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
    }
}
