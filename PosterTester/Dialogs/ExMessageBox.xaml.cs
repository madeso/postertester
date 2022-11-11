using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PosterTester.Dialogs;

/// <summary>
/// Hacky custom MessageBox to center it on the parent window
/// </summary>
public partial class ExMessageBox : Window
{
	public ExMessageBox(string message, string title, System.Drawing.Icon img)
	{
		InitializeComponent();
		this.Message = message;
		this.Title = title;
		this.Image = Imaging.CreateBitmapSourceFromHIcon(img.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		this.DataContext = this;
	}

	public string Message { get; }
	public BitmapSource Image { get; }
}
