using System.Windows;

namespace PosterTester;


/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
		var window = new MainWindow();
        window.Show();
    }
}
