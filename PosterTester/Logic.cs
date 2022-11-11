using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PosterTester.Data;
using PosterTester.Data.Saved;

namespace PosterTester;


[JsonConverter(typeof(StringEnumConverter))]
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

internal class SingleAttackResult
{
	public string Error { get; set; }
	public TimeSpan Result { get; set; }
}

public static class Logic
{
	private static string FormatJson(string t)
	{
		object obj = JsonConvert.DeserializeObject(t);
		string f = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
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

	private static readonly HttpClientHandler handler = new HttpClientHandler();
	private static readonly CookieContainer cookieContainer = handler.CookieContainer;
	private static readonly HttpClient client = new HttpClient(handler);
	// private static readonly HttpClient client = new HttpClient();


	private static async Task<HttpResponseMessage> GetResponse(HttpMethod action, Uri url, HttpContent content)
	{
		return action switch
		{
			HttpMethod.Get => await client.GetAsync(url),
			HttpMethod.Post => await client.PostAsync(url, content),
			HttpMethod.Put => await client.PutAsync(url, content),
			HttpMethod.Delete => await client.DeleteAsync(url),
			HttpMethod.Patch => await client.PatchAsync(url, content),

			_ => throw new Exception($"Invalid action: {action}"),
		};
	}

	internal static bool HasContent(HttpMethod action)
	{
		return action switch
		{
			HttpMethod.Get => false,
			HttpMethod.Post => true,
			HttpMethod.Put => true,
			HttpMethod.Delete => false,
			HttpMethod.Patch => true,

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

	public static async Task<Data.Response> GetUrl(HttpMethod action, Uri url, HttpContent content)
	{
		// todo(Gustav): expose and enrich headers
		var headers = client.DefaultRequestHeaders.ToArray();
		var cookies = cookieContainer.GetAllCookies().ToList();
		using var response = await GetResponse(action, url, content);
		string responseBody = await response.Content.ReadAsStringAsync();

		var resh = Data.Headers.Collect(response.Headers);
		var conh = Data.Headers.Collect(response.Content.Headers);

		return new Data.Response(status: response.StatusCode, body: responseBody, responseHeaders: resh);
	}

	public static async Task Request(Data.Root root, Data.Request r)
	{
		// silently ignore double commands
		if (r.IsWorking == true) { return; }

		r.Response = null;
		using var locked = new WorkingLock(r);

		var start = DateTime.Now;

		try
		{
			var data = await MakeRequest(r);
			var end = DateTime.Now;
			r.Response = data;
			r.Response.Time = end.Subtract(start);

			if (root.FormatResponse)
			{
				r.Response.Body = FormatJsonOrNot(r.Response.Body);
			}
		}
		catch (Exception xx)
		{
			var end = DateTime.Now;
			string builder = String.Empty;

			var x = xx;
			while (x != null)
			{
				builder += x.Message + "\r\n";
				x = x.InnerException;
			}

			r.Response = new Data.Response(status: HttpStatusCode.BadRequest, body: builder, new Data.Headers());
			r.Response.Time = end.Subtract(start);
		}
	}

	private static async Task<Data.Response> MakeRequest(Data.Request r)
	{
		var url = new Uri(r.Url);
		var data = HasContent(r.Method)
			? await GetUrl(HttpMethod.Post, url, r.GetContent())
			: await GetUrl(HttpMethod.Get, url, null);
		return data;
	}

	internal static void BrowseFolder(string dir)
	{
		var startInfo = new ProcessStartInfo
		{
			FileName = "explorer.exe",
			Arguments = dir
		};
		Process.Start(startInfo);
	}

	internal static async Task<SingleAttackResult> SingleAttack(Data.Request r)
	{
		try
		{
			var start = DateTime.Now;
			var data = await MakeRequest(r);
			// todo(Gustav): verify data...
			if (data.Status != HttpStatusCode.OK)
			{
				return new SingleAttackResult { Error = data.Body };
			}
			else
			{
				var end = DateTime.Now;
				var time = end.Subtract(start);
				return new SingleAttackResult { Result = time };
			}
		}
		catch (Exception xx)
		{
			string builder = String.Empty;

			var x = xx;
			while (x != null)
			{
				builder += x.Message + "\r\n";
				x = x.InnerException;
			}

			return new SingleAttackResult { Error = builder };
		}
	}

	internal static async Task<AttackResult> Attack(Data.Root root, Data.Request r)
	{
		// silently ignore double commands
		if (r.IsWorking == true) { return null; }

		using var locked = new WorkingLock(r);

		var attack = root.Attack.Clone();
		var ret = new AttackResult();

		if(attack.AtTheSameTime)
		{
			var tasks = Enumerable.Range(0, attack.Count).Select(i => SingleAttack(r));
			var results = await Task.WhenAll(tasks);
			foreach (SingleAttackResult callResult in results)
			{
				AddAttackResult(ret, callResult);
			}
		}
		else
		{
			for (int i = 0; i < attack.Count; i += 1)
			{
				var callResult = await SingleAttack(r);
				AddAttackResult(ret, callResult);
			}
		}

		return ret;

		static void AddAttackResult(AttackResult ret, SingleAttackResult callResult)
		{
			if (callResult.Error != null)
			{
				ret.Errors.Add(callResult.Error);
			}
			else
			{
				ret.Result.Add(callResult.Result);
			}
		}
	}
}

public class WorkingLock : IDisposable
{
	private readonly Data.Request r;

	public WorkingLock(Data.Request r)
	{
		this.r = r;
		r.IsWorking = true;
	}

	public void Dispose()
	{
		r.IsWorking = false;
	}
}

