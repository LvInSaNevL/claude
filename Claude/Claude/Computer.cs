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
using System.Net.Http;
using System.Text;

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

        public static void CallClaude()
        {
            LilGame testGame = new LilGame()
            {
                Id = "275850",
                Launcher = "Steam"
            };
            var content = JsonConvert.SerializeObject(testGame);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = client.UploadString("https://localhost:44337/getgames", content);
                Console.WriteLine(result);
            }
        }

        public static List<Game> GetGames()
        {
            //CallClaude();
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

        public struct LilGame
        {
            public string Id { get; set; }
            public string Launcher { get; set; }
        }

        public struct Game
        {
            /// <summary>
            /// The launcher based ID for the game
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// "Normal" human readable title of the game
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// A short description of the game
            /// </summary>
            public string About { get; set; }
            /// <summary>
            /// Initial release date
            /// </summary>
            public string Release { get; set; }
            /// <summary>
            /// Primary developer
            /// </summary>
            public string Developer { get; set; }
            /// <summary>
            /// Primary publisher
            /// </summary>
            public string Publisher { get; set; }
            /// <summary>
            /// Which launcher the game uses
            /// </summary>
            public string Launcher { get; set; }
            /// <summary>
            /// File path to executable
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// Which stack panel the game is added to
            /// </summary>
            public StackPanel detailFrame { get; set; }
        }
    }
}