using System.Windows;
using System.Windows.Controls;

namespace Claude
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class ClaudeSettings : Window
    {
        public ClaudeSettings(string target)
        {
            InitializeComponent();
            Window parentWindow = Window.GetWindow(this);
            SettingsContentLoader(target);
        }

        public void SettingsContentLoader(string target)
        {
            contentField.Children.Clear();

            switch (target)
            {
                case "claude":
                    contentField.Children.Add(SettingsContent.ClaudeSettings());
                    break;
                case "steam":
                    contentField.Children.Add(SettingsContent.SteamSettings());
                    break;
                case "origin":
                    contentField.Children.Add(SettingsContent.OriginSettings());
                    break;
                case "battlenet":
                    contentField.Children.Add(SettingsContent.BattleNetSettings());
                    break;
                case "other":
                    contentField.Children.Add(SettingsContent.OtherGamesSettings());
                    break;
                case "donate":
                    contentField.Children.Add(SettingsContent.Donate());
                    break;
                default:
                    TextBlock errorText = new TextBlock();
                    errorText.Text = $"Page not found: {target}";
                    contentField.Children.Add(errorText);
                    break;
            }
        }

        public void settingsButtonClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            SettingsContentLoader(item.Tag.ToString());
        }
    }
}