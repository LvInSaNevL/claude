using System;
using System.Collections.Generic;
using System.Text;

namespace Claude
{
    public class BattleNet
    {
        static readonly Dictionary<string, string> Args = new Dictionary<string, string>()
        {
            // Battle.Net commands
            { "games", "--exec=\"focus play\""},
            { "shop",  "--exec=\"focus shop\"" },
            { "social", "--exec=\"focus socia\"" },
            { "news", "--exec=\"focus news\"" },
            { "friends", "--exec=\"dialog friends\"" },
            { "settings", "--exec=\"dialog settings\"" },
            // Blizzard Games
            { "diablo3", "--exec=\"launch D3\"" },
            { "diablo2", "--exec=\"launch d2r\"" },
            { "hearthstone", "--exec=\"launch WTGG\"" },
            { "heros of the storm", "--exec=\"launch Hero\"" },
            { "overwatch", "--exec=\"launch Pro\"" },
            { "starcraft", "--exec=\"launch S1\"" },
            { "starcraft2", "--exec=\"launch S2\"" },
            { "warcraft3", "--exec=\"launch W3\"" },
            { "wow", "--exec=\"launch WoW\"" },
            { "wow classic", "--exec=\"launch wow_classic\"" },
            { "diablo3 public test", "--exec=\"launch d3t\"" },
            { "heros of the store public test", "--exec=\"launch herot\"" },
            { "wow public test", "--exec=\"launch wowt\"" },
            { "crash4", "--exec=\"launch cb4\"" },
            // Activision Games
            { "cod bo4", "--exec=\"launch VIPR\"" },
            { "cod mw", "--exec=\"launch ODIN\"" },
            { "cod mw2cr", "--exec=\"launch LAZR\"" },
            { "cod bocw", "--exec=\"launch ZEUS\"" }
        };

        public static void Launch(string target)
        {
            Computer.Terminal($"echo \"Launching {target} on Battle.Net!\" && \"C:\\Program Files (x86)\\Battle.net\\Battle.net\\Launcher.exe\" {Args[target]}");
        }
    }
}
