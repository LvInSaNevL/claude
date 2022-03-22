using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Claude
{
    public class Computer
    {     
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

            Directory.CreateDirectory(FilePaths.cache);
            Directory.CreateDirectory(FilePaths.cache);

            bool forceInstaller = false;
            if (forceInstaller)
            {
                Installer wizard = new Installer();
                wizard.Show();
                return;
            }
            else
            {
                try { dynamic dynamic = FileIn.ReadUserData(); }
                catch (Exception e)
                {
                    Installer wizard = new Installer();
                    wizard.Show();
                    return;
                }

                MainWindow main = new MainWindow();
                main.Show();
            }
        }

        public static void ShutDown()
        {
            System.Windows.Application.Current.Shutdown();
        }
                
        public static List<Game> CallClaude(List<Game> games)
        {
            List<LilGame> list = new List<LilGame>();
            foreach (Game game in games)
            {
                list.Add(new LilGame()
                {
                    Id = game.Id,
                    Launcher = game.Launcher
                });
            }
            string content = JsonConvert.SerializeObject(list);
            if (content == null) { return new List<Game>(); }

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var result = client.UploadString("https://localhost:44337/getgames", content);

                    List<Game> resultGames = JsonConvert.DeserializeObject<List<Game>>(result);
                    resultGames.RemoveAll(x => x.Title == "null");

                    return resultGames;
                }
            }
            catch { return new List<Game>(); }
        }

        public static List<Game> GetGames()
        {
            // Final list of all games
            List<Game> allGames = new List<Game>();
            List<Game> lilGames = new List<Game>();

            // Read from user data file
            allGames.AddRange(FileIn.ReadUserGames());

            // Adding individual launchers
            lilGames.AddRange(Steam.InstalledGames());
            lilGames.AddRange(BattleNet.InstalledGames());

            // And adding any new games
            List<Game> diffGames = lilGames.Where(l => !allGames.Select(a => a.Id).Contains(l.Id)).ToList();
            List<Game> newGames = CallClaude(diffGames);
            foreach (Game newGame in newGames)
            {
                FileOut.AddUserGames(newGame);
                allGames.Add(newGame); 
            }

            // Organizing and saving everything before returning
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
            /// The launcher code for the game
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// The "normal" human readable name
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// A short little description of the game
            /// </summary>
            public string About { get; set; }
            /// <summary>
            /// The date the game released, formatted in RFC1123 without timestamp
            /// </summary>
            public string Release { get; set; }
            /// <summary>
            /// The developer of the game
            /// </summary>
            public string Developer { get; set; }
            /// <summary>
            /// The publisher of the game
            /// </summary>
            public string Publisher { get; set; }
            /// <summary>
            /// The launcher claude code for the launcher
            /// Currently supported: "Steam", "BattleNet", "Other"
            /// </summary>
            public string Launcher { get; set; }
            /// <summary>
            /// The URL to the main thumbnail
            /// </summary>
            public string Thumbnail { get; set; }
            /// <summary>
            /// An array of URLs to any promotional images
            /// </summary>
            public string[] Screenshots { get; set; }
            /// <summary>
            /// Path on disk to executable
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// Which stack panel the game is added to
            /// </summary>
            public StackPanel DetailFrame { get; set; }
        }
    }
}