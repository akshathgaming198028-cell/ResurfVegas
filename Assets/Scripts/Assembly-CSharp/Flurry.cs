using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common.FlurryAnalytics;
using UnityEngine;

public static class Flurry
{
	public const string EVENT_UISCREEN_CHANGED_PREFIX = "UI Screen ";

	public const string EVENT_POPUPSCREEN_CHANGED_PREFIX = "POPUP Screen ";

	public const string EVENT_10_SOCIAL_ACTIONS_TAKEN = "10 social actions taken";

	public const string EVENT_FIRST_GAMECENTER_LOGIN = "First GameCenter Login";

	public const string EVENT_FIRST_FACEBOOK_LOGIN = "First Facebook Login";

	public const string EVENT_SOCIAL_POKE = "Social friend poked";

	public const string EVENT_SOCIAL_DUELPOKE = "Social duel poked";

	public const string EVENT_SOCIAL_BRAG = "Social bragged";

	public const string EVENT_SOCIAL_BRAGFACEBOOK = "Social bragged Facebook";

	public const string EVENT_SOCIAL_FACEBOOK_FRIENDS_RETRIEVED = "Social Facebook Friends Retrieved";

	public const string EVENT_SOCIAL_GC_FRIENDS_RETRIEVED = "Social GameCenter Friends Retrieved";

	public const string EVENT_SOCIAL_FRIENDS_CONSOLIDATED = "Social Friends Consolidated";

	public const string EVENT_FACEBOOK_STATUS = "Facebook status";

	public const string EVENT_MYSTERY_BOX_OPENED = "Mystery Box opened";

	public const string EVENT_MINIMYSTERY_BOX_OPENED = "Mini Mystery Box opened";

	public const string EVENT_SUPER_MYSTERY_BOX_OPENED = "Super mystery box opened";

	public const string EVENT_INAPPPURCHASE_COMPLETED = "InApp purchase completed";

	public const string EVENT_INAPPPURCHASE_COINPACK1 = "InApp Coin Pack 1 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK2 = "InApp Coin Pack 2 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK3 = "InApp Coin Pack 3 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK4 = "InApp Coin Pack 4 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK5 = "InApp Coin Pack 5 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK1 = "InApp Key Pack 1 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK2 = "InApp Key Pack 2 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK3 = "InApp Key Pack 3 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK4 = "InApp Key Pack 4 purchased";

	public const string EVENT_INAPPPURCHASE_DOUBLECOINS = "Double Coin purchased";

	public const string EVENT_INAPPPURCHASE_DOUBLECOINS_POPUP = "Double Coin purchased GameOver";

	public const string EVENT_INAPPPURCHASE_BUNDLE1 = "InApp Bundle 1 purchased";

	public const string EVENT_INAPPPURCHASE_BUNDLE2 = "InApp Bundle 2 purchased";

	public const string EVENT_INAPPPURCHASE_BUNDLE3 = "InApp Bundle 3 purchased";

	public const string EVENT_INAPPPURCHASE_BUNDLE4 = "InApp Bundle 4 purchased";

	public const string EVENT_INAPPPURCHASE_BUNDLE5 = "InApp Bundle 5 purchased";

	public const string EVENT_CHARACTER_UNLOCKED = "Character unlocked";

	public const string EVENT_AUTOMESSAGE_TURNED_OFF = "AutoBrag turned off";

	public const string EVENT_DAYS_PASSED_SINCE_LAST_PLAY = "Days passed since last play";

	public const string EVENT_BUY_HOVERBOARD_FROM_POPUP = "Purchase hoverboard from popup";

	public const string EVENT_APPLICATION_AUTHENTICITY = "Application Authenticity";

	public const string EVENT_START_DUEL = "Start duel request";

	public const string EVENT_UNKNOWN_GPU = "Unknown GPU";

	public const string EVENT_DEVICE_PERFORMANCE = "Device performance";

	public const string EVENT_END_OF_RUN_AVG_FPS = "InRunAvgFPS";

