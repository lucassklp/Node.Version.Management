using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Version.Management.Exceptions
{
    public class InvalidVersionFormatException : Exception
    {
        public InvalidVersionFormatException() : base("Invalid version format was provided") { }
    }
}
