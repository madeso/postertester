using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace PostTestr
{
    public static class Logic
    {
        private static string FormatJson(string t)
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(t);
            var f = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return f;
        }

        public static string FormatJsonOrNot(string t)
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

        private static string FetchStringAdvanced(string url, string requestData, CookieContainer cookies)
        {
            var request = GetWebRequest(url);

            request.CookieContainer = cookies;

            if (requestData != null)
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

        public static string Request(string url, string requestData, CookieContainer cookies)
        {
            try
            {
                return FetchStringAdvanced(url, requestData, cookies);
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        public static void Request(Request r, CookieContainer cookies)
        {
            if(r.Worker != null)
            {
                if(r.Worker.IsBusy)
                {
                    return;
                }
            }
            r.Worker = new System.ComponentModel.BackgroundWorker();
            r.Worker.DoWork += (sender, args) =>
            {
                r.Response = Logic.Request(r.Url, r.HasPost ? r.Post ?? string.Empty : null, cookies);
            };
            r.Worker.RunWorkerCompleted += (sender, e) =>
            {
                r.IsWorking = false;
            };
            r.Response = string.Empty;
            r.IsWorking = true;
            r.Worker.RunWorkerAsync();
        }
    }
}
