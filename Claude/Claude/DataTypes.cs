using System.Windows.Controls;

namespace Claude
{
    public class DataTypes
    {
        public struct LilGame
        {
            /// <summary>
            /// The launcher code for the game
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// The launcher claude code for the launcher
            /// Currently supported: "Steam", "BattleNet", "Other"
            /// </summary>
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

        public struct UserData
        {
            public UserDataClaude Claude { get; set; }
            public UserDataLaunchers Steam { get; set;}
            public UserDataLaunchers BattleNet { get; set; }
            public UserDataLaunchers Origin { get; set; }
            public UserDataLaunchers Ubisoft { get; set; }
            public UserDataOthers Others { get; set; }
        }

        public struct UserDataClaude { }

        public struct UserDataLaunchers
        {
            public string Exe { get; set; }
            public bool Start { get; set; }
            public bool Stop { get; set; } 
            public string[] Install { get; set; }
        }
        
        public struct UserDataOthers { }
    }
}
