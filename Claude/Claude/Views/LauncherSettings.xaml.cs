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
using Microsoft.Win32;

namespace Claude.Views
{
    /// <summary>
    /// Interaction logic for LauncherSettings.xaml
    /// </summary>
    public partial class LauncherSettings : UserControl
    {
        public LauncherSettings()
        {
            InitializeComponent();
        }
        public static void ChangePathClick(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            (TextBlock, string) data = ((TextBlock, string))item.Tag;
            TextBlock text = data.Item1;

            OpenFileDialog openFile = new OpenFileDialog()
            {
                DefaultExt = ".exe",
                InitialDirectory = "C:\\Program Files (x86)"
            };
            openFile.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
            openFile.FilterIndex = 1;
            var result = openFile.ShowDialog();

            if (result == true)
            {
                string filename = openFile.FileName;
                dynamic userData = Computer.ReadUserData();
                Computer.ChangeUserData(data.Item2, filename);
                text.Text = filename;
            }
        }

    }
}
