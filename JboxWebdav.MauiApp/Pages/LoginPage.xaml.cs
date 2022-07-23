
using Jbox.Service;
using System.ComponentModel;

namespace JboxWebdav.MauiApp.Pages;

public partial class LoginPage : ContentPage, INotifyPropertyChanged
{
	public LoginPage()
	{
		InitializeComponent();
        this.BindingContext = this;
	}

    private BackgroundWorker worker;

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


    private void AccountLoginButton_Clicked(object sender, EventArgs e)
    {
        AccountLoginButton.IsEnabled = false;
        AccountLoginButton.Text = "ÕýÔÚµÇÂ¼...";

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
        AccountLoginButton.IsEnabled = true;
        AccountLoginButton.Text = "µÇÂ¼";
        if (res)
        {
            this.Dispatcher.Dispatch(async() => { await Shell.Current.GoToAsync("MainPage"); });
            
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
                OnRecieveMessage(res.message);
                break;
            case Jac.LoginState.captchafail:
                OnRecieveMessage(res.message);
                break;
            case Jac.LoginState.novpn:
                OnRecieveMessage(res.message);
                break;
        }
        return false;
    }

    public void OnRecieveMessage(string message)
    {
        this.Dispatcher.Dispatch(() => { Shell.Current.DisplayAlert("´íÎó", message, "OK"); });
    }


    //private void PasswordBox_KeyUp(object sender, KeyEventArgs e)
    //{
    //    if (e.Key == Key.Return)
    //        ButtonLogin_Click(null, new RoutedEventArgs());
    //}

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