using Jbox.Service;
using JboxWebdav.Test.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JboxWebdav.Test.Views
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Account
        {
            get { return Jac.account; }
            set
            {
                Jac.account = value;
                this.RaisePropertyChanged("Account");
            }
        }
        public string Password
        {
            get { return Jac.password; }
            set
            {
                Jac.password = value;
                this.RaisePropertyChanged("Password");
            }
        }
        public string module;

        private BackgroundWorker worker;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            ButtonProgressAssist.SetValue(AccountLoginButton, -1);
            ButtonProgressAssist.SetIsIndicatorVisible(AccountLoginButton, true);
            ButtonProgressAssist.SetIsIndeterminate(AccountLoginButton, true);
            AccountLoginButton.Content = "正在登录...";
            MainGrid.IsEnabled = false;

            worker = new BackgroundWorker();
            worker.DoWork += Login_DoWork;
            worker.RunWorkerCompleted += Login_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Login_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = DoLogin();
        }

        private void Login_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var res = (bool)e.Result;
            this.Dispatcher.Invoke(() => {
                ButtonProgressAssist.SetIsIndicatorVisible(AccountLoginButton, false);
                AccountLoginButton.Content = "登录";
            });
            MainGrid.IsEnabled = true;
            if (res)
            {
                this.Dispatcher.Invoke(() => {
                    var m = new MainWindow();
                    Application.Current.MainWindow = m;
                    m.Show();
                    this.Close();
                });
            }
        }

        public bool DoLogin()
        {
            var res = Jac.Login(Account, Password);
            switch (res.state)
            {
                case Jac.LoginState.success:
                    return true;
                case Jac.LoginState.fail:
                    OnRecieveMessage(new MySnackBarMessage(res.message, TimeSpan.FromSeconds(3)));
                    break;
                case Jac.LoginState.captchafail:
                    OnRecieveMessage(new MySnackBarMessage(res.message, TimeSpan.FromSeconds(3)));
                    break;
                case Jac.LoginState.novpn:
                    OnRecieveMessage(new MySnackBarMessage(res.message, TimeSpan.FromSeconds(3)));
                    break;
            }
            return false;
        }

        public void OnRecieveMessage(MySnackBarMessage obj)
        {
            this.Dispatcher.Invoke(() => { MainSnackbar.MessageQueue.Enqueue(obj.message, null, null, null, false, true, obj.duration); });
        }


        private void PasswordBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                ButtonLogin_Click(null, new RoutedEventArgs());
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
