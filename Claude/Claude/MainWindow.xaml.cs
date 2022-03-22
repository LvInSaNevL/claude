using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Claude
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Computer.Initialize();

            SplashScreen splashScreen = new SplashScreen("Resources\\Splashscreen.png");
            splashScreen.Show(true);
            

            Application.Current.MainWindow.WindowState = WindowState.Maximized;


            this.Loaded += MainWindow_Loaded;
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
            splashScreen.Show(false);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<Computer.Game> installedGames = Computer.GetGames();

            StackPanel boxArtStack = new StackPanel() { Orientation = Orientation.Vertical, Name = "boxArtStack" };

            int gridWidth = (int)biggerBoxInstalled.ActualWidth / 345;
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
                detailMenus.Add(details);
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
                    Computer.Game nowgame = new Computer.Game();
                    try { nowgame = installedGames[counter - 1]; }
                    catch (ArgumentOutOfRangeException) { break; }

                    // Adding button image to big box art
                    BitmapImage target = new BitmapImage();
                    try { target = new BitmapImage(new Uri($"{FilePaths.cache}/{nowgame.Id}.jpg")); }
                    catch { target = new BitmapImage(new Uri($"pack://application:,,,/Resources/{nowgame.Launcher}Holder.jpg", UriKind.Absolute)); }

                    nowgame.DetailFrame = details;
                    Button gameButton = ControlBuilder.BoxArtButton(nowgame, target);
                    gameButton.Click += GameButtonClick;
                    gameButton.MouseDoubleClick += LauncherButton;
                    Grid.SetColumn(gameButton, y);

                    currentRow.Children.Add(gameButton);

                    // Adding text to small box art
                    Button gameText = new Button
                    {
                        Tag = nowgame.Id,
                        Content = nowgame.Title,
                        Height = 50,
                        ToolTip = $"{nowgame.Launcher}: {nowgame.Id}"
                    };
                    gameText.Click += GameButtonClick;
                    gameText.MouseDoubleClick += LauncherButton;

                    if (nowgame.Launcher == "Steam") { SteamExpanderStack.Children.Add(gameText); }
                    if (nowgame.Launcher == "BattleNet") { BattleNetExpanderStack.Children.Add(gameText); }
                }

                fullCurrentRow.Children.Add(currentRow);
                fullCurrentRow.Children.Add(details);
                boxArtStack.Children.Add(fullCurrentRow);
            }

            biggerBoxInstalled.Content = boxArtStack;
        }


        /// <summary>
        /// Start button handlers 
        /// </summary>
        private List<StackPanel> detailMenus = new List<StackPanel>();
        public void GameButtonClick(object sender, RoutedEventArgs e)
        {
            // Cleaning up old menus
            foreach (StackPanel menu in detailMenus)
            {
                menu.Children.Clear();
                menu.Visibility = Visibility.Hidden;
            }


            Computer.Game button = (Computer.Game)(sender as Button).Tag;
            StackPanel details = button.DetailFrame;

            details.Children.Add(new Views.GameDetails(button, (biggerBoxInstalled.ActualWidth, biggerBoxInstalled.ActualHeight)));
            details.Visibility = Visibility.Visible;
            details.BringIntoView();
        }

        public static void LauncherButton(object sender, RoutedEventArgs e) { Steam.Launch((sender as Button).Tag.ToString()); }
        void BattleNetLauncherButton(object sender, RoutedEventArgs e) { BattleNet.Launch((sender as Button).Tag.ToString()); }

        public static void GameDetailThumbnailSwitcher(object sender, RoutedEventArgs e)
        {
            Button caller = sender as Button;

            StackPanel thumbParent = FindBigThumbnail(caller);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(thumbParent); i++)
            {
                var element = VisualTreeHelper.GetChild(thumbParent, i);
                if (element.GetType() == typeof(Image))
                {
                    Image parsed = (Image)element;
                    parsed.Source = new BitmapImage(new Uri(caller.Tag.ToString()));
                }
            }
        }

        private static StackPanel FindBigThumbnail(DependencyObject starter)
        {
            if (starter == null) { return null; }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(starter); i++)
            {
                var element = VisualTreeHelper.GetChild(starter, i);
                if (element.GetType() == typeof(StackPanel))
                {
                    StackPanel castedElement = (StackPanel)element;
                    if (castedElement.Name == "leftDetails") { return castedElement; }
                }
            }
            return FindBigThumbnail(VisualTreeHelper.GetParent(starter));
        }

        public void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            ClaudeSettings settings = new ClaudeSettings(item.Tag.ToString());
            settings.Show();
        }

        public void settingsMenuClick(object sender, RoutedEventArgs e)
        {
            var addButton = sender as FrameworkElement;
            if (addButton != null)
            {
                addButton.ContextMenu.IsOpen = true;
            }
        }

        public void userDataNotFound(object sender, RoutedEventArgs e)
        {
            Claude.Installer instalelr = new Claude.Installer();
            Close();
            instalelr.Show();
        }

        public void ShutDown(object sender, RoutedEventArgs e) { Computer.ShutDown(); }

        private void steamAuthRoute(object sender, RoutedEventArgs e) { Console.Write("This was supposed to do something lol"); }
    }
}
