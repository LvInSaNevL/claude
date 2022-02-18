using System.Windows;

namespace Claude
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void StartFunc(object sender, StartupEventArgs e) { Computer.Initialize(); }
    }      
}
