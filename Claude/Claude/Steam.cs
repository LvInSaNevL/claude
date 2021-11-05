using System.IO;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using System;
using System.Collections.Generic;
using Gameloop.Vdf.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

public class Steam
{
    private static readonly string baseLocation = "C:\\Program Files (x86)\\Steam";
	public static List<string> Installed()
    {
        string installList = baseLocation + "\\steamapps\\libraryfolders.vdf";
        VProperty rawFolders = VdfConvert.Deserialize(File.ReadAllText(installList));
        JToken libraryFolders = rawFolders.ToJson().Last;

        List<string> appList = new List<string>();
        int counter = 0;
        for (int s = 0; s < (libraryFolders.Count() - 1); s++)
        {
            try
            {
                var temp = libraryFolders[s.ToString()]["apps"];
                foreach (var g in temp)
                {
                    string[] bits = g.ToString().Split(":");
                    appList.Add(bits[0].Replace("\"", ""));
                }

                counter++;
            }
            catch (Exception) { Console.WriteLine("Null reference exception"); }
        };

        return appList;
        
    }

    
}