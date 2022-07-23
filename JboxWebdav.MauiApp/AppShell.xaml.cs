
using JboxWebdav.MauiApp.Pages;

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
            //this.AddLogicalChild(new ShellContent() { Title = "MainPage", Content = new MainPage(), Route = "MainPage" });
        }

        private async void Shell_Loaded(object sender, EventArgs e)
        {
            
        }
    }
}