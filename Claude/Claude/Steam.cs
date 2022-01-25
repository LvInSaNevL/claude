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

public class Steam
{
    private static readonly string baseLocation = "C:\\Program Files (x86)\\Steam";
    public static async Task<List<Computer.Game>> InstalledAsync()
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
                        Computer.Game currentGame = new Computer.Game();
                        currentGame.Launcher = "steam";
                        currentGame.Path = path.ToString();
                        currentGame.Id = currentID;

                        Computer.TempDownload($"https://store.steampowered.com/api/appdetails?appids={currentID}", $"steamapps/{currentID}.json");
                        try
                        {
                            Uri test = new Uri($"{Directory.GetCurrentDirectory()}/cache/steamapps/{currentID}.json");
                            string gameFile = File.ReadAllText($"{Directory.GetCurrentDirectory()}/cache/steamapps/{currentID}.json");
                            dynamic parsedGame = Details(currentID);
                            currentGame.Title = parsedGame["name"];
                        }
                        catch (Exception) { currentGame.Title = $"Steam Utility - {currentID}"; }

                        Computer.TempDownload($"https://cdn.akamai.steamstatic.com/steam/apps/{bits[0].Replace("\"", "")}/header.jpg", bits[0].Replace("\"", "") + ".jpg");

                        appList.Add(currentGame);
                    }
                }

                counter++;
            }
            catch (Exception) { Console.WriteLine("Null reference exception"); }
        };

        return appList;
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
        Computer.Terminal($"echo \"Launching {gameID} on Steam!\" && start steam://rungameid/{gameID}");
    }

    private static bool Banned(string id)
    {
        string[] blacklist = new string[] { "228980", "250820" };

        return Array.Exists(blacklist, x => x == id);
    }
}