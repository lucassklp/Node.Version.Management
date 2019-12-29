using Node.Version.Management.Extensions;
using Node.Version.Management.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Node.Version.Management.Utils
{
    public static class NodeUtils
    {
        private static string nodePath = null;

        public static bool IsNodeInstalled()
        {
            return !string.IsNullOrWhiteSpace(GetNodePath());
        }

        public static string GetNodePath()
        {
            if(nodePath == null)
            {
                var paths = Environment.GetEnvironmentVariable("Path")
                    .Split(";")
                    .ToList();

                nodePath = paths.FirstOrDefault(path => File.Exists(PathUtils.Combine(path, "node.exe")));
            }
            return nodePath;

        }


        public static NodeVersion GetCurrentNodeVersion()
        {
            var nodeProcess = new Process();
            nodeProcess.StartInfo.UseShellExecute = false;
            nodeProcess.StartInfo.RedirectStandardOutput = true;
            nodeProcess.StartInfo.RedirectStandardError = true;
            nodeProcess.StartInfo.RedirectStandardInput = true;
            nodeProcess.StartInfo.FileName = PathUtils.Combine(GetNodePath(), "node.exe");
            nodeProcess.StartInfo.Arguments = @"--version";
            nodeProcess.Start();

            var output = nodeProcess.StandardOutput.ReadToEnd();
            return output.GetVersion();
        }
    }
}
