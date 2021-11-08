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
using IdentityModel.OidcClient;
using System.Threading.Tasks;

public class Steam
{
    private static readonly string baseLocation = "C:\\Program Files (x86)\\Steam";
	public static async Task<List<string>> InstalledAsync()
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

                    Computer.TempDownload($"https://cdn.akamai.steamstatic.com/steam/apps/{bits[0].Replace("\"", "")}/header.jpg", bits[0].Replace("\"", "") + ".jpg");
                }

                counter++;
            }
            catch (Exception) { Console.WriteLine("Null reference exception"); }
        };

        return appList;
    }

    public static async Task AuthenticateAsync()
    {
        var options = new OidcClientOptions
        {
            Authority = "https://demo.identityserver.io",
            ClientId = "native.hybrid",
            Scope = "openid profile api offline_access",
            RedirectUri = "io.identityserver.demo.uwp://callback"
        };
        var client = new OidcClient(options);

        var state = await client.PrepareLoginAsync();

        Computer.Terminal(state.ToString());
    }  
}