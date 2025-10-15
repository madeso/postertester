using System.Collections.Generic;
using Newtonsoft.Json;

namespace PosterLib.Saved;

internal class PostmanFile
{
	[JsonProperty("variable")]
	public List<PostmanVariable> Variables { get; set; } = new();

	[JsonProperty("item")]
	public List<PostmanItem> Items { get; set; } = new();
}

internal class PostmanItem
{
	[JsonProperty("name")]
	public string Name { get; set; } = string.Empty;

	[JsonProperty("request")]
	public PostmanRequest Request { get; set; } = new();
}

internal class PostmanRequest
{
	[JsonProperty("method")]
	public string Method { get; set; } = string.Empty;

	[JsonProperty("body")]
	public PostmanBody Body { get; set; } = new();

	[JsonProperty("url")]
	public PostmanUrl Url { get; set; } = new();
}

internal class PostmanUrl
{
	[JsonProperty("raw")]
	public string Raw { get; set; } = string.Empty;
}

internal class PostmanBody
{
	[JsonProperty("mode")]
	public string Mode { get; set; } = string.Empty;

	[JsonProperty("raw")]
	public string Raw { get; set; } = string.Empty;
}

internal class PostmanVariable
{
	[JsonProperty("key")]
	public string Key { get; set; } = string.Empty;

	[JsonProperty("value")]
	public string Value { get; set; } = string.Empty;
}


