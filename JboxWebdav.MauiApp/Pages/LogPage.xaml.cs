using JboxWebdav.MauiApp.Services;
using System.ComponentModel;
using System.Linq;

namespace JboxWebdav.MauiApp.Pages;

public partial class LogPage : ContentPage
{
	public LogPage()
	{
		InitializeComponent();
        Items = LogStorage.logs.Reverse<string>();
        this.BindingContext = this;
	}

    public IEnumerable<string> items;
    public IEnumerable<string> Items
    {
        get { return items; }
        set { items = value; OnPropertyChanged("Items"); }
    }

    private void ListView_Refreshing(object sender, EventArgs e)
	{
        Items = LogStorage.logs.Reverse<string>();
        RefreshView1.IsRefreshing = false;
    }
}