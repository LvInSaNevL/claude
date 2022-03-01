using Claude;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            case "ubisoft":
                return "#202124";
            case "origin":
                return "#e45420";
            case "battlenet":
                return "#0566b0";
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
            Height = 160,
            Width = 345,
            ToolTip = BigBoyLetters(game.Title)
        };
        string color = LauncherColor(game.Launcher);
        gameButton.Background = (Brush)new BrushConverter().ConvertFromString(color);

        Image gameArt = new Image { Source = picture };
        var buttonStyle = new Style
        {
            TargetType = typeof(Border),
            Setters = { new Setter { Property = Border.CornerRadiusProperty, Value = new CornerRadius(15) } }

        };
        gameArt.Resources.Add(buttonStyle.TargetType, buttonStyle);
        gameButton.Resources.Add(buttonStyle.TargetType, buttonStyle);

        gameButton.Content = gameArt;
        return gameButton;
    }

    public struct RGB
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }

    public static Color BoxArtAverage(BitmapImage boxart)
    {
        RGB rgb = new RGB();

        int width = (int)boxart.Width - 120;
        int height = (int)boxart.Height - 120;
        int total = (width * height) / 50;

        for (int x = 0; x < width; x += 50)
        {
            for (int y = 0; y < height; y += 50)
            {
                CroppedBitmap cb = new CroppedBitmap(boxart, new Int32Rect(x, y, 1, 1));
                byte[] result = new byte[4];
                cb.CopyPixels(result, 4, 0);
                rgb.R = (rgb.R + (result[1] * result[1]));
                rgb.G = (rgb.G + (result[2] * result[2]));
                rgb.B = (rgb.B + (result[3] * result[3]));
            }
        }

        return Color.FromRgb(
            Convert.ToByte(Math.Sqrt(rgb.R / total)),
            Convert.ToByte(Math.Sqrt(rgb.G / total)),
            Convert.ToByte(Math.Sqrt(rgb.B / total)));
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

    public static Button SteamButton(string id)
    {
        Image launcherImage = new Image();
        launcherImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Claude;component/Resources/SteamHolder.jpg", UriKind.Absolute));
        Button steamButton = new Button()
        {
            Width = launcherImage.Width,
            Content = launcherImage,
            ToolTip = "Steam",
            Tag = id
        };
        steamButton.Click += MainWindow.LauncherButton;

        return steamButton;
    }
    public static Button BattleNetButton(string id)
    {
        Image launcherImage = new Image();
        launcherImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Claude;component/Resources/BattleNetHolder.jpg", UriKind.Absolute));
        Button steamButton = new Button()
        {
            Width = launcherImage.Width,
            Content = launcherImage,
            ToolTip = "Battle.Net",
            Tag = id
        };
        steamButton.Click += MainWindow.LauncherButton;

        return steamButton;
    }
}