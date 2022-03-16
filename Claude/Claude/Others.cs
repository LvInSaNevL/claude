using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Claude
{
    internal class Others
    {
        public static List<Computer.Game> Install(Computer.Game game)
        {
            // Read from user data file
            Uri maybePath = new Uri("pack://application:,,,/Resources/UserGames.json");
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            fullPath = $"{fullPath}\\{maybePath.LocalPath}";

            using (StreamReader reader = new StreamReader(fullPath))
            {
                string result = reader.ReadToEnd().ToString();
                reader.Dispose();

                List<Computer.Game> userGames = JsonConvert.DeserializeObject<List<Computer.Game>>(result);
                userGames.Add(game);
                var sortedGames = userGames.OrderBy(Game => Game.Title);

                using (StreamWriter writer = File.CreateText(fullPath))
                {
                    string stringData = JsonConvert.SerializeObject(sortedGames);
                    writer.WriteLine(stringData);
                    writer.Dispose();
                }

                return sortedGames.ToList();
            }
        }
    }
}
