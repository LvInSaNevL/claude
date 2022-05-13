using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
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
                    try { parsedData = JObject.Parse(resutl); }
                    catch (JsonReaderException e) { return null; }
                }
            }
            catch (FileNotFoundException e) { return new FileNotFoundException(); }

            return parsedData;
        }

        public static List<DataTypes.Game> ReadUserGames()
        {
            try
            {
                string fullPath = FilePaths.resources("UserGames.json");
                using StreamReader reader = new StreamReader(fullPath);
                string result = reader.ReadToEnd().ToString();
                reader.Dispose();

                return JsonConvert.DeserializeObject<List<DataTypes.Game>>(result);
            }
            catch { return new List<DataTypes.Game>(); }
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

        public static string IconDownload(string exePath, string exeName)
        {
            string savePath = $"{FilePaths.cache}/{exeName}.jpg";

            if (File.Exists(savePath)) { return savePath; }

            Bitmap icon = Icon.ExtractAssociatedIcon(exePath).ToBitmap();
            Bitmap strechedIcon = new Bitmap(icon, 460, 216);
            
            // Save the file and return the path
            strechedIcon.Save(savePath);
            return savePath;
        }

        public static object ChangeUserData(string target, object newVal)
        {
            string fullPath = FilePaths.resources("UserData.json");

            try
            {
                lock (fullPath)
                {
                    dynamic data = FileIn.ReadUserData();

                    JToken token = data.SelectToken(target);
                    token.Replace((JToken)newVal);
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

        public static object AddUserGames(DataTypes.Game test)
        {
            string fullPath = FilePaths.resources("UserGames.json");

            try 
            { 
                lock (fullPath)
                {
                    List<DataTypes.Game> list = FileIn.ReadUserGames();
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

        public static object RemoveUserGames(DataTypes.Game test)
        {
            string fullPath = FilePaths.resources("UserGames.json");

            try
            {
                lock (fullPath)
                {
                    List<DataTypes.Game> list = FileIn.ReadUserGames();
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
