using JboxWebdav.Server.Jbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JboxWebdav.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                this.RaisePropertyChanged("FilePath");
            }
        }

        private string uploadPath = "/test/test.torrent";

        public string UploadPath
        {
            get { return uploadPath; }
            set
            {
                uploadPath = value;
                this.RaisePropertyChanged("UploadPath");
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => {
                FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                var res = JboxService.UploadFile(UploadPath, fs, fs.Length);
                Debug.WriteLine(res.success);
            });
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
    }
}
