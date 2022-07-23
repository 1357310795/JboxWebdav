using JboxWebdav.MauiApp.Services;

namespace JboxWebdav.MauiApp.Pages;

public partial class LogPage : ContentPage
{
	public LogPage()
	{
		InitializeComponent();
        this.BindingContext = this;
	}

    public List<string> Items
    {
        get { return LogStorage.logs; }
        set { OnPropertyChanged("Items"); }
    }

    private void ListView_Refreshing(object sender, EventArgs e)
	{
        Items = LogStorage.logs;
        List1.IsRefreshing = false;
    }
}