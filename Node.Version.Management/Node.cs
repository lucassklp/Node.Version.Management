using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Version.Management
{
    public class Node
    {
        public NodeVersion Version { get; set; }
        public string Path { get; set; }
        public Platform Platform { get; set; }
    }
}
