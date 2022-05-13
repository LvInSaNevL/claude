using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using IdentityModel.OidcClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Claude
{
    public class Steam
    {
        private static readonly string baseLocation = "C:\\Program Files (x86)\\Steam";
        
        public static List<DataTypes.Game> Installed()
        {
            var installDirs = FileIn.ReadUserData()["Steam"]["install"];
            List<DataTypes.Game> appList = new List<DataTypes.Game>();

            foreach (string nowDir in installDirs)
            {
                string[] files = Directory.GetFiles(nowDir);
                var regex = new Regex(@"^appmanifest_(.*).acf$");
                foreach (string file in files)
                {
                    if (regex.IsMatch(Path.GetFileName(file)))
                    {
                        VProperty rawFile = VdfConvert.Deserialize(File.ReadAllText(file));
                        JToken parsed = rawFile.ToJson().Last;

                        if (Banned(parsed["appid"].ToString())) { continue; }
                        else
                        {
                            appList.Add(new DataTypes.Game()
                            {
                                Id = parsed["appid"].ToString(),
                                Launcher = "Steam"
                            });
                        }

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