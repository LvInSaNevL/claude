using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Controls;

public class Computer
{
    public static readonly string cache = Directory.GetCurrentDirectory() + "/cache";
    public static readonly string steamapps = Directory.GetCurrentDirectory() + "/cache/steamapps";

    public static void TempDownload(string url, string filename)
    {
        if (!File.Exists($"{cache}/{filename}"))
        {
            using WebClient client = new WebClient(); client.DownloadFile(new Uri(url), $"{cache}/{filename}");
        }
    }

    public static string CachePath(string target) { return $"{cache}/{target}"; }

    public static void Terminal(string command)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C start " + command;

        process.StartInfo = startInfo;
        process.Start();
    }

    public static void Initialize()
    {
        Directory.CreateDirectory(cache);
        Directory.CreateDirectory(steamapps);
    }

    public struct Game
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Launcher { get; set; }
        public string Path { get; set; }
        public StackPanel detailFrame { get; set; }
    }
}
