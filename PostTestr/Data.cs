﻿using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PostTestr
{
    [AddINotifyPropertyChangedInterface]
    public class Request
    {
        public string Url { get; set; } = "http://localhost:8080/";
        public string Post { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;

        public override string ToString()
        {
            return this.Url;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class Data
    {
        public ObservableCollection<Request> Requests { get; set; } = new ObservableCollection<Request>();
        public Request SelectedRequest { get; set; } = null;
        public CookieContainer Cookies { get; set; }

        public void AddNewRequest()
        {
            var r = new Request();
            this.Requests.Add(r);
            this.SelectedRequest = r;
        }
    }

    public class RequestFile
    {
        public Request[] Requests { get; set; }
    }

    public static class Disk
    {
        public static string SettingsFile => "";

        public static Data Load(string file)
        {
            var data = File.ReadAllText(file);
            var container = JsonConvert.DeserializeObject<RequestFile>(data);
            return new Data { Requests = new ObservableCollection<Request>(container.Requests) };
        }

        public static Data LoadOrCreateNew()
        {
            if(File.Exists(SettingsFile))
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

        internal static void Save(Data data)
        {
        }
    }
}
