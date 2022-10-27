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
    using Microsoft.Win32;
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

        public Action OnSave { get; set; } = null;

        private void Save()
        {
            this.OnSave?.Invoke();
        }

        public void ExecuteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var r = Data.SelectedRequest;
            if(r == null)
            {
                return;
            }
            Logic.Request(r, Data.Cookies);
            Save();
        }

        private void FormatExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var r = Data.SelectedRequest;
            if (r == null)
            {
                return;
            }
            r.Post = Logic.FormatJsonOrNot(r.Post);
            Save();
        }

        private void NewRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Data.AddNewRequest();
            Save();
        }

        private void ExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteSelectedRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Data.DeleteSelectedRequest();
            Save();
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void TogglePostExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var r = Data.SelectedRequest;
            if (r == null)
            {
                return;
            }
            r.HasPost = !r.HasPost;
        }

        private void LoadPostExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var r = Data.SelectedRequest;
            if (r == null)
            {
                return;
            }


            var fdlg = new OpenFileDialog();
            fdlg.Title = "Browse post data";
            fdlg.Filter = "Text files (json,txt,xml)|*.json;*.txt;*.xml|All files (*.*)|*.*";
            fdlg.RestoreDirectory = true;
            var dr = fdlg.ShowDialog();
            if (dr == false) { return; }

            r.Post = File.ReadAllText(fdlg.FileName);
        }

        private void FocusRequestsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            uiRequests.Focus();
        }

        private void FocusUrlExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            uiUrl.Focus();
            uiUrl.SelectAll();
        }

        private void FocusPostExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var r = Data.SelectedRequest;
            if (r == null)
            {
                return;
            }
            uiPost.Focus();
            uiPost.SelectAll();
        }


        void SelectRequest(int i1)
        {
            var i = i1 - 1;
            if(i < 0 ) return;
            if(i >= Data.Requests.Count) return;
            Data.SelectedRequest = Data.Requests[i];
        }

        private void SelectRequest1Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(1);
        }

        private void SelectRequest2Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(2);
        }

        private void SelectRequest3Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(3);
        }

        private void SelectRequest4Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(4);
        }

        private void SelectRequest5Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(5);
        }

        private void SelectRequest6Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(6);
        }

        private void SelectRequest7Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(7);
        }

        private void SelectRequest8Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(8);
        }

        private void SelectRequest9Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRequest(9);
        }
    }
}
