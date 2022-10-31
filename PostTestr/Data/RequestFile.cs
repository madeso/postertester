using Newtonsoft.Json;
namespace PostTestr.Data;

public class RequestFile
{
    [JsonProperty("requests")]
    public Request[] Requests { get; set; }

    [JsonProperty("selected_request")]
    public int SelectedRequest { get; set; }

    [JsonProperty("left_compare")]
    public int LeftCompare { get; set; }

    [JsonProperty("right_compare")]
    public int RightCompare { get; set; }
}
