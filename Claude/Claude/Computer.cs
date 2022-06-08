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
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            startInfo.Arguments = command;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd().ToString();
        }

        public static void Initialize(string[] args)
        {
            Directory.CreateDirectory(FilePaths.cache);
            Directory.CreateDirectory(FilePaths.cache);

            if (args.Contains("--NoApi")) { NoApi = true; }

            if (args.Contains("--installer")) 
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

                GetGames();

                MainWindow main = new MainWindow();
                main.Show();
            }
        }

        public static void ShutDown()
        {
            System.Windows.Application.Current.Shutdown();
        }
        
        private static bool NoApi = false;
        public static List<DataTypes.Game> CallClaude(List<DataTypes.Game> games)
        {
            if (NoApi) 
            {
                ErrorHandling.Logger("Not calling Claude-API due to command line argument --NoApi");
                return new List<DataTypes.Game>();
            }

            List<DataTypes.LilGame> list = new List<DataTypes.LilGame>();
            foreach (DataTypes.Game game in games)
            {
                list.Add(new DataTypes.LilGame()
                {
                    Id = game.Id,
                    Launcher = game.Launcher
                });
            }
            string content = JsonConvert.SerializeObject(list);
            if (content == null) { return new List<DataTypes.Game>(); }

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var result = client.UploadString("https://localhost:44337/getgames", content);

                    List<DataTypes.Game> resultGames = JsonConvert.DeserializeObject<List<DataTypes.Game>>(result);
                    resultGames.RemoveAll(x => x.Title == "null");

                    return resultGames;
                }
            }
            catch { return new List<DataTypes.Game>(); }
        }

        public static List<DataTypes.Game> GetGames()
        {
            // Final list of all games
            List<DataTypes.Game> allGames = new List<DataTypes.Game>();
            List<DataTypes.Game> lilGames = new List<DataTypes.Game>();

            // Read from user data file
            try { allGames.AddRange(FileIn.ReadUserGames()); }
            catch { ErrorHandling.Logger(new NullValueHandling()); }

            // Adding individual launchers
            lilGames.AddRange(Steam.Installed());
            lilGames.AddRange(BattleNet.InstalledGames());

            // And adding any new games
            List<DataTypes.Game> diffGames = lilGames.Where(l => !allGames.Select(a => a.Id).Contains(l.Id)).ToList();
            List<DataTypes.Game> newGames = CallClaude(diffGames);
            foreach (DataTypes.Game newGame in newGames)
            {
                FileOut.AddUserGames(newGame);
                allGames.Add(newGame); 
            }

            // Organizing and saving everything before returning
            var sortedGames = allGames.OrderBy(game => game.Title);
            foreach (DataTypes.Game game in sortedGames) 
            {
                if (game.Launcher != "Others") { FileOut.TempDownload(game.Thumbnail, $"{game.Id}.jpg"); }
            }
            return sortedGames.ToList<DataTypes.Game>();
        }        
    }
}