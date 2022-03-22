using System;
using System.Collections.Generic;
using System.Text;
using TACTLib.Agent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Claude
{
    public class BattleNet
    {
        static readonly Dictionary<string, string> LauncherArgs = new Dictionary<string, string>()
        {
            // Battle.Net commands
            { "games", "--exec=\"focus play\""},
            { "shop",  "--exec=\"focus shop\"" },
            { "social", "--exec=\"focus socia\"" },
            { "news", "--exec=\"focus news\"" },
            { "friends", "--exec=\"dialog friends\"" },
            { "settings", "--exec=\"dialog settings\"" }
        };
        // --exec="launch {key}
        static readonly Dictionary<string, string> GameArgs = new Dictionary<string, string>()
        {
            // Blizzard Games
            { "d3", "Diablo III" },
            { "d2r", "Diablo II: Resurrected" },
            { "wtgg", "Hearthstone" },
            { "hero", "Heros of the Storm" },
            { "pro", "Overwatch" },
            { "s1", "StarCraft" },
            { "s2", "StartCraft II" },
            { "w3", "Warcraft III" },
            { "wow", "World of Warcraft" },
            { "wow_classic", "World of Warcraft Classic" },
            { "d3t", "Diablo III: Public Test" },
            { "herot", "Heros of teh Storm: Public Test" },
            { "wowt", "World of Warcraft: Public Test" },
            { "cb4", "Crash bandicoot 4" },
            // Activision Games
            { "vipr", "COD: Black Ops 4" },
            { "odin", "COD: Modern Warfare" },
            { "lazr", "COD: Modern Warfare 2 Campaign Remastered" },
            { "zeus", "COD: Black Ops Cold War" }
        };

        public static List<Computer.Game> InstalledGames()
        {
            List<Computer.Game> games = new List<Computer.Game>();
            dynamic userData = FileIn.ReadUserData();
            var userDirs = userData.SelectToken("BattleNet.install");

            foreach (string dir in userDirs)
            {
                foreach (string gameDir in Directory.GetDirectories(dir))
                {
                    string path = $"{gameDir}\\.product.db";
                    AgentDatabase handler = new AgentDatabase(path);
                    var result = handler.Data.ToString();
                    dynamic data = JObject.Parse(result);

                    games.Add(new Computer.Game()
                    {
                        Id = data["productInstall"][0]["uid"],
                        Launcher = "BattleNet"
                    });                
                }
            }


            return games;
        }

        public static void Launch(string target)
        {
            string _ = Computer.Terminal($"echo \"Launching {target} on Battle.Net!\" && \"C:\\Program Files (x86)\\Battle.net\\Battle.net\\Launcher.exe\"");
            _ = Computer.Terminal($"cd  \"C:\\Program Files (x86)\\Battle.net\\Battle.net\" && Launcher.exe --exec=\"launch {GameArgs[target]}\"");
        }
    }
}