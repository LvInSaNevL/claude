using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using IdentityModel.OidcClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Claude
{
    public class Steam
    {
        private static readonly string baseLocation = "C:\\Program Files (x86)\\Steam";
        public static List<Computer.Game> UserVdiReader()
        {
            string installList = baseLocation + "\\steamapps\\libraryfolders.vdf";
            VProperty rawFolders = VdfConvert.Deserialize(File.ReadAllText(installList));
            JToken libraryFolders = rawFolders.ToJson().Last;

            List<Computer.Game> appList = new List<Computer.Game>();
            int counter = 0;
            for (int s = 0; s < (libraryFolders.Count() - 1); s++)
            {
                try
                {
                    var path = libraryFolders[s.ToString()]["path"];
                    var temp = libraryFolders[s.ToString()]["apps"];
                    foreach (var g in temp)
                    {
                        string[] bits = g.ToString().Split(":");
                        string currentID = bits[0].Replace("\"", "");

                        if (!Banned(currentID))
                        {
                            Computer.Game currentGame = new Computer.Game()
                            {
                                Launcher = "Steam",
                                Path = path.ToString(),
                                Id = currentID
                            };

                            FileOut.TempDownload($"https://store.steampowered.com/api/appdetails?appids={currentID}", $"steamapps/{currentID}.json");
                            try
                            {
                                Uri test = new Uri($"{Directory.GetCurrentDirectory()}/cache/steamapps/{currentID}.json");
                                string gameFile = File.ReadAllText($"{Directory.GetCurrentDirectory()}/cache/steamapps/{currentID}.json");
                                dynamic parsedGame = Details(currentID);
                                currentGame.Title = parsedGame["name"];
                                currentGame.Release = parsedGame["release_date"]["date"];
                                currentGame.About = parsedGame["about_the_game"];
                                currentGame.Developer = parsedGame["developers"][0];
                                currentGame.Publisher = parsedGame["publishers"][0];

                            }
                            catch (Exception) { currentGame.Title = $"Steam Utility - {currentID}"; }

                            FileOut.TempDownload($"https://cdn.akamai.steamstatic.com/steam/apps/{bits[0].Replace("\"", "")}/header.jpg", bits[0].Replace("\"", "") + ".jpg");

                            appList.Add(currentGame);
                        }
                    }

                    counter++;
                }
                catch (Exception) { Console.WriteLine("Null reference exception"); }
            };

            return appList;
        }

        public static List<Computer.Game> InstalledGames()
        {
            List<Computer.Game> games = UserVdiReader();
            List<Computer.Game> lilGames = new List<Computer.Game>();

            foreach (Computer.Game game in games)
            {
                lilGames.Add(new Computer.Game()
                {
                    Id = game.Id,
                    Launcher = game.Launcher,
                });
            }


            return lilGames;
        }

        public static string GetExe()
        {
            try
            {
                JToken token = FileIn.ReadUserData().SelectToken("Steam.exe");
                return token.ToString();
            }
            catch { }
            if (File.Exists($"{baseLocation}\\steam.exe")) { return $"{baseLocation}\\steam.exe"; }
            else { return "File location not found"; }
        }

        public static dynamic Details(string id)
        {
            Uri test = new Uri($"{Directory.GetCurrentDirectory()}/cache/steamapps/{id}.json");
            string gameFile = File.ReadAllText($"{Directory.GetCurrentDirectory()}/cache/steamapps/{id}.json");
            dynamic parsedGame = JObject.Parse(gameFile);

            return parsedGame[id]["data"];
        }

        public static List<String> InstallLocs()
        {
            string installList = baseLocation + "\\steamapps\\libraryfolders.vdf";
            VProperty rawFolders = VdfConvert.Deserialize(File.ReadAllText(installList));
            JToken libraryFolders = rawFolders.ToJson().Last;

            List<String> paths = new List<string>();
            for (int x = 0; x <= 1; x++)
            {
                paths.Add(libraryFolders[x.ToString()]["path"].ToString());
            }

            return paths;
        }

        public static void Launch(string gameID)
        {
            string _ = Computer.Terminal($"echo \"Launching {gameID} on Steam!\" && start steam://rungameid/{gameID}");
        }

        private static bool Banned(string id)
        {
            string[] blacklist = new string[] { "228980", "250820" };

            return Array.Exists(blacklist, x => x == id);
        }
    }
}