using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PosterTester.Extensions;

public static class EnumerableExtensions
{
	public static ObservableCollection<T> ToObservableCollectionOrEmpty<T>(this IEnumerable<T> col)
	{
		if (col == null)
		{
			return new ObservableCollection<T>();
		}
		else
		{
			return new ObservableCollection<T>(col);
		}
	}
}
