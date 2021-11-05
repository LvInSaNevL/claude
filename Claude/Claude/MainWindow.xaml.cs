using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Claude
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<string> steamGames = Steam.Installed();
            
            foreach (string t in steamGames)
            {
                Button gameButton = new Button { Content = t };
                gameButton.Click += GameButtonClick;
                gameButton.Tag = "steam";
                windowMain.Children.Add(gameButton);
            };
        }

        public static void GameButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C start steam://rungameid/" + button.Content;

            process.StartInfo = startInfo;
            process.Start();
        }
        
    }
}
