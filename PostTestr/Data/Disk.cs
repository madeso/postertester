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
            var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(root, "PostTestr");
            if(Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, "settings.json");
        }
    }

    private static Data Load(string file)
    {
        static RequestGroup TransformGroup(RequestFileGroup g)
        {
            var req = new ObservableCollection<Request>(g.Requests);
            return new RequestGroup
            {
                Requests = req,
                SelectedRequest = req[g.SelectedRequest]
            };
        }

        static Request FindRequest(ObservableCollection<RequestGroup> groups, RequestInGroup i)
        {
            if(i == null) {  return null; }
            return groups[i.Group].Requests[i.Request];
        }

        var data = File.ReadAllText(file);
        var container = JsonConvert.DeserializeObject<RequestFile>(data);
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
        static RequestFileGroup TransformGroup(RequestGroup g)
        {
            return new RequestFileGroup
            {
                Requests = g.Requests.ToArray(),
                SelectedRequest = g.Requests.IndexOf(g.SelectedRequest)
            };
        }

        RequestInGroup FindRequest(Request r)
        {
            if(r == null) { return null; }

            for(int g=0; g<data.Groups.Count; g+=1)
            {
                var index = data.Groups[g].Requests.IndexOf(r);
                if(index != -1) { continue; }
                return new RequestInGroup { Group = g, Request = index };
            }

            return null;
        }

        var jsonFile = new RequestFile
        {
            Groups = data.Groups.Select(TransformGroup).ToArray(),
            SelectedGroup = data.Groups.IndexOf(data.SelectedGroup),
            LeftCompare = FindRequest(data.LeftCompare),
            RightCompare = FindRequest(data.RightCompare)
        };
        var jsonData = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
        File.WriteAllText(file, jsonData);
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
            r.AddNewRequest();
            return r;
        }
    }

    public static void Save(Data data)
    {
        Save(data, SettingsFile);
    }
}