	public const string EVENT_DEVICE_MEMORY = "Device Memory";

	public const string EVENT_CHARACTERS_OWNED = "Characters owned";

	public const string EVENT_HOVERBOARDS_OWNED = "Hoverboards owned";

	public const string EVENT_CHARACTER_OUTFITS_OWNED = "Character outfits owned";

	public const string EVENT_HOVERBOARD_ABILITIES_OWNED = "Hoverboard abilities owned";

	public const string EVENT_ACTIVATED_HOVERBOARD = "Boost hoverboard activated";

	public const string EVENT_DAILY_BUNDLEIDENTIFIER = "The current bundle identifier";

	public const string EVENT_MISSIONSET_COMPLETED = "Mission Set completed";

	public const string EVENT_DAILY_CHALLENGE_COMPLETED = "Daily Challenge completed";

	public const string EVENT_UPDATE_APP_POPUP_RESULT = "New Version Popup Result";

	public const string EVENT_FILEUTIL_LOAD_CORRUPTED = "FileUtil load corrupted";

	public const string EVENT_PLAYERINFO_LOAD_EMPTYPATH = "PlayerInfo.Load Empty path";

	public const string EVENT_PLAYERINFO_LOAD_FILENOTFOUND = "PlayerInfo.Load FileNotFound";

	public const string EVENT_PLAYERINFO_LOAD_EXCEPTION = "PlayerInfo.Load Exception";

	public const string EVENT_VIDEOADS_ATTEMPT_TO_PLAY = "VideoAds attempt to play";

	public const string EVENT_VIDEOADS_PROVIDER_REQUEST = "VideoAds {0} request";

	public const string EVENT_FRIENDREWARD_COLLECTED = "Friend reward collect";

	public const string EVENT_EARNCOINS_ITEM_CLICKED = "Earn Coins item clicked";

	public const string EVENT_VIDEO_ADS_CLICKED = "Video Ads button clicked";

	public const string EVENT_QUICKVIDEO_ADS_CLICKED = "Quick Video Ads button clicked";

	public const string EVENT_EARNCOINS_SCREEN_OPENED = "Earn coins screen opened";

	public const string EVENT_OLD_DEVICE_TOKEN_UNREGISTERED = "Old Device Token Unregistered";

	public const string EVENT_CHARACTER_THEME_UNLOCKED = "Outfit bought";

	public const string EVENT_HOVERBOARD_BOUGHT = "Hoverboard bought";

	public const string EVENT_FACEBOOK_SHARE = "Share link on Facebook";

	public const string EVENT_BREADCRUMBS_BEFORE_FREE_HOVERBOARD = "Bread crumbs before purchasing the free hoverboard";

	public const string EVENT_MORE_COINS_CLICKED = "More coins button clicked";

	public const string EVENT_BACK_BTN_PRESSED = "Back button pressed";

	public const string EVENT_HOME_BUTTON_PRESSED = "Home button pressed";

	public const string EVENT_RATING_POPUP = "Rate the app popup";

	public const string EVENT_AMOUNT_OF_COINS = "Daily report of coins";

	public const string EVENT_AMOUNT_OF_KEYS = "Daily keys report";

	public const string EVENT_AMOUNT_OF_CONSUMABLE_HOVERBOARDS = "Daily report of consumable hoverboards";

	public const string EVENT_AMOUNT_OF_HEADSTARTS = "Daily report of headstarts owned";

	public const string EVENT_AMOUNT_OF_SCOREBOOSTERS = "Daily report of score boosters owned";

	public const string EVENT_ONLINE_STATUS = "Daily online status";

	public const string EVENT_DAILY_ONLINE_STATUS = "Daily Check of Online Status";

	public const string EVENT_NOTIFICATIONS_STATE = "State of notifications";

	public const string EVENT_REMINDERS_STATE = "State of reminders";

	public const string EVENT_SSEVENT_POPUP_SHOWN = "Event Popup was shown";

	public const string EVENT_HOVERBOARD_TRIED = "Event Popup was shown";

