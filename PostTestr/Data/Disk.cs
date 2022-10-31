using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
namespace PostTestr.Data;

public static class Disk
{
    private static string SettingsFile
    {
        get
        {
            return GetLocalFile("settings.json");
        }
    }

    public static string RequestsFile
    {
        get
        {
            return GetLocalFile("requests.json");
        }
    }

    private static string GetLocalFile(string fileName)
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(root, "PostTestr");
        if (Directory.Exists(folder) == false)
        {
            Directory.CreateDirectory(folder);
        }
        return Path.Combine(folder, fileName);
    }

    private static Data Load(string file)
    {
        static T ReadFile<T>(string path)
        {
            var data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(data);
        }
        static RequestGroup TransformGroup(Saved.Group g)
        {
            var isbuiltin = g.File == Saved.Group.BuiltinFile;
            var file = isbuiltin ? RequestsFile : g.File;
            var json = ReadFile<Saved.RequestsFile>(file);
            var req = new ObservableCollection<Request>(json.Requests);
            for(int i=0; i < g.Responses.Length; i+=1)
            {
                req[i].Response = g.Responses[i];
            }
            return new RequestGroup
            {
                Requests = req,
                File = file,
                Name = g.Name,
                Builtin = isbuiltin, 
                SelectedRequest = req[g.SelectedRequest]
            };
        }

        static Request FindRequest(ObservableCollection<RequestGroup> groups, Saved.RequestInGroup i)
        {
            if(i == null) {  return null; }
            return groups[i.Group].Requests[i.Request];
        }

        var container = ReadFile<Saved.Root>(file);
        var rc = container.Groups.Select(TransformGroup);
        var groups = new ObservableCollection<RequestGroup>(rc);
        return new Data
        {
            Groups = groups,
            SelectedGroup = container.SelectedGroup == -1 ? null : groups[container.SelectedGroup],
            LeftCompare = FindRequest(groups, container.LeftCompare),
            RightCompare  = FindRequest(groups, container.RightCompare)
        };
    }

    private static void Save(Data data, string file)
    {
        static void WriteJson<T>(T jsonFile, string file)
        {
            var jsonData = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
            File.WriteAllText(file, jsonData);
        }
        static Saved.Group TransformGroup(RequestGroup g)
        {
            var rf = new Saved.RequestsFile { Requests = g.Requests.ToArray() };
            WriteJson(rf, g.File);

            return new Saved.Group
            {
                File = g.Builtin ? Saved.Group.BuiltinFile : g.File,
                Responses = g.Requests.Select(x => x.Response).ToArray(),
                Name = g.Name,
                SelectedRequest = g.Requests.IndexOf(g.SelectedRequest)
            };
        }

        Saved.RequestInGroup FindRequest(Request r)
        {
            if(r == null) { return null; }

            for(int g=0; g<data.Groups.Count; g+=1)
            {
                var index = data.Groups[g].Requests.IndexOf(r);
                if(index != -1) { continue; }
                return new Saved.RequestInGroup { Group = g, Request = index };
            }

            return null;
        }

        var jsonFile = new Saved.Root
        {
            Groups = data.Groups.Select(TransformGroup).ToArray(),
            SelectedGroup = data.Groups.IndexOf(data.SelectedGroup),
            LeftCompare = FindRequest(data.LeftCompare),
            RightCompare = FindRequest(data.RightCompare)
        };
        WriteJson(jsonFile, file);
    }

    public static Data LoadOrCreateNew()
    {
        if (File.Exists(SettingsFile))
        {
            return Load(SettingsFile);
        }
        else
        {
            var r = new Data { };
            r.CreateBuiltinIfMissing();
            r.AddNewRequest();
            return r;
        }
    }

    public static void Save(Data data)
    {
        Save(data, SettingsFile);
    }
}
