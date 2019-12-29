using Node.Version.Management.Exceptions;
using Node.Version.Management.Extensions;
using Node.Version.Management.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Node.Version.Management.Utils
{
    public static class InputUtils
    {
        public static NodeVersion GetTypedVersion(string message)
        {
            NodeVersion? version = null;
            do
            {
                Console.WriteLine(message);
                Console.Write(">>");
                var typedVersion = Console.ReadLine();
                typedVersion = typedVersion.StartsWith("v") ? typedVersion : $"v{typedVersion}";

                string pattern = @"^v[0-9]+\.[0-9]+\.[0-9]*";
                RegexOptions options = RegexOptions.Singleline;

                if(Regex.IsMatch(typedVersion, pattern, options))
                {
                    version = typedVersion.GetVersion();
                }
            } while (!version.HasValue);

            return version.Value;
        }
    }
}