	public const string EVENT_ACTIVE_CHARACTER = "Currently active character";

	public const string EVENT_ACTIVE_BOARD = "Currently active board";

	public const string EVENT_SKIPPED_CHARACTER_FOR_CHARACTER_NAME_EVENT = "Skipped character for Wordy Weekend";

	public const string EVENT_UNLOCKED_CHARACTER_FOR_CHARACTER_NAME_EVENT_WITH_PARAMETERS = "Unlocked character for Wordy Weekend";

	public const string EVENT_UNLOCKED_CHARACTER_FOR_CHARACTER_NAME_EVENT = "{0} unlocked for Wordy Weekend";

	public const string EVENT_ATTEMPT_TO_BUY_COINS_FOR_CHARACTER_NAME_EVENT = "Attempt to buy coins for Wordy Weekend";

	public const string EVENT_ATTEMPT_TO_BUY_KEYS_FOR_CHARACTER_NAME_EVENT = "Attempt to buy keys for the Wordy Weekend";

	public const string EVENT_BOUGHT_COINS_FOR_CHARACTER_NAME_EVENT = "Bought coins for Wordy Weekend";

	public const string EVENT_BOUGHT_KEYS_FOR_CHARACTER_NAME_EVENT = "Bought keys for Wordy Weekend";

	public const string EVENT_CANCELED_BUYING_COINS_FOR_CHARACTER_NAME_EVENT = "Canceled buying coins for Wordy Weekend";

	public const string EVENT_CANCELED_BUYING_KEYS_FOR_CHARACTER_NAME_EVENT = "Canceled buying keys for Wordy Weekend";

	public const string EVENT_UNABLE_TO_BUY_COINS_FOR_CHARACTER_NAME_EVENT = "Unable to buy coins for Wordy Weekend";

	public const string EVENT_UNABLE_TO_BUY_KEYS_FOR_CHARACTER_NAME_EVENT = "Unable to buy keys for Wordy Weekend";

	public const string EVENT_ATTEMPTED_TO_SHOW_DOUBLE_UP_BUTTON = "Attempted to show double up button";

	public const string EVENT_DOUBLE_UP_CHOICE_RESULT = "Double up choice";

	public const string EVENT_DOUBLE_UP_BUTTON_SHOWN = "Double up button shown";

	public const string EVENT_DOUBLE_UP_BUTTON_CLICKED = "Double up button clicked";

	public const string EVENT_VIDEOADS_END_OF_WATERFALL = "VideoAds end of waterfall";

	public const string EVENT_NO_FILL_FOR_DOUBLE_BUTTON = "No fill for double button";

	public const string EVENT_VIDEOADS_CACHE_REQUEST_ENDED = "VideoAds Cache Request Ended";

	public const string EVENT_VIDEOADS_CACHE_REQUEST_ENDED_COUNTRY_INFO = "VideoAds Cache Request Ended (Country Info)";

	public const string EVENT_LIMITED_CHARACTER_POPUP_CLOSED = "Limited character popup closed";

	public const string EVENT_VIDEOADS_CACHING_TIME = "VideoAds Caching Time(2)";

	public const string EVENT_LIMITED_BOARD_POPUP_CLOSED = "Limited board popup closed";

	public const string EVENT_OFFER_REDEEMED = "Offerwall Offer Redeemed";

	public const string EVENT_OFFERS_REQUESTED = "Offerwall Offers Requested and Returned";

	public const string EVENT_OFFER_CLICKED = "Offerwall Offer Clicked";

	public const string EVENT_HOVERBOARD_THEME_BOUGHT = "Hoverboard Theme bought";

	public const string EVENT_HOVERBOARD_THEME_ACTIVATED = "Hoverboard Theme activated";

	public const string EVENT_HOVERBOARD_THEME_DEACTIVATED = "Hoverboard Theme deactivated";

	public const string EVENT_BOOST_SCOREBOOSTER_PURCHASED = "Boost Scorebooster purchased";

