﻿using Jbox.Service;
using JboxWebdav.WpfApp.Helpers;
using JboxWebdav.WpfApp.Services;
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

            Jac.InitStorage(new WinStorage());
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
            if (Jac.CheckVPN())
            {
                var w = new LoginWindow();
                App.Current.MainWindow = w;
                w.Show();
                return;
            }
            else
            {
                var w = new NoVPNWindow();
                App.Current.MainWindow = w;
                w.Show();
                return;
            }
        }
    }
}
