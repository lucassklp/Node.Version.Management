using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Version.Management.Exceptions
{
    public class NotSupportedVersionException : Exception
    {
        public NotSupportedVersionException() : base("This version is not supported yet.") 
        {
        }
    }
}
