using System.Windows;
using System.Windows.Input;

namespace PosterTester;

/// <summary>
/// Interaction logic for CompareRequests.xaml
/// </summary>
public partial class CompareRequests : Window
{
	public CompareRequests(Data.Root root)
    {
        InitializeComponent();
        this.DataContext = root;
    }

    private void CompareExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.DialogResult = true;
        Close();
    }
}