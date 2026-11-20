using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Extensions
{
	public static class CollectionExtensions
	{
		public static void AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
		{
			if (list.Count == 0)
			{
				list.Add(item);
				return;
			}
			if (list[list.Count - 1].CompareTo(item) <= 0)
			{
				list.Add(item);
				return;
			}
			if (list[0].CompareTo(item) >= 0)
			{
				list.Insert(0, item);
				return;
			}
			int num = list.BinarySearch(item);
			if (num < 0)
			{
				num = ~num;
			}
			list.Insert(num, item);
		}

		public static T GetRandomElement<T>(this IEnumerable<T> sequence)
		{
			T[] array = (sequence as T[]) ?? sequence.ToArray();
			return array.ElementAt(KRandom.Range(0, array.Length));
		}

		public static T GetRandomElement<T>(this IList<T> list)
		{
			return list[KRandom.Range(0, list.Count)];
		}

		public static void Shuffle<T>(this IList<T> array)
		{
			for (int num = array.Count; num > 1; num--)
			{
				int index = KRandom.Range(0, num);
				T value = array[index];
				array[index] = array[num - 1];
				array[num - 1] = value;
			}
		}

		public static void SwitchPlaces<T>(this IList<T> arr, int i, int j)
		{
			if (i < arr.Count && j < arr.Count)
			{
				T value = arr[i];
				arr[i] = arr[j];
				arr[j] = value;
			}
		}

		public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select((T item) => (T)item.Clone()).ToList();
		}

		public static Dictionary<T, U> ToDictionary<T, U>(this List<KeyValuePair<T, U>> keyvalueList)
		{
			Dictionary<T, U> dictionary = new Dictionary<T, U>(keyvalueList.Count);
			foreach (KeyValuePair<T, U> keyvalue in keyvalueList)
			{
				dictionary.Add(keyvalue.Key, keyvalue.Value);
			}
			return dictionary;
		}

		public static List<KeyValuePair<T, U>> ToKeyValueList<T, U>(this Dictionary<T, U> dictionary)
		{
			List<KeyValuePair<T, U>> list = new List<KeyValuePair<T, U>>(dictionary.Count);
			foreach (KeyValuePair<T, U> item in dictionary)
			{
				list.Add(item);
			}
			return list;
		}

		public static bool ValidIndex<T>(this IList<T> l, int index)
		{
			return index >= 0 && index < l.Count;
		}
	}
}
