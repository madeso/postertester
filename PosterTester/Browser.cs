using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterTester;

internal static class Browser
{
	internal static void BrowseFolder(string dir)
	{
		var startInfo = new ProcessStartInfo
		{
			FileName = "explorer.exe",
			Arguments = dir
		};
		Process.Start(startInfo);
	}
}
