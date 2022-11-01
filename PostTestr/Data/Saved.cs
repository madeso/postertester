using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PostTestr.Data.Saved;

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
    public string Url { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("method")]
    public HttpMethod Method { get; set; }

    [JsonProperty("post_type")]
    public ContentType ContentType { get; set; }

    [JsonProperty("post")]
    public string TextContent { get; set; }
}

public class RequestsFile
{
    [JsonProperty("requests")]
    public Request[] Requests { get; set; }
}

public class Response
{
    [JsonProperty("body")]
    public string Body { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("seconds")]
    public double Seconds { get; internal set; }
}

public class Group
{
    [JsonProperty("selected_request")]
    public int SelectedRequest { get; set; }

    [JsonProperty("responses")]
    public Response[] Responses { get; set; }

    [JsonProperty("file")]
    public string File { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

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
    public Group[] Groups { get; set; }

    [JsonProperty("selected_group")]
    public int SelectedGroup { get; set; }

    [JsonProperty("left_compare")]
    public RequestInGroup LeftCompare { get; set; }

    [JsonProperty("right_compare")]
    public RequestInGroup RightCompare { get; set; }

    [JsonProperty("format_response")]
    public bool FormatResponse { get; set; } = false;
}
