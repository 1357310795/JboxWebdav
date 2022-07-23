
using Jbox.Service;

namespace JboxWebdav.MauiApp.Pages;

public partial class NoVPNPage : ContentPage
{
	public NoVPNPage()
	{
		InitializeComponent();
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
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