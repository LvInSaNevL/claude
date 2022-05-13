using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Claude.Views
{
    internal class GameViewer
    {
        public static void BigBoxArt(MainWindow mainWindow)
        {
            List<DataTypes.Game> installedGames = Computer.GetGames();

            StackPanel boxArtStack = new StackPanel() { Orientation = Orientation.Vertical, Name = "boxArtStack" };

            int gridWidth = (int)mainWindow.biggerBoxInstalled.ActualWidth / 345;
            float gridHeight = (installedGames.Count + 1) / gridWidth + 1;

            int counter = 0;
            for (int x = 0; x < gridHeight; x++)
            {
                // Setting up this row
                StackPanel fullCurrentRow = new StackPanel() { Orientation = Orientation.Vertical };
                StackPanel details = new StackPanel()
                {
                    Visibility = Visibility.Collapsed,
                    Name = $"detailStack{x}",
                    Focusable = true
                };
                mainWindow.detailMenus.Add(details);
                Grid currentRow = new Grid() { Margin = new Thickness { Top = 10, Right = 0, Bottom = 10, Left = 0 } };
                currentRow.RowDefinitions.Add(new RowDefinition());
                ColumnDefinition[] columns = new ColumnDefinition[gridWidth];


                for (int i = 0; i < gridWidth; i++)
                {
                    columns[i] = new ColumnDefinition();
                    currentRow.ColumnDefinitions.Add(columns[i]);
                }

                for (int y = 0; y < gridWidth; y++)
                {
                    counter++;
                    DataTypes.Game nowgame = new DataTypes.Game();
                    try { nowgame = installedGames[counter - 1]; }
                    catch (ArgumentOutOfRangeException) { break; }

                    // Adding button image to big box art
                    BitmapImage target = new BitmapImage();
                    try { target = new BitmapImage(new Uri($"{FilePaths.cache}/{nowgame.Id}.jpg")); }
                    catch { target = new BitmapImage(new Uri($"pack://application:,,,/Resources/{nowgame.Launcher}Holder.jpg", UriKind.Absolute)); }

                    nowgame.DetailFrame = details;
                    Button gameButton = ControlBuilder.BoxArtButton(nowgame, target);
                    gameButton.Click += mainWindow.GameButtonClick;
                    gameButton.MouseDoubleClick += MainWindow.LauncherButtonClick;
                    Grid.SetColumn(gameButton, y);

                    currentRow.Children.Add(gameButton);

                    // Adding text to small box art
                    Button gameText = new Button
                    {
                        Tag = nowgame,
                        Content = nowgame.Title,
                        Height = 50,
                        ToolTip = $"{nowgame.Launcher}: {nowgame.Id}"
                    };
                    gameText.Click += mainWindow.GameButtonClick;
                    gameText.MouseDoubleClick += MainWindow.LauncherButtonClick;

                    if (nowgame.Launcher == "Steam") { mainWindow.SteamExpanderStack.Children.Add(gameText); }
                    if (nowgame.Launcher == "BattleNet") { mainWindow.BattleNetExpanderStack.Children.Add(gameText); }
                    if (nowgame.Launcher == "Others") { mainWindow.OthersExpanderStack.Children.Add(gameText); }
                }

                fullCurrentRow.Children.Add(currentRow);
                fullCurrentRow.Children.Add(details);
                boxArtStack.Children.Add(fullCurrentRow);
            }

            mainWindow.biggerBoxInstalled.Content = boxArtStack;
        }
    }
}
