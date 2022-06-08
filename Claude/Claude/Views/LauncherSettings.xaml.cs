using System;
using System.Collections.Generic;
using System.Reflection;
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
            DataTypes.UserData data = FileIn.ReadUserData();
            Type udType = typeof(DataTypes.UserData);
            PropertyInfo myFieldInfo1 = udType.GetProperty(launcher);
            var formatData = (DataTypes.UserDataLaunchers)myFieldInfo1.GetValue(data, null);

            InitializeComponent();

            // There has to be a better way to do this
            // Maybe static properties?
            LauncherTitle.Text = launcher;
            CurrentExeLoc.Text = formatData.Exe;
            ExeChangeButt.Tag = launcher;
            AddDirButt.Tag = launcher;
            AutoStart.Tag = launcher;
            OnStart.Tag = (launcher, false);
            OnStart.IsChecked = (bool)formatData.Start;
            OnStop.Tag = (launcher, true);
            OnStop.IsChecked = (bool)formatData.Stop;

            string[] installDirs = formatData.Install;
            for (int i = 0; i < installDirs.Length; i++)
            {
                string nowDir = installDirs[i].ToString();
                IndividualDirs.Children.Add(DirPanel(i, nowDir, launcher));
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
            List<DataTypes.Game> games = FileIn.ReadUserGames();
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

        void LauncherAutoControl(object sender, RoutedEventArgs e)
        {
            var item = sender as CheckBox;
            (string, bool) launcher = ((string, bool))item.Tag;

            DataTypes.UserData data = FileIn.ReadUserData();
            Type myTypeB = typeof(DataTypes.UserData);
            PropertyInfo myFieldInfo1 = myTypeB.GetProperty(launcher.Item1);
            var formatData = (DataTypes.UserDataLaunchers)myFieldInfo1.GetValue(data, null);
                        
            if (!launcher.Item2)
            {
                bool isStart = (bool)formatData.Start;
                FileOut.ChangeUserData($"{launcher.Item1}.start", (!isStart).ToString());
                OnStart.IsChecked = !isStart;
            }
            else if (launcher.Item2) 
            {
                bool isStop = (bool)formatData.Stop;
                FileOut.ChangeUserData($"{launcher.Item1}.stop", (!isStop).ToString());
                OnStop.IsChecked = !isStop;
            }
        }
    }
}
