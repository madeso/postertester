using PostTestr.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PostTestr;

public static class DiffTool
{
    public static void LaunchDiff(Response leftData, Response rightData, string leftName="left.json", string rightName="right.json")
    {
        // todo(Gustav) read from registry
        // HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Thingamahoochie\WinMerge
        // Executable
        var paths = new List<string>
            {
                @"C:\Program Files (x86)\WinMerge\WinMergeU.exe"
            };

        foreach (var p in paths)
        {
            if (File.Exists(p) == false)
            {
                continue;
            }

            var root = Path.GetTempPath();
            var leftPath = Path.Join(root, leftName);
            var rightPath = Path.Join(root, rightName);
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
