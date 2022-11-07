using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PosterTester;

public static class Extensions
{
	public static ObservableCollection<T> ToObservableCollectionOrEmpty<T>(this IEnumerable<T> col)
	{
		if(col == null)
		{
			return new ObservableCollection<T>();
		}
		else
		{
			return new ObservableCollection<T>(col);
		}
	}
}
