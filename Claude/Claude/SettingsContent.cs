using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Claude
{
    class SettingsContent
    {
        public static StackPanel Claude()
        {
            StackPanel stack = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Claude Settings";
            stack.Children.Add(text);

            Button wizardButt = new Button() { Content = "Launch Wizard" };
            wizardButt.Click += ClaudeSettings.InstallWizard;
            stack.Children.Add(wizardButt);

            return stack;
        }
        public static StackPanel SteamSettings()
        {
            StackPanel stack = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "[-- Steam Settings --]";
            stack.Children.Add(text);

            StackPanel exeStack = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0, 0, 0, 15) };
            StackPanel gameStack = new StackPanel() { Orientation = Orientation.Vertical };

            TextBlock exeLocText = new TextBlock { Text = Computer.ReadUserData().SelectToken("Steam.exe").ToString() };
            exeStack.Children.Add(exeLocText);
            Button exeLocButton = new Button { 
                Content = "Change",
                Tag = (exeLocText, "steam.exe")
            };
            exeLocButton.Click += ClaudeSettings.ChangePathClick;
            exeStack.Children.Add(exeLocButton);
            stack.Children.Add(exeStack);

            gameStack.Children.Add(new TextBlock() { Text = "Game Directories" });
            List<String> installPaths = Steam.InstallLocs();
            int counter = 0;
            foreach (string path in installPaths)
            {
                counter++;
                gameStack.Children.Add(new TextBlock() { Text = $"{counter}: {path}" } );
            }
            stack.Children.Add(gameStack);

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
            text.Text = "[-- Battle.Net Settings --]";
            stack.Children.Add(text);

            StackPanel exeStack = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0, 0, 0, 15) };

            TextBlock exeLocText = new TextBlock { Text = Computer.ReadUserData().SelectToken("BattleNet.exe").ToString() };
            exeStack.Children.Add(exeLocText);
            Button exeLocButton = new Button
            {
                Content = "Change",
                Tag = (exeLocText, "battlenet.exe")
            };
            exeLocButton.Click += ClaudeSettings.ChangePathClick;
            exeStack.Children.Add(exeLocButton);
            stack.Children.Add(exeStack);

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
