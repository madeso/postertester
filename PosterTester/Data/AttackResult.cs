using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PosterTester.Data;

public class AttackResult : INotifyPropertyChanged
{
	private ObservableCollection<string> error = new();
	private ObservableCollection<TimeSpan> result = new();

	public ObservableCollection<string> Errors
	{
		get => error; set
		{
			error = value;
			OnPropertyChanged();
		}
	}

	public ObservableCollection<TimeSpan> Result
	{
		get => result; set
		{
			result = value;
			OnPropertyChanged();
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
