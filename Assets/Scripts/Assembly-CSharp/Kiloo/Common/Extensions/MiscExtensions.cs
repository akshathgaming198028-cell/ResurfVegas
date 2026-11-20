using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class MiscExtensions
	{
		public static bool TryParseHexToColor(this string hexString, out Color color)
		{
			try
			{
				hexString = hexString.Trim('#');
				byte r = byte.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
				byte g = byte.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
				byte b = byte.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
				color = new Color32(r, g, b, byte.MaxValue);
				return true;
			}
			catch (Exception)
			{
				color = Color.white;
				return false;
			}
		}

		public static string ToHex(this Color color)
		{
			return ((Color32)color).ToHex();
		}

		public static string ToHex(this Color32 color)
		{
			return new StringBuilder("#").Append(string.Format("{0:X2}", color.r)).Append(string.Format("{0:X2}", color.g)).Append(string.Format("{0:X2}", color.b))
				.ToString();
		}

		public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
		{
			if (strEnumValue == null || !Enum.IsDefined(typeof(TEnum), strEnumValue))
			{
				return defaultValue;
			}
			return (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
		}

		public static T SafeInvoke<T>(this Func<T> f)
		{
			return (f == null) ? default(T) : f();
		}

		public static void SafeInvoke(this Action action)
		{
			if (action != null)
			{
				action();
			}
		}

		public static void SafeInvoke<T>(this Action<T> action, T argument)
		{
			if (action != null)
			{
				action(argument);
			}
		}

		public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 argument1, T2 argument2)
		{
			if (action != null)
			{
				action(argument1, argument2);
			}
		}

		public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 argument1, T2 argument2, T3 argument3)
		{
			if (action != null)
			{
				action(argument1, argument2, argument3);
			}
		}

		public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 argument1, T2 argument2, T3 argument3, T4 argument4)
		{
			if (action != null)
			{
				action(argument1, argument2, argument3, argument4);
			}
		}
	}
}
