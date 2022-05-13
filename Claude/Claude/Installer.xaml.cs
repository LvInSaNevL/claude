using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Claude
{
    /// <summary>
    /// Interaction logic for Installer.xaml
    /// </summary>
    public partial class Installer : Window
    {
        // The generic empty UserData.json file, in case the file is missing. 
        private readonly string jsonstring = @"
                                                {
                                                  ""claude"": {},
                                                  ""steam"": {
                                                        ""exe"": ""C:\\Program Files(x86)\\Steam\\steam.exe"",
                                                        ""install"": [""C:\\Program Files(x86)\\Steam\\steamapps""]
                                                  },
                                                  ""battlenet"": {
                                                        ""exe"": ""C:\\Program Files(x86)\\Battle.Net\\Battle.Net.exe"",
                                                        ""install"": [""C:\\Program Files(x86)\\Battle.Net""]
                                                  }
                                                }";

        private static dynamic jsondata;
        public static List<DataTypes.Game> installerGames = new List<DataTypes.Game>();

        public Installer()
        {
            jsondata = FileIn.ReadUserData() ?? JObject.Parse(jsonstring);
            InitializeComponent();
            Welcome();                       
        }

        private void Welcome(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();
            // Welcome Text
            ContentField.Children.Add(new TextBlock() { Text = "Welcome to the Claude installer!" });
            // Info Text
            ContentField.Children.Add(new TextBlock() { Text = "We have just a bit of setup to do first though." });

            NextButton.Click += Games;
            PreviousButton.Visibility = Visibility.Collapsed;
            PreviousButton.Click += DoNothing;
        }

        private void Games(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();
            PreviousButton.Visibility = Visibility.Visible;

            // Welcome Text
            ContentField.Children.Add(new TextBlock() { Text = "Here you can add your launchers" });
            // Info Text
            ContentField.Children.Add(new TextBlock() { Text = "Just click below" });

            String[] launcherCodes = new string[]
            {
                "Steam",
                "BattleNet",
                "Origin",
                "Ubisoft"
            };

            foreach (string launcherCode in launcherCodes)
            {
                Expander dropStack = new Expander()
                {
                    Name = launcherCode,
                    ExpandDirection = ExpandDirection.Down,
                    Header = launcherCode
                };

                dropStack.Content = new Views.LauncherSettings(launcherCode);
                ContentField.Children.Add(dropStack);
            }                      

            NextButton.Click += OtherPage;
            PreviousButton.Click += Welcome;
        }

        private void OtherPage(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();

            Views.LocalSettings localSettings = new Views.LocalSettings();
            ContentField.Children.Add(localSettings);

            NextButton.Click += Finally;
            PreviousButton.Click += Games;
        }

        private void Finally(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();

            TextBlock welcomeText = new TextBlock() { Text = "You have got Claude all set up!" };
            TextBlock infoText = new TextBlock() { Text = "Hit the next button to start playing!" };

            ContentField.Children.Add(welcomeText);
            ContentField.Children.Add(infoText);

            NextButton.Content = "Start Playing";
            NextButton.Click += EndSetup;
            PreviousButton.Click += OtherPage;
        }

        private void EndSetup(object sender = null, RoutedEventArgs e = null)
        {
            // Finally write the data to the file
            Uri maybePath = new Uri("pack://application:,,,/Resources/UserData.json");
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            fullPath = $"{fullPath}\\{maybePath.LocalPath}";

            using (StreamWriter writer = File.CreateText(fullPath))
            {
                writer.WriteLine(jsondata.ToString());
            }

            MainWindow window = new MainWindow();
            window.Show();

            this.Close();
        }

        private void AddInstallLoc(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            (StackPanel, string, string) data = ((StackPanel, string, string))item.Tag;
            StackPanel stack = data.Item1;

            string startDir = InstallLocation(data.Item3);

            var dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = InstallLocation(data.Item3);
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                StackPanel newStack = new StackPanel() { Orientation = Orientation.Horizontal };
                newStack.Children.Add(new TextBlock() { Text = dialog.FileName });
                Button removeButton = new Button() { Content = "Remove", Tag = (newStack, data.Item2, dialog.FileName) };
                removeButton.Click += RemoveInstallLoc;
                newStack.Children.Add(removeButton);
                stack.Children.Insert(stack.Children.Count - 1, newStack);

                var token = jsondata.SelectToken(data.Item2);
                List<string> newLocations = new List<string>();
                foreach(var oldLoc in token.Children()) { newLocations.Add(oldLoc.ToString()); }
                newLocations.Add(dialog.FileName);
                token.Replace(new JArray(newLocations));
            }
        }

        private void RemoveInstallLoc(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            (StackPanel, string, string) data = ((StackPanel, string, string))item.Tag;
            StackPanel stack = data.Item1;

            var token = jsondata.SelectToken(data.Item2);
            List<string> newLocations = new List<string>();
            foreach (var oldLoc in token.Children()) { newLocations.Add(oldLoc.ToString()); }
            newLocations.Remove(data.Item3);
            token.Replace(new JArray(newLocations));

            ContentField.Children.Remove(data.Item1);
            OtherPage();
        }               

        private static void ChangeExePath(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            (TextBlock, string) data = ((TextBlock, string))item.Tag;
            TextBlock text = data.Item1;

            OpenFileDialog openFile = new OpenFileDialog()
            {
                DefaultExt = ".exe",
                InitialDirectory = "C:\\Program Files (x86)"
            };
            openFile.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
            openFile.FilterIndex = 1;
            var result = openFile.ShowDialog();

            if (result == true)
            {
                string fileName = openFile.FileName;
                text.Text = fileName;
                JToken token = jsondata.SelectToken(data.Item2);
                token.Replace(fileName);
            }
        }

        //Get-ChildItem -Path C:\ -Include "steam.exe" -File -Recurse -ErrorAction SilentlyContinue | % { $_.FullName }
        private static string InstallLocation(string exe)
        {
            Dictionary<string, string> defaults = new Dictionary<string, string>()
            {
                { "Steam", "C:\\Program Files (x86)\\Steam" },
                { "BattleNet", "C:\\Program Files(x86)\\Battle.net" },
                { "Origin", "C:\\Program Files(x86)\\Origin" },
                { "Ubisoft", "C:\\Program Files(x86)\\Ubisoft" }
            };
            return jsondata.SelectToken($"{exe}.exe") == null ? (string)jsondata.SelectToken($"{exe}.exe") : defaults[exe];
        }

        private void DoNothing(object sender, RoutedEventArgs e) { }
    }
}
