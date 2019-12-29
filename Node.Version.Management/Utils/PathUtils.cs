using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Node.Version.Management.Utils
{
    public static class PathUtils
    {
        public static string Combine(params string[] pathParts)
        {
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (pathParts[i].EndsWith('\\'))
                {
                    pathParts[i] = pathParts[i].Remove(pathParts[i].Length - 1);
                }
            }
            return string.Join(Path.DirectorySeparatorChar, pathParts);
        }

        public static string VersionsPath = Combine(Directory.GetCurrentDirectory(), "versions");
    }
}
