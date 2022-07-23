using JboxWebdav.MauiApp.Services;
using JboxWebdav.Server.Jbox;
using System.Diagnostics;

namespace JboxWebdav.MauiApp
{
    public partial class MainPage : ContentPage
    {
        IStorage storage;
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext= this;
            storage = new MauiStorage();
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

        #region Settings
        private void ReadSettings()
        {
            IpAddress = storage.GetKeyValue("MauiApp", "IpAddress", "http://127.0.0.1:65472/");
        }

        private void SaveSettings()
        {
            storage.SetKeyValue("MauiApp", "IpAddress", IpAddress);
        }
        #endregion

        #region Webdav Service
        private void ButtonWebdavLog_Click(object sender, EventArgs e)
        {

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
    }
}