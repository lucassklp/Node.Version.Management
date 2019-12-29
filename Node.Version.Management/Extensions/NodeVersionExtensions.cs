using Node.Version.Management.Exceptions;
using Node.Version.Management.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Version.Management.Extensions
{
    public static class NodeVersionExtensions
    {

        public static string AsString(this NodeVersion version)
        {
            return version.ToString().Replace('_', '.');
        }

        public static NodeVersion GetVersion(this string version)
        {
            var enumVersion = Enum.Parse(typeof(NodeVersion), version.Replace('.', '_')) as NodeVersion?;
            if(!enumVersion.HasValue)
            {
                throw new NotSupportedVersionException();
            }
            return enumVersion.Value;
        }
    }
}
