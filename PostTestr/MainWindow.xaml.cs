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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PostTestr
{
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Net.Cache;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using PostTestr.Properties;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Data Data { get; internal set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ExecuteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var r = Data.SelectedRequest;
            if(r == null)
            {
                return;
            }
            r.Response = Logic.Request(r.Url, r.Post, Data.Cookies);
        }

        private void FormatExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var r = Data.SelectedRequest;
            if (r == null)
            {
                return;
            }
            r.Post = Logic.FormatJsonOrNot(r.Post);
        }

        private void NewRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var r = new Request();
            Data.Reguests.Add(r);
            Data.SelectedRequest = r;
        }

        private void ExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
