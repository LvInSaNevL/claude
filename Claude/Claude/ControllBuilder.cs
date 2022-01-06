using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

    public static Button BoxArtButton(string mouseover, string tag, BitmapImage picture, string launcher)
	{
        Button gameButton = new Button
        {
            Content = tag,
            Tag = tag,
            Background = (SolidColorBrush)new BrushConverter().ConvertFrom(LauncherColor(launcher)),
            Height = 160,
            Width = 345,
            ToolTip = BigBoyLetters(mouseover)
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
    
}
