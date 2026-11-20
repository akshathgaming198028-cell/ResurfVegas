using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HoverboardCustomizationData
{
	private struct CustomizationData
	{
		public bool ownsTheme;

		public bool isThemeActive;

		public bool themeHasBeenSeen;

		public override string ToString()
		{
			return ownsTheme.ToString() + ':' + isThemeActive.ToString() + ':' + themeHasBeenSeen.ToString();
		}
	}

	public const int MAX_THEMES_PER_BOARD = 2;

	private const char DATA_CHUNK_SEPARATOR = ';';

	private const char DATA_ASSIGNATION_SEPARATOR = '=';

	private const char DATA_VALUE_GROUP_SEPARATOR = ',';

	private const char DATA_VALUE_SEPARATOR = ':';

	private const char DATA_THEME_IDENTIFIER_SEPARATOR = '#';

	private bool _dirty;

	private Dictionary<Hoverboards.BoardType, Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData>> _customizationData;

	public bool IsDirty
	{
		get
		{
			return _dirty;
		}
		private set
		{
		}
	}

	public HoverboardCustomizationData(string boardCustomizationData)
	{
		LoadCustomizationData(boardCustomizationData);
	}

	private void LoadCustomizationData(string boardCustmoizationData)
	{
		_customizationData = ParseCustomizationData(boardCustmoizationData);
	}

	private Dictionary<Hoverboards.BoardType, Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData>> ParseCustomizationData(string rawData)
	{
		Dictionary<Hoverboards.BoardType, Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData>> dictionary = new Dictionary<Hoverboards.BoardType, Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData>>();
		if (!string.IsNullOrEmpty(rawData))
		{
			string[] array = rawData.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				try
				{
					int num = text.IndexOf('=');
					string value = text.Substring(0, num);
					string rawValues = text.Substring(num + 1);
					dictionary.Add((Hoverboards.BoardType)(int)Enum.Parse(typeof(Hoverboards.BoardType), value, true), GetCustomizationValues(rawValues));
				}
				catch (Exception ex)
				{
					Debug.LogError("HoverboardCustomizationData parse failed for item ERROR = " + ex.ToString());
				}
			}
		}
		return dictionary;
	}

	private Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> GetCustomizationValues(string rawValues)
	{
		Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary = new Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData>();
		if (!string.IsNullOrEmpty(rawValues))
		{
			string[] array = rawValues.Split(',');
			int num = array.Length;
			if (num > 0 && num <= 2)
			{
				for (int i = 0; i < num; i++)
				{
					string[] array2 = array[i].Split('#');
					string value = array2[0];
					string[] array3 = array2[1].Split(':');
					if (array3.Length == 3)
					{
						dictionary.Add(value: new CustomizationData
						{
							ownsTheme = bool.Parse(array3[0]),
							isThemeActive = bool.Parse(array3[1]),
							themeHasBeenSeen = bool.Parse(array3[2])
						}, key: (HoverboardThemes.ThemeIdentifier)(int)Enum.Parse(typeof(HoverboardThemes.ThemeIdentifier), value));
					}
					else
					{
						Debug.LogError("Incorrect number of values for a specific theme.");
					}
				}
				return dictionary;
			}
			Debug.LogError("Incorrect number of themes for a specific board.");
		}
		return null;
	}

	public bool IsThemeOwned(Hoverboards.BoardType boardType, HoverboardThemes.ThemeIdentifier themeId)
	{
		if (Globals.DEBUG_ALL_BOARDS)
		{
			return true;
		}
		if (themeId != HoverboardThemes.ThemeIdentifier.none && _customizationData.ContainsKey(boardType))
		{
			Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary = _customizationData[boardType];
			if (dictionary != null && dictionary.ContainsKey(themeId))
			{
				return dictionary[themeId].ownsTheme;
			}
			return false;
		}
		return false;
	}

	public bool IsThemeActive(Hoverboards.BoardType boardType, HoverboardThemes.ThemeIdentifier themeId)
	{
		if (themeId != HoverboardThemes.ThemeIdentifier.none && _customizationData.ContainsKey(boardType))
		{
			Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary = _customizationData[boardType];
			if (dictionary != null && dictionary.ContainsKey(themeId))
			{
				return dictionary[themeId].isThemeActive;
			}
			return false;
		}
		return false;
	}

	public bool HasThemeBeenSeen(Hoverboards.BoardType boardType, HoverboardThemes.ThemeIdentifier themeId)
	{
		if (themeId != HoverboardThemes.ThemeIdentifier.none && _customizationData.ContainsKey(boardType))
		{
			Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary = _customizationData[boardType];
			if (dictionary != null && dictionary.ContainsKey(themeId))
			{
				return dictionary[themeId].themeHasBeenSeen;
			}
			return false;
		}
		return false;
	}

	public void SetThemeAcquired(Hoverboards.BoardType boardType, HoverboardThemes.ThemeIdentifier themeId)
	{
		if (themeId != HoverboardThemes.ThemeIdentifier.none)
		{
			_dirty = true;
			if (_customizationData.ContainsKey(boardType))
			{
				Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary = _customizationData[boardType];
				if (dictionary != null && dictionary.ContainsKey(themeId))
				{
					CustomizationData customizationData = _customizationData[boardType][themeId];
					CustomizationData value = new CustomizationData
					{
						ownsTheme = true,
						isThemeActive = customizationData.isThemeActive,
						themeHasBeenSeen = customizationData.themeHasBeenSeen
					};
					_customizationData[boardType][themeId] = value;
				}
				else
				{
					CustomizationData value2 = new CustomizationData
					{
						ownsTheme = true,
						themeHasBeenSeen = true,
						isThemeActive = false
					};
					_customizationData[boardType].Add(themeId, value2);
				}
			}
			else
			{
				Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary2 = new Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData>();
				dictionary2.Add(themeId, new CustomizationData
				{
					ownsTheme = true,
					themeHasBeenSeen = true,
					isThemeActive = false
				});
				_customizationData.Add(boardType, dictionary2);
			}
		}
		else
		{
			Debug.LogError("Trying to acquire an invalid theme.");
		}
	}

	public void SetThemeActive(Hoverboards.BoardType boardType, HoverboardThemes.ThemeIdentifier themeId, bool active)
	{
		if (themeId != HoverboardThemes.ThemeIdentifier.none)
		{
			_dirty = true;
			if (_customizationData.ContainsKey(boardType))
			{
				Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary = _customizationData[boardType];
				if (dictionary != null && dictionary.ContainsKey(themeId))
				{
					if (_customizationData[boardType][themeId].ownsTheme || Globals.DEBUG_ALL_BOARDS)
					{
						CustomizationData customizationData = _customizationData[boardType][themeId];
						CustomizationData value = new CustomizationData
						{
							ownsTheme = customizationData.ownsTheme,
							isThemeActive = active,
							themeHasBeenSeen = customizationData.themeHasBeenSeen
						};
						_customizationData[boardType][themeId] = value;
					}
					else
					{
						Debug.LogError("Theme id " + themeId.ToString() + " for board " + boardType.ToString() + " is not owned or data is not available. Activation aborted.");
					}
				}
				else
				{
					Debug.LogError("Theme id " + themeId.ToString() + " for board " + boardType.ToString() + " is not owned or data is not available. Activation aborted.");
				}
			}
			else
			{
				Debug.LogError("Theme id " + themeId.ToString() + " for board " + boardType.ToString() + " is not owned or data is not available. Activation aborted.");
			}
		}
		else
		{
			Debug.LogError("Trying to activate an invalid theme.");
		}
	}

	public void SetThemeSeen(Hoverboards.BoardType boardType, HoverboardThemes.ThemeIdentifier themeId)
	{
		if (themeId != HoverboardThemes.ThemeIdentifier.none)
		{
			_dirty = true;
			if (_customizationData.ContainsKey(boardType))
			{
				Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary = _customizationData[boardType];
				if (dictionary != null && dictionary.ContainsKey(themeId))
				{
					CustomizationData customizationData = _customizationData[boardType][themeId];
					CustomizationData value = new CustomizationData
					{
						ownsTheme = customizationData.ownsTheme,
						isThemeActive = customizationData.isThemeActive,
						themeHasBeenSeen = true
					};
					_customizationData[boardType][themeId] = value;
				}
				else
				{
					CustomizationData value2 = new CustomizationData
					{
						ownsTheme = false,
						themeHasBeenSeen = true,
						isThemeActive = false
					};
					_customizationData[boardType].Add(themeId, value2);
				}
			}
			else
			{
				Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData> dictionary2 = new Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData>();
				dictionary2.Add(themeId, new CustomizationData
				{
					ownsTheme = false,
					themeHasBeenSeen = true,
					isThemeActive = false
				});
				_customizationData.Add(boardType, dictionary2);
			}
		}
		else
		{
			Debug.LogError("Trying to set an invalid theme as seen.");
		}
	}

	public string ToSerializedString()
	{
		string[] array = new string[_customizationData.Count];
		int num = 0;
		foreach (KeyValuePair<Hoverboards.BoardType, Dictionary<HoverboardThemes.ThemeIdentifier, CustomizationData>> customizationDatum in _customizationData)
		{
			string[] array2 = new string[customizationDatum.Value.Count()];
			int num2 = 0;
			foreach (KeyValuePair<HoverboardThemes.ThemeIdentifier, CustomizationData> item in customizationDatum.Value)
			{
				array2[num2] = item.Key.ToString() + '#' + item.Value.ToString();
				num2++;
			}
			string text = string.Join(','.ToString(), array2);
			array[num] = customizationDatum.Key.ToString() + '=' + text;
			num++;
		}
		return string.Join(';'.ToString(), array);
	}

	public int GetNumberOfNewUnseenBoardThemes()
	{
		int num = 0;
		foreach (KeyValuePair<Hoverboards.BoardType, List<HoverboardThemes.HoverboardTheme>> hoverboardCustomTheme in HoverboardThemes.hoverboardCustomThemes)
		{
			foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[hoverboardCustomTheme.Key])
			{
				if (!HasThemeBeenSeen(hoverboardCustomTheme.Key, item.identifier) && item.isNewInThisUpdate)
				{
					num++;
				}
			}
		}
		return num;
	}

	public bool DoesBoardHaveThemes(Hoverboards.BoardType boardType)
	{
		return HoverboardThemes.hoverboardCustomThemes.ContainsKey(boardType);
	}

	public bool AreAllNewThemesSeenForBoard(Hoverboards.BoardType boardType)
	{
		bool flag = true;
		foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[boardType])
		{
			if (item.isNewInThisUpdate)
			{
				flag = flag && HasThemeBeenSeen(boardType, item.identifier);
			}
		}
		return flag;
	}
}