	public const string EVENT_BOOST_HEADSTART500_PURCHASED = "Boost Headstart500 purchased";

	public const string EVENT_BOOST_HEADSTART2000_PURCHASED = "Boost Headstart2000 purchased";

	public const string EVENT_BOOST_HOVERBOARD_PURCHASED = "Boost Hoverboard purchased";

	public const string EVENT_BOOST_COINMAGNET_PURCHASED = "Boost Coinmagnet purchased";

	public const string EVENT_BOOST_DOUBLEMULTIPLIER_PURCHASED = "Boost 2x multiplier purchased";

	public const string EVENT_BOOST_JETPACK_PURCHASED = "Boost jetpack purchased";

	public const string EVENT_BOOST_LETTERS_PURCHASED = "Boost letters purchased";

	public const string EVENT_BOOST_SUPERSNEAKERS_PURCHASED = "Boost supersneakers purchased";

	public const string EVENT_BOOST_MYSTERYBOX_PURCHASED = "Boost MysteryBox purchased";

	public const string EVENT_BOOST_MISSION_SKIP_PURCHASED = "Boost Mission Skip purchased";

	public const string EVENT_BOOST_MYSTERYBOX_VIEW_PRIZES = "Mysterybox view prices";

	public const string EVENT_SUPER_MYSTERYBOX_VIEW_PRIZES = "Super Mystery Box prizes";

	public const string EVENT_MINI_MYSTERYBOX_VIEW_PRIZES = "Mini Mystery Box prizes";

	public const string EVENT_SAVE_ME_PERFORMED = "Save Me performed";

	public const string EVENT_WEEKLY_CHALLENGE_COMPLETED = "Weekly Challenge Completed";

	public const string EVENT_INTERSTITIAL_PROVIDER_REQUEST = "Interstitial Providers Request";

	public const string EVENT_INTERSTITIAL_PROVIDER_SHOW = "Interstitial Providers Show {0}";

	public const string EVENT_INTERSTITIAL_SHOW_DAILY_LIMIT_REACHED = "Interstitials Show Daily Limit Reached";

	public const string EVENT_INTERSTITIAL_SHOW_HOURLY_LIMIT_REACHED = "Interstitials Show Hourly Limit Reached";

	public const string EVENT_INTERSTITIAL_GAME_OVER_COUNTER = "Interstitial Game Over Counter";

	public const string EVENT_AWARD_TIER_COMPLETED = "Award tier completed";

	public const string EVENT_AWARD_COMPLETED = "Award completed";

	public const string EVENT_AWARD_TIER_COMPLETED_TIME = "Award tier completed time";

	public const string EVENT_AWARD_TIER_COLLECTED_TIME = "Award tier collected time";

	public const string EVENT_AMOUNT_OF_GOLD_AWARDS_FINISHED = "Total amount of gold awards finished";

	public const string EVENT_AMOUNT_OF_SILVER_AWARDS_FINISHED = "Total amount of silver awards finished";

	public const string EVENT_ASSET_BUNDLE_FAILED_TO_CACHE = "Asset Bundle Failed to be Cached";

	public const string EVENT_ASSET_BUNDLE_FAILED_LOAD_FROM_CACHE = "Asset Bundle Failed to be loaded from Cache";

	public const string EVENT_ASSET_BUNDLE_FAILED_DOWNLOAD = "Asset Bundle Failed to Download";

	public const string EVENT_ASSET_BUNDLE_AVAILABILITY_CHECK_FAILED = "Asset Bundles Availability Check Failed";

	public const string EVENT_ASSET_BUNDLES_ALL_DOWNLOADED = "All Asset Bundle Downloads completed";

	public const string EVENT_ARGKEY_ID = "Id";

	public const string EVENT_ARGKEY_TIER = "Tier";

	public const string EVENT_ARGKEY_UI_SCREENNAME = "Screen Name";

	public const string EVENT_ARGKEY_MISSIONSET = "Mission Set";

	public const string EVENT_ARGKEY_MISSIONSET_AND_INDEX = "Mission Set and Index";

