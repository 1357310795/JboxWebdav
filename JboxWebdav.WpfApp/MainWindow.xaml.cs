using Hardcodet.Wpf.TaskbarNotification;
using JboxWebdav.WpfApp.Helpers;
using JboxWebdav.WpfApp.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JboxWebdav.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DriveTypes = new List<DriveTypeItem>();
            DriveTypes.Add(new DriveTypeItem("网络磁盘", 1));
            DriveTypes.Add(new DriveTypeItem("本地磁盘", 0));
            DriveType = DriveTypes[0];
            this.DataContext = this;
        }

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

        private bool isRcloneRunning = false;
        public bool IsRcloneRunning
        {
            get { return isRcloneRunning; }
            set
            {
                isRcloneRunning = value;
                this.RaisePropertyChanged("IsRcloneRunning");
            }
        }

        private string driveName;
        public string DriveName
        {
            get { return driveName; }
            set
            {
                driveName = value;
                this.RaisePropertyChanged("DriveName");
            }
        }

        private string rcloneMessage;
        public string RcloneMessage
        {
            get { return rcloneMessage; }
            set
            {
                rcloneMessage = value;
                this.RaisePropertyChanged("RcloneMessage");
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

        private List<DriveTypeItem> driveTypes;
        public List<DriveTypeItem> DriveTypes
        {
            get { return driveTypes; }
            set
            {
                driveTypes = value;
                this.RaisePropertyChanged("DriveTypes");
            }
        }

        private List<DriveSymbolItem> driveSymbols;
        public List<DriveSymbolItem> DriveSymbols
        {
            get { return driveSymbols; }
            set
            {
                driveSymbols = value;
                this.RaisePropertyChanged("DriveSymbols");
            }
        }

        private DriveTypeItem driveType;
        public DriveTypeItem DriveType
        {
            get { return driveType; }
            set
            {
                driveType = value;
                this.RaisePropertyChanged("DriveType");
            }
        }

        private DriveSymbolItem driveSymbol;
        public DriveSymbolItem DriveSymbol
        {
            get { return driveSymbol; }
            set
            {
                driveSymbol = value;
                this.RaisePropertyChanged("DriveSymbol");
            }
        }
        #endregion

        private void GetFreeDrives()
        {
            DriveSymbols = new List<DriveSymbolItem>();
            var drives = DriveInfo.GetDrives();
            bool[] t =new bool[26];
            foreach (var d in drives)
            {
                var alpha = d.Name.ToUpper()[0];
                if (alpha >= 'A' && alpha <= 'Z')
                    t[(int)alpha - 65] = true;
            }
            for (int i = 25; i >= 0; i--)
            {
                if (!t[i])
                {
                    var item = new DriveSymbolItem(((char)(i + 65)).ToString());
                    DriveSymbols.Add(item);
                    if (DriveSymbol?.Id == item.Id)
                        DriveSymbol = item;
                }
            }
            if (DriveSymbol == null)
                DriveSymbol = DriveSymbols[0];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetFreeDrives();
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
            if (IsRcloneRunning)
            {
                ButtonRcloneStop_Click(null, new RoutedEventArgs());
            }
            SaveSettings();
            App.Current.Shutdown();
        }

        private void ReadSettings()
        {
            IpAddress = IniHelper.GetKeyValue("WpfApp", "IpAddress", "http://127.0.0.1:11111/", IniHelper.inipath);
            DriveName = IniHelper.GetKeyValue("WpfApp", "DriveName", "MyJbox", IniHelper.inipath);
            int type = int.Parse(IniHelper.GetKeyValue("WpfApp", "DriveType", "1", IniHelper.inipath));
            DriveType = DriveTypes.First(x => x.Id == type);
            string symbol = IniHelper.GetKeyValue("WpfApp", "DriveSymbol", "Z", IniHelper.inipath);
            DriveSymbol = DriveSymbols.First(x => x.Id == symbol);
        }

        private void SaveSettings()
        {
            IniHelper.SetKeyValue("WpfApp", "IpAddress", IpAddress, IniHelper.inipath);
            IniHelper.SetKeyValue("WpfApp", "DriveName", DriveName, IniHelper.inipath);
            if (DriveType != null)
                IniHelper.SetKeyValue("WpfApp", "DriveType", DriveType.Id.ToString(), IniHelper.inipath);
            if (DriveSymbol != null)
                IniHelper.SetKeyValue("WpfApp", "DriveSymbol", DriveSymbol.Id.ToString(), IniHelper.inipath);
        }

        Process RcloneProcess, WebdavProcess;
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
            WebdavHttpListener.cancellationTokenSource.Cancel();
            WebdavHttpListener.httpListener.Stop();
            IsWebdavRunning = false;
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

        private void ButtonRcloneConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo(basepath + @"Data\rclone-v1.58.1-windows-amd64\rclone.exe");
                psi.Arguments = "config";
                var p = new Process();
                p.StartInfo = psi;
                p.Start();
            }
            catch (Exception ex)
            {
                RcloneMessage = ex.Message;
            }
        }

        private void ButtonRcloneStop_Click(object sender, RoutedEventArgs e)
        {
            if (RcloneProcess == null || RcloneProcess.HasExited)
            {
                RcloneMessage = "Rclone已退出。如果Rclone还在运行，请使用任务管理器结束Rclone";
                return;
            }
            try
            {
                RcloneProcess.Kill();
                Thread.Sleep(100);
                if (!RcloneProcess.HasExited)
                {
                    RcloneProcess.Kill();
                }
            }
            catch (Exception ex)
            {
                RcloneMessage = ex.Message;
            }
        }

        private void ButtonRcloneStart_Click(object sender, RoutedEventArgs e)
        {
            if (DriveSymbol == null)
            {
                RcloneMessage = "请先指定盘符";
                return;
            }
            if (RcloneProcess != null && !RcloneProcess.HasExited)
            {
                RcloneMessage = "Rclone正在运行！";
                return;
            }
            try
            {
                var psi = new ProcessStartInfo(basepath + @"Data\rclone-v1.58.1-windows-amd64\rclone.exe");
                psi.Arguments = $"mount jbox: {DriveSymbol.Id}: --vfs-cache-mode full {(DriveType.Id == 1 ? "--network-mode" : "")} --volname {DriveName}";
                psi.CreateNoWindow = true;
                psi.RedirectStandardError = true;
                psi.RedirectStandardInput = true;
                RcloneProcess = new Process();
                RcloneProcess.Exited += RcloneProcess_Exited;
                RcloneProcess.ErrorDataReceived += RcloneProcess_ErrorDataReceived;
                RcloneProcess.StartInfo = psi;
                RcloneProcess.Start();
                IsRcloneRunning = true;
            }
            catch (Exception ex)
            {
                RcloneMessage = ex.Message;
                IsRcloneRunning = false;
            }
        }

        private void RcloneProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var err = e.Data ?? "";
            RcloneMessage = err;
        }

        private void RcloneProcess_Exited(object? sender, EventArgs e)
        {
            IsRcloneRunning = false;
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

    public class DriveTypeItem
    {
        public DriveTypeItem(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class DriveSymbolItem
    {
        public DriveSymbolItem(string desc, string id)
        {
            Desc = desc;
            Id = id;
        }

        public DriveSymbolItem(string id)
        {
            Desc = id + ":";
            Id = id;
        }

        public string Desc { get; set; }
        public string Id { get; set; }
    }
}
