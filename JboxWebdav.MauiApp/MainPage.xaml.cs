namespace JboxWebdav.MauiApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("LoginPage");
        }

        private void ButtonWebdavLog_Click(object sender, EventArgs e)
        {

        }

        private void ButtonWebdavStop_Click(object sender, EventArgs e)
        {

        }

        private void ButtonWebdavStart_Click(object sender, EventArgs e)
        {

        }
    }
}