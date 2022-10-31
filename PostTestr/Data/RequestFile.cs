﻿using Newtonsoft.Json;
namespace PostTestr.Data.Saved;

public class Group
{
    [JsonProperty("requests")]
    public Request[] Requests { get; set; }

    [JsonProperty("selected_request")]
    public int SelectedRequest { get; set; }
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
}