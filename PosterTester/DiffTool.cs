using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using PosterLib.Data;

namespace PosterTester;

public static class DiffTool
{
	internal static string? Compare(Root root)
	{
		var lhs = root.LeftCompare;
		var rhs = root.RightCompare;
		if (lhs == null || rhs == null)
		{
			return "Either left or right aren't selected!";
		}

		if (lhs.Response == null || rhs.Response == null)
		{
			return "Either left or right have missing response";
		}

		LaunchDiff(lhs.Response, rhs.Response);
		return null;
	}

	public static void LaunchDiff(Response leftData, Response rightData, string leftName = "left.json", string rightName = "right.json")
	{
		// todo(Gustav) read from registry
		// HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Thingamahoochie\WinMerge
		// Executable
		var paths = new List<string>
			{
				@"C:\Program Files (x86)\WinMerge\WinMergeU.exe"
			};

		foreach (string p in paths)
		{
			if (File.Exists(p) == false)
			{
				continue;
			}

			string root = Path.GetTempPath();
			string leftPath = Path.Join(root, leftName);
			string rightPath = Path.Join(root, rightName);
			File.WriteAllText(leftPath, leftData.Body);
			File.WriteAllText(rightPath, rightData.Body);

			RunCmd(p, $"{leftPath} {rightPath}");
			return;
		}

		MessageBox.Show("Unable to find winmerge...", "Not installed or bug", MessageBoxButton.OK, MessageBoxImage.Warning);
	}

	private static void RunCmd(string app, string arg)
	{
		var p = new Process();
		p.StartInfo.FileName = app;
		p.StartInfo.Arguments = arg;
		p.Start();
	}
}
