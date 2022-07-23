using Jbox.Service;
using JboxWebdav.MauiApp.Pages;
using JboxWebdav.MauiApp.Services;

namespace JboxWebdav.MauiApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(NoVPNPage), typeof(NoVPNPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(LogPage), typeof(LogPage));
            //this.AddLogicalChild(new ShellContent() { Title = "MainPage", Content = new MainPage(), Route = "MainPage" });
        }

        private async void Shell_Loaded(object sender, EventArgs e)
        {
            Jac.InitStorage(new MauiStorage());
            Jac.ReadInfo();
            if (Jac.dic.Count > 0)
            {
                var ac = Jac.dic.Keys.First();
                if (Jac.TryLastCookie(ac))
                {
                    await Shell.Current.GoToAsync("MainPage");
                    return;
                }
            }
            if (Jac.CheckVPN())
                await Shell.Current.GoToAsync("LoginPage");
            else
                await Shell.Current.GoToAsync("NoVPNPage");
        }
    }
}