	public const string EVENT_ARGKEY_TOTAL = "Total";

	public const string EVENT_ARGKEY_POPUPRESULT = "Result";

	public const string EVENT_ARGKEY_FILENAME = "Filename";

	public const string EVENT_ARGKEY_VIDEOADRESULT = "Result";

	public const string EVENT_ARGKEY_ITEM_IAP = "Item tiggered iap";

	public const string EVENT_ARGKEYS_SOCIAL_FRIENDS_RETRIEVED = "Total friends;Installed friends";

	public const string EVENT_ARGKEYS_SOCIAL_FRIENDS_CONSOLIDATED = "Total;Facebook;GameCenter";

	public const string EVENT_ARGKEY_IS_FROM_DAILY_CHALLENGE = "SMB from daily challenge";

	public const string EVENT_ARGKEY_RATE_ASKAGAIN = "Should ask again";

	public const string EVENT_ARGKEY_AMOUNT = "Amount";

	public const string EVENT_ARGKEY_NOTIFICATIONS_ENABLED = "Enabled";

	public const string EVENT_ARGKEY_APPLICATION_AUTHENTICITY = "Type";

	public const string EVENT_ARGKEY_RESULT = "Result";

	public const string EVENT_ARGKEY_STATE = "State";

	public const string EVENT_ARGKEY_STATUS = "Status";

	public const string EVENT_ARGKEY_VALUE = "Value";

	public const string EVENT_ARGKEY_AWARD = "Award";

	public const string EVENT_ARGKEY_TIME = "Time";

	public const string EVENT_ARGKEY_DEVICE = "Device";

	public const string EVENT_ARGKEY_ERRORCODE = "Error code";

	public const string EVENT_ARGKEY_EXCEPTION = "Exception";

	public const string EVENT_ARGKEY_STACKTRACE = "Stacktrace";

	public const string EVENT_ARGKEY_OFFERWALL_OFFER_COIN_AMOUNT = "Coin amount";

	public const string EVENT_ARGKEY_OFFERWALL_OFFERS_REQUESTED = "Returned for {0} requested";

	public const string EVENT_ARGKEY_OFFERWALL_OFFERS_RETURNED = "Valid for {0} returned";

	public const string EVENT_ARGKEY_CHARACTER_NAME_ACTION_PRICE = "Price";

	public const string EVENT_ARGKEY_CHARACTER_NAME_CHARACTER = "Character";

	public const string EVENT_ARGKEY_GPU_NAME = "GPU name";

	public const string EVENT_ARGKEY_PERFOMRNACE_LEVEL = "Level";

	public const string EVENT_ARGKEY_FPS_RANGE = "FPS range {0}";

	public const string EVENT_ARGKEY_DEVICE_MEMORY_AMOUNT = "Amount";

	public const string EVENT_ARGKEY_ITEMS_OWNED_REGULAR = "regular";

	public const string EVENT_ARGKEY_ITEMS_OWNED_LIMITED = "limited";

	public const string EVENT_ARGKEY_ITEMS_OWNED_TOKEN = "token";

	public const string EVENT_ARGKEY_ITEMS_OWNED_FACEBOOK = "facebook";

	public const string EVENT_ARGKEY_ITEMS_OWNED_TOTAL = "total";

	public const string EVENT_ARGKEY_ACTIVATED_HOVERBOARD_SUCCESSFULLY = "successfully";

	public const string EVENT_ARGKEY_AWARDS_GOLD_COMPLETED = "Completed Gold Awards";

	public const string EVENT_ARGKEY_AWARDS_SILVER_COMPLETED = "Completed Silver Awards";

	public const string EVENT_ARGKEY_COUNTRY = "Country";

	public const string EVENT_ARGKEY_ONLINE_COUNTRY = "Online Country";

	public const string EVENT_ARGKEY_OFFLINE_COUNTRY = "Offline Country";

	public const string EVENT_ARGKEY_LIMITED_CHARACTER_POPUP_CHARACTER_BOUGHT = "Character {0} bought";

