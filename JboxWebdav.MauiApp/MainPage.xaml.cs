using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using JboxWebdav.MauiApp.Extensions;
using JboxWebdav.MauiApp.Models;
using JboxWebdav.MauiApp.Services;
using JboxWebdav.Server.Jbox;
using System.Diagnostics;
using System.Threading;
using Font = Microsoft.Maui.Font;

namespace JboxWebdav.MauiApp
{
    public partial class MainPage : ContentPage
    {
        IStorage storage;
        public MainPage()
        {
            storage = new MauiStorage();
            AccessModes = new List<AccessModeDto>();
            AccessModes.Add(new AccessModeDto("完全模式", AccessModeEnum.Full, 1023));
            AccessModes.Add(new AccessModeDto("只读模式", AccessModeEnum.ReadOnly, 5));
            AccessModes.Add(new AccessModeDto("读写模式", AccessModeEnum.ReadWrite, 39));
            AccessModes.Add(new AccessModeDto("防止删除模式", AccessModeEnum.NoDelete, 1023 - 64));

            InitializeComponent();
            this.BindingContext = this;
        }

        #region Fields
        private bool isWebdavRunning = false;
        public bool IsWebdavRunning
        {
            get { return isWebdavRunning; }
            set
            {
                isWebdavRunning = value;
                this.OnPropertyChanged("IsWebdavRunning");
            }
        }

        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                ipAddress = value;
                this.OnPropertyChanged("IpAddress");
            }
        }

        private string webdavMessage;
        public string WebdavMessage
        {
            get { return webdavMessage; }
            set
            {
                webdavMessage = value;
                this.OnPropertyChanged("WebdavMessage");
            }
        }

        #endregion

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            ReadSettings();
        }
        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private async void ButtonSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            var toast = Toast.Make("保存成功！", ToastDuration.Short, 14);
            await toast.Show();
            //var snackbarOptions = new SnackbarOptions
            //{
            //    BackgroundColor = Colors.Red,
            //    TextColor = Colors.Green,
            //    ActionButtonTextColor = Colors.Yellow,
            //    CornerRadius = new CornerRadius(10),
            //    Font = Font.SystemFontOfSize(14),
            //    ActionButtonFont = Font.SystemFontOfSize(14),
            //    CharacterSpacing = 0.5
            //};

            //string text = "This is a Snackbar";
            //string actionButtonText = "Click Here to Dismiss";
            //Action action = async () => await DisplayAlert("Snackbar ActionButton Tapped", "The user has tapped the Snackbar ActionButton", "OK");
            //TimeSpan duration = TimeSpan.FromSeconds(3);

            //var snackbar = Snackbar.Make(text, action, actionButtonText, duration, snackbarOptions);

            //await snackbar.Show();
        }
        #region Settings
        private void ReadSettings()
        {
            IpAddress = storage.GetKeyValue("MauiApp", "IpAddress", "http://127.0.0.1:65472/");
            JboxPublicEnabled = bool.Parse(storage.GetKeyValue("MauiApp", "JboxPublicEnabled", "true"));
            JboxSharedEnabled = bool.Parse(storage.GetKeyValue("MauiApp", "JboxSharedEnabled", "true"));
            var accesstype = storage.GetKeyValue("MauiApp", "AccessMode", "Full").ToEnum<AccessModeEnum>();
            AccessMode = AccessModes.FirstOrDefault(x => x.Type == accesstype);
        }

        private void SaveSettings()
        {
            storage.SetKeyValue("MauiApp", "IpAddress", IpAddress);
            storage.SetKeyValue("MauiApp", "JboxPublicEnabled", JboxPublicEnabled.ToString());
            storage.SetKeyValue("MauiApp", "JboxSharedEnabled", JboxSharedEnabled.ToString());
            storage.SetKeyValue("MauiApp", "AccessMode", AccessMode.Type.ToString());
        }
        #endregion

        #region Webdav Service
        private async void ButtonWebdavLog_Click(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("LogPage");
        }

        private void ButtonWebdavStop_Click(object sender, EventArgs e)
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

        private void ButtonWebdavStart_Click(object sender, EventArgs e)
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

        #region Settings Fields

        public bool? JboxPublicEnabled
        {
            get { return Config.PublicEnabled; }
            set
            {
                Config.PublicEnabled = value.Value;
                this.OnPropertyChanged("JboxPublicEnabled");
            }
        }

        public bool? JboxSharedEnabled
        {
            get { return Config.SharedEnabled; }
            set
            {
                Config.SharedEnabled = value.Value;
                this.OnPropertyChanged("JboxSharedEnabled");
            }
        }

        private List<AccessModeDto> accessModes;

        public List<AccessModeDto> AccessModes
        {
            get { return accessModes; }
            set
            {
                accessModes = value;
                this.OnPropertyChanged("AccessModes");
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
                this.OnPropertyChanged("AccessMode");
            }
        }

        #endregion

    }
}