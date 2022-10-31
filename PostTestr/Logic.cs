using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PostTestr;

public class Response
{
    public HttpStatusCode Status { get; set; }
    public string Body { get; set; }
}

public enum HttpMethod
{
    [EnumMember(Value = "get")]
    Get,
    // Head,

    [EnumMember(Value = "post")]
    Post,

    [EnumMember(Value = "put")]
    Put,

    [EnumMember(Value = "delete")]
    Delete,
    // Connect,
    // Options,
    // Trace,

    [EnumMember(Value = "patch")]
    Patch
    
    // Link,
    // Unlink,
    // Purge,
    // View,

    // web dav
    // Copy,
    // Lock,
    // PropFind,
    // Unlock
}

public static class Logic
{
    private static string FormatJson(string t)
    {
        var obj = JsonConvert.DeserializeObject(t);
        var f = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
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

    static readonly HttpClient client = new HttpClient();

    private static async Task<HttpResponseMessage> GetResponse(HttpMethod action, Uri url, HttpContent content)
    {
        return action switch
        {
            HttpMethod.Get =>      await client.GetAsync(url),
            // Method.Head =>     break,
            HttpMethod.Post =>     await client.PostAsync(url, content),
            HttpMethod.Put =>      await client.PutAsync(url, content),
            HttpMethod.Delete =>   await client.DeleteAsync(url),
            // Method.Connect =>  break,
            // Method.Options =>  break,
            // Method.Trace =>    break,
            HttpMethod.Patch =>    await client.PatchAsync(url, content),

            // Method.Link =>     break,
            // Method.Unlink =>   break,
            // Method.Purge =>    break,
            // Method.View =>     break,

            // Method.Copy =>     break,
            // Method.Lock =>     break,
            // Method.PropFind => break,
            // Method.Unlock =>   break,

        _ => throw new Exception($"Invalid action: {action}"),
        };
    }

    internal static bool HasContent(HttpMethod action)
    {
        return action switch
        {
            HttpMethod.Get => false,
            // Method.Head =>     break,
            HttpMethod.Post => true,
            HttpMethod.Put => true,
            HttpMethod.Delete => false,
            // Method.Connect =>  break,
            // Method.Options =>  break,
            // Method.Trace =>    break,
            HttpMethod.Patch => true,

            // Method.Link =>     break,
            // Method.Unlink =>   break,
            // Method.Purge =>    break,
            // Method.View =>     break,

            // Method.Copy =>     break,
            // Method.Lock =>     break,
            // Method.PropFind => break,
            // Method.Unlock =>   break,

            _ => throw new Exception($"Invalid action: {action}"),
        };
    }

        // todo(Gustav): read file
        // todo(Gustav): form input

    public static HttpContent WithStringContent(string t)
    {
        return new StringContent(t, Encoding.UTF8, "text/plain");
    }

    public static HttpContent WithJsonContent(string t)
    {
        return new StringContent(t, Encoding.UTF8, "application/json");
    }

    public static async Task<Response> GetUrl(HttpMethod action, Uri url, HttpContent content)
    {
        // todo(Gustav): expose and enrich headers
        var headers = client.DefaultRequestHeaders;
        using HttpResponseMessage response = await GetResponse(action, url, content);
        var status = response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        return new Response { Status = status.StatusCode, Body = responseBody };
    }

    public static async Task Request(Data.Data root, Data.Request r)
    {
        // silently ignore double commands
        if(r.IsWorking == true) { return; }

        r.Response = string.Empty;
        r.IsWorking = true;

        try
        {
            var url = new Uri(r.Url);
            var data = HasContent(r.Method)
                ? await GetUrl(HttpMethod.Post, url, r.GetContent())
                : await GetUrl(HttpMethod.Get, url, null);
            r.Response = data.Body;

            if(root.FormatResponse)
            {
                r.Response = FormatJsonOrNot(r.Response);
            }
        }
        catch(Exception xx)
        {
            r.Response = String.Empty;

            var x = xx;
            while(x != null)
            {
                r.Response += x.Message + "\r\n";
                x = x.InnerException;
            }
        }

        r.IsWorking = false;
    }
}
