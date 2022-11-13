using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PosterTester.Extensions;

public static class EnumerableExtensions
{
	public static ObservableCollection<T> ToObservableCollectionOrEmpty<T>(this IEnumerable<T>? col)
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

	// https://stackoverflow.com/a/59434717
	public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : class
	{
		return enumerable.Where(e => e != null).Select(e => e!);
	}
}
