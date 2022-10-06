using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PostTestr
{
    public class Request : INotifyPropertyChanged
    {
        private bool hasPost = false;
        private string url = "http://localhost:8080/";
        private string post = string.Empty;
        private string response = string.Empty;
        private bool isWorking = false;
        private BackgroundWorker worker = null;

        [JsonProperty("url")]
        public string Url
        {
            get => url; set
            {
                url = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("has_post")]
        public bool HasPost
        {
            get => hasPost; set
            {
                hasPost = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("post")]
        public string Post
        {
            get => post; set
            {
                post = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("response")]
        public string Response
        {
            get => response; set
            {
                response = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool IsWorking
        {
            get => isWorking; set
            {
                isWorking = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public BackgroundWorker Worker
        {
            get => worker; set
            {
                worker = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Data : INotifyPropertyChanged
    {
        private Request selectedRequest = null;
        private CookieContainer cookies;
        private ObservableCollection<Request> requests = new ObservableCollection<Request>();

        public ObservableCollection<Request> Requests
        {
            get => requests; set
            {
                requests = value;
                OnPropertyChanged();
            }
        }

        public Request SelectedRequest
        {
            get => selectedRequest; set
            {
                selectedRequest = value;
                OnPropertyChanged();
            }
        }

        public CookieContainer Cookies
        {
            get => cookies; set
            {
                cookies = value;
                OnPropertyChanged();
            }
        }

        public void AddNewRequest()
        {
            var r = new Request();
            if (this.SelectedRequest != null)
            {
                var newUrl = new UriBuilder(new Uri(this.SelectedRequest.Url))
                {
                    Path = "",
                    Query = ""
                };
                r.Url = newUrl.Uri.AbsoluteUri;
            }
            this.Requests.Add(r);
            this.SelectedRequest = r;
        }

        public void DeleteSelectedRequest()
        {
            if (this.SelectedRequest == null) { return; }
            if (this.Requests.Count <= 1) { return; }

            var index = this.Requests.IndexOf(this.SelectedRequest);
            this.Requests.RemoveAt(index);
            this.SelectedRequest = this.Requests[Math.Min(index, this.Requests.Count - 1)];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RequestFile
    {
        [JsonProperty("requests")]
        public Request[] Requests { get; set; }

        [JsonProperty("selected_request")]
        public int SelectedRequest { get; set; }
    }

    public static class Disk
    {
        private static string SettingsFile
        {
            get
            {
                var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var folder = Path.Combine(root, "PostTestr");
                if(Directory.Exists(folder) == false)
                {
                    Directory.CreateDirectory(folder);
                }
                return Path.Combine(folder, "settings.json"); ;
            }
        }

        private static Data Load(string file)
        {
            var data = File.ReadAllText(file);
            var container = JsonConvert.DeserializeObject<RequestFile>(data);
            var requests = new ObservableCollection<Request>(container.Requests);
            return new Data { Requests = requests, SelectedRequest = requests[container.SelectedRequest]};
        }

        private static void Save(Data data, string file)
        {
            var jsonFile = new RequestFile { Requests = data.Requests.ToArray(), SelectedRequest = data.Requests.IndexOf(data.SelectedRequest) };
            var jsonData = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
            File.WriteAllText(file, jsonData);
        }

        public static Data LoadOrCreateNew()
        {
            if (File.Exists(SettingsFile))
            {
                return Load(SettingsFile);
            }
            else
            {
                var r = new Data { };
                r.AddNewRequest();
                return r;
            }
        }

        public static void Save(Data data)
        {
            Save(data, SettingsFile);
        }
    }
}
