using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            dynamic data = Computer.ReadUserData();
            dynamic launcherData = data[launcher];

            InitializeComponent();

            LauncherTitle.Text = launcher;
            CurrentExeLoc.Text = launcherData["exe"];

            for (int i = 0; i < launcherData["install"].Count; i++)
            {
                string test = launcherData["install"][i].ToString();
                IndividualDirs.Children.Add(DirPanel(i, test));
            }
        }

        private StackPanel DirPanel(int count, string path)
        {
            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
            panel.Children.Add(new TextBlock() { Text = $"{count.ToString()}: " });

            StackPanel dirStack = new StackPanel() { Orientation = Orientation.Horizontal };
            dirStack.Children.Add(new TextBlock() { Text = $"{count.ToString()}: {path}" });
            dirStack.Children.Add(new Button() { Content = "Remove" });
            panel.Children.Add(dirStack);

            Expander presenter = new Expander();
            StackPanel gamesStack = new StackPanel() { Orientation = Orientation.Vertical };
            List<Computer.Game> games = Computer.ReadUserGames();
            for (int i = 0; i < 5; i++)
            {
                gamesStack.Children.Add(new TextBlock() { Text =games[i].Title.ToString() });
            }
            presenter.Content = gamesStack;
            panel.Children.Add(presenter);

            return panel;
        }

        public static void ChangeExePathClick(object sender, RoutedEventArgs e)
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
                string filename = openFile.FileName;
                dynamic userData = Computer.ReadUserData();
                Computer.ChangeUserData(data.Item2, filename);
                text.Text = filename;
            }
        }


        private struct LauncherConfig
        {
            public string LauncherLoc { get; set; }
            public string[] InstallDirs { get; set; }
        }
    }
}
