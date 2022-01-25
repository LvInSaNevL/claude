using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

            StackPanel boxArtStack = new StackPanel() { Orientation = Orientation.Vertical, Name = "boxArtStack" };

            int gridWidth = (int)biggerBoxInstalled.ActualWidth / 345;
            int gridHeight = steamGames.Count / gridWidth;

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
                    Computer.Game nowgame = steamGames[counter - 1];

                    // Adding button image to big box art
                    BitmapImage target = new BitmapImage();
                    try { target = new BitmapImage(new Uri(Computer.CachePath($"{nowgame.Id}.jpg"))); }
                    catch { target = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }

                    nowgame.detailFrame = details;
                    Button gameButton = ControlBuilder.BoxArtButton(nowgame, target);
                    gameButton.Click += GameButtonClick;
                    Grid.SetColumn(gameButton, y);

                    currentRow.Children.Add(gameButton);

                    // Adding text to small box art
                    Button gameText = new Button
                    {
                        Tag = nowgame,
                        Content = nowgame.Title,
                        Height = 50,
                    };
                    gameText.Click += GameButtonClick;
                    textStack.Children.Add(gameText);
                }

                fullCurrentRow.Children.Add(currentRow);
                fullCurrentRow.Children.Add(details);
                boxArtStack.Children.Add(fullCurrentRow);
            }

            steamLibrary.Content = textStack;
            leftHandMenu.Content = steamLibrary;
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
            StackPanel details = button.detailFrame;

            details.Children.Add(ControlBuilder.gameDetails(button, (biggerBoxInstalled.ActualWidth, biggerBoxInstalled.ActualHeight)));
            details.Visibility = Visibility.Visible;
            details.BringIntoView();
        }

        public static void LauncherButton(object sender, RoutedEventArgs e) { Steam.Launch((sender as Button).Tag.ToString()); }

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

        public void settingsButtonClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            ClaudeSettings settings = new ClaudeSettings(item.Tag.ToString());
            settings.Show();
            settings.Focus();
        }

        public void settingsMenuClick(object sender, RoutedEventArgs e)
        {
            var addButton = sender as FrameworkElement;
            if (addButton != null)
            {
                addButton.ContextMenu.IsOpen = true;
            }
        }

        private void steamAuthRoute(object sender, RoutedEventArgs e) { Console.Write("This was supposed to do something lol"); }
    }
}
