using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Claude
{
    public class FilePaths
    {
        public static readonly string cache = Directory.GetCurrentDirectory() + "/cache";
        public static readonly string steamapps = Directory.GetCurrentDirectory() + "/cache/steamapps";
        public static string resources(string resource)
        {
            Uri maybePath = new Uri($"pack://application:,,,/Resources/{resource}");
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            fullPath = $"{fullPath}\\{maybePath.LocalPath}";

            return fullPath;
        }
    }

    public class FileIn
    {
        public static dynamic ReadUserData()
        {
            dynamic parsedData;
            string fullPath = FilePaths.resources("UserData.json");

            try
            {
                using StreamReader reader = new StreamReader(fullPath);
                string resutl = reader.ReadToEnd().ToString();
                if (resutl == null) { return new NullReferenceException(); }
                else
                {
                    parsedData = JObject.Parse(resutl);
                }
            }
            catch (FileNotFoundException e) { return new FileNotFoundException(); }

            return parsedData;
        }

        public static List<Computer.Game> ReadUserGames()
        {
            string fullPath = FilePaths.resources("UserGames.json");
            using StreamReader reader = new StreamReader(fullPath);
            string result = reader.ReadToEnd().ToString();
            reader.Dispose();
            try { return JsonConvert.DeserializeObject<List<Computer.Game>>(result); }
            catch { return new List<Computer.Game>(); }
        }
    }

    public class FileOut
    {
        public static void TempDownload(string url, string filename)
        {
            if (!File.Exists($"{FilePaths.cache}/{filename}"))
            {
                using WebClient client = new WebClient(); client.DownloadFile(new Uri(url), $"{FilePaths.cache}/{filename}");
            }
        }

        public static object ChangeUserData(string target, string newVal)
        {
            string fullPath = FilePaths.resources("UserData.json");

            try
            {
                lock (fullPath)
                {
                    dynamic data = FileIn.ReadUserData();

                    JToken token = data.SelectToken(target);
                    token.Replace(newVal);
                    using (StreamWriter writer = File.CreateText(fullPath))
                    {
                        string stringData = JsonConvert.SerializeObject(data);
                        writer.WriteLine(stringData);

                    }

                    return true;
                }
            }
            catch (Exception e) { return e; }
        }

        public static object AddUserGames(Computer.Game test)
        {
            string fullPath = FilePaths.resources("UserGames.json");

            try 
            { 
                lock (fullPath)
                {
                    List<Computer.Game> list = FileIn.ReadUserGames();
                    if (list.Contains(test)) { return false; }
                    else
                    {
                        list.Add(test);
                        var sortedGames = list.OrderBy(Game => Game.Title);
                        using (StreamWriter writer = File.CreateText(fullPath))
                        {
                            string stringData = JsonConvert.SerializeObject(sortedGames);
                            writer.WriteLine(stringData);
                        }

                        return true;
                    }
                }
            }
            catch (Exception e) { return e; }
        }

        public static object RemoveUserGames(Computer.Game test)
        {
            string fullPath = FilePaths.resources("UserGames.json");

            try
            {
                lock (fullPath)
                {
                    List<Computer.Game> list = FileIn.ReadUserGames();
                    if (!list.Contains(test)) { return false; }
                    else
                    {
                        list.Remove(test);
                        var sortedGames = list.OrderBy(Game => Game.Title);
                        using (StreamWriter writer = File.CreateText(fullPath))
                        {
                            string stringData = JsonConvert.SerializeObject(sortedGames);
                            writer.WriteLine(stringData);
                        }

                        return true;
                    }
                }
            }
            catch (Exception e) { return e; }
        }
    }
}
