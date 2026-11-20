#define ENABLE_ERROR_LOGS
using System;
using System.Diagnostics;
using Kiloo.Common;
using UnityEngine;

public class Strings
{
	private static string language;

	private static string[] values;

	public static string LOCALE_FILES_FOLDER = "text";

	public static string LOCALE_FILES_MANDATORY_ENDING = "_locale";

	public static string Language
	{
		get
		{
			return language;
		}
		set
		{
			Load(value);
		}
	}

	public static string Get(StringID key)
	{
		if (language == null)
		{
			LogWarning("Strings not loaded. Loading default language. Tried to get string: " + key);
			Language = "english";
		}
		return values[(int)key];
	}

	public static string Get(string keyString)
	{
		return (!string.IsNullOrEmpty(keyString)) ? Get((StringID)(int)Enum.Parse(typeof(StringID), keyString, true)) : null;
	}

	public static bool Exists(string keyString)
	{
		try
		{
			Get(keyString);
		}
		catch (ArgumentException)
		{
			return false;
		}
		return true;
	}

	private static void Load(string language)
	{
		if (!(Strings.language != language))
		{
			return;
		}
		Strings.language = language;
		if (values == null)
		{
			int[] array = (int[])Enum.GetValues(typeof(StringID));
			values = new string[array[array.Length - 1] + 1];
		}
		else
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = null;
			}
		}
		string path = LOCALE_FILES_FOLDER + "/" + language + LOCALE_FILES_MANDATORY_ENDING;
		TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
		string text = textAsset.text;
		int num = 0;
		string key;
		string value;
		while ((num = StringUtility.GetNextKeyValuePair(text, num, out key, out value)) >= 0)
		{
			int num2 = (int)Enum.Parse(typeof(StringID), key, true);
			values[num2] = value;
			if (num == text.Length)
			{
				break;
			}
		}
		if (values[0] != null)
		{
			throw new Exception("Strings.Load: String set for " + Enum.GetName(typeof(StringID), 0));
		}
		for (int j = 1; j < values.Length; j++)
		{
			if (values[j] == null && Enum.IsDefined(typeof(StringID), j))
			{
				throw new Exception("Strings.Load: String not set for " + Enum.GetName(typeof(StringID), j));
			}
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_STRINGS_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