	public const string EVENT_ARGKEY_LIMITED_CHARACTER_POPUP_OUTFIT_BOUGHT = "{0}_{1} bought";

	public const string EVENT_ARGKEY_ASSET_BUNDLE_NAME = "Asset Bundle Name";

	public const string EVENT_ARGKEY_ASSET_BUNDLE_VERSION = "Asset Bundle Version";

	public const string EVENT_ARGKEY_ASSET_BUNDLE_DOWNLOAD_ERROR = "Asset Bundle Download Error";

	public const string EVENT_ARGKEY_ALL_ASSET_BUNDLES_DOWNLOADED = "All Bundles Downloaded?";

	public const string EVENT_ARGKEY_ALL_ASSET_BUNDLES_DOWNLOADED_COUNTRY = "All Bundles Downloaded {0} in Country";

	public const string EVENT_ARGKEY_LIMITED_BOARD_POPUP_BOARD_BOUGHT = "Board {0} bought";

	public const string EVENT_ARGKEY_LIMITED_BOARD_POPUP_OUTFIT_BOUGHT = "{0}_{1} bought";

	public const string EVENT_ARGKEY_DAILT_BUNDLEIDENTIFIER = "Daily bundle identifier";

	public const string EVENT_ARGKEY_POPUPRESULT_OK = "Ok";

	public const string EVENT_ARGKEY_POPUPRESULT_CANCEL = "Cancel";

	public const string EVENT_ARGKEY_VIDEOADRESULT_OK = "Ok";

	public const string EVENT_ARGKEY_VIDEOADRESULT_NOVIDEO = "No video";

	public const string EVENT_ARGKEY_ASKRATE = "Ask again";

	public const string EVENT_ARGKEY_DONTASKRATE = "Do not ask again";

	public const string EVENT_ARGKEY_INTERSTITIAL_REQUEST_SUCCESS = "Request Success";

	public const string EVENT_ARGKEY_INTERSTITIAL_REQUEST_TIMEOUT = "Request TimeOut";

	public const string EVENT_ARGKEY_INTERSTITIAL_REQUEST_FAILED = "Request Failed";

	public const string EVENT_ARGKEY_INTERSTITIAL_REQUEST_FAILED_TO_INIT = "Request Failed To Init";

	public const string EVENT_ARGKEY_INTERSTITIAL_REQUEST_FAILED_TO_SHOW = "Request Failed To Show";

	public const string EVENT_ARGKEY_INTERSTITIAL_SHOWN = "Show Ad";

	public const string EVENT_ARGKEY_INTERSTITIAL_CLICKED = "Click";

	public const string EVENT_ARGKEY_INTERSTITIAL_LIMIT = "Limit";

	public const string EVENT_ARGKEY_PERFORMANCE_LEVEL_LOW = "Low";

	public const string EVENT_ARGKEY_PERFORMANCE_LEVEL_HIGH = "High";

	public const string EVENT_ARGKEY_GPU_NAME_WITH_VALUE = "GPU name {0}";

	public const string EVENT_ARGKEY_YES = "yes";

	public const string EVENT_ARGKEY_NO = "no";

	public const string EVENT_ARGKEY_DOUBLE_UP_BUTTON_CHANCE = "Chance";

	public const string EVENT_ARGKEY_DOUBLE_UP_BUTTON_FILL = "Fill";

	public const string EVENT_ARGKEY_DOUBLE_UP_BUTTON_NO_CHANCE = "No Chance";

	public const string EVENT_ARGKEY_DOUBLE_UP_BUTTON_NO_FILL = "No Fill";

	public const string EVENT_ARGKEY_DOUBLE_UP_OPTION_RESULT_DOUBLED = "Doubled";

	public const string EVENT_ARGKEY_DOUBLE_UP_OPTION_RESULT_NOT_DOUBLED = "Not Doubled";

	public const string EVENT_ARGKEY_SUCCESS = "Success";

