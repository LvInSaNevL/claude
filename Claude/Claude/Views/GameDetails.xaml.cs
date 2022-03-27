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
using Newtonsoft.Json.Linq;

namespace Claude.Views
{
    /// <summary>
    /// Interaction logic for GameDetails.xaml
    /// </summary>
    public partial class GameDetails : UserControl
    {
        public GameDetails(Computer.Game game, (double width, double height) dimensions)
        {
            InitializeComponent();
            List<Computer.Game> allGames = FileIn.ReadUserGames();
            Computer.Game gameInfo = allGames.Find(x => x.Id == game.Id);

            // Setting sizes
            leftDetails.Width = dimensions.width * 0.6;
            leftDetails.Height = dimensions.height - 250;
            rightDetails.Width = dimensions.width * 0.4;
            rightDetails.Height = dimensions.height - 250;

            // Header image, Grid[1, 0]
            try { headerPic.Source = new BitmapImage(new Uri($"{FilePaths.cache}/{game.Id}.jpg")); }
            catch { headerPic.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }
            Color bgColor = ControlBuilder.BoxArtAverage((BitmapImage)headerPic.Source);
            leftDetails.Background = new SolidColorBrush(bgColor);
            rightDetails.Background = new SolidColorBrush(bgColor);

            // Thumbnails
            bigThumbnail.MaxHeight = (dimensions.height - 250) * 0.60;
            BigThumbnailData(bigThumbnail, gameInfo.Thumbnail);

            // Little thumbnails
            thumbStack.MaxHeight = (dimensions.height * 0.10);
            for (int i = 0; i < gameInfo.Screenshots.Length; i++)
            {
                BitmapImage littleThumb = new BitmapImage();
                try { littleThumb = new BitmapImage(new Uri(gameInfo.Screenshots[i])); }
                catch { littleThumb = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }
                Image littleThumbnail = new Image()
                {
                    Source = littleThumb,
                    Margin = new Thickness(3)
                };
                Button littleButton = new Button()
                {
                    Content = littleThumbnail,
                    Tag = (bigThumbnail, gameInfo.Screenshots[i])

                };
                littleButton.Click += LittleThumbnailSwitcher;

                thumbStack.Children.Add(littleButton);
            }

            // Game launchers
            Button steamButton = ControlBuilder.SteamButton(game.Id);
            steamButton.MaxHeight = (dimensions.height * 0.12);
            launcherButtons.Children.Add(steamButton);

            Button battleButton = ControlBuilder.BattleNetButton(game.Id);
            battleButton.MaxHeight = (dimensions.height * 0.12);
            launcherButtons.Children.Add(battleButton);

            // Game title, Grid[1, 1]
            gameTitle.Text = game.Title;
            // Game details, Grid[1, 2]
            details.Text = game.About;
            detailTextContainer.MaxHeight = dimensions.height * 0.3;
            releaseDate.Text = $"Release Date: {game.Release}";
            developer.Text = $"Developer: {game.Developer}";
            publisher.Text = $"Publisher: {game.Publisher}";
        }

        private void LittleThumbnailSwitcher(object sender, RoutedEventArgs e)
        {
            Button caller = sender as Button;
            (Image, string) data = ((Image, string))caller.Tag;

            BigThumbnailData(data.Item1, data.Item2);
        }

        private static void BigThumbnailData(Image image, string target)
        {
            BitmapImage source;
            try { source = new BitmapImage(new Uri(target)); }
            catch { source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }

            image.Source = source;
        }
    }
}
