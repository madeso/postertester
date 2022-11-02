using System.Windows;

namespace PosterTester;


/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var data = Data.Disk.LoadOrCreateNew();
        var window = new MainWindow { DataContext = data, Data = data };
        window.Closed += (closedSender, closedArgs) => Data.Disk.Save(data);
        window.OnSave = () => Data.Disk.Save(data);
        window.Show();
    }
}
