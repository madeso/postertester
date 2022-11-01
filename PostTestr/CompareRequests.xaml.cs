using System.Windows;
using System.Windows.Input;

namespace PostTestr;

/// <summary>
/// Interaction logic for CompareRequests.xaml
/// </summary>
public partial class CompareRequests : Window
{
    public CompareRequests(Data.Data data)
    {
        InitializeComponent();
        this.DataContext = data;
    }

    private void CompareExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
