using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using PosterTester.Data;
using PosterTester.Extensions;

namespace PosterTester.Domain;

public static class Disk
{
	// if there was a error generating the sources for any files,
	// this makes sure we don't write partial data
	internal class Writer
	{
		public void Add(string path, string contents)
		{
			files.Add(new(path, contents));
		}

		internal class ToWrite
		{
			public ToWrite(string path, string contents)
			{
				this.Path = path;
				this.Contents = contents;
			}

			public string Path { get; internal set; }
			public string Contents { get; internal set; }
		}

		private readonly List<ToWrite> files = new();

		public void WriteAll()
		{
			foreach (var file in files)
			{
				File.WriteAllText(file.Path, file.Contents);
			}
		}
	}

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
		var parsed = JsonConvert.DeserializeObject<T>(data);
		if(parsed == null)
		{
			throw new Exception(@"Failed to parse json {path}");
		}
		return parsed;
	}


	internal class RequestsWithGuid
	{
		public RequestsWithGuid(ObservableCollection<Request> requests, Guid guid)
		{
			this.Requests = requests;
			this.Guid = guid;
		}

		public ObservableCollection<Request> Requests { get; private set; }
		public Guid Guid { get; private set; }
	}


	internal static RequestsWithGuid LoadRequests(string file)
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
				Guid = FromSavedGuid(r.Guid),
				Url = r.Url,
				Title = r.Title,
				Method = r.Method,
				ContentType = MakeType(r.ContentType),
				TextContent = r.TextContent
			};
		}

		var json = ReadFile<Saved.RequestsFile>(file);
		var req = json.Requests?.Select(ToReq);
		return new RequestsWithGuid(req.ToObservableCollectionOrEmpty(), FromSavedGuid(json?.Guid));
	}

	private static Root Load(string file)
	{
		static Headers TransformHeaders(Saved.Headers? src)
		{
			if (src == null) { return new Headers(); }
			return new Headers { Rows = src.Rows.Select(x => new Headers.Row(x.Name, x.Values)).ToArray() };
		}

		static RequestGroup? TransformGroupOrNull(Saved.Group sourceGroup)
		{
			bool isbuiltin = sourceGroup.File == Saved.Group.BuiltinFile;
			string file = isbuiltin ? PathToMyRequests : sourceGroup.File;
			if (File.Exists(file) == false)
			{
				if (isbuiltin == false)
				{
					return null;
				}
				else
				{
					return Data.Root.CreateDefaultGroup();
				}
			}
			else
			{
				var loadedRequests = LoadRequests(file);
				var requests = loadedRequests.Requests;
				if (sourceGroup.Results != null)
				{
					var reqDict = requests.ToDictionary(x => ToSaved(x.Guid));
					foreach (var saved in sourceGroup.Results)
					{
						if (saved == null) { continue; }
						if(saved.Guid == null) { continue; }

						var req = reqDict[saved.Guid];
						if (req == null) { continue; }

						var savedResponse = saved.Response;
						if (savedResponse != null)
						{
							var status = IntToStatus(savedResponse.Status);
							string body = savedResponse.Body ?? string.Empty;
							var headers = TransformHeaders(savedResponse.ResponseHeaders);
							req.Response = new Response(status, body, headers)
							{
								Time = TimeSpan.FromSeconds(savedResponse.Seconds),
								ParentRequest = req
							};
						}

						var attack = saved.Attack;
						if (attack != null && attack.AttackResult != null)
						{
							req.AttackOptions = new AttackOptions { AtTheSameTime = attack.AttackAtTheSameTime, Count = attack.AttackCount };
							var spans = attack.AttackResult.Select(x => TimeSpan.FromMilliseconds(x));
							req.AttackResult = new AttackResult { Errors = attack.AttackErrors.ToObservableCollectionOrEmpty(), Result = spans.ToObservableCollectionOrEmpty() };
						}
					}
				}
				var ret = new RequestGroup
				{
					Requests = requests,
					File = file,
					Name = sourceGroup.Name,
					Builtin = isbuiltin,
					Guid = FromSavedGuid(sourceGroup.Guid),
					SelectedRequest = sourceGroup.SelectedRequest == -1 ? null : requests[sourceGroup.SelectedRequest]
				};
				ret.LinkParents();
				return ret;
			}
		}

		static RequestGroup? FindGroup(ObservableCollection<RequestGroup> groups, Saved.RequestInGroup? i)
		{
			if (i == null) { return null; }
			if (i.Group == -1) { return null; }
			return groups[i.Group];
		}

		static Request? FindRequest(ObservableCollection<RequestGroup> groups, Saved.RequestInGroup? i)
		{
			if (i == null) { return null; }
			if (i.Group == -1) { return null; }
			if (i.Request == -1) { return null; }
			return groups[i.Group].Requests[i.Request];
		}

		var container = ReadFile<Saved.Root>(file);
		var rc = container.Groups?.Select(TransformGroupOrNull).NotNull();
		var groups = rc.ToObservableCollectionOrEmpty();
		return new Root
		{
			BinSize = container.BinSize,
			Groups = groups,
			SelectedGroup = container.SelectedGroup == -1 ? null : groups[container.SelectedGroup],
			LeftGroup = FindGroup(groups, container.LeftCompare),
			RightGroup = FindGroup(groups, container.RightCompare),
			LeftCompare = FindRequest(groups, container.LeftCompare),
			RightCompare = FindRequest(groups, container.RightCompare),
			FormatResponse = container.FormatResponse,
			Attack = new AttackOptions { AtTheSameTime = container.AttackAtTheSameTime, Count = container.AttackCount },
			SelectedResponseTab = container.SelectedResponseTab
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

	private static void WriteJson<T>(Writer writer, T jsonFile, string file)
	{
		string jsonData = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
		writer.Add(file, jsonData);
	}

	internal static void SaveGroup(Writer writer, RequestGroup g)
	{
		static Saved.Request ToReq(Request r)
		{
			return new Saved.Request
			{
				Guid = ToSaved(r.Guid),
				Url = r.Url,
				Title = r.Title,
				Method = r.Method,
				ContentType = r.ContentType.SavedType,
				TextContent = r.TextContent
			};
		}
		var rf = new Saved.RequestsFile { Guid = ToSaved(g.Guid), Requests = g.Requests.Select(ToReq).ToArray() };
		VerifyGuids(rf.Requests.Select(x => x.Guid));
		WriteJson(writer, rf, g.File);
	}

	private static void VerifyGuids(IEnumerable<string?> itguids)
	{
		var guids = itguids.ToArray();
		var dist = guids.Distinct().Count();
		if (dist != guids.Length)
		{
			throw new Exception("Guids were not distinct");
		}
	}

	private static void Save(Writer writer, Root data, string file)
	{
		static Saved.Headers TransformHeaders(Headers src)
		{
			return new Saved.Headers { Rows = src.Rows.Select(x => new Saved.HeaderRow { Name = x.Name, Values = x.Values }).ToArray() };
		}

		static Saved.Group TransformGroup(Writer writer, RequestGroup g)
		{
			SaveGroup(writer, g);

			return new Saved.Group
			{
				Guid = ToSaved(g.Guid),
				File = g.Builtin ? Saved.Group.BuiltinFile : g.File,
				Results = g.Requests.Select(x =>
				new Saved.Result
				{
					Guid = ToSaved(x.Guid),
					Attack = x.AttackOptions == null || x.AttackResult == null ? null : new Saved.Attack
					{
						AttackAtTheSameTime = x.AttackOptions.AtTheSameTime,
						AttackCount = x.AttackOptions.Count,
						AttackErrors = x.AttackResult.Errors.ToArray(),
						AttackResult = x.AttackResult.Result.Select(x => x.TotalMilliseconds).ToArray()
					},
					Response = x.Response == null ? null : new Saved.Response
					{
						Body = x.Response.Body,
						Status = StatusToInt(x.Response.Status),
						Seconds = x.Response.Time.TotalSeconds,
						ResponseHeaders = TransformHeaders(x.Response.ResponseHeaders)
					}
				}).ToArray(),
				Name = g.Name,
				SelectedRequest = g.Requests.IndexOf(g.SelectedRequest)
			};
		}

		Saved.RequestInGroup? FindRequest(RequestGroup? g, Request? r)
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
			BinSize = data.BinSize,
			Groups = data.Groups.Select(x => TransformGroup(writer, x)).ToArray(),
			SelectedGroup = FindGroup(data.Groups, data.SelectedGroup),
			LeftCompare = FindRequest(data.LeftGroup, data.LeftCompare),
			RightCompare = FindRequest(data.RightGroup, data.RightCompare),
			FormatResponse = data.FormatResponse,
			AttackAtTheSameTime = data.Attack.AtTheSameTime,
			AttackCount = data.Attack.Count,
			SelectedResponseTab = data.SelectedResponseTab
		};
		VerifyGuids(jsonFile.Groups.Select(x => x.Guid));
		WriteJson(writer, jsonFile, file);

		static int FindGroup(ObservableCollection<RequestGroup> groups, RequestGroup? selectedGroup)
		{
			if (selectedGroup == null) { return -1; }
			return groups.IndexOf(selectedGroup);
		}
	}

	private static string ToSaved(Guid guid)
	{
		/*
		 * D  32 digits separated by hyphens:
			  00000000-0000-0000-0000-000000000000
		 */
		return guid.ToString("D");
	}

	private static Guid FromSavedGuid(string? guid)
	{
		if (string.IsNullOrEmpty(guid))
		{
			return Guid.NewGuid();
		}
		else
		{
			return Guid.Parse(guid);
		}
	}

	public static Root LoadOrCreateNew()
	{
		if (File.Exists(PathToSettings))
		{
			return Load(PathToSettings);
		}
		else
		{
			var r = new Root { };
			r.CreateBuiltinIfMissing();
			return r;
		}
	}

	public static void Save(Root data)
	{
		Writer writer = new();
		Save(writer, data, PathToSettings);
		writer.WriteAll();
	}
}
