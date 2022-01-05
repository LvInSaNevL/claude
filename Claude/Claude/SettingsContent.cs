using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace Claude
{
    class SettingsContent
    {
        public static StackPanel ClaudeSettings()
        {
            StackPanel stack = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Claude Settings";
            stack.Children.Add(text);

            return stack;
        }
        public static StackPanel SteamSettings()
        {
            StackPanel stack = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "[-- Steam Settings --]";
            stack.Children.Add(text);

            TextBlock exeLocText = new TextBlock();

            List<String> installPaths = Steam.InstallLocs();
            int counter = 0;
            foreach(string path in installPaths)
            {
                counter++;
                TextBlock installText = new TextBlock();
                installText.Text = $"{counter}: {path}";
                stack.Children.Add(installText);
            }

            return stack;
        }
        public static StackPanel OriginSettings()
        {
            StackPanel stack = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Origin Settings";
            stack.Children.Add(text);

            return stack;
        }
        public static StackPanel BattleNetSettings()
        {
            StackPanel stack = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Battle.Net Settings";
            stack.Children.Add(text);

            return stack;
        }
        public static StackPanel OtherGamesSettings()
        {
            StackPanel stack = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Other Settings";
            stack.Children.Add(text);

            return stack;
        }
        public static StackPanel Donate()
        {
            StackPanel stack = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Donate";
            stack.Children.Add(text);

            return stack;
        }
    }
}
