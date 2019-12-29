using Node.Version.Management.Maps;
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


            var paths = Environment.GetEnvironmentVariable("Path")
                .Split(";")
                .ToList();

            var nodePath = paths.First(path => File.Exists($"{path}{Path.DirectorySeparatorChar}node.exe"));

            var nodeProcess = new Process();
            nodeProcess.StartInfo.UseShellExecute = false;
            nodeProcess.StartInfo.RedirectStandardOutput = true;
            nodeProcess.StartInfo.RedirectStandardError = true;
            nodeProcess.StartInfo.RedirectStandardInput = true;
            nodeProcess.StartInfo.FileName = @$"{nodePath}{Path.DirectorySeparatorChar}node.exe";
            nodeProcess.StartInfo.Arguments = @"--version";
            nodeProcess.Start();

            var output = nodeProcess.StandardOutput.ReadToEnd();
            var version = output.GetVersion();

            Console.WriteLine($"You are currently using Node {version?.AsString()}");
            Console.WriteLine($"Use command 'nvm use <version> to switch version'");
            Console.WriteLine();
            Console.WriteLine("The following versions are available:");

            var versionsDirectory = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}versions";
            if (!Directory.Exists(versionsDirectory))
            {
                Directory.CreateDirectory(versionsDirectory);
            }

            var files = Directory.GetFiles(versionsDirectory, string.Empty, SearchOption.TopDirectoryOnly)
                .Select(x => x.Replace($"{versionsDirectory}{Path.DirectorySeparatorChar}", string.Empty));

            Console.WriteLine(string.Join('\n', files));


            Console.Write("Digite a versão do node que deseja usar:\n>>");
            var typedVersion = Console.ReadLine();
            typedVersion = typedVersion.StartsWith("v") ? typedVersion : $"v{typedVersion}";

            var nodeExternal = new NodeExternal()
            {
                Platform = Environment.Is64BitProcess ? Platform.x64 : Platform.x86,
                Version = typedVersion.GetVersion().Value,
            };

            var selectedVersionFile = files.FirstOrDefault(x => x.Contains(typedVersion));


            void InstallNode()
            {
                var backupPath = $"{nodePath}previous_version_installed";
                var directoryInfo = new DirectoryInfo(nodePath);
                if (Directory.Exists(backupPath))
                {
                    Directory.Delete(backupPath, true);
                }
                Directory.CreateDirectory(backupPath);
                foreach (var process in Process.GetProcessesByName("node.exe"))
                {
                    process.Kill();
                }
                foreach (var entry in directoryInfo.GetFiles())
                {
                    File.Move(entry.FullName, $"{backupPath}{Path.DirectorySeparatorChar}{entry.Name}");
                }
                foreach (var entry in directoryInfo.GetDirectories().Where(x => x.FullName != backupPath))
                {
                    Directory.Move(entry.FullName, $"{backupPath}{Path.DirectorySeparatorChar}{entry.Name}");
                }

                ZipFile.ExtractToDirectory($"{versionsDirectory}{Path.DirectorySeparatorChar}{nodeExternal.FileName}", versionsDirectory);

                var tempDirectoryInfo = new DirectoryInfo($"{versionsDirectory}{Path.DirectorySeparatorChar}{nodeExternal.FileName.Replace(".zip", string.Empty)}");

                foreach (var entry in tempDirectoryInfo.GetFiles())
                {
                    File.Move(entry.FullName, $"{nodePath}{Path.DirectorySeparatorChar}{entry.Name}");
                }
                foreach (var entry in tempDirectoryInfo.GetDirectories())
                {
                    Directory.Move(entry.FullName, $"{nodePath}{Path.DirectorySeparatorChar}{entry.Name}");
                }

            }


            if (!string.IsNullOrEmpty(selectedVersionFile))
            {
                InstallNode();
            }
            else
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                    {
                        Console.WriteLine($"{e.ProgressPercentage}% - {e.BytesReceived}/{e.TotalBytesToReceive}");
                    };
                    wc.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                    {
                        InstallNode();
                    };
                    wc.DownloadFileAsync(new Uri(nodeExternal.UrlToDownload), $"{versionsDirectory}{Path.DirectorySeparatorChar}{nodeExternal.FileName}");
                }
            }

            Console.ReadLine();

        }
    }
}
