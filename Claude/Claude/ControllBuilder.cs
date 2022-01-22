using Claude;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public class ControlBuilder
{
    public static string LauncherColor(string launcher)
    {
        switch (launcher.ToLower())
        {
            case "steam":
                return "#0e2f64";
            case "uplay":
                return "#202124";
            case "origin":
                return "#e45420";
            default:
                return "NO VALUE GIVEN";
        }
    }

    public static Button BoxArtButton(Computer.Game game, BitmapImage picture)
	{
        Button gameButton = new Button
        {
            Content = game.Id,
            Tag = game,
            Background = (SolidColorBrush)new BrushConverter().ConvertFrom(LauncherColor(game.Launcher)),
            Height = 160,
            Width = 345,
            ToolTip = BigBoyLetters(game.Title)
        };

        Image gameArt = new Image { Source = picture };
        var buttonStyle = new Style
        {
            TargetType = typeof(Border),
            Setters = {new Setter { Property = Border.CornerRadiusProperty, Value = new CornerRadius(15)} }
            
        };
        gameArt.Resources.Add(buttonStyle.TargetType, buttonStyle);
        gameButton.Resources.Add(buttonStyle.TargetType, buttonStyle);

        gameButton.Content = gameArt;
        return gameButton;
    }

    private static string BigBoyLetters(string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }

    public static Grid gameDetails(Computer.Game game, (double width, double height) dimensions)
    {
        JObject gameInfo = Steam.Details(game.Id);

        Grid fullDetails = new Grid();
        ColumnDefinition c1 = new ColumnDefinition();
        c1.Width = new GridLength(60, GridUnitType.Star);
        ColumnDefinition c2 = new ColumnDefinition();
        c2.Width = new GridLength(40, GridUnitType.Star);
        fullDetails.ColumnDefinitions.Add(c1);
        fullDetails.ColumnDefinitions.Add(c2);
        StackPanel leftDetails = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            Background = Brushes.Red,
            Width = dimensions.width * 0.6,
            Height = dimensions.height - 250
        };
        Grid.SetColumn(leftDetails, 0);
        StackPanel rightDetails = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            Background = Brushes.Green,
            Width = dimensions.width * 0.4,
            Height = dimensions.height - 250
        };
        Grid.SetColumn(rightDetails, 1);

        // Header image, Grid[1,0]
        Image headerPic = new Image();
        try { headerPic.Source = new BitmapImage(new Uri(Computer.CachePath($"{game.Id}.jpg"))); }
        catch { headerPic.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }
        rightDetails.Children.Add(headerPic);

        rightDetails.Children.Add(new TextBlock()
        {
            Text = gameInfo["name"].ToString()
        });

        // Game details, Grid[1,1]
        ScrollViewer details = new ScrollViewer()
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MaxHeight = dimensions.height * 0.3
        };
        TextBlock detailText = new TextBlock()
        {
            Text = gameInfo["about_the_game"].ToString(),
            TextWrapping = TextWrapping.Wrap
        };
        details.Content = detailText;
        rightDetails.Children.Add(details);

        // Developer info
        StackPanel devInfo = new StackPanel() { Orientation = Orientation.Vertical };
        devInfo.Children.Add(new TextBlock() { Text = $"Release Date: {gameInfo["release_date"]["date"]}" });
        devInfo.Children.Add(new TextBlock() { Text = $"Developer: {gameInfo["developers"][0]}" });
        devInfo.Children.Add(new TextBlock() { Text = $"Publisher: {gameInfo["publishers"][0]}" });
        rightDetails.Children.Add(devInfo);

        fullDetails.Children.Add(leftDetails);
        fullDetails.Children.Add(rightDetails);


        // Screen shots
        List<JObject> screenshots = new List<JObject>();
        foreach (JObject bundle in gameInfo["screenshots"])
        {
            screenshots.Add(bundle);
        }

        BitmapImage thumbnail = new BitmapImage();
        try { thumbnail = new BitmapImage(new Uri(screenshots[0]["path_full"].ToString())); }
        catch { thumbnail = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }
        Image bigThumbnail = new Image()
        {
            MaxHeight = (dimensions.height - 250) * 0.60,
            Source = thumbnail,
            Margin = new Thickness(12)
        };
        leftDetails.Children.Add(bigThumbnail);


        ScrollViewer thumbScroller = new ScrollViewer()
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
            MaxWidth = dimensions.width * 0.58
        };
        StackPanel thumbStack = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            MaxHeight = dimensions.height * 0.10
        };
        thumbScroller.Content = thumbStack;
        for (int i = 0; i < screenshots.Count; i++)
        {
            BitmapImage littleThumb = new BitmapImage();
            try { littleThumb = new BitmapImage(new Uri(screenshots[i]["path_thumbnail"].ToString())); }
            catch { littleThumb = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute)); }
            Image littleThumbnail = new Image()
            {
                Source = littleThumb,
                Margin = new Thickness(3)
            };
            Button littleButton = new Button()
            {
                Content = littleThumbnail,
                Tag = new object[2] { screenshots[i]["path_full"].ToString(), dimensions.height }
            };
            littleButton.Click += MainWindow.GameDetailThumbnailSwitcher;

            thumbStack.Children.Add(littleButton);
        }
        leftDetails.Children.Add(thumbScroller);

        leftDetails.Children.Add(new TextBlock() { Text = "Play On: " });
        Image launcherImage = new Image();
        launcherImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/SteamHolder.jpg", UriKind.Absolute));
        Button steamButton = new Button()
        {
            MaxHeight = dimensions.height * 0.15,
            Width = launcherImage.Width,
            Content = launcherImage,
            ToolTip = "Steam",
            Tag = game.Id
        };
        steamButton.Click += MainWindow.LauncherButton;
        leftDetails.Children.Add(steamButton);

        return fullDetails;
    }

    public static Image DetailsBigThumbnail(object target)
    {
        return new Image()
        {
             ///MaxHeight = (Convert.ToInt32(target[1]) - 250) * 0.60,
           // Source = new BitmapImage(new Uri(target[0].ToString())),
            Margin = new Thickness(12)
        };
    }
}
