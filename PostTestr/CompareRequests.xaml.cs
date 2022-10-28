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
using System.Windows.Shapes;

namespace PostTestr;

/// <summary>
/// Interaction logic for CompareRequests.xaml
/// </summary>
public partial class CompareRequests : Window
{
    public CompareRequests(Data data)
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
