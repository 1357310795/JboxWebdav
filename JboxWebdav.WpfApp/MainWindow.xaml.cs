using Hardcodet.Wpf.TaskbarNotification;
using JboxWebdav.WpfApp.Helpers;
using JboxWebdav.WpfApp.Models;
using JboxWebdav.WpfApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Interop;

namespace JboxWebdav.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        #endregion

        #region Fields
        private bool isWebdavRunning = false;
        public bool IsWebdavRunning
        {
            get { return isWebdavRunning; }
            set
            {
                isWebdavRunning = value;
                this.RaisePropertyChanged("IsWebdavRunning");
            }
        }

        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                ipAddress = value;
                this.RaisePropertyChanged("IpAddress");
            }
        }

        private string webdavMessage;
        public string WebdavMessage
        {
            get { return webdavMessage; }
            set
            {
                webdavMessage = value;
                this.RaisePropertyChanged("WebdavMessage");
            }
        }

        #endregion

        #region Window Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadSettings();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                var myTaskbarIcon = (TaskbarIcon)FindResource("Taskbar");
                myTaskbarIcon.ShowBalloonTip("程序将在后台运行", "若要退出：右击托盘区图标点击退出程序", BalloonIcon.Info);
                this.Hide();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (IsWebdavRunning)
            {
                ButtonWebdavStop_Click(null, new RoutedEventArgs());
            }
            SaveSettings();
            App.Current.Shutdown();
        }
        #endregion

        #region Settings
        private void ReadSettings()
        {
            IpAddress = IniHelper.GetKeyValue("WpfApp", "IpAddress", "http://127.0.0.1:65472/", IniHelper.inipath);
        }

        private void SaveSettings()
        {
            IniHelper.SetKeyValue("WpfApp", "IpAddress", IpAddress, IniHelper.inipath);
        }
        #endregion

        #region Webdav Service
        private string basepath => System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private void ButtonWebdavLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo("explorer");
                psi.Arguments = basepath + "Logs";
                var p = new Process();
                p.StartInfo = psi;
                p.Start();
            }
            catch (Exception ex)
            {
                WebdavMessage = ex.Message;
            }
        }

        private void ButtonWebdavStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebdavHttpListener.cancellationTokenSource.Cancel();
                WebdavHttpListener.httpListener.Stop();
                IsWebdavRunning = false;
            }
            catch (Exception ex)
            {
                WebdavMessage = ex.Message;
            }
        }

        private void ButtonWebdavStart_Click(object sender, RoutedEventArgs e)
        {
            if (IsWebdavRunning)
            {
                WebdavMessage = "Webdav服务正在运行！";
                return;
            }
            try
            {
                WebdavHttpListener.Main(IpAddress);
                IsWebdavRunning = true;
            }
            catch (Exception ex)
            {
                WebdavMessage = ex.Message;
                IsWebdavRunning = false;
            }
        }
        #endregion

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
