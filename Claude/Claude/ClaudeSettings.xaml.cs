using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
            Keyboard.Focus(this);
            this.Topmost = true;
            Window parentWindow = GetWindow(this);
            SettingsContentLoader(target);
        }

        public void SettingsContentLoader(string target)
        {
            contentField.Children.Clear();

            switch (target)
            {
                //case "claude":
                //    contentField.Children.Add(new Views.LauncherSettings("Claude"));
                //    break;
                case "steam":
                    contentField.Children.Add(new Views.LauncherSettings("Steam"));
                    break;
                //case "origin":
                //    contentField.Children.Add(new Views.LauncherSettings("Origin"));
                //    break;
                case "battlenet":
                    contentField.Children.Add(new Views.LauncherSettings("BattleNet"));
                    break;
                case "other":
                    contentField.Children.Add(new Views.LocalSettings());
                    break;
                //case "donate":
                //    contentField.Children.Add(new Views.LauncherSettings("Donate"));
                //    break;
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