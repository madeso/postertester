using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PosterLib.Domain;

namespace PosterLib.Saved;

[JsonConverter(typeof(StringEnumConverter))]
public enum ContentType
{
	[EnumMember(Value = "json")]
	Json,

	[EnumMember(Value = "text")]
	Text
}

public class Request
{
	[JsonProperty("url")]
	public string? Url { get; set; }

	[JsonProperty("title")]
	public string? Title { get; set; }

	[JsonProperty("method")]
	public HttpMethod? Method { get; set; }

	[JsonProperty("post_type")]
	public ContentType? ContentType { get; set; }

	[JsonProperty("post")]
	public string? TextContent { get; set; }

	[JsonProperty("guid")]
	public string? Guid { get; set; }

	[JsonProperty("timeout_ms")]
	public long? TimeoutInMs { get; set; }

	[JsonProperty("use_auth")]
	public bool? UseAuth { get; set; }
}

public class RequestsFile
{
	[JsonProperty("requests")]
	public Request[]? Requests { get; set; }

	[JsonProperty("guid")]
	public string? Guid { get; internal set; }
}

public class HeaderRow
{
	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("values")]
	public string[]? Values { get; set; }
}

public class Headers
{
	[JsonProperty("rows")]
	public HeaderRow[]? Rows { get; set; }
}

public class Attack
{
	[JsonProperty("at_the_same_time")]
	public bool AttackAtTheSameTime { get; internal set; }

	[JsonProperty("count")]
	public int AttackCount { get; internal set; }

	[JsonProperty("got_errors")]
	public string[]? AttackErrors { get; internal set; }

	[JsonProperty("got_result")]
	public double[]? AttackResult { get; internal set; }
}

public class Response
{
	[JsonProperty("body")]
	public string? Body { get; set; }

	[JsonProperty("status")]
	public int Status { get; set; }

	[JsonProperty("seconds")]
	public double Seconds { get; internal set; }

	[JsonProperty("response_headers")]
	public Headers? ResponseHeaders { get; internal set; }
}

public class Result
{
	[JsonProperty("response")]
	public Response? Response { get; internal set; }

	[JsonProperty("attack")]
	public Attack? Attack { get; internal set; }

	[JsonProperty("guid")]
	public string? Guid { get; internal set; }
}

public class Group
{
	[JsonProperty("selected_request")]
	public int SelectedRequest { get; set; }

	[JsonProperty("result")]
	public Result[]? Results { get; set; }

	[JsonProperty("file")]
	public string? File { get; set; }

	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("guid")]
	public string? Guid { get; internal set; }

	[JsonProperty("bearer_token")]
	public string? BearerToken { get; internal set; }

	public const string BuiltinFile = "<my requests>";
}

public class RequestInGroup
{
	[JsonProperty("group")]
	public int Group { get; set; }

	[JsonProperty("request")]
	public int Request { get; set; }
}

public class Root
{
	[JsonProperty("groups")]
	public Group[]? Groups { get; set; }

	[JsonProperty("selected_group")]
	public int SelectedGroup { get; set; }

	[JsonProperty("left_compare")]
	public RequestInGroup? LeftCompare { get; set; }

	[JsonProperty("right_compare")]
	public RequestInGroup? RightCompare { get; set; }

	[JsonProperty("format_response")]
	public bool FormatResponse { get; set; } = false;

	[JsonProperty("attack_count")]
	public int AttackCount { get; set; }

	[JsonProperty("selected_response_tab")]
	public int SelectedResponseTab { get; set; }

	[JsonProperty("attack_at_the_same_time")]
	public bool AttackAtTheSameTime { get; set; }

	[JsonProperty("bin_size")]
	public double BinSize { get; set; } = 50.0;
}

public class AuthFile
{
	[JsonProperty("bearer_token")]
	public string? BearerToken { get; set; }
}

