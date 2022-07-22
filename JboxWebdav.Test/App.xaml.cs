using Jbox.Service;
using JboxWebdav.Test.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JboxWebdav.Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            Jac.ReadInfo();
            if (Jac.dic.Count > 0)
            {
                var ac = Jac.dic.Keys.First();
                if (Jac.TryLastCookie(ac))
                {
                    var w = new MainWindow();
                    App.Current.MainWindow = w;
                    w.Show();
                    return;
                }
            }

            var w1 = new LoginWindow();
            App.Current.MainWindow = w1;
            w1.Show();
        }
    }
}
