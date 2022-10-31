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
            return Path.Combine(folder, "settings.json"); ;
        }
    }

    private static Data Load(string file)
    {
        var data = File.ReadAllText(file);
        var container = JsonConvert.DeserializeObject<RequestFile>(data);
        var requests = new ObservableCollection<Request>(container.Requests);
        return new Data
        {
            Requests = requests,
            SelectedRequest = requests[container.SelectedRequest],
            LeftCompare = requests[container.LeftCompare],
            RightCompare  = requests[container.RightCompare]
        };
    }

    private static void Save(Data data, string file)
    {
        var jsonFile = new RequestFile
        {
            Requests = data.Requests.ToArray(),
            SelectedRequest = data.Requests.IndexOf(data.SelectedRequest),
            LeftCompare = data.Requests.IndexOf(data.LeftCompare),
            RightCompare = data.Requests.IndexOf(data.RightCompare)
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
