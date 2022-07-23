namespace JboxWebdav.MauiApp.Pages;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}

	private void Button_Clicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("LoginPage");
	}
}