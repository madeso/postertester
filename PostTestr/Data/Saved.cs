using Newtonsoft.Json;
namespace PostTestr.Data.Saved;


public class RequestsFile
{
    [JsonProperty("requests")]
    public Request[] Requests { get; set; }
}


public class Group
{
    [JsonProperty("selected_request")]
    public int SelectedRequest { get; set; }

    [JsonProperty("responses")]
    public string[] Responses { get; set; }

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
