using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
        private readonly string jsonstring = "{\"claude\":{},\"Steam\":{\"exe\":\"\",\"install\":[]},\"BattleNet\":{\"exe\":\"\",\"install\":[]},\"Origin\":{\"exe\":\"\",\"install\":[]},\"Ubisoft\":{\"exe\":\"\",\"install\":[]}}";
        private static dynamic jsondata;
        public static List<Computer.Game> installerGames = new List<Computer.Game>();

        public Installer()
        {
            jsondata = JObject.Parse(jsonstring);
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
            PreviousButton.Click += DoNothing;
        }

        private void Games(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();

            // Welcome Text
            ContentField.Children.Add(new TextBlock() { Text = "Here you can add your launchers" });
            // Info Text
            ContentField.Children.Add(new TextBlock() { Text = "Just click below" });

            // Steam
            ContentField.Children.Add(LauncherDropDown("Steam",
                                                       "Steam",
                                                       "steam"));
            // Battle.Net
            ContentField.Children.Add(LauncherDropDown("Battle.Net",
                                                       "BattleNet",
                                                       "Battle.net Launcher"));
            // Origin
            ContentField.Children.Add(LauncherDropDown("Origin",
                                                       "Origin",
                                                       "Origin"));
            // Ubisoft
            ContentField.Children.Add(LauncherDropDown("Ubisoft Connect",
                                                       "Ubisoft",
                                                       "UbisoftConnect"));
                      

            NextButton.Click += Others;
            PreviousButton.Click += Welcome;
        }

        private void Others(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();

            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };
            stackPanel.Children.Add(new TextBlock() { Text = "Add any other games here: " });

            if (installerGames != null)
            {
                foreach (Computer.Game game in installerGames)
                {
                    stackPanel.Children.Add(new TextBlock() { Text = game.Title });
                }
            }
            Button addButton = new Button() 
            { 
                Content = new TextBlock() { Text = "Add game" },
                Tag = stackPanel
            };
            addButton.Click += AddOtherGame;
            stackPanel.Children.Add(addButton);

            ContentField.Children.Add(stackPanel);

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

            NextButton.Click += EndSetup;
            PreviousButton.Click += Others;
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

        private StackPanel LauncherDropDown(string launcher, string shortName, string executable)
        {
            StackPanel dropStack = new StackPanel() { Orientation = Orientation.Vertical };
            dropStack.Children.Add(new TextBlock() { Text = $"{launcher}" });

            StackPanel launcherLoc = new StackPanel() { Orientation = Orientation.Horizontal };
            launcherLoc.Children.Add(new TextBlock() { Text = "Installer Location: " });
            TextBlock installLoc = new TextBlock() { Text = InstallLocation(shortName) };
            launcherLoc.Children.Add(installLoc);
            Button changePathButton = new Button() { Content = "Change", Tag = (installLoc, $"{shortName}.exe") };
            changePathButton.Click += ChangeExePath;
            launcherLoc.Children.Add(changePathButton);
            dropStack.Children.Add(launcherLoc);

            StackPanel installLocs = new StackPanel() { Orientation = Orientation.Vertical };
            installLocs.Children.Add(new TextBlock() { Text = "Install Directories: " });
            Button addButton = new Button() { Content = "Add install location", Tag = (installLocs, $"{shortName}.install", shortName) };
            addButton.Click += AddInstallLoc;
            installLocs.Children.Add(addButton);
            dropStack.Children.Add(installLocs);

            return dropStack;
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
            Others();
        }

        private void AddOtherGame(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            StackPanel stack = (StackPanel)item.Tag;

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                StackPanel newStack = new StackPanel() { Orientation = Orientation.Horizontal };
                newStack.Children.Add(new TextBlock() { Text = dialog.FileName });
                Button removeButton = new Button() { Content = "Remove", Tag = (newStack, dialog.FileName) };
                removeButton.Click += RemoveOtherGame;
                newStack.Children.Add(removeButton);
                stack.Children.Insert(stack.Children.Count - 1, newStack);

                Computer.Game game = new Computer.Game()
                {
                    Title = dialog.FileName,
                    Launcher = "Other",
                    Path = dialog.FileName,
                };
                installerGames.Add(game);
            }


        }

        private void RemoveOtherGame(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            (StackPanel, string) data = ((StackPanel, string))item.Tag;
            StackPanel stack = data.Item1;

            for (int i = 0; i < installerGames.Count; i++)
            {
                if (installerGames[i].Title == data.Item2)
                {
                    installerGames.RemoveAt(i);
                }
            }
            ContentField.Children.Remove(data.Item1);
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
