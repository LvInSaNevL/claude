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
using System.Windows.Controls.Primitives;
using Microsoft.CSharp.RuntimeBinder;
using System.Windows.Media.Imaging;

namespace Claude
{
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

        public static string Terminal(string command)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = command;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd().ToString();
        }

        public static void Initialize()
        {
            Directory.CreateDirectory(cache);
            Directory.CreateDirectory(steamapps);

            try { dynamic dynamic = ReadUserData(); }
            catch (Exception e)
            { 
                Installer wizard = new Installer();
                wizard.Show();
                return;
            }

            MainWindow main = new MainWindow();
            main.Show();
        }

        public static void ShutDown()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public static string ChangeUserData(string target, string newVal)
        {
            string[] splitTarget = newVal.Split(".");
            dynamic data = ReadUserData();

            Uri maybePath = new Uri("pack://application:,,,/Resources/UserData.json");
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            fullPath = $"{fullPath}\\{maybePath.LocalPath}";

            JToken token = data.SelectToken(target);
            token.Replace(newVal);
            using (StreamWriter writer = File.CreateText(fullPath))
            {
                string stringData = JsonConvert.SerializeObject(data);
                writer.WriteLine(stringData);
            }

            return "true";
        }

        public static dynamic ReadUserData()
        {
            dynamic parsedData;
            Uri maybePath = new Uri("pack://application:,,,/Resources/UserData.json");
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            fullPath = $"{fullPath}\\{maybePath.LocalPath}";

            try
            {
                using StreamReader reader = new StreamReader(fullPath);
                string resutl = reader.ReadToEnd().ToString();
                if (resutl == null) { return new NullReferenceException(); }
                else
                {
                    parsedData = JObject.Parse(resutl);
                }
            }
            catch (FileNotFoundException e) { throw new FileNotFoundException(); }

            return parsedData;
        }

        public static List<Game> GetGames()
        {
            // Final list of all games
            List<Game> allGames = new List<Game>();
            // and individual launchers
            List<Game> steamGames = Steam.UserVdiReader();
            List<Game> battleNetGames = BattleNet.InstalledGames();

            foreach (Game nowgame in steamGames) { allGames.Add(nowgame); }
            foreach (Game nowgame in battleNetGames) { allGames.Add(nowgame); }

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


}