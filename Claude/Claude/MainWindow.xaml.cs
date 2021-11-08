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
            List<string> steamGames = Steam.InstalledAsync().Result;

            Grid gameGrid = new Grid();
            int gridWidth = (int)biggerBoxInstalled.ActualWidth / 460;
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

            StackPanel textStack = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            int counter = 0;
            for (int x = 0; x < gridHeight; x++)
            {
                for (int y = 0; y < gridWidth; y++)
                {
                    counter++;
                    string gameID = steamGames[counter - 1];

                    // Adding button image to big box art
                    BitmapImage target = new BitmapImage();
                    try { target = new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}/cache/{gameID}.jpg")); }
                    catch { target = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }

                    Button gameButton = new Button
                    {
                        Tag = gameID,
                        Content = new Image { Source = target },
                        Height = 215,
                        Width = 460,
                        ToolTip = gameID,
                    };
                    gameButton.Click += GameButtonClick;
                    Grid.SetColumn(gameButton, y);
                    Grid.SetRow(gameButton, x);

                    gameGrid.Children.Add(gameButton);

                    // Adding text to small box art
                    Button gameText = new Button
                    {
                        Tag = gameID,
                        Content = gameID,
                        Height = 50
                    };
                    gameText.Click += GameButtonClick;
                    textStack.Children.Add(gameText);
                }
            };

            smallerBoxInstalled.Content = textStack;
            biggerBoxInstalled.Content = gameGrid;
        }

        public static void GameButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Computer.Terminal("steam://rungameid/" + button.Content);
        }
        

        private async void steamAuthRoute(object sender, RoutedEventArgs e) { await Steam.AuthenticateAsync(); }
    }
}
