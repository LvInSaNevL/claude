using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace Claude
{
    /// <summary>
    /// Interaction logic for Installer.xaml
    /// </summary>
    public partial class Installer : Window
    {
        private static DataTypes.UserData jsondata;
        public static List<DataTypes.Game> installerGames = new List<DataTypes.Game>();

        public Installer()
        {
            InitializeComponent();
            Welcome();                       
        }

        private void Welcome(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();
            // Welcome Text
            ContentField.Children.Add(new TextBlock() { Text = "Welcome to the Claude installer!" });
            // Info Text
            ContentField.Children.Add(new TextBlock() { Text = "We have just a bit of setup to do first though." });

            NextButton.Click += Games;
            PreviousButton.Visibility = Visibility.Collapsed;
            PreviousButton.Click += DoNothing;
        }

        private void Games(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();
            PreviousButton.Visibility = Visibility.Visible;

            // Welcome Text
            ContentField.Children.Add(new TextBlock() { Text = "Here you can add your launchers" });
            // Info Text
            ContentField.Children.Add(new TextBlock() { Text = "Just click below" });

            String[] launcherCodes = new string[]
            {
                "Steam",
                "BattleNet",
                "Origin",
                "Ubisoft"
            };

            foreach (string launcherCode in launcherCodes)
            {
                Expander dropStack = new Expander()
                {
                    Name = launcherCode,
                    ExpandDirection = ExpandDirection.Down,
                    Header = launcherCode
                };

                dropStack.Content = new Views.LauncherSettings(launcherCode);
                ContentField.Children.Add(dropStack);
            }                      

            NextButton.Click += OtherPage;
            PreviousButton.Click += Welcome;
        }

        private void OtherPage(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();

            Views.LocalSettings localSettings = new Views.LocalSettings();
            ContentField.Children.Add(localSettings);

            NextButton.Click += Finally;
            PreviousButton.Click += Games;
        }

        private void Finally(object sender = null, RoutedEventArgs e = null)
        {
            ContentField.Children.Clear();

            TextBlock welcomeText = new TextBlock() { Text = "You have got Claude all set up!" };
            TextBlock infoText = new TextBlock() { Text = "Hit the next button to start playing!" };

            ContentField.Children.Add(welcomeText);
            ContentField.Children.Add(infoText);

            NextButton.Content = "Start Playing";
            NextButton.Click += EndSetup;
            PreviousButton.Click += OtherPage;
        }

        private void EndSetup(object sender = null, RoutedEventArgs e = null)
        {
            // Finally write the data to the file
            Uri maybePath = new Uri("pack://application:,,,/Resources/UserData.json");
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            fullPath = $"{fullPath}\\{maybePath.LocalPath}";

            using (StreamWriter writer = File.CreateText(fullPath))
            {
                writer.WriteLine(jsondata.ToString());
            }

            MainWindow window = new MainWindow();
            window.Show();

            this.Close();
        } 

        private void DoNothing(object sender, RoutedEventArgs e) { }
    }
}
