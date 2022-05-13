using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

            this.Loaded += MainWindow_Loaded;
            this.WindowState = WindowState.Maximized;

            InitializeComponent();
            splashScreen.Show(false);
        }

        /// <summary>
        /// Helper class to be able to call in the games view
        /// </summary>
        public static StackPanel stackPanel { get; }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e) => Views.GameViewer.BigBoxArt(this);


        /// <summary>
        /// Start button handlers 
        /// </summary>
        public List<StackPanel> detailMenus = new List<StackPanel>();
        public void GameButtonClick(object sender, RoutedEventArgs e)
        {
            DataTypes.Game button = (DataTypes.Game)(sender as Button).Tag;
            StackPanel details = button.DetailFrame;

            // Local game check
            if (button.Launcher == "Others") { Others.Launch(button.Path); return; }

            // Cleaning up old menus
            foreach (StackPanel menu in detailMenus)
            {
                menu.Children.Clear();
                menu.Visibility = Visibility.Hidden;
            }
                     
            details.Children.Add(new Views.GameDetails(button, (biggerBoxInstalled.ActualWidth, biggerBoxInstalled.ActualHeight)));
            details.Visibility = Visibility.Visible;
            details.BringIntoView();
        }

        public static void LauncherButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataTypes.Game game = (DataTypes.Game)button.Tag;

            switch (game.Launcher)
            {
                case "Steam":
                    Steam.Launch(game.Id);
                    break;
                case "BattleNet":
                    BattleNet.Launch(game.Id);
                    break;
                case "Others":
                    Others.Launch(game.Path);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

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
