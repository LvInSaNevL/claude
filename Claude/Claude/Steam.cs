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
        
        public static List<Computer.Game> InstalledGames()
        {
            string installList = baseLocation + "\\steamapps\\libraryfolders.vdf";
            VProperty rawFolders = VdfConvert.Deserialize(File.ReadAllText(installList));
            JToken libraryFolders = rawFolders.ToJson().Last;

            List<Computer.Game> appList = new List<Computer.Game>();
            for (int s = 0; s < (libraryFolders.Count() - 1); s++)
            {
                foreach (var nowGame in libraryFolders[s.ToString()]["apps"])
                {
                    string[] bits = nowGame.ToString().Split(":");
                    string currentID = bits[0].Replace("\"", "");

                    if(Banned(currentID)) { continue; }
                    else
                    {
                        FileOut.TempDownload($"https://cdn.akamai.steamstatic.com/steam/apps/{currentID}/header.jpg", $"{currentID}.jpg");

                        appList.Add(new Computer.Game()
                        {
                            Id = currentID,
                            Launcher = "Steam"
                        });
                    }
                }
            }

            return appList;
        }

        public static void Launch(string gameID)
        {
            string _ = Computer.Terminal($"echo \"Launching {gameID} on Steam!\" && start steam://rungameid/{gameID}");
        }

        private static bool Banned(string id)
        {
            string[] blacklist = new string[] 
            { 
                "228980",  // Steam Common Redistributables
                "250820",  // SteamVR
                "1391110", // Steam Linux Runtime - Soldier
                "1580130", // Steam Proton
            };

            return Array.Exists(blacklist, x => x == id);
        }
    }
}