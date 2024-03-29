﻿using Hardcodet.Wpf.TaskbarNotification;
using JboxWebdav.Server.Jbox;
using JboxWebdav.WpfApp.Extensions;
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
            AccessModes = new List<AccessModeDto>();
            AccessModes.Add(new AccessModeDto("完全模式", AccessModeEnum.Full, 1023));
            AccessModes.Add(new AccessModeDto("只读模式", AccessModeEnum.ReadOnly, 5));
            AccessModes.Add(new AccessModeDto("读写模式", AccessModeEnum.ReadWrite, 39));
            AccessModes.Add(new AccessModeDto("防止删除模式", AccessModeEnum.NoDelete, 1023 - 64));
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

        #region IpAddress
        private void ReadSettings()
        {
            IpAddress = IniHelper.GetKeyValue("WpfApp", "IpAddress", "http://127.0.0.1:65472/", IniHelper.inipath);
            JboxPublicEnabled = bool.Parse(IniHelper.GetKeyValue("WpfApp", "JboxPublicEnabled", "true", IniHelper.inipath));
            JboxSharedEnabled = bool.Parse(IniHelper.GetKeyValue("WpfApp", "JboxSharedEnabled", "true", IniHelper.inipath));
            var accesstype = IniHelper.GetKeyValue("WpfApp", "AccessMode", "Full", IniHelper.inipath).ToEnum<AccessModeEnum>();
            AccessMode = AccessModes.FirstOrDefault(x => x.Type == accesstype);
        }

        private void SaveSettings()
        {
            IniHelper.SetKeyValue("WpfApp", "IpAddress", IpAddress, IniHelper.inipath);
            IniHelper.SetKeyValue("WpfApp", "JboxPublicEnabled", JboxPublicEnabled.ToString(), IniHelper.inipath);
            IniHelper.SetKeyValue("WpfApp", "JboxSharedEnabled", JboxSharedEnabled.ToString(), IniHelper.inipath);
            IniHelper.SetKeyValue("WpfApp", "AccessMode", AccessMode.Type.ToString(), IniHelper.inipath);
        }
        #endregion

        #region Settings Fields

        public bool? JboxPublicEnabled
        {
            get { return Config.PublicEnabled; }
            set
            {
                Config.PublicEnabled = value.Value;
                this.RaisePropertyChanged("JboxPublicEnabled");
            }
        }

        public bool? JboxSharedEnabled
        {
            get { return Config.SharedEnabled; }
            set
            {
                Config.SharedEnabled = value.Value;
                this.RaisePropertyChanged("JboxSharedEnabled");
            }
        }

        private List<AccessModeDto> accessModes;

        public List<AccessModeDto> AccessModes
        {
            get { return accessModes; }
            set
            {
                accessModes = value;
                this.RaisePropertyChanged("AccessModes");
            }
        }
        private AccessModeDto accessMode;

        public AccessModeDto AccessMode
        {
            get { return accessMode; }
            set
            {
                accessMode = value;
                Config.AccessMode = value.AccessCode;
                this.RaisePropertyChanged("AccessMode");
            }
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
