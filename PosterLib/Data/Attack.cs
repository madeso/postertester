using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PosterLib.Data;

public class AttackOptions : INotifyPropertyChanged
{
	private bool atTheSameTime = false;
	private int count = 10;



	public int Count
	{
		get => count; set
		{
			count = value;
			OnPropertyChanged();
		}
	}

	public bool AtTheSameTime
	{
		get => atTheSameTime; set
		{
			atTheSameTime = value;
			OnPropertyChanged();
		}
	}

	public AttackOptions Clone()
	{
		return new AttackOptions {
			Count = Count,
			AtTheSameTime = AtTheSameTime
		};
	}

	public void GetFrom(AttackOptions other)
	{
		this.Count = other.Count;
		this.AtTheSameTime = other.AtTheSameTime;
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}

