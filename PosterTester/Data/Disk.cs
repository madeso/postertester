using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
// using static System.Net.WebRequestMethods;

namespace PosterTester.Data;

public static class Disk
{
    private static string PathToSettings
    {
        get
        {
            return GetLocalFile("settings.json");
        }
    }

    public static string PathToMyRequests
    {
        get
        {
            return GetLocalFile("my-requests.json");
        }
    }

    private static string GetLocalFile(string fileName)
	{
		string folder = GetAppFolder();
		return Path.Combine(folder, fileName);
	}

	public static string GetAppFolder()
	{
		string root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		string folder = Path.Combine(root, "PosterTester");
		if (Directory.Exists(folder) == false)
		{
			Directory.CreateDirectory(folder);
		}

		return folder;
	}

	private static T ReadFile<T>(string path)
    {
        string data = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(data);
    }

    internal static ObservableCollection<Request> LoadRequests(string file)
    {
        static ContentType MakeType(Saved.ContentType t)
        {
            return t switch
            {
                Saved.ContentType.Json => ContentTypeJson.Instance,
                Saved.ContentType.Text => ContentTypeText.Instance,
                _ => throw new NotImplementedException(),
            };
        }
        static Request ToReq(Saved.Request r)
        {
            return new Request
            {
                Url = r.Url,
                Title = r.Title,
                Method = r.Method,
                ContentType = MakeType(r.ContentType),
                TextContent = r.TextContent
            };
        }

        var json = ReadFile<Saved.RequestsFile>(file);
        var req = new ObservableCollection<Request>(json.Requests.Select(ToReq));
        return req;
    }

    private static Data Load(string file)
    {
		static Headers TransformHeaders(Saved.Headers src)
		{
			if(src == null) { return new Headers(); }
			return new Headers { Rows = src.Rows.Select(x => new HeaderRow { Name=x.Name, Values = x.Values }).ToArray() };
		}

		static RequestGroup TransformGroupOrNull(Saved.Group g)
        {
            bool isbuiltin = g.File == Saved.Group.BuiltinFile;
            string file = isbuiltin ? PathToMyRequests : g.File;
			if(File.Exists(file) == false)
			{
				if(isbuiltin == false)
				{
					return null;
				}
				else
				{
					return Data.CreateDefaultGroup();
				}
			}
			else
			{
				var req = LoadRequests(file);
				if (g.Responses != null)
				{
					for (int i = 0; i < g.Responses.Length; i += 1)
					{
						var re = g.Responses[i];
						if (re == null) { continue; }
						req[i].Response = new Response(IntToStatus(re.Status), re.Body, TransformHeaders(re.ResponseHeaders)) { Time = TimeSpan.FromSeconds(re.Seconds) };
					}
				}
				return new RequestGroup
				{
					Requests = req,
					File = file,
					Name = g.Name,
					Builtin = isbuiltin,
					SelectedRequest = g.SelectedRequest == -1 ? null : req[g.SelectedRequest]
				};
			}
        }

        static RequestGroup FindGroup(ObservableCollection<RequestGroup> groups, Saved.RequestInGroup i)
        {
            if (i == null) { return null; }
            if (i.Group == -1) { return null; }
            return groups[i.Group];
        }

        static Request FindRequest(ObservableCollection<RequestGroup> groups, Saved.RequestInGroup i)
        {
            if (i == null) { return null; }
            if (i.Group == -1) { return null; }
            if (i.Request == -1) { return null; }
            return groups[i.Group].Requests[i.Request];
        }

        var container = ReadFile<Saved.Root>(file);
        var rc = container.Groups.Select(TransformGroupOrNull).Where(x => x != null);
        var groups = new ObservableCollection<RequestGroup>(rc);
        return new Data
        {
            Groups = groups,
            SelectedGroup = container.SelectedGroup == -1 ? null : groups[container.SelectedGroup],
            LeftGroup = FindGroup(groups, container.LeftCompare),
            RightGroup = FindGroup(groups, container.RightCompare),
            LeftCompare = FindRequest(groups, container.LeftCompare),
            RightCompare = FindRequest(groups, container.RightCompare),
            FormatResponse = container.FormatResponse
        };
    }

	private static HttpStatusCode IntToStatus(int status)
    {
        return (HttpStatusCode)status;
    }
    private static int StatusToInt(HttpStatusCode status)
    {
        return (int)status;
    }

    private static void WriteJson<T>(T jsonFile, string file)
    {
        string jsonData = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
        File.WriteAllText(file, jsonData);
    }

    internal static void SaveGroup(RequestGroup g)
    {
        static Saved.Request ToReq(Request r)
        {
            return new Saved.Request
            {
                Url = r.Url,
                Title = r.Title,
                Method = r.Method,
                ContentType = r.ContentType.SavedType,
                TextContent = r.TextContent
            };
        }
        var rf = new Saved.RequestsFile { Requests = g.Requests.Select(ToReq).ToArray() };
        WriteJson(rf, g.File);
    }

    private static void Save(Data data, string file)
    {
		static Saved.Headers TransformHeaders(Headers src)
		{
			return new Saved.Headers { Rows = src.Rows.Select(x => new Saved.HeaderRow { Name = x.Name, Values = x.Values }).ToArray() };
		}

		static Saved.Group TransformGroup(RequestGroup g)
        {
            SaveGroup(g);

            return new Saved.Group
            {
                File = g.Builtin ? Saved.Group.BuiltinFile : g.File,
                Responses = g.Requests.Select(x => x.Response == null ? null : new Saved.Response {
					Body = x.Response.Body,
					Status = StatusToInt(x.Response.Status),
					Seconds = x.Response.Time.TotalSeconds,
					ResponseHeaders = TransformHeaders(x.Response.ResponseHeaders)
				}).ToArray(),
                Name = g.Name,
                SelectedRequest = g.Requests.IndexOf(g.SelectedRequest)
            };
        }

        Saved.RequestInGroup FindRequest(RequestGroup g, Request r)
        {
            if (g == null) { return null; }
            if (r == null) { return null; }

            int gi = data.Groups.IndexOf(g);
            int ri = g.Requests.IndexOf(r);

            if (gi == -1 || ri == -1) { return null; }

            return new Saved.RequestInGroup { Group = gi, Request = ri };
        }

        var jsonFile = new Saved.Root
        {
            Groups = data.Groups.Select(TransformGroup).ToArray(),
            SelectedGroup = data.Groups.IndexOf(data.SelectedGroup),
            LeftCompare = FindRequest(data.LeftGroup, data.LeftCompare),
            RightCompare = FindRequest(data.RightGroup, data.RightCompare),
            FormatResponse = data.FormatResponse
        };
        WriteJson(jsonFile, file);
    }

    public static Data LoadOrCreateNew()
    {
        if (File.Exists(PathToSettings))
        {
            return Load(PathToSettings);
        }
        else
        {
            var r = new Data { };
            r.CreateBuiltinIfMissing();
            return r;
        }
    }

    public static void Save(Data data)
    {
        Save(data, PathToSettings);
    }
}