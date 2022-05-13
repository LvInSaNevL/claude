using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json.Linq;

namespace Claude.Views
{
    /// <summary>
    /// Interaction logic for LauncherSettings.xaml
    /// </summary>
    public partial class LocalSettings : UserControl
    {
        public LocalSettings()
        {
            dynamic games = FileIn.ReadUserGames();
            List<DataTypes.Game> gamesList = new List<DataTypes.Game>();
            foreach (DataTypes.Game game in games) { if (game.Launcher == "Others") { gamesList.Add(game); } }

            InitializeComponent();

            for (int i = 0; i < gamesList.Count; i++)
            {
                UpdateGamesListing(gamesList[i], true);
            }
        }

        private void UpdateGamesListing(DataTypes.Game target, bool add)
        {
            if (add)
            {
                int count = IndividualGames.Children.Count;
                string entry = $"{count}: {target.Title}";
                StackPanel newGame = DirPanel(count, target);
                IndividualGames.Children.Insert(count - 1, newGame);
            }
            else
            {
                var children = IndividualGames.Children;
                foreach (StackPanel child in children)
                {
                    var childTitle = child.Children.OfType<TextBlock>().FirstOrDefault();
                    if (childTitle.Text.Contains(target.Title))
                    {
                        IndividualGames.Children.Remove(child);
                        foreach (DataTypes.Game game in FileIn.ReadUserGames())
                        {
                            if (game.Launcher == "Others") { UpdateGamesListing(game, true); }
                        }
                        return;
                    }
                }
            }

            IndividualGames.UpdateLayout();
        }

        private StackPanel DirPanel(int count, DataTypes.Game game)
        {
            StackPanel dirStack = new StackPanel()
            {
                Name = $"dirStack_{count}",
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top
            };

            string path = $"{count}: {game.Title}";
            dirStack.Children.Add(new TextBlock() { Name = "titleField", Text = path });

            Button removeButt = new Button() 
            { 
                Name = $"removeButt_{count}", 
                Content = "Remove", 
                MaxHeight=20,
                Tag = game,
            };
            removeButt.Click += RemoveGameClick;
            dirStack.Children.Add(removeButt);

            return dirStack;
        }
        
        private void AddGameClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            var parent = item.Parent;

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string id = dialog.FileName;
                string title = Path.GetFileNameWithoutExtension(dialog.FileName);

                DataTypes.Game game = new DataTypes.Game()
                {
                    Id = title,
                    Title = title,
                    Launcher = "Others",
                    Path = dialog.FileName,
                    Thumbnail = FileOut.IconDownload(id, title)
                };

                Others.Install(game);
                UpdateGamesListing(game, true);

                MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().SingleOrDefault();
                if (mainWindow != null) { GameViewer.BigBoxArt(mainWindow); }
            }


        }

        private void RemoveGameClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            var game = (DataTypes.Game)item.Tag;

            FileOut.RemoveUserGames(game);
            UpdateGamesListing(game, false);
        }        
    }
}
