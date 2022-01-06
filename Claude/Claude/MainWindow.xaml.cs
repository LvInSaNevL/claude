using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IdentityModel.OidcClient;
using System.IO;

namespace Claude
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SplashScreen splashScreen = new SplashScreen("Resources\\Splashscreen.png");
            splashScreen.Show(true);

            Computer.Initialize();

            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

            splashScreen.Show(false);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<Computer.Game> steamGames = Steam.InstalledAsync().Result;

            StackPanel gameRows = new StackPanel();
            gameRows.Orientation = Orientation.Vertical;

            Grid gameGrid = new Grid();
            int gridWidth = (int)biggerBoxInstalled.ActualWidth / 345;
            int gridHeight = steamGames.Count / gridWidth;

            RowDefinition[] rows = new RowDefinition[gridHeight];
            ColumnDefinition[] columns = new ColumnDefinition[gridWidth];

            for (int i = 0; i < gridHeight; i++)
            {
                rows[i] = new RowDefinition();
                gameGrid.RowDefinitions.Add(rows[i]);
            }
            for (int i = 0; i < gridWidth; i++)
            {
                columns[i] = new ColumnDefinition();
                gameGrid.ColumnDefinitions.Add(columns[i]);
            }

            Expander steamLibrary = new Expander
            {
                ExpandDirection = ExpandDirection.Down,
                IsExpanded = true,
                Header = "Steam Library",
            };
            StackPanel textStack = new StackPanel { Orientation = Orientation.Vertical };

            int counter = 0;
            for (int x = 0; x < gridHeight; x++)
            {
                for (int y = 0; y < gridWidth; y++)
                {
                    counter++;
                    Computer.Game nowgame = steamGames[counter - 1];

                    // Adding button image to big box art
                    BitmapImage target = new BitmapImage();
                    try { target = new BitmapImage(new Uri(Computer.CachePath($"{nowgame.Id}.jpg"))); }
                    catch { target = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }

                    Button gameButton = ControlBuilder.BoxArtButton(nowgame.Launcher, nowgame.Id, target, nowgame.Launcher);
                    gameButton.Click += GameButtonClick;
                    Grid.SetColumn(gameButton, y);
                    Grid.SetRow(gameButton, x);

                    gameGrid.Children.Add(gameButton);

                    // Adding text to small box art
                    Button gameText = new Button
                    {
                        Tag = nowgame.Id,
                        Content = nowgame.Title,
                        Height = 50
                    };
                    gameText.Click += GameButtonClick;
                    textStack.Children.Add(gameText);
                }
            };

            steamLibrary.Content = textStack;
            leftHandMenu.Content = steamLibrary;
            biggerBoxInstalled.Content = gameGrid;
        }


        /// <summary>
        /// Start button handlers 
        /// </summary>
        

        public void GameButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Steam.Launch(button.Tag.ToString());
        }
        
        public void settingsButtonClick(object sender, RoutedEventArgs e)
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

        private async void steamAuthRoute(object sender, RoutedEventArgs e) { await Steam.AuthenticateAsync(); }
    }
}
