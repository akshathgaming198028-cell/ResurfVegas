using System.Globalization;
using System.Text;

namespace Kiloo.Common.Extensions
{
	public static class StringExtensions
	{
		public static string FilterWhiteSpaces(this string input)
		{
			if (input == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(input.Length);
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (i == 0 || c != ' ' || (c == ' ' && input[i - 1] != ' '))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Trim();
		}

		public static string Populate(this string str, params object[] args)
		{
			return string.Format(str, args);
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static string ToTitleCase(this string str)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
		}
	}
}
