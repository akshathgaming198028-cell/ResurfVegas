using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Plugins.DeviceUtility;
using TopRun;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CountryManager
{
	private enum LocaleComponents
	{
		language = 0,
		countryCode = 1
	}

	private static CountryManager _instance;

	public static CountryManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CountryManager();
			}
			return _instance;
		}
	}

	private CountryManager()
	{
	}

	public string GetDefaultCountryCodeFromDevice()
	{
		if (Application.isEditor)
		{
			return "DK";
		}
		Country country = null;
		string contryCodeFromLocale = GetContryCodeFromLocale(DeviceUtility.GetLocale());
		country = CountryData.GetCountryForID(contryCodeFromLocale);
		if (country == null)
		{
			country = CountryData.GetMotherCountryForCountryID(contryCodeFromLocale);
		}
		if (country == null)
		{
			return "US";
		}
		return country.countryCode.ToUpper();
	}

	public List<Country> getListOfCountries()
	{
		return CountryData.getListOfCountries();
	}

	public int getCurrentCountryIndex()
	{
		string empty = string.Empty;
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			empty = ((manager.tournament == null) ? manager.country.ToLower() : manager.tournament.country.ToLower());
			for (int i = 0; i < CountryData.getListOfCountries().Count; i++)
			{
				if (CountryData.getListOfCountries()[i].countryCode == empty)
				{
					return i;
				}
			}
		}
		return 0;
	}

	private static string GetContryCodeFromLocale(string deviceLocale)
	{
		string result = string.Empty;
		if (!string.IsNullOrEmpty(deviceLocale))
		{
			deviceLocale = deviceLocale.Trim().ToLower();
			deviceLocale = deviceLocale.Replace('-', '_');
			if (deviceLocale.Contains("_"))
			{
				string[] array = deviceLocale.Split('_');
				if (array.Length == 2)
				{
					result = array[1];
				}
			}
		}
		return result;
	}

	public Country GetCurrentTournamentCountry()
	{
		Country country = null;
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			if (manager.tournament != null)
			{
				string country2 = manager.tournament.country;
				country = CountryData.GetCountryForID(country2);
			}
			else
			{
				string country3 = manager.country;
				country = CountryData.GetCountryForID(country3);
			}
		}
		if (country == null)
		{
			country = new Country(string.Empty, string.Empty, string.Empty);
		}
		return country;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, Object context = null)
	{
		Debug.LogError("[CountryManager] " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning("[CountryManager] " + msg, context);
	}

	[Conditional("ENABLE_COUNTRYMANAGER_LOGS")]
	public static void Log(string msg, Object context = null)
	{
		Debug.Log("[CountryManager] " + msg, context);
	}
}
