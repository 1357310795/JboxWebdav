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
            DriveTypes = new List<DriveTypeItem>();
            DriveTypes.Add(new DriveTypeItem("网络磁盘", 1));
            DriveTypes.Add(new DriveTypeItem("本地磁盘", 0));
            DriveType = DriveTypes[0];
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

        #region Window Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetFreeDrives();
            ReadSettings();
            ApplyHook();
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
        #endregion

        #region Settings
        private void ReadSettings()
        {
            IpAddress = IniHelper.GetKeyValue("WpfApp", "IpAddress", "http://127.0.0.1:65472/", IniHelper.inipath);
            DriveName = IniHelper.GetKeyValue("WpfApp", "DriveName", "MyJbox", IniHelper.inipath);
            int type = int.Parse(IniHelper.GetKeyValue("WpfApp", "DriveType", "1", IniHelper.inipath));
            DriveType = DriveTypes.First(x => x.Id == type);
            string symbol = IniHelper.GetKeyValue("WpfApp", "DriveSymbol", "Z", IniHelper.inipath);
            DriveSymbol = DriveSymbols.FirstOrDefault(x => x.Id == symbol);
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

        #region Rclone Service
        Process RcloneProcess;
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
            try
            {
                CommandRunner cr = new CommandRunner(basepath + @"Data\rclone-v1.58.1-windows-amd64\rclone.exe");
                var arguments = $"rc mount/unmountall";
                cr.Run(arguments);
                var res = cr.GetOutput();
                if (res.Trim() == "{}")
                {
                    IsRcloneRunning = false;
                    return;
                }
                Debug.WriteLine(res);
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
            try
            {
                if (RcloneProcess == null || RcloneProcess.HasExited)
                    if (!StartRcloneService())
                        return;
                
                CommandRunner cr = new CommandRunner(basepath + @"Data\rclone-v1.58.1-windows-amd64\rclone.exe"); 
                var arguments = $"rc mount/mount fs=jbox: mountPoint={DriveSymbol.Id}:";
                if (DriveType.Id == 1)
                {
                    arguments += " mountOpt={\\\"NetworkMode\\\":true,\\\"VolumeName\\\":\\\"" + DriveName + "\\\"}";
                }
                else
                {
                    arguments += " mountOpt={\\\"VolumeName\\\":\\\"" + DriveName + "\\\"}";
                }
                cr.Run(arguments);
                var res = cr.GetOutput();
                if (res.Trim()=="{}")
                {
                    IsRcloneRunning = true;
                    return;
                }
                Debug.WriteLine(res);
            }
            catch (Exception ex)
            {
                RcloneMessage = ex.Message;
                IsRcloneRunning = false;
            }
        }

        private bool StartRcloneService()
        {
            try
            {
                var psi = new ProcessStartInfo(basepath + @"Data\rclone-v1.58.1-windows-amd64\rclone.exe");
                psi.Arguments = "rcd --rc-no-auth";
                psi.CreateNoWindow = true;
                RcloneProcess = new Process();
                RcloneProcess.Exited += RcloneProcess_Exited;
                RcloneProcess.StartInfo = psi;
                RcloneProcess.Start();
                return true;
            }
            catch (Exception ex)
            {
                RcloneMessage = ex.Message;
            }
            return false;
        }

        private void RcloneProcess_Exited(object? sender, EventArgs e)
        {
            IsRcloneRunning = false;
            RcloneProcess = null;
        }
        #endregion

        #region Device Symbol
        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;
        public IntPtr hwnd;
        private void ApplyHook()
        {
            hwnd = ((HwndSource)PresentationSource.FromVisual(this)).Handle;
            HwndSource hWndSource = HwndSource.FromHwnd(hwnd);
            if (hWndSource != null) hWndSource.AddHook(WndProc);
        }
        public IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wideParam, IntPtr longParam, ref bool handled)
        {
            try
            {
                if (msg == WM_DEVICECHANGE)
                {
                    switch (wideParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL:
                            GetFreeDrives();
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            GetFreeDrives();
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return IntPtr.Zero;
        }
        private void GetFreeDrives()
        {
            DriveSymbols = new List<DriveSymbolItem>();
            var drives = DriveInfo.GetDrives();
            bool[] t = new bool[26];
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
