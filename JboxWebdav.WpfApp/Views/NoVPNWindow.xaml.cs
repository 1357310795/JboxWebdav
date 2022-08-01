using Jbox.Service;
using System;
using System.Collections.Generic;
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

namespace JboxWebdav.WpfApp.Views
{
    /// <summary>
    /// NoVPNWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NoVPNWindow : Window
    {
        public NoVPNWindow()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (Jac.dic.Count > 0)
            {
                var ac = Jac.dic.Keys.First();
                if (Jac.TryLastCookie(ac))
                {
                    var w = new MainWindow();
                    App.Current.MainWindow = w;
                    w.Show();
                    this.Close();
                    return;
                }
            }
            if (Jac.CheckVPN())
            {
                var w = new LoginWindow();
                App.Current.MainWindow = w;
                w.Show();
                this.Close();
            }
            else
            {
                MainSnackbar.MessageQueue.Enqueue("请重试", null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }
    }
}
