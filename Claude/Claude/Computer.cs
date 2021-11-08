using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

public class Computer
{
    // File Paths
    private static readonly string cache = Directory.GetCurrentDirectory() + "/cache";

    public static void TempDownload(string url, string filename)
    {
        if (!File.Exists($"{cache}/{filename}"))
        {
            using WebClient client = new WebClient(); client.DownloadFile(new Uri(url), $"{cache}/{filename}");
        }
    }

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
    }
}
