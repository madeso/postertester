using System.Net.Http;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PosterTester.Data;


public interface ContentType
{
    string Name { get; }
    Saved.ContentType SavedType { get; }

    HttpContent CreateContent(Request request);
}

public class ContentTypeJson : ContentType
{
    public string Name => "Json";

    public Saved.ContentType SavedType => Saved.ContentType.Json;

    public static ContentTypeJson Instance { get; } = new ContentTypeJson();

    public HttpContent CreateContent(Request request)
    {
        return Logic.WithJsonContent(request.TextContent);
    }
}

public class ContentTypeText : ContentType
{
    public string Name => "Text";

    public Saved.ContentType SavedType => Saved.ContentType.Text;

    public static ContentTypeText Instance { get; } = new ContentTypeText();

    public HttpContent CreateContent(Request request)
    {
        return Logic.WithStringContent(request.TextContent);
    }
}