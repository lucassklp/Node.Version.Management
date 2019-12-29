using Node.Version.Management.Extensions;
using Node.Version.Management.Models;
using Node.Version.Management.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Node.Version.Management
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Node.Version.Nanagement!");
            Console.WriteLine();

            if (NodeUtils.IsNodeInstalled())
            {
                Console.WriteLine($"You are currently using Node {NodeUtils.GetCurrentNodeVersion()}");
            }
            else
            {
                //TODO: Adicionar uma pasta do node para ser a do Path
                Console.WriteLine("No version of node is currently installed");
            }


            Console.WriteLine("The following versions are available locally to install:");
            Console.WriteLine();

            if (!Directory.Exists(PathUtils.VersionsPath))
            {
                Directory.CreateDirectory(PathUtils.VersionsPath);
            }

            var files = Directory.GetFiles(PathUtils.VersionsPath, string.Empty, SearchOption.TopDirectoryOnly)
                .Select(x => x.Replace($"{PathUtils.VersionsPath}{Path.DirectorySeparatorChar}", string.Empty));

            Console.WriteLine(string.Join('\n', files));

            var typedVersion = InputUtils.GetTypedVersion("Please, tell the node version you want to install");

            var nodeExternal = new NodeExternal()
            {
                Platform = Environment.Is64BitProcess ? Platform.x64 : Platform.x86,
                Version = typedVersion
            };

            var selectedVersionFile = files.FirstOrDefault(x => x.Contains(typedVersion.AsString()));

            if (!string.IsNullOrEmpty(selectedVersionFile))
            {
                InstallNode(nodeExternal);
            }
            else
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                    {
                        Console.WriteLine($"Download progress: {e.ProgressPercentage}% - {e.BytesReceived}/{e.TotalBytesToReceive} bytes");
                    };
                    wc.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                    {
                        InstallNode(nodeExternal);
                    };
                    wc.DownloadFileAsync(new Uri(nodeExternal.UrlToDownload), PathUtils.Combine(PathUtils.VersionsPath, nodeExternal.FileName));
                    while (wc.IsBusy) { }
                }
            }

            Console.ReadLine();

        }

        public static void InstallNode(NodeExternal nodeExternal)
        {
            var backupPath = PathUtils.Combine(NodeUtils.GetNodePath(), "previous_version_installed");
            var directoryInfo = new DirectoryInfo(NodeUtils.GetNodePath());
            if (Directory.Exists(backupPath))
            {
                Directory.Delete(backupPath, true);
            }
            Directory.CreateDirectory(backupPath);

            Console.WriteLine("Killing node processes to avoid errors...");
            Process.GetProcessesByName("node.exe").ToList().ForEach(process => process.Kill());

            Console.WriteLine("Backuping the current version of node...");
            foreach (var entry in directoryInfo.GetFiles())
            {
                File.Move(entry.FullName, PathUtils.Combine(backupPath, entry.Name));
            }
            foreach (var entry in directoryInfo.GetDirectories().Where(x => x.FullName != backupPath))
            {
                Directory.Move(entry.FullName, PathUtils.Combine(backupPath, entry.Name));
            }

            Console.WriteLine("Extracting downloaded file");

            var extractPath = PathUtils.Combine(PathUtils.VersionsPath, nodeExternal.FileName.Replace(".zip", string.Empty));
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }

            ZipFile.ExtractToDirectory(PathUtils.Combine(PathUtils.VersionsPath, nodeExternal.FileName), PathUtils.VersionsPath);

            var tempDirectoryInfo = new DirectoryInfo(PathUtils.Combine(PathUtils.VersionsPath, nodeExternal.FileName.Replace(".zip", string.Empty)));

            Console.WriteLine("Copying files to Node's folder");
            foreach (var entry in tempDirectoryInfo.GetFiles())
            {
                File.Move(entry.FullName, PathUtils.Combine(NodeUtils.GetNodePath(), entry.Name));
            }
            foreach (var entry in tempDirectoryInfo.GetDirectories())
            {
                Directory.Move(entry.FullName, PathUtils.Combine(NodeUtils.GetNodePath(), entry.Name));
            }

            Console.WriteLine("Installation Completed!");

        }
    }
}
