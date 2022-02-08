using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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

    public static string ChangeUserData(string target, string newVal)
    {
        dynamic parsedData = ReadUserData();
        string[] splitTarget = newVal.Split(".");

        Uri maybePath = new Uri("pack://application:,,,/Resources/UserData.json");
        string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        fullPath = $"{fullPath}\\{maybePath.LocalPath}";

        JToken token = parsedData.SelectToken(target);
        token.Replace(newVal);
        using (StreamWriter writer = File.CreateText(fullPath))
        {
            string stringData = JsonConvert.SerializeObject(parsedData);
            writer.WriteLine(stringData);
        }

        return "true";
    }

    public static Uri GetResource(string file)
    {
        return new Uri($"pack://application:,,,/Resources/{file}");
    }

    public static dynamic ReadUserData()
    {
        dynamic parsedData;
        Uri maybePath = new Uri("pack://application:,,,/Resources/UserData.json");
        string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        fullPath = $"{fullPath}\\{maybePath.LocalPath}";

        try
        {
            using (StreamReader reader = new StreamReader(fullPath))
            {
                string resutl = reader.ReadToEnd().ToString();
                parsedData = JObject.Parse(resutl);
            }
        }
        catch (System.IO.FileNotFoundException e) { return JObject.Parse("holder"); }

        return parsedData;
    }

    public static dynamic FindUserData(dynamic data, string key)
    {
        string[] keys = key.Split(".");        
        string firstKey = keys[0];

        if (keys.Length == 1)
            return data[key];
        else
        {
            string[] newKeys = new string[keys.Length - 1];
            Array.Copy(keys, 1, newKeys, 0, (keys.Length - 1));
            string returnVal = string.Join(".", newKeys);
            return FindUserData(data[firstKey], returnVal);
        }
    }

    public static List<Game> GetGames()
    {
        // Final list of all games
        SortedSet<Game> allGames = new SortedSet<Game>(new GameSorter());
        // and individual launchers
        List<Game> steamGames = Steam.InstalledAsync().Result;

        foreach (Game nowgame in steamGames)
        {
            allGames.Add(nowgame);
        }

        var sortedGames = allGames.OrderBy(Game => Game.Title);
        return sortedGames.ToList<Game>();
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

public class GameSorter : IComparer<Computer.Game>
{
    public int Compare(Computer.Game gameA, Computer.Game gameB)
    {
        return gameA.Title.CompareTo(gameB.Title);
    }
}