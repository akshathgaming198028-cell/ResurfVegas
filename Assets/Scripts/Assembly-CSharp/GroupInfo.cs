#define ENABLE_ERROR_LOGS
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class GroupInfo
{
	private const string INTERSTITIAL_GROUPS_ONLINESETTINGS_KEY = "interstitial_groups";

	private const string INTERSTITIAL_GROUPS_COUNTRY_CODES_ENDING = "country_codes";

	private const string INTERSTITIAL_GROUPS_PRIORITY_ENDING = "priority";

	private const string INTERSTITIAL_GROUPS_WEIGHT_ENDING = "weight";

	private const char SEPARATOR = ';';

	private static List<Group> _currentGroups;

	private static List<Group> _defaultGroups;

	static GroupInfo()
	{
		_currentGroups = new List<Group>();
		_defaultGroups = new List<Group>
		{
			new Group
			{
				id = "default",
				countryCodes = new List<string>(),
				providerSettings = new List<ProviderSettings>
				{
					new ProviderSettings("inmobi", 1, 100),
					new ProviderSettings("chartboost", 2, 100),
					new ProviderSettings("mdotm", 3, 0)
				}
			}
		};
	}

	public static List<ProviderSettings> GetProvidersForLocale(string deviceLocale, bool fromOnlineSettings)
	{
		List<ProviderSettings> list = new List<ProviderSettings>();
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS)
		{
			foreach (Group defaultGroup in _defaultGroups)
			{
				if (!(defaultGroup.id == "default"))
				{
					continue;
				}
				foreach (ProviderSettings providerSetting in defaultGroup.providerSettings)
				{
					if (InterstitialProviderIDs.GetDebugProviderIDs().Contains(providerSetting.id))
					{
						list.Add(new ProviderSettings(providerSetting.id, providerSetting.priority, 100));
					}
				}
				break;
			}
			return list;
		}
		string contryCodeFromLocale = Utils.GetContryCodeFromLocale(deviceLocale);
		if (fromOnlineSettings || _currentGroups.Count == 0)
		{
			_currentGroups.Clear();
			_currentGroups = GetCurrentGroups();
		}
		if (!string.IsNullOrEmpty(contryCodeFromLocale))
		{
			foreach (Group currentGroup in _currentGroups)
			{
				if (currentGroup.countryCodes.Contains(contryCodeFromLocale))
				{
					list = currentGroup.providerSettings;
					break;
				}
			}
		}
		else
		{
			LogError("The device country code was null or empty");
		}
		if (list.Count == 0)
		{
			foreach (Group currentGroup2 in _currentGroups)
			{
				if (currentGroup2.id == "default")
				{
					list = currentGroup2.providerSettings;
					break;
				}
			}
		}
		return list;
	}

	private static List<Group> GetCurrentGroups()
	{
		List<Group> list = new List<Group>();
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("interstitial_groups", out valueString))
		{
			string[] array = valueString.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				List<string> list2 = new List<string>();
				List<ProviderSettings> list3 = new List<ProviderSettings>();
				Group? obj = null;
				string text2 = text.Trim();
				foreach (Group defaultGroup in _defaultGroups)
				{
					if (defaultGroup.id == text2)
					{
						obj = defaultGroup;
						break;
					}
				}
				string key = string.Format("{0}_{1}_{2}", "interstitial_groups", text2, "country_codes");
				List<string> list4 = new List<string>();
				string valueString2 = string.Empty;
				if (OnlineSettings.instance.TryGetValue(key, out valueString2))
				{
					string[] array3 = valueString2.Split(';');
					string[] array4 = array3;
					foreach (string item in array4)
					{
						list4.Add(item);
					}
				}
				else if (obj.HasValue)
				{
					list4.AddRange(obj.Value.countryCodes);
				}
				foreach (string item5 in list4)
				{
					string item2 = item5.Trim().ToLower();
					list2.Add(item2);
				}
				foreach (string allValidProviderID in InterstitialProviderIDs.allValidProviderIDs)
				{
					string key2 = string.Format("{0}_{1}_{2}_{3}", "interstitial_groups", text2, allValidProviderID, "priority");
					string key3 = string.Format("{0}_{1}_{2}_{3}", "interstitial_groups", text2, allValidProviderID, "weight");
					int defaultValue = int.MaxValue;
					int defaultValue2 = 0;
					if (obj.HasValue)
					{
						foreach (ProviderSettings providerSetting in obj.Value.providerSettings)
						{
							if (providerSetting.id == allValidProviderID)
							{
								defaultValue = providerSetting.priority;
								defaultValue2 = providerSetting.weight;
								break;
							}
						}
					}
					defaultValue = OnlineSettings.instance.GetValue(key2, defaultValue);
					defaultValue2 = OnlineSettings.instance.GetValue(key3, defaultValue2);
					ProviderSettings item3 = new ProviderSettings(allValidProviderID, defaultValue, defaultValue2);
					list3.Add(item3);
				}
				Group item4 = new Group
				{
					id = text2,
					countryCodes = list2,
					providerSettings = list3
				};
				list.Add(item4);
			}
		}
		else
		{
			list.AddRange(_defaultGroups);
		}
		return list;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_GROUPINFO_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
