using Node.Version.Management.Extensions;

namespace Node.Version.Management.Models
{
    public class NodeExternal : Node
    {
        public string UrlToDownload => MakeUrl();
        public string FileName => MakeFilename();

        private string MakeUrl()
        {
            var version = this.Version.AsString();
            return $"https://nodejs.org/dist/{version}/{FileName}";
        }
        private string MakeFilename()
        {
            var version = this.Version.AsString();
            return $"node-{version}-win-{this.Platform.ToString()}.zip";
        }
    }
}
