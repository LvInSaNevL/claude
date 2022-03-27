using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace Claude.Views
{
    /// <summary>
    /// Interaction logic for LauncherSettings.xaml
    /// </summary>
    public partial class LauncherSettings : UserControl
    {
        public LauncherSettings(string launcher)
        {
            dynamic data = FileIn.ReadUserData();
            dynamic launcherData = data[launcher];

            InitializeComponent();

            LauncherTitle.Text = launcher;
            CurrentExeLoc.Text = launcherData["exe"];
            ExeChangeButt.Tag = launcher;
            AddDirButt.Tag = launcher;

            for (int i = 0; i < launcherData["install"].Count; i++)
            {
                string test = launcherData["install"][i].ToString();
                IndividualDirs.Children.Add(DirPanel(i, test, launcher));
            }
        }

        private StackPanel DirPanel(int count, string path, string launcher)
        {
            StackPanel dirStack = new StackPanel() 
            { 
                Name = $"dirStack_{count}", 
                Orientation = Orientation.Horizontal, 
                VerticalAlignment = VerticalAlignment.Top 
            };            

            Expander presenter = new Expander() { Name = "presenter", Header = $"{count.ToString()}: {path}" };
            StackPanel gamesStack = new StackPanel() { Orientation = Orientation.Vertical };
            List<Computer.Game> games = FileIn.ReadUserGames();
            for (int i = 0; i < 5; i++)
            {
                gamesStack.Children.Add(new TextBlock() { Text =games[i].Title.ToString() });
            }
            presenter.Content = gamesStack;
            dirStack.Children.Add(presenter);

            Button removeButt = new Button() 
            { 
                Name = $"removeButt_{count}", 
                Content = "Remove", 
                MaxHeight=20,
                Tag = launcher
            };
            removeButt.Click += RemoveGameDirClick;
            dirStack.Children.Add(removeButt);

            return dirStack;
        }

        void ChangeExePathClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            string targetLauncher = item.Tag as string;

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
                string filename = openFile.FileName;
                FileOut.ChangeUserData($"{targetLauncher}.exe", filename);
                CurrentExeLoc.Text = filename;
            }
        }

        void AddGameDirClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            string targetLauncher = (string)item.Tag;

            System.Windows.Forms.FolderBrowserDialog openDir = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = false
            };
            var result = openDir.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string filename = openDir.SelectedPath;

                dynamic userData = FileIn.ReadUserData();
                var dirs = userData[targetLauncher]["install"];

                if (!dirs.Contains(filename))
                {
                    dirs.Add(filename);
                    FileOut.ChangeUserData($"{targetLauncher}.install", dirs);
                    IndividualDirs.Children.Add(DirPanel(dirs.Count, filename, targetLauncher));
                }
            }
        }

        void RemoveGameDirClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            string launcher = (string)item.Tag;

            StackPanel parent = (StackPanel)item.Parent;
            Expander children = (Expander)parent.Children[0];
            string target = children.Header.ToString().Split(": ")[1];

            dynamic userData = FileIn.ReadUserData();
            var dirs = userData[launcher]["install"];

            for(int i = 0; i < dirs.Count; i++)
            {
                if (dirs[i] == target)
                {
                    dirs.RemoveAt(i);
                    FileOut.ChangeUserData($"{launcher}.install", dirs);
                    IndividualDirs.Children.Remove(parent);
                    IndividualDirs.UpdateLayout();
                }
            }
        }
    }
}
