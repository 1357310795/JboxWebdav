using Jbox.Service;
using JboxWebdav.WpfApp.Helpers;
using JboxWebdav.WpfApp.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JboxWebdav.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            log4net.Config.XmlConfigurator.Configure();
            ThemeHelper.ChangeHue("#c8161e");

            bool firstrun = bool.Parse(IniHelper.GetKeyValue("WpfApp", "firstrun", "true", IniHelper.inipath));
            if (firstrun)
            {
                if (MessageBox.Show("第一次启动程序，需要安装依赖，点击“是”开始安装", "提示 - JboxWebdav", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    var w = new InstallWindow();
                    App.Current.MainWindow = w;
                    w.Show();
                    return;
                }
                else
                    App.Current.Shutdown();
            }

            Jac.ReadInfo();
            if (Jac.dic.Count>0)
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
