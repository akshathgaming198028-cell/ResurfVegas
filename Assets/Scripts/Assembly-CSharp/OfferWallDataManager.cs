using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Monetization.Offerwall;
using UnityEngine;

public class OfferWallDataManager : IOfferWallDataManager
{
	private const string OFFERWALL_DISABLED_ONLINE_SETTING = "offerwall_disabled";

	private const string NUMBER_OF_EARNERS_ONLINE_SETTING = "offerwall_number_of_earners";

	private const string OFFERS_REFRESH_INTERVAL_AFTER_SUCCESS_ONLINE_SETTING = "offerwall_offers_refresh_interval_after_success";

	private const string OFFERS_REFRESH_INTERVAL_AFTER_FAIL_ONLINE_SETTING = "offerwall_offers_refresh_interval_after_fail";

	private const string REWARDS_REFRESH_INTERVAL_AFTER_SUCCESS_ONLINE_SETTING = "offerwall_rewards_refresh_interval_after_success";

	private const string REWARDS_REFRESH_INTERVAL_AFTER_FAIL_ONLINE_SETTING = "offerwall_rewards_refresh_interval_after_fail";

	private const string MIN_REWARD_ALLOWED_ONLINE_SETTING = "offerwal_min_reward_allowed";

	private const string MAX_REWARD_ALLOWED_ONLINE_SETTING = "offerwall_max_reward_allowed";

	private const string FILTER_MINMAX_REWARD_ONLINE_SETTING = "offerwall_filter_min_max_reward";

	private const string FILTER_FREE_OFFERS_ONLINE_SETTING = "offerwall_filter_free";

	private const string FILTER_OFFER_TYPE = "offerwall_filter_offer_type";

	private const string OFFERWALL_REQUEST_TIMEOUT = "offerwall_request_timeout";

	private const bool DEFAULT_DISABLED_ONLINE_SETTING = false;

	private const int DEFAULT_NUMBER_OF_EARNERS = 8;

	private const int DEFAULT_OFFERS_REFRESH_INTERVAL_AFTER_FAIL = 30;

	private const int DEFAULT_OFFERS_REFRESH_INTERVAL_AFTER_SUCCESS = 3600;

	private const int DEFAULT_REWARDS_REFRESH_INTERVAL_AFTER_FAIL = 30;

	private const int DEFAULT_REWARDS_REFRESH_INTERVAL_AFTER_SUCCESS = 120;

	private const int DEFAULT_OFFERWALL_REQUEST_TIMEOUT = 10;

	private const int DEFAULT_MIN_REWARD = 10;

	private const int DEFAULT_MAX_REWARD = 10000;

	private const string DEFAULT_OFFER_COST = "Free";

	private const string DEFAULT_OFFER_TYPE = "App";

	private const bool DEFAULT_FILTER_MIN_MAX = true;

	private const bool DEFAULT_FILTER_FREE_OFFERS = true;

	private const bool DEFAULT_FILTER_OFFER_TYPE = true;

	private const string DEFAULT_EXTERNAL_IP_URL = "http://hoodrunner.kiloo.com/externalip.php";

	public bool isOfferWallDisabled
	{
		get
		{
			return OnlineSettings.instance.GetValue("offerwall_disabled", false);
		}
	}

	public int offersRefreshIntervalAfterSuccess
	{
		get
		{
			return OnlineSettings.instance.GetValue("offerwall_offers_refresh_interval_after_success", 3600);
		}
	}

	public int offersRefreshIntervalAfterFail
	{
		get
		{
			return OnlineSettings.instance.GetValue("offerwall_offers_refresh_interval_after_fail", 30);
		}
	}

	public int rewardsRefreshIntervalAfterSuccess
	{
		get
		{
			return OnlineSettings.instance.GetValue("offerwall_rewards_refresh_interval_after_success", 120);
		}
	}

	public int rewardsRefreshIntervalAfterFail
	{
		get
		{
			return OnlineSettings.instance.GetValue("offerwall_rewards_refresh_interval_after_fail", 30);
		}
	}

	public int requestTimeout
	{
		get
		{
			return OnlineSettings.instance.GetValue("offerwall_request_timeout", 10);
		}
	}

	public string externalIpUrl
	{
		get
		{
			return "http://hoodrunner.kiloo.com/externalip.php";
		}
	}

	public int GetNumberOfOffersToRequest()
	{
		int valueFromOnlineSettings = GetValueFromOnlineSettings(8, "offerwall_number_of_earners");
		valueFromOnlineSettings = ((valueFromOnlineSettings > 0) ? valueFromOnlineSettings : 0);
		int numberOfDisplayedNonOfferEarners = EarnCoinsData.GetNumberOfDisplayedNonOfferEarners();
		return (numberOfDisplayedNonOfferEarners >= valueFromOnlineSettings) ? 1 : (valueFromOnlineSettings - numberOfDisplayedNonOfferEarners);
	}

	public string GetSDKAppID(OfferWallProviderID providerID)
	{
		if (providerID == OfferWallProviderID.TapJoy)
		{
			return AdConstants.instance.TapJoyAppId;
		}
		return string.Empty;
	}

	public string GetSDKAppKey(OfferWallProviderID providerID)
	{
		if (providerID == OfferWallProviderID.TapJoy)
		{
			return AdConstants.instance.TapJoyAppKey;
		}
		return string.Empty;
	}

	public string GetSDKEventName(OfferWallProviderID providerID)
	{
		if (providerID == OfferWallProviderID.TapJoy)
		{
			return AdConstants.instance.TapJoyEventName;
		}
		return string.Empty;
	}

	public Dictionary<OfferWallFilterType, string> GetFilters()
	{
		Dictionary<OfferWallFilterType, string> dictionary = new Dictionary<OfferWallFilterType, string>();
		if (OnlineSettings.instance.GetValue("offerwall_filter_min_max_reward", true))
		{
			dictionary.Add(OfferWallFilterType.minReward, GetValueFromOnlineSettings(10, "offerwal_min_reward_allowed").ToString());
			dictionary.Add(OfferWallFilterType.maxReward, GetValueFromOnlineSettings(10000, "offerwall_max_reward_allowed").ToString());
		}
		if (OnlineSettings.instance.GetValue("offerwall_filter_free", true))
		{
			dictionary.Add(OfferWallFilterType.costForInstall, "Free");
		}
		if (OnlineSettings.instance.GetValue("offerwall_filter_offer_type", true))
		{
			dictionary.Add(OfferWallFilterType.offerType, "App");
		}
		return dictionary;
	}

	private int GetValueFromOnlineSettings(int defaultValue, string onlineSettingKey)
	{
		int result = defaultValue;
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue(onlineSettingKey, out valueString))
		{
			int result2 = 0;
			if (int.TryParse(valueString, out result2))
			{
				result = result2;
			}
		}
		return result;
	}

	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError("OfferWallManager: " + msg, context);
	}

	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning("OfferWallManager: " + msg, context);
	}

	[Conditional("OFFERWALL_DEBUG_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log("OfferWallManager: " + msg, context);
	}
}
