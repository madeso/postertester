using System;
using System.Windows;
using System.Windows.Media.Effects;

namespace PosterTester;

public class DialogBackground : IDisposable
{
	private Window Parent { get; }
	public DialogBackground(Window parent)
	{
		this.Parent = parent;

		parent.Opacity = 0.5;
		parent.Effect = new BlurEffect();
	}

	public void Dispose()
	{
		this.Parent.Opacity = 1;
		this.Parent.Effect = null;
	}
}


public class DialogBackgroundWithDialog : DialogBackground
{
    public DialogBackgroundWithDialog(Window parent, Window dialog)
		: base(parent)
    {
        dialog.Owner = parent;
        dialog.ShowInTaskbar = false;
        dialog.Topmost = true;
    }
}
