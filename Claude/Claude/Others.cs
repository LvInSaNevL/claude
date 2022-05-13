using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace Claude
{
    internal class Others
    {
        public static List<DataTypes.Game> Install(DataTypes.Game game)
        {
            List<DataTypes.Game> userGames = FileIn.ReadUserGames();
            userGames.Add(game);
            var sortedGames = userGames.OrderBy(Game => Game.Title);

            using (StreamWriter writer = File.CreateText(FilePaths.resources("UserGames.json")))
            {
                string stringData = JsonConvert.SerializeObject(sortedGames);
                writer.WriteLine(stringData);
                writer.Dispose();
            }

            return sortedGames.ToList();
        }

        public static void Launch(string target)
        {
            System.Diagnostics.Process.Start(target);
        }
    }
}
