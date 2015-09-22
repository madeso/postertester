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
        private static Settings Set
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        public CookieContainer Cookies { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            UiPost.Text = Set.Post;
            UiResponse.Text = Set.Response;
            UiUrl.Text = Set.Url;
            UiLogin.Text = Set.Login;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Set.Post = UiPost.Text;
            Set.Response = UiResponse.Text;
            Set.Url = UiUrl.Text;
            Set.Login = UiLogin.Text;
            Set.Save();
            e.Cancel = false;
        }

        public void ExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("press x");
        }

        private static string FormatJson(string t)
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(t);
            var f = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return f;
        }

        private static string FormatJsonOrNot(string t)
        {
            try
            {
                return FormatJson(t);
            }
            catch (JsonReaderException)
            {
                return t;
            }
        }

        private static HttpWebRequest GetWebRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            request.AllowAutoRedirect = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;

            return request;
        }

        public string FetchStringAdvanced(string url, string requestData)
        {
            var request = GetWebRequest(url);

            if( this.Cookies == null )
            {
                MessageBox.Show(this, "Started new cookie container", "Cookies", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Cookies = new CookieContainer();
            }
            request.CookieContainer = this.Cookies;

            if (string.IsNullOrEmpty(requestData) == false)
            {
                request.Method = "POST";
                var bytes = Encoding.UTF8.GetBytes(requestData);
                request.ContentLength = bytes.Length;
                request.ContentType = "application/json";
                var postStream = request.GetRequestStream();
                postStream.Write(bytes, 0, bytes.Length);
                postStream.Close();
            }

            WebResponse response;
            WebException exp = null;

            try
            {
                response = request.GetResponse();
            }
            catch (WebException web)
            {
                exp = web;
                response = web.Response;
                if (response == null)
                {
                    throw;
                }
            }

            Stream dataStream = response.GetResponseStream();
            if (dataStream == null)
            {
                throw new Exception("No response stream present");
            }

            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            // if we didn't get any response and we failed earlier, rethrow that web-error
            if (string.IsNullOrEmpty(responseFromServer) && exp != null)
            {
                throw exp;
            }

            if (exp == null) return FormatJsonOrNot(responseFromServer);
            else return string.Format("{0}\n{1}", exp.Message, responseFromServer);
        }

        private string Request(string url, string requestData)
        {
            string responseData;

            try
            {
                responseData = FetchStringAdvanced(url, requestData);
            }
            catch (Exception x)
            {
                responseData = x.Message;
            }

            return responseData;
        }

        public void ExecuteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var webData = Request(UiUrl.Text, UiPost.Text);
            UiResponse.Text = webData;
        }

        private void LoginExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var webData = Request(UiLogin.Text, string.Empty);
            UiResponse.Text = webData;
        }

        private void FormatExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                UiPost.Text = FormatJson(UiPost.Text);
            }
            catch (Exception x)
            {
                MessageBox.Show(this, x.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
