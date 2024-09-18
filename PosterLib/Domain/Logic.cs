using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PosterLib.Data;

namespace PosterLib.Domain;


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


public static class Logic
{
	private static string FormatJson(string t)
	{
		object? obj = JsonConvert.DeserializeObject(t);
		string f = JsonConvert.SerializeObject(obj, Formatting.Indented);
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

	private static readonly HttpClientHandler handler = new();
	private static readonly CookieContainer cookieContainer = handler.CookieContainer;
	private static readonly HttpClient client = new(handler);


	private static async Task<HttpResponseMessage> GetResponse(HttpMethod action, Uri url, HttpContent? content)
	{
		return action switch
		{
			HttpMethod.Get => await client.GetAsync(url),
			HttpMethod.Post => await client.PostAsync(url, content!),
			HttpMethod.Put => await client.PutAsync(url, content!),
			HttpMethod.Delete => await client.DeleteAsync(url),
			HttpMethod.Patch => await client.PatchAsync(url, content!),

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

	public static async Task<Response> GetUrl(HttpMethod action, Uri url, HttpContent? content, long timeoutMs)
	{
		// todo(Gustav): expose and enrich headers
		client.Timeout = new TimeSpan(timeoutMs * 10000);
		var headers = client.DefaultRequestHeaders.ToArray();
		var cookies = cookieContainer.GetAllCookies().ToList();
		using var response = await GetResponse(action, url, content);
		string responseBody = await response.Content.ReadAsStringAsync();

		var resh = Headers.Collect(response.Headers);
		var conh = Headers.Collect(response.Content.Headers);

		return new Response(status: response.StatusCode, body: responseBody, responseHeaders: resh);
	}

	public static async Task Request(Root root, Request r)
	{
		// silently ignore double commands
		if (r.IsWorking == true) { return; }

		r.Response = null;
		using var locked = new WorkingLock(r);

		var res = await RunRequest(root.FormatResponse, r);
		r.Response = res;
	}

	private static async Task<Response> RunRequest(bool formatResponse, Request r)
	{
		var start = DateTime.Now;

		try
		{
			var response = await MakeRequest(r);
			var end = DateTime.Now;
			response.Time = end.Subtract(start);

			if (formatResponse)
			{
				response.Body = FormatJsonOrNot(response.Body);
			}

			return response;
		}
		catch (Exception xx)
		{
			var end = DateTime.Now;
			string builder = string.Empty;

			var x = xx;
			while (x != null)
			{
				builder += x.Message + "\r\n";
				x = x.InnerException;
			}

			var response = new Response(status: HttpStatusCode.BadRequest, body: builder, new Headers());
			response.Time = end.Subtract(start);
			return response;
		}
	}

	private static async Task<Response> MakeRequest(Request r)
	{
		var url = new Uri(r.Url);
		var data = HasContent(r.Method)
			? await GetUrl(HttpMethod.Post, url, r.GetContent(), r.Timeout.TotalMilliSeconds)
			: await GetUrl(HttpMethod.Get, url, null, r.Timeout.TotalMilliSeconds);
		return data;
	}

	public class SingleAttackResult
	{
		private SingleAttackResult(string error)
		{
			this.Error = error;
		}

		private SingleAttackResult(TimeSpan result)
		{
			this.Result = result;
		}

		public string? Error { get; private set; }
		public TimeSpan Result { get; private set; }

		public static SingleAttackResult FromSuccess(TimeSpan sp)
		{
			return new SingleAttackResult(sp);
		}

		public static SingleAttackResult FromError(String error)
		{
			return new SingleAttackResult(error);
		}
	}


	public static async Task<SingleAttackResult> SingleAttack(Request r)
	{
		try
		{
			var start = DateTime.Now;
			var data = await MakeRequest(r);
			// todo(Gustav): verify data...
			if (data.Status != HttpStatusCode.OK)
			{
				return SingleAttackResult.FromError(data.Body);
			}
			else
			{
				var end = DateTime.Now;
				var time = end.Subtract(start);
				return SingleAttackResult.FromSuccess(time);
			}
		}
		catch (Exception xx)
		{
			string builder = string.Empty;

			var x = xx;
			while (x != null)
			{
				builder += x.Message + "\r\n";
				x = x.InnerException;
			}

			return SingleAttackResult.FromError($"Exception: {builder}");
		}
	}

	public static async Task<AttackResult?> Attack(Root root, Request r)
	{
		// silently ignore double commands
		if (r.IsWorking == true) { return null; }

		using var locked = new WorkingLock(r);

		var attack = root.Attack.Clone();
		var ret = new AttackResult();

		if (attack.AtTheSameTime)
		{
			var tasks = Enumerable.Range(0, attack.Count).Select(i => SingleAttack(r));
			var results = await Task.WhenAll(tasks);
			foreach (var callResult in results)
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

	public static string TimeSpanToString(TimeSpan t)
	{
		static int Floor(double totalMinutes)
		{
			return (int)Math.Floor(totalMinutes);
		}

		static string S(int x)
		{
			if (x == 0) { return string.Empty; }
			else { return "s"; }
		}

		var r = new List<string>();

		if (t.TotalMinutes > 1)
		{
			int min = Floor(t.TotalMinutes);
			r.Add($"{min} minute{S(min)}");
			t = t.Subtract(TimeSpan.FromMinutes(min));
		}

		if (t.TotalSeconds > 1)
		{
			int min = Floor(t.TotalSeconds);
			r.Add($"{min} second{S(min)}");
			t = t.Subtract(TimeSpan.FromSeconds(min));
		}

		if (t.TotalMilliseconds > 1)
		{
			int min = Floor(t.TotalMilliseconds);
			r.Add($"{min} millisecond{S(min)}");
		}

		if (r.Count == 0)
		{
			return "Less than 1 ms";
		}
		else
		{
			return string.Join(" ", r);
		}
	}
}

public class WorkingLock : IDisposable
{
	private readonly Request r;

	public WorkingLock(Request r)
	{
		this.r = r;
		r.IsWorking = true;
	}

	public void Dispose()
	{
		r.IsWorking = false;
	}
}

