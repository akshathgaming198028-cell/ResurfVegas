#define ENABLE_ERROR_LOGS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class ScreenAdsManager
{
	private enum ChangeProviderListLevel
	{
		dontRepopulate = 0,
		repopulate_UsingCachedVersion_IfEmpty = 1,
		repopulate_UsingCachedVersion_Forced = 2,
		repopulate_UsingOnlineSettings_Forced = 3
	}

	private const string INTERSTITIAL_LOCATION_PREFIX = "interstitial_location";

	private const string INTERSTITIAL_LOCATION_ENABLED_ENDING = "enabled";

	private const string INTERSTITIAL_LOCATION_TIME_LIMIT_ENDING = "time_limit";

	private const string ALLOWNEXT_TICKS_KEY = "cb_alwnxt_ticks";

	private const string HAS_SEEN_FIRST_INTERSTITIAL_KEY = "seen_first_interstitial";

	private const uint FIRSTTIME_DELAY_SECONDS = 72000u;

	private const uint DEFAULT_DELAY_SECONDS = 10u;

	private const string DELAY_SECONDS_ONLINESETTINGSKEY = "interstitials_delay_seconds";

	private const uint DEFAULT_NUMBER_OF_FAILS_FOR_EACH_PROVIDER = 1u;

	private const string NUMBER_OF_FAILS_FOR_EACH_PROVIDR = "number_of_fails_for_interstitial";

	private const string LIMIT_PER_HOUR = "interstitials_limit_per_hour";

	private const uint DEFAULT_LIMIT_PER_HOUR = 5u;

	private const string LIMIT_PER_DAY = "interstitials_limit_per_day";

	private const uint DEFAULT_LIMIT_PER_DAY = 20u;

	private const uint DEFAULT_TIME_LIMIT_BEFORE_CHANGE_PROVIDER = 30u;

	private const string PROVIDERS_LIST_VERSION = "interstitial_providers_list_version";

	public const int DEFAULT_PROVIDERS_LIST_VERSION = 0;

	private const string PROVIDER_LOCK_ON_TIME_LIMIT = "interstitial_provider_lock_on_time_limit";

	private const uint DEFAULT_PROVIDER_LOCK_ON_TIME_LIMIT = 72000u;

	private const string COUNTRY_CODES_FOR_FLURRY_LOGGING_KEY = "interstitial_flurry_logging_country_codes";

	private const char SEPARATOR = ';';

	private static ScreenAdsManager _instance;

	private ScreenAdProvider _currentProvider;

	private ScreenAdProvider _lastShownProvider;

	private List<ProviderSettings> _screenAdProviderSettingsList = new List<ProviderSettings>();

	private int _currentNumberOfFails;

	private bool _isCaching;

	private DateTime expireTime;

	private bool _allowedToCache;

	private Location _currentShowLocation = InterstitialLocations.GetLocation(LocationID.DisableShow);

	private bool _isPresentingInterstitial;

	public bool isPresentingInterstitial
	{
		get
		{
			return _isPresentingInterstitial;
		}
	}

	public static ScreenAdsManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ScreenAdsManager();
			}
			return _instance;
		}
	}

	private bool HasCachedInterstial
	{
		get
		{
			if (_currentProvider != null)
			{
				return _currentProvider.HasCachedInterstials;
			}
			return false;
		}
	}

	private bool interstitialsEnabled
	{
		get
		{
			if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IN_APP_PURCHASES)
			{
				return true;
			}
			if (Application.systemLanguage == SystemLanguage.Chinese)
			{
				return true;
			}
			bool flag = Screen.height >= 2500;
			return PlayerInfo.Instance.inAppConsumablePurchaseCount <= 0 && PlayerInfo.Instance.inAppNonconsumablePurchaseCount <= 0 && PlayerInfo.Instance.inAppRestoredPurchaseCount <= 0 && PlayerInfo.Instance.inAppLegacyPurchaseCount <= 0 && !flag;
		}
	}

	public string GetCurrentProvider(out bool hasCached, out bool isCaching)
	{
		hasCached = false;
		isCaching = false;
		if (_currentProvider != null && _currentProvider.isInitialized)
		{
			hasCached = _currentProvider.HasCachedInterstials;
			isCaching = _isCaching;
			return _currentProvider.id;
		}
		return null;
	}

	public string GetLastShownProvider()
	{
		if (_lastShownProvider != null)
		{
			return _lastShownProvider.id;
		}
		return null;
	}

	public void InitProviderListAndCache()
	{
		if (interstitialsEnabled)
		{
			_allowedToCache = true;
			ChooseProviderAndTryCache(true, ChangeProviderListLevel.repopulate_UsingCachedVersion_IfEmpty);
		}
	}

	public void SetAllowCache(bool allow)
	{
		_allowedToCache = allow;
		if (_allowedToCache && interstitialsEnabled)
		{
			ChooseProviderAndTryCache(false, ChangeProviderListLevel.repopulate_UsingCachedVersion_IfEmpty);
		}
	}

	public void EnableShowLocation(Location location, bool enable)
	{
		if (enable)
		{
			if (GetLocationEnabledState(location))
			{
				_currentShowLocation = location;
				if (interstitialsEnabled)
				{
					ChooseProviderAndTryCache(false, ChangeProviderListLevel.repopulate_UsingCachedVersion_IfEmpty);
					ShowInterstitialIfAble();
				}
			}
		}
		else if (_currentShowLocation.id == location.id)
		{
			_currentShowLocation = InterstitialLocations.GetLocation(LocationID.DisableShow);
		}
	}

	private void ChooseProviderAndTryCache(bool forceChangeProvider, ChangeProviderListLevel level)
	{
		if (HasCachedInterstial)
		{
			return;
		}
		ChangeProviderListLevel changeProviderListLevel = level;
		if (OnlineSettingsProvidersVersionChanged())
		{
			changeProviderListLevel = ChangeProviderListLevel.repopulate_UsingOnlineSettings_Forced;
		}
		else if (HasLockOnTimeLimitPassed())
		{
			changeProviderListLevel = ChangeProviderListLevel.repopulate_UsingCachedVersion_Forced;
		}
		bool flag = changeProviderListLevel == ChangeProviderListLevel.repopulate_UsingOnlineSettings_Forced || changeProviderListLevel == ChangeProviderListLevel.repopulate_UsingCachedVersion_Forced || forceChangeProvider || _currentProvider == null;
		if (flag || (_isCaching && DidExceedTimeLimitSinceLastCache()))
		{
			if (_currentProvider != null && !flag)
			{
				LogFlurryEventForProvider("Interstitial Providers Request", _currentProvider.id, "Request TimeOut");
				_isCaching = false;
			}
			ChangeProvider(changeProviderListLevel);
			CacheNowInterstitial();
		}
		else if (!_isCaching)
		{
			CacheNowInterstitial();
		}
	}

	private void ShowInterstitialIfAble()
	{
		if (HasCachedInterstial && interstitialsEnabled && !VideoAdsManager.instance.isVideoPlaying && _currentShowLocation.id != LocationID.DisableShow)
		{
			DateTime now = DateTime.Now;
			if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT || (now >= NextAllowedShowTime() && DidNotExceedNumberOfInterstitials()))
			{
				ShowInterstitial();
			}
		}
	}

	private void CacheNowInterstitial()
	{
		if (_allowedToCache && _currentProvider != null && !_isCaching)
		{
			_currentProvider.CacheNowInterstitials();
			expireTime = DateTime.UtcNow.AddSeconds(30.0);
			_isCaching = true;
		}
	}

	private void GetBestProvider()
	{
		ProviderSettings providerSettings = null;
		if (_screenAdProviderSettingsList != null && _screenAdProviderSettingsList.Count > 0)
		{
			providerSettings = _screenAdProviderSettingsList[0];
			List<ProviderSettings> list = new List<ProviderSettings>();
			list.Add(providerSettings);
			for (int i = 1; i < _screenAdProviderSettingsList.Count; i++)
			{
				if (providerSettings.priority > _screenAdProviderSettingsList[i].priority)
				{
					providerSettings = _screenAdProviderSettingsList[i];
					list.Clear();
					list.Add(providerSettings);
				}
				else if (providerSettings.priority == _screenAdProviderSettingsList[i].priority)
				{
					list.Add(_screenAdProviderSettingsList[i]);
				}
			}
			if (list.Count > 1)
			{
				int num = 0;
				for (int j = 0; j < list.Count; j++)
				{
					num += list[j].weight;
				}
				System.Random random = new System.Random();
				int num2 = random.Next(0, num);
				int num3 = 0;
				for (int k = 0; k < list.Count; k++)
				{
					if (num2 >= num3 && num2 < num3 + list[k].weight)
					{
						providerSettings = list[k];
						break;
					}
					num3 += list[k].weight;
				}
			}
		}
		ScreenAdProvider currentProvider = null;
		if (providerSettings != null)
		{
			currentProvider = GetProviderForSettings(providerSettings);
		}
		SetCurrentProvider(currentProvider);
	}

	private ScreenAdProvider GetProviderForSettings(ProviderSettings settings)
	{
		ScreenAdProvider result = null;
		switch (settings.id)
		{
		case "inmobi":
			result = InmobiAdsProvider.instance;
			break;
		case "chartboost":
			result = ChartboostAdsProvider.instance;
			break;
		case "mdotm":
			result = MdotMAdsProvider.instance;
			break;
		}
		return result;
	}

	private ProviderSettings GetSettingsForProvider(ScreenAdProvider provider)
	{
		ProviderSettings result = null;
		foreach (ProviderSettings screenAdProviderSettings in _screenAdProviderSettingsList)
		{
			if (provider.id == screenAdProviderSettings.id)
			{
				result = screenAdProviderSettings;
				break;
			}
		}
		return result;
	}

	private void ChangeProvider(ChangeProviderListLevel changeLevel)
	{
		SetCurrentProvider(null);
		switch (changeLevel)
		{
		case ChangeProviderListLevel.repopulate_UsingOnlineSettings_Forced:
			SetProviderList(true);
			break;
		case ChangeProviderListLevel.repopulate_UsingCachedVersion_Forced:
			SetProviderList(false);
			break;
		case ChangeProviderListLevel.repopulate_UsingCachedVersion_IfEmpty:
			if (_screenAdProviderSettingsList.Count <= 0)
			{
				SetProviderList(false);
			}
			break;
		}
		GetBestProvider();
	}

	private bool DidExceedTimeLimitSinceLastCache()
	{
		bool result = false;
		if (_isCaching && (expireTime - DateTime.UtcNow).TotalSeconds <= 0.0)
		{
			result = true;
		}
		return result;
	}

	private void ShowInterstitial()
	{
		if (_currentProvider != null)
		{
			_isPresentingInterstitial = true;
			UIScreenController.Instance.ShowTransitionAdsOverlay();
			if (!PlayerPrefs.HasKey("seen_first_interstitial"))
			{
				PlayerPrefs.SetInt("seen_first_interstitial", 1);
			}
			string eventName = string.Format("Interstitial Providers Show {0}", _currentShowLocation.id.ToString());
			Flurry.LogEventWithAParameter(eventName, _currentProvider.id, "Show Ad");
			_currentProvider.ShowInterstitial();
			_lastShownProvider = _currentProvider;
			_currentShowLocation = InterstitialLocations.SetLocationLastShownAdTimeStamp(_currentShowLocation, DateTime.Now);
			PlayerInfo.Instance.lastTimeInterstitialWasSeen = DateTime.Now;
			PlayerInfo.Instance.numberOfInterstitialsPerHour++;
			PlayerInfo.Instance.numberOfInterstitialsPerDay++;
		}
		else
		{
			LogError("ScreenAdsManager: ShowInterstitial() -->>> the currentProvider is NULL");
		}
	}

	private DateTime NextAllowedShowTime()
	{
		DateTime result = DateTime.MaxValue;
		if (_currentShowLocation.id != LocationID.DisableShow)
		{
			if (!PlayerPrefs.HasKey("seen_first_interstitial"))
			{
				if (PlayerPrefs.HasKey("cb_alwnxt_ticks"))
				{
					string s = PlayerPrefs.GetString("cb_alwnxt_ticks");
					long result2;
					if (!long.TryParse(s, out result2))
					{
						result2 = DateTime.Now.Ticks;
					}
					result = new DateTime(result2);
				}
				else
				{
					result = DateTime.Now + TimeSpan.FromSeconds(72000.0);
					PlayerPrefs.SetString("cb_alwnxt_ticks", result.Ticks.ToString());
				}
			}
			else if (DateTime.Now < PlayerInfo.Instance.lastTimeInterstitialWasSeen)
			{
				result = DateTime.MinValue;
			}
			else
			{
				uint value = OnlineSettings.instance.GetValue("interstitials_delay_seconds", 10u);
				DateTime dateTime = PlayerInfo.Instance.lastTimeInterstitialWasSeen + TimeSpan.FromSeconds(value);
				DateTime dateTime2 = _currentShowLocation.lastShownAdTimeStamp + TimeSpan.FromSeconds(GetLocationTimeLimitToNextShow(_currentShowLocation));
				result = ((DateTime.Compare(dateTime, dateTime2) <= 0) ? dateTime2 : dateTime);
			}
		}
		return result;
	}

	private bool DidNotExceedNumberOfInterstitials()
	{
		DateTime dailyInterstitialsTimeLeft = PlayerInfo.Instance.dailyInterstitialsTimeLeft;
		DateTime hourlyInterstitialsTimeLeft = PlayerInfo.Instance.hourlyInterstitialsTimeLeft;
		int numberOfInterstitialsPerHour = PlayerInfo.Instance.numberOfInterstitialsPerHour;
		int numberOfInterstitialsPerDay = PlayerInfo.Instance.numberOfInterstitialsPerDay;
		TimeSpan timeSpan = dailyInterstitialsTimeLeft - DateTime.UtcNow;
		TimeSpan timeSpan2 = hourlyInterstitialsTimeLeft - DateTime.UtcNow;
		if (timeSpan.TotalSeconds < 0.0)
		{
			PlayerInfo.Instance.dailyInterstitialsTimeLeft = DateTime.UtcNow.AddHours(24.0);
			PlayerInfo.Instance.hourlyInterstitialsTimeLeft = DateTime.UtcNow.AddHours(1.0);
			PlayerInfo.Instance.numberOfInterstitialsPerDay = 0;
			PlayerInfo.Instance.numberOfInterstitialsPerHour = 0;
			return true;
		}
		if (timeSpan.TotalSeconds > 0.0)
		{
			uint value = OnlineSettings.instance.GetValue("interstitials_limit_per_day", 20u);
			if (value <= numberOfInterstitialsPerDay)
			{
				Flurry.LogEventWithAParameter("Interstitials Show Daily Limit Reached", "Limit", value.ToString());
				return false;
			}
			if (timeSpan2.TotalSeconds <= 0.0)
			{
				PlayerInfo.Instance.numberOfInterstitialsPerHour = 0;
				PlayerInfo.Instance.hourlyInterstitialsTimeLeft = DateTime.UtcNow.AddHours(1.0);
				return true;
			}
			uint value2 = OnlineSettings.instance.GetValue("interstitials_limit_per_hour", 5u);
			if (value2 <= numberOfInterstitialsPerHour)
			{
				Flurry.LogEventWithAParameter("Interstitials Show Hourly Limit Reached", "Limit", value2.ToString());
				return false;
			}
			return true;
		}
		return false;
	}

	private uint GetLocationTimeLimitToNextShow(Location location)
	{
		uint num = 0u;
		string key = string.Format("{0}_{1}_{2}", "interstitial_location", location.id, "time_limit");
		return OnlineSettings.instance.GetValue(key, location.defaultTimeLimitToNextShow);
	}

	private bool GetLocationEnabledState(Location location)
	{
		bool flag = false;
		string key = string.Format("{0}_{1}_{2}", "interstitial_location", location.id, "enabled");
		return OnlineSettings.instance.GetValue(key, location.defaultEnabled);
	}

	private bool OnlineSettingsProvidersVersionChanged()
	{
		bool result = false;
		int value = OnlineSettings.instance.GetValue("interstitial_providers_list_version", 0);
		int interstitialProviderListVersion = PlayerInfo.Instance.interstitialProviderListVersion;
		if (value != interstitialProviderListVersion)
		{
			PlayerInfo.Instance.interstitialProviderListVersion = value;
			result = true;
		}
		return result;
	}

	private bool HasLockOnTimeLimitPassed()
	{
		bool result = false;
		uint value = OnlineSettings.instance.GetValue("interstitial_provider_lock_on_time_limit", 72000u);
		DateTime t = PlayerInfo.Instance.lastTimeInterstitialProviderHasChanged + TimeSpan.FromSeconds(value);
		if (DateTime.Compare(DateTime.Now, t) > 0)
		{
			result = true;
		}
		return result;
	}

	private void SetProviderList(bool fromOnlineSettings)
	{
		_screenAdProviderSettingsList.Clear();
		List<ProviderSettings> providersForLocale = GroupInfo.GetProvidersForLocale(DeviceUtility.GetLocale(), fromOnlineSettings);
		foreach (ProviderSettings item in providersForLocale)
		{
			ScreenAdProvider providerForSettings = GetProviderForSettings(item);
			if (providerForSettings.isScreenAdProviderSupported && item.weight > 0)
			{
				_screenAdProviderSettingsList.Add(item);
			}
		}
	}

	private void SetCurrentProvider(ScreenAdProvider provider)
	{
		if (provider != _currentProvider)
		{
			if (_currentProvider != null)
			{
				_currentProvider.onDidCacheInterstitial -= OnProviderChachedInterstitial;
				_currentProvider.onDidFailToLoadInterstitial -= OnProviderDidFailToLoadInterstitial;
				_currentProvider.onDidClickInterstitial -= OnProviderDidClickInterstitial;
				_currentProvider.onDidFailToShowInterstitial -= OnProviderDidFailToShowInterstitial;
				_currentProvider.onDidShowInterstitial -= OnProviderDidShowInterstitial;
				_currentProvider.onDidDismissInterstitial -= OnProviderDidDismissInterstitial;
				_isCaching = false;
				ProviderSettings settingsForProvider = GetSettingsForProvider(_currentProvider);
				_screenAdProviderSettingsList.Remove(settingsForProvider);
				_currentNumberOfFails = 0;
				_currentProvider = null;
			}
			if (provider != null)
			{
				_currentProvider = provider;
				_currentProvider.onDidCacheInterstitial += OnProviderChachedInterstitial;
				_currentProvider.onDidFailToLoadInterstitial += OnProviderDidFailToLoadInterstitial;
				_currentProvider.onDidClickInterstitial += OnProviderDidClickInterstitial;
				_currentProvider.onDidFailToShowInterstitial += OnProviderDidFailToShowInterstitial;
				_currentProvider.onDidShowInterstitial += OnProviderDidShowInterstitial;
				_currentProvider.onDidDismissInterstitial += OnProviderDidDismissInterstitial;
			}
		}
		if (_currentProvider != null)
		{
			PlayerInfo.Instance.lastTimeInterstitialProviderHasChanged = DateTime.Now;
			if (!_currentProvider.isInitialized)
			{
				_currentProvider.Init();
			}
		}
	}

	private void OnProviderChachedInterstitial(ScreenAdProvider provider)
	{
		if (_currentProvider != null && provider != null && _currentProvider.id == provider.id)
		{
			LogFlurryEventForProvider("Interstitial Providers Request", _currentProvider.id, "Request Success");
			_isCaching = false;
			ShowInterstitialIfAble();
			return;
		}
		LogError("Recieved OnProviderChachedInterstitial from non-current provider: " + provider.id + ", current is: " + _currentProvider.id + ". A delegate might not have been un-hooked.");
	}

	private void OnProviderDidFailToLoadInterstitial(ScreenAdProvider provider)
	{
		if (_currentProvider != null && provider != null && _currentProvider.id == provider.id)
		{
			_isPresentingInterstitial = false;
			_currentNumberOfFails++;
			LogFlurryEventForProvider("Interstitial Providers Request", _currentProvider.id, "Request Failed");
			_isCaching = false;
			uint value = OnlineSettings.instance.GetValue("number_of_fails_for_interstitial", 1u);
			if (_currentNumberOfFails >= value)
			{
				ChooseProviderAndTryCache(true, ChangeProviderListLevel.dontRepopulate);
			}
		}
		else
		{
			LogError("Recieved OnProviderDidFailToLoadInterstitial from non-current provider: " + provider.id + ", current is: " + _currentProvider.id + ". A delegate might not have been un-hooked.");
		}
	}

	private void OnProviderDidClickInterstitial(ScreenAdProvider provider)
	{
		if (_currentProvider != null && provider != null && _currentProvider.id == provider.id)
		{
			if (UIScreenController.IsInstanced && _currentProvider.id == InmobiAdsProvider.instance.id)
			{
				UIScreenController.Instance.HideAdsOverlay();
			}
			_isPresentingInterstitial = false;
			Flurry.LogEventWithAParameter("Interstitial Providers Show {0}", _currentProvider.id, "Click");
		}
		else
		{
			LogError("Recieved OnProviderDidClickInterstitial from non-current provider: " + provider.id + ", current is: " + _currentProvider.id + ". A delegate might not have been un-hooked.");
		}
	}

	private void OnProviderDidFailToShowInterstitial(ScreenAdProvider provider)
	{
		if (_currentProvider != null && provider != null && _currentProvider.id == provider.id)
		{
			LogFlurryEventForProvider("Interstitial Providers Request", _currentProvider.id, "Request Failed To Show");
			ChangeProvider(ChangeProviderListLevel.dontRepopulate);
			return;
		}
		LogError("Recieved OnProviderDidFailToShowInterstitial from non-current provider: " + provider.id + ", current is: " + _currentProvider.id + ". A delegate might not have been un-hooked.");
	}

	private void OnProviderDidShowInterstitial(ScreenAdProvider provider)
	{
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.ShowAdsOverlay();
		}
		if (_currentProvider == null || provider == null || !(_currentProvider.id == provider.id))
		{
			LogError("Recieved OnProviderDidShowInterstitial from non-current provider: " + provider.id + ", current is: " + _currentProvider.id + ". A delegate might not have been un-hooked.");
		}
	}

	private void OnProviderDidDismissInterstitial(ScreenAdProvider provider)
	{
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.HideAdsOverlay();
		}
		_isPresentingInterstitial = false;
		if (_currentProvider == null || provider == null || !(_currentProvider.id == provider.id))
		{
			LogError("Recieved OnProviderDidDismissInterstitial from non-current provider: " + provider.id + ", current is: " + _currentProvider.id + ". A delegate might not have been un-hooked.");
		}
	}

	private void LogFlurryEventForProvider(string flurryEvent, string providerID, string eventParameter)
	{
		string empty = string.Empty;
		string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
		if (!string.IsNullOrEmpty(contryCodeFromLocale))
		{
			empty = contryCodeFromLocale.ToUpper();
			List<string> countryCodesForFlurryLogging = GetCountryCodesForFlurryLogging();
			if (countryCodesForFlurryLogging.Contains(empty))
			{
				Flurry.LogEventWithAParameter(string.Format("{0}-{1}", flurryEvent, empty), providerID, eventParameter);
			}
		}
		Flurry.LogEventWithAParameter(flurryEvent, providerID, eventParameter);
	}

	private List<string> GetCountryCodesForFlurryLogging()
	{
		List<string> list = new List<string>();
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("interstitial_flurry_logging_country_codes", out valueString))
		{
			string[] collection = valueString.Split(';');
			list.AddRange(collection);
		}
		else
		{
			list.AddRange(new List<string> { "US", "GB", "CA", "DK", "BR", "MX", "IN", "ID" });
		}
		List<string> list2 = new List<string>();
		foreach (string item in list)
		{
			list2.Add(item.ToUpper());
		}
		return list2;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_SCREENADSMANAGER_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
