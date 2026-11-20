using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Kiloo.HTTP
{
	public class Headers : IEnumerable, IEnumerable<KeyValuePair<string, string>>
	{
		private readonly Dictionary<string, List<string>> _headers = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

		private string _contentType;

		private Dictionary<string, string> _parsedContentType;

		private static readonly string[] NICE_NAME_EXCEPTIONS = new string[5] { "ETag", "P3P", "Content-MD5", "WWW-Authenticate", "TE" };

		public string this[string name]
		{
			get
			{
				return Get(name, string.Empty);
			}
			set
			{
				name = NicifyName(name);
				_headers[name] = new List<string> { value };
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<string, List<string>> header in _headers)
				{
					num += header.Value.Count;
				}
				return num;
			}
		}

		public IDictionary<string, string> contentType
		{
			get
			{
				string text = this["Content-Type"];
				if (_contentType != text)
				{
					string[] array = text.Split(';');
					Dictionary<string, string> dictionary = new Dictionary<string, string>(array.Length, StringComparer.InvariantCultureIgnoreCase);
					dictionary["Content-Type"] = array[0].Trim();
					for (int i = 1; i < array.Length; i++)
					{
						int num = array[i].IndexOf('=');
						if (num >= 0)
						{
							dictionary[array[i].Substring(0, num).Trim()] = array[i].Substring(num + 1).Trim();
						}
						else
						{
							dictionary[array[i].Trim()] = string.Empty;
						}
					}
					_contentType = text;
					_parsedContentType = dictionary;
				}
				return _parsedContentType;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(string name)
		{
			return _headers.ContainsKey(name);
		}

		public void Clear()
		{
			_headers.Clear();
		}

		public void Remove(string name)
		{
			_headers.Remove(name);
		}

		public Headers Add(string name, object value)
		{
			List<string> value2;
			if (_headers.TryGetValue(name, out value2))
			{
				value2.Add(ConvertValue(value));
			}
			else
			{
				name = NicifyName(name);
				_headers[name] = new List<string> { ConvertValue(value) };
			}
			return this;
		}

		public Headers AddAll(IEnumerable<KeyValuePair<string, string>> headers)
		{
			foreach (KeyValuePair<string, string> header in headers)
			{
				Add(header.Key, header.Value);
			}
			return this;
		}

		public string Get(string name)
		{
			return Get(name, string.Empty);
		}

		public string Get(string name, string defaultValue)
		{
			List<string> value;
			if (_headers.TryGetValue(name, out value))
			{
				return value[value.Count - 1];
			}
			return defaultValue;
		}

		public DateTime Get(string name, DateTime defaultValue, bool allowDeltaTimeSeconds = false)
		{
			string text = Get(name, null);
			if (text != null)
			{
				DateTime result;
				if (DateTime.TryParseExact(text, DateTimeFormatInfo.InvariantInfo.RFC1123Pattern, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out result))
				{
					return result;
				}
				long result2;
				if (allowDeltaTimeSeconds && long.TryParse(text, out result2) && result2 >= 0)
				{
					return DateTime.UtcNow.AddSeconds(result2);
				}
			}
			return defaultValue;
		}

		public long Get(string name, long defaultValue)
		{
			string text = Get(name, null);
			long result;
			if (text != null && long.TryParse(text, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public IEnumerable<string> GetAll(string name)
		{
			List<string> value;
			if (_headers.TryGetValue(name, out value))
			{
				return _headers[name];
			}
			return new string[0];
		}

		public Headers Set(string name, object value)
		{
			name = NicifyName(name);
			this[name] = ConvertValue(value);
			return this;
		}

		public Headers SetAll(IEnumerable<KeyValuePair<string, string>> headers)
		{
			foreach (KeyValuePair<string, string> header in headers)
			{
				Set(header.Key, header.Value);
			}
			return this;
		}

		public Headers SetAll(string name, IEnumerable<string> values)
		{
			name = NicifyName(name);
			List<string> list = new List<string>();
			foreach (string value in values)
			{
				list.Add(value);
			}
			_headers[name] = list;
			return this;
		}

		public void ReadFrom(BinaryReader reader)
		{
			_headers.Clear();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadString();
				int num2 = reader.ReadInt32();
				for (int j = 0; j < num2; j++)
				{
					Add(name, reader.ReadString());
				}
			}
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(_headers.Count);
			foreach (KeyValuePair<string, List<string>> header in _headers)
			{
				writer.Write(header.Key);
				List<string> value = header.Value;
				writer.Write(value.Count);
				foreach (string item in value)
				{
					writer.Write(item);
				}
			}
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			foreach (KeyValuePair<string, List<string>> header in _headers)
			{
				using (List<string>.Enumerator enumerator2 = header.Value.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						yield return new KeyValuePair<string, string>(value: enumerator2.Current, key: header.Key);
					}
				}
			}
		}

		private static bool IsNameNice(string name)
		{
			int length = name.Length;
			for (int i = 0; i < length; i++)
			{
				if (i == 0 || name[i - 1] == '-')
				{
					if (char.ToUpperInvariant(name[i]) != name[i])
					{
						return false;
					}
				}
				else if (char.ToLowerInvariant(name[i]) != name[i])
				{
					return false;
				}
			}
			return true;
		}

		private static string NicifyName(string name)
		{
			for (int i = 0; i < NICE_NAME_EXCEPTIONS.Length; i++)
			{
				if (NICE_NAME_EXCEPTIONS[i].Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return NICE_NAME_EXCEPTIONS[i];
				}
			}
			if (IsNameNice(name))
			{
				return name;
			}
			int length = name.Length;
			char[] array = new char[length];
			for (int j = 0; j < length; j++)
			{
				bool flag = j == 0 || name[j - 1] == '-';
				array[j] = ((!flag) ? char.ToLowerInvariant(name[j]) : char.ToUpperInvariant(name[j]));
			}
			return new string(array);
		}

		private string ConvertValue(object value)
		{
			if (value is DateTime)
			{
				return ((DateTime)value).ToUniversalTime().ToString(DateTimeFormatInfo.InvariantInfo.RFC1123Pattern);
			}
			return value.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (IEnumerator<KeyValuePair<string, string>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.Current;
					stringBuilder.Append(current.Key).Append(": ").Append(current.Value)
						.Append('\n');
				}
			}
			return stringBuilder.ToString();
		}
	}
}