	public const string EVENT_ARGKEY_FAILED = "Failed";

	public const string EVENT_ARGKEY_TIMEDOUT = "Timedout";

	private static readonly int[][] FRIEND_BRACKETS = new int[5][]
	{
		new int[3] { -1, 10, 1 },
		new int[3] { 11, 30, 5 },
		new int[3] { 31, 100, 10 },
		new int[3] { 101, 1000, 100 },
		new int[3] { 1001, 999999, 1000 }
	};

	private static char[] PARAMETER_SPLITTERS = new char[1] { ';' };

	public static void LogGameCenterLogin()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer && !PlayerPrefs.HasKey("flurry_has_logged_gc"))
		{
			LogEvent("First GameCenter Login");
			PlayerPrefs.SetInt("flurry_has_logged_gc", 1);
		}
	}

	public static void LogFacebookLogin()
	{
		if (!PlayerPrefs.HasKey("flurry_has_logged_fb"))
		{
			LogEvent("First Facebook Login");
			PlayerPrefs.SetInt("flurry_has_logged_fb", 1);
		}
	}

	public static void LogOkPressedOnPopup(string eventName)
	{
		if (eventName != string.Empty)
		{
			LogEventWithAParameter(eventName, "Result", "Ok");
		}
	}

	public static void LogClosePressedOnPopup(string eventName)
	{
		if (eventName != string.Empty)
		{
			LogEventWithAParameter(eventName, "Result", "Cancel");
		}
	}

	public static string ConvertFriendCountToBracket(int count)
	{
		for (int i = 0; i < FRIEND_BRACKETS.Length; i++)
		{
			int num = FRIEND_BRACKETS[i][0];
			int num2 = FRIEND_BRACKETS[i][1];
			int num3 = FRIEND_BRACKETS[i][2];
			if (count < num || count > num2)
			{
				continue;
			}
			while (num <= num2)
			{
				int num4 = num + num3;
				if (count >= num && count < num4)
				{
					if (num3 == 1)
					{
						return count.ToString();
					}
					return num + "-" + (num4 - 1);
				}
				num = num4;
			}
		}
		return count.ToString();
	}

	public static bool ShouldLogDailyEvent(string eventName)
	{
		int dayOfYear = DateTime.Now.DayOfYear;
		string key = "socflurdaily" + eventName;
		if (PlayerPrefs.HasKey(key))
		{
			int num = PlayerPrefs.GetInt(key);
			if (dayOfYear == num)
			{
				return false;
			}
		}
		PlayerPrefs.SetInt(key, dayOfYear);
		return true;
	}

	public static void StartSession(string apiKey, string userId)
	{
		FlurryAnalytics.SetContinueSessionMillis(10000L);
		FlurryAnalytics.SetUserId(userId);
		FlurryAnalytics.StartSession(apiKey);
	}

	public static void EndSession()
	{
		FlurryAnalytics.EndSession();
	}

	public static void LogEvent(string eventName)
	{
		FlurryAnalytics.LogEvent(eventName);
	}

	public static void LogEventWithAParameter(string eventName, string argKey, string argValue)
	{
		FlurryAnalytics.LogEventWithParams(eventName, new Dictionary<string, string> { { argKey, argValue } });
	}

	public static void LogEventWithSeveralParameters(string eventName, string argKeys, string argValues)
	{
		FlurryAnalytics.LogEventWithParams(eventName, ToDictionary(argKeys, argValues));
	}

	private static Dictionary<string, string> ToDictionary(string argKeys, string argValues)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = argKeys.Split(PARAMETER_SPLITTERS);
		string[] array2 = argValues.Split(PARAMETER_SPLITTERS);
		for (int i = 0; i < array.Length && i < array2.Length; i++)
		{
			dictionary.Add(array[i], array2[i]);
		}
		return dictionary;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError("[Flurry] " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning("[Flurry] " + msg, context);
	}

	[Conditional("ENABLE_FLURRY_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log("[Flurry] " + msg, context);
	}
}
