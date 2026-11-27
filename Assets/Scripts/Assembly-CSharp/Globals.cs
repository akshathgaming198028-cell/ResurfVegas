#define ENABLE_ERROR_LOGS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Subway.Info;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Globals
{
	public enum LogInState
	{
		Offline = 0,
		Facebook = 1,
		GameCenter = 2,
		Both = 3
	}

	private struct AddedAnimationEventInfo
	{
		public Animation animation;

		public string clipName;

		public float time;

		public string functionName;

		public AddedAnimationEventInfo(Animation animation, string clipName, float time, string functionName)
		{
			this.animation = animation;
			this.clipName = clipName;
			this.time = time;
			this.functionName = functionName;
		}
	}

	public const bool DEBUG_SOCIAL_MANAGER_SERVER = false;

	public const string OVERRIDEN_ONLINE_SETTINGS_BASE_URL = "http://10.0.0.169/hoodrunner";

	public const string DEBUG_DAILYWORD = "Day";

	public const LogInState DEBUG_LOGIN_STATE = LogInState.Facebook;

	public const Characters.CharacterType DEBUG_LIMITED_CHARACTER_TYPE = Characters.CharacterType.mumbaiCharacter01;

	public const Hoverboards.BoardType DEBUG_LIMITED_BOARD_TYPE = Hoverboards.BoardType.cherry;

	public const string fakeOnlineSettings = "efe40d19b11ddd485b4188664e9969f4bc89de0d\n[latestversion] 1.56.0\n[latestversion_changelist] \n\n\n* Subway Surfers in the Snow!\n* Limited Holiday character\n* Discover special Boards\n* Receive a free GIFT!\n* Optimized for iPhone 5!\n[in_app_tier_2] 20000\n[discount_end_time] 1814313600\n[videoads_providerlist] vungleclips\n[hoverboard_expiretime_liberty] 23-12-2012 22:00:00\n[hoverboard_starttime_liberty] 20-11-2012 22:00:00\n[character_expiretime_nychar] 23-12-2012 22:00:00\n[character_starttime_nychar] 13-10-2012 22:00:00\n[character_expiretime_elftricky] 23-11-2012 22:00:00\n[character_starttime_elftricky] 23-11-2012 22:00:00\n[enable_local_notifications] true[save_me_locales] en_US;en_UK;[offerwall_number_of_offers] 4[offerwall_session_timeout] 24[offerwall_offers_timeout] 24[offerwall_balance_timeout] 24[offerwal_min_reward_allowed] 10[offerwall_max_reward_allowed] 10000[offerwall_filter_free] true[save_me_locales] all[event_id]1496318400[event_value] 0;0[event_type] CharacterName[event_start] 1764547200[event_end] 1830000000[event_ribbon_text]ad3;3;6[number_of_fails_for_interstitial] 1[interstitials_limit_per_hour] 10[interstitials_limit_per_day] 40[intesrtitials_delay_seconds] 10[keys_rewarded_for_video_ad] 2";

	public const int MAX_MULTIPLIER = 30;

	public const int MAX_RANK = 1;

	public const int BONUS_FACEBOOK = 5000;

	public const int BONUS_GAMECENTER = 250;

	public const int INITIAL_AMOUNT_OF_KEYS = 5;

	public const int MIN_FRIEND_SCORE_REQUEST_INTERVAL = 15;

	public const int GIFT_BREADCRUMBS_TO_SHOW = 30;

	public const string LAST_SEEN_BUNDLE_VERSION_KEY = "lastSeenBundleVersionKey";

	public const string BASE_URL = "http://hoodrunner.kiloo.com";

	public const string URL_FOR_FACEBOOK_POST = "http://redirect.kiloo.com/subwayapp.php";

	public const string ICON_URL_FOR_FACEBOOK_POST = "http://hoodrunner.kiloo.com/fblogo_lasvegas2016.png";

	public const int PLAYERINFO_REDUNDANT_FILES = 1;

	public const int PLAYERINFO_BACKUP_SLOTS = 5;

	public const int PLAYERINFO_ALTERNATE_REDUNDANT_FILES = 0;

	public const int PLAYERINFO_ALTERNATE_BACKUP_SLOTS = 0;

	public const int DAILY_QUEST_MIN_REQUEST_INTERVAL = 120;

	public const string DAILY_QUEST_SECRET = "aIN0UXP4NNoANVGi5w3raGAFN1n5OLQZFDhwjs6HoX";

	public const string DAILY_QUEST_URL = "http://hoodrunner.kiloo.com/hr_dailyquests2.php";

	public const string PRIVACY_POLICY_URL = "http://www.kiloo.com/privacy/";

	public const int LAYER_2DGUI = 30;

	public const int LAYER_3DGUI = 31;

	public const int LAYER_2DOVERLAY = 28;

	public const int LAYER_3DCLIP = 29;

	public const int LAYER_2DOVERLAY_CLIP = 26;

	public const float DRAG_THRESHOLD = 0.1f;

	public const float BUTTON_TWEEN_DURATION = 0f;

	public static readonly bool DEBUG = false;

	public static readonly bool DEBUG_POPUP = false;

	public static readonly bool DEBUG_POPUP_TESTOLOGY;

	public static readonly bool DEBUG_FREE_PURCHASES;

	public static bool DEBUG_ALL_CHARS;

	public static bool DEBUG_ALL_BOARDS;

	public static readonly bool DEBUG_SCROLL_READOUT = true;

	public static bool DEBUG_FREE_INAPP_PURCHASE;

	public static bool DEBUG_SKIP_VIDEO_ADS;

	public static readonly bool DEBUG_SKIP_TUTORIAL_IN_EDITOR;

	public static readonly bool DEBUG_USE_DEBUG_LOGIN_STATE;

	public static readonly bool DEBUG_USE_FAKE_ONLINE_SETTINGS;

	public static readonly bool DEBUG_OVERRIDE_ONLINE_SETTINGS_URL;

	public static bool DEBUG_USE_ONLINESETTINGS_TESTTIME;

	public static bool DEBUG_USE_ONLINESETTINGS_SET;

	public static readonly bool DEBUG_USE_TEST_AD_CONSTANTS;

	public static bool DEBUG_PRINT_SPAWNED_POWER_UPS;

	public static bool DEBUG_SHOW_FRONT_SCREEN_IF_NEW_USER;

	public static bool DEBUG_INTERSTITIAL_PROVIDERS;

	public static bool DEBUG_INTERSTITIAL_PROVIDER_CHARTBOOST;

	public static bool DEBUG_INTERSTITIAL_PROVIDER_INMOBI;

	public static bool DEBUG_INTERSTITIAL_PROVIDER_MDOTM;

	public static bool DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT;

	public static bool DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IN_APP_PURCHASES;

	public static bool DEBUG_VIDEOADS_PROVIDERS;

	public static bool DEBUG_VIDEOADS_PROVIDERS_ADCOLONY;

	public static bool DEBUG_VIDEOADS_PROVIDERS_FLURRY;

	public static bool DEBUG_VIDEOADS_PROVIDERS_VUNGLE;

	public static bool DEBUG_VIDEOADS_PROVIDERS_TABJOY;

	public static bool DEBUG_VIDEOADS_PROVIDERS_INMOBI;

	public static bool DEBUG_VIDEOADS_PROVIDERS_CHARTBOOST;

	public static bool DEBUG_USE_CELEBRATION_TESTER;

	public static bool DEBUG_USE_EVENT_TESTER;

	public static bool DEBUG_SHOW_PERFORMANCE_INFO;

	public static bool DEBUG_USE_TIMESCALE_PAUSE_BUTTON;

	public static bool DEBUG_SHADERS;

	public static bool DEBUG_CHARACTER_INVINCIBLE;

	public static bool DEBUG_CHARACTER_MOVE_THROUGH_COLLIDERS;

	public static bool DEBUG_LIMITED_CHARACTER_POPUP;

	public static bool DEBUG_LIMITED_CHARACTER_POPUP_IGNORE_EXPIRE;

	public static bool DEBUG_LIMITED_BOARD_POPUP;

	public static bool DEBUG_LIMITED_BOARD_POPUP_IGNORE_EXPIRE;

	public static readonly bool DEBUG_LOG_COINS;

	public static readonly bool DEBUG_OVERWRITE_PICKUP_SYSTEM_VERSION;

	public static readonly bool DEBUG_USE_ENABLE_SS_EVENT_TYPE___SUPER_MYSTERY_BOX;

	public static readonly bool DEBUG_USE_ENABLE_SS_EVENT_TYPE___MYSTERY_POWER_UP;

	public static readonly bool DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER;

	public static bool DEBUG_SKIP_DAILY_LETTERS;

	public static readonly bool DEBUG_USE_ENABLE_PLAYER_INFO___IS_ACTIVE_DAILY_LETTER_EVENT;

	public static readonly bool DEBUG_USE_ENABLE_PLAYER_INFO___HAS_DAILY_WORD_TIME_LEFT;

	public static bool DEBUG_USE_DEBUG_DAILYWORD;

	public static readonly bool DEBUG_IS_HOVERBOARD_ALWAYS_REWARDED_FOR_MYSERY_POWERUP;

	public static string ONLINESETTINGS_SET;

	public static Characters.CharacterType DEBUG_LIMITED_CHARACTER_POPUP_TYPE;

	public static Hoverboards.BoardType DEBUG_LIMITED_BOARD_POPUP_TYPE;

	private static List<AddedAnimationEventInfo> addedAnimEvents;

	public static string RatingURL
	{
		get
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return "itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=512939461";
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				if (DeviceUtility.isKindleDevice)
				{
					return "http://www.amazon.com/review/create-review/ref=cm_cr_dp_wrt_btm?ie=UTF8&asin=B009UX2YAC";
				}
				if (DeviceUtility.isFlexionDevice)
				{
					return string.Empty;
				}
				return "market://details?id=com.kiloo.subwaysurf";
			}
			return string.Empty;
		}
	}

	public static string AppStoreURL
	{
		get
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return "http://itunes.apple.com/app/subway-surfers/id512939461";
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				if (DeviceUtility.isKindleDevice)
				{
					return "http://www.amazon.com/gp/mas/dl/android?p=com.kiloo.subwaysurf";
				}
				if (DeviceUtility.isFlexionDevice)
				{
					return "http://subwaysurfers.kiloo.flexionmobile.com";
				}
				return "https://play.google.com/store/apps/details?id=com.kiloo.subwaysurf";
			}
			if (Application.platform == RuntimePlatform.WP8Player)
			{
				return "http://windowsphone.com/s?appId=bedff828-2697-4cc5-8661-1206d15e3e0c";
			}
			return "test";
		}
	}

	static Globals()
	{
		bool dEBUG_POPUP;
		int num;
		if (DEBUG)
		{
			dEBUG_POPUP = DEBUG_POPUP;
		}
		else
			num = 0;
		DEBUG_POPUP_TESTOLOGY = false;
		DEBUG_FREE_PURCHASES = false;
		DEBUG_ALL_CHARS = false;
		DEBUG_ALL_BOARDS = false;
		DEBUG_FREE_INAPP_PURCHASE = false;
		DEBUG_SKIP_VIDEO_ADS = false;
		DEBUG_SKIP_TUTORIAL_IN_EDITOR = false;
		DEBUG_USE_DEBUG_LOGIN_STATE = false;
		DEBUG_USE_FAKE_ONLINE_SETTINGS = true;
		DEBUG_OVERRIDE_ONLINE_SETTINGS_URL = false;
		DEBUG_USE_ONLINESETTINGS_TESTTIME = false;
		DEBUG_USE_ONLINESETTINGS_SET = false;
		DEBUG_USE_TEST_AD_CONSTANTS = false;
		DEBUG_PRINT_SPAWNED_POWER_UPS = false;
		DEBUG_SHOW_FRONT_SCREEN_IF_NEW_USER = false;
		DEBUG_INTERSTITIAL_PROVIDERS = false;
		bool dEBUG_INTERSTITIAL_PROVIDER;
		int num2;
		if (DEBUG)
		{
			dEBUG_INTERSTITIAL_PROVIDER = DEBUG_INTERSTITIAL_PROVIDERS;
		}
		else
			num2 = 0;
		DEBUG_INTERSTITIAL_PROVIDER_CHARTBOOST = false;
		bool dEBUG_INTERSTITIAL_PROVIDER2;
		int num3;
		if (DEBUG)
		{
			dEBUG_INTERSTITIAL_PROVIDER2 = DEBUG_INTERSTITIAL_PROVIDERS;
		}
		else
			num3 = 0;
		DEBUG_INTERSTITIAL_PROVIDER_INMOBI = false;
		bool dEBUG_INTERSTITIAL_PROVIDER3;
		int num4;
		if (DEBUG)
		{
			dEBUG_INTERSTITIAL_PROVIDER3 = DEBUG_INTERSTITIAL_PROVIDERS;
		}
		else
			num4 = 0;
		DEBUG_INTERSTITIAL_PROVIDER_MDOTM = false;
		DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT = false;
		DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IN_APP_PURCHASES = false;
		DEBUG_VIDEOADS_PROVIDERS = false;
		bool dEBUG_VIDEOADS_PROVIDER;
		int num5;
		if (DEBUG)
		{
			dEBUG_VIDEOADS_PROVIDER = DEBUG_VIDEOADS_PROVIDERS;
		}
		else
			num5 = 0;
		DEBUG_VIDEOADS_PROVIDERS_ADCOLONY = false;
		bool dEBUG_VIDEOADS_PROVIDER2;
		int num6;
		if (DEBUG)
		{
			dEBUG_VIDEOADS_PROVIDER2 = DEBUG_VIDEOADS_PROVIDERS;
		}
		else
			num6 = 0;
		DEBUG_VIDEOADS_PROVIDERS_FLURRY = false;
		bool dEBUG_VIDEOADS_PROVIDER3;
		int num7;
		if (DEBUG)
		{
			dEBUG_VIDEOADS_PROVIDER3 = DEBUG_VIDEOADS_PROVIDERS;
		}
		else
			num7 = 0;
		DEBUG_VIDEOADS_PROVIDERS_VUNGLE = false;
		bool dEBUG_VIDEOADS_PROVIDER4;
		int num8;
		if (DEBUG)
		{
			dEBUG_VIDEOADS_PROVIDER4 = DEBUG_VIDEOADS_PROVIDERS;
		}
		else
			num8 = 0;
		DEBUG_VIDEOADS_PROVIDERS_TABJOY = false;
		bool dEBUG_VIDEOADS_PROVIDER5;
		int num9;
		if (DEBUG)
		{
			dEBUG_VIDEOADS_PROVIDER5 = DEBUG_VIDEOADS_PROVIDERS;
		}
		else
			num9 = 0;
		DEBUG_VIDEOADS_PROVIDERS_INMOBI = false;
		bool dEBUG_VIDEOADS_PROVIDER6;
		int num10;
		if (DEBUG)
		{
			dEBUG_VIDEOADS_PROVIDER6 = DEBUG_VIDEOADS_PROVIDERS;
		}
		else
			num10 = 0;
		DEBUG_VIDEOADS_PROVIDERS_CHARTBOOST = false;
		DEBUG_USE_CELEBRATION_TESTER = false;
		DEBUG_USE_EVENT_TESTER = false;
		DEBUG_SHOW_PERFORMANCE_INFO = false;
		DEBUG_USE_TIMESCALE_PAUSE_BUTTON = false;
		DEBUG_SHADERS = false;
		DEBUG_CHARACTER_INVINCIBLE = false;
		DEBUG_CHARACTER_MOVE_THROUGH_COLLIDERS = false;
		DEBUG_LIMITED_CHARACTER_POPUP = false;
		DEBUG_LIMITED_CHARACTER_POPUP_IGNORE_EXPIRE = false;
		DEBUG_LIMITED_BOARD_POPUP = false;
		DEBUG_LIMITED_BOARD_POPUP_IGNORE_EXPIRE = false;
		DEBUG_LOG_COINS = false;
		DEBUG_OVERWRITE_PICKUP_SYSTEM_VERSION = false;
		DEBUG_USE_ENABLE_SS_EVENT_TYPE___SUPER_MYSTERY_BOX = false;
		DEBUG_USE_ENABLE_SS_EVENT_TYPE___MYSTERY_POWER_UP = false;
		DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER = false;
		DEBUG_SKIP_DAILY_LETTERS = false;
		DEBUG_USE_ENABLE_PLAYER_INFO___IS_ACTIVE_DAILY_LETTER_EVENT = false;
		DEBUG_USE_ENABLE_PLAYER_INFO___HAS_DAILY_WORD_TIME_LEFT = false;
		DEBUG_USE_DEBUG_DAILYWORD = false;
		DEBUG_IS_HOVERBOARD_ALWAYS_REWARDED_FOR_MYSERY_POWERUP = false;
		ONLINESETTINGS_SET = "qa";
		DEBUG_LIMITED_CHARACTER_POPUP_TYPE = Characters.CharacterType.slick;
		DEBUG_LIMITED_BOARD_POPUP_TYPE = Hoverboards.BoardType.normal;
		addedAnimEvents = new List<AddedAnimationEventInfo>();
	}

	public static Friend[] GetDebugFriends(int numberOfFriends = 10)
	{
		return new Friend[numberOfFriends];
	}

	public static LogInState GetLoginState()
	{
		LogInState logInState = LogInState.Offline;
		if (DEBUG_USE_DEBUG_LOGIN_STATE)
		{
			logInState = LogInState.Facebook;
		}
		else
		{
			if (SocialManager.instance.gamecenterIsLoggedIn)
			{
				logInState = LogInState.GameCenter;
			}
			if (SocialManager.instance.facebookIsLoggedIn)
			{
				logInState = ((logInState != LogInState.GameCenter) ? LogInState.Facebook : LogInState.Both);
			}
		}
		return logInState;
	}

	public static Dictionary<T, int> convertStringToEnumIntDictionary<T>(string sourceString)
	{
		string[] array = sourceString.Split(',');
		Dictionary<T, int> dictionary = new Dictionary<T, int>(array.Length / 2);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string value = array[i];
			string s = array[i + 1];
			if (Enum.IsDefined(typeFromHandle, value))
			{
				T key = (T)Enum.Parse(typeFromHandle, value, true);
				int result;
				if (int.TryParse(s, out result))
				{
					dictionary[key] = result;
				}
				else
				{
					LogError("Source string could not parse int");
				}
			}
		}
		return dictionary;
	}

	public static string convertEnumIntDictionaryToString<T>(Dictionary<T, int> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, int> item in sourceDict)
		{
			string name = Enum.GetName(typeof(T), item.Key);
			string arg = item.Value.ToString();
			stringBuilder.AppendFormat("{0},{1},", name, arg);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<T, bool> convertStringToEnumBoolDictionary<T>(string sourceString)
	{
		string[] array = sourceString.Split(',');
		Dictionary<T, bool> dictionary = new Dictionary<T, bool>(array.Length / 2);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string value = array[i];
			string value2 = array[i + 1];
			if (Enum.IsDefined(typeFromHandle, value))
			{
				T key = (T)Enum.Parse(typeFromHandle, value, true);
				bool result;
				if (bool.TryParse(value2, out result))
				{
					dictionary[key] = result;
				}
				else
				{
					LogError("Source string could not parse bool");
				}
			}
		}
		return dictionary;
	}

	public static string convertEnumBoolDictionaryToString<T>(Dictionary<T, bool> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, bool> item in sourceDict)
		{
			string name = Enum.GetName(typeof(T), item.Key);
			string arg = item.Value.ToString();
			stringBuilder.AppendFormat("{0},{1},", name, arg);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<string, string> convertStringToStringStringDictionary(string sourceString)
	{
		string[] array = sourceString.Split(',');
		Dictionary<string, string> dictionary = new Dictionary<string, string>(array.Length / 2);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string key = array[i];
			string value = array[i + 1];
			dictionary[key] = value;
		}
		return dictionary;
	}

	public static string convertStringStringDictionaryToString(Dictionary<string, string> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> item in sourceDict)
		{
			string key = item.Key;
			string value = item.Value;
			stringBuilder.AppendFormat("{0},{1},", key, value);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<T, int[]> convertStringToEnumIntArrayDictionary<T>(string sourceString)
	{
		string[] array = sourceString.Split(';');
		Dictionary<T, int[]> dictionary = new Dictionary<T, int[]>(array.Length);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length; i++)
		{
			if (string.IsNullOrEmpty(array[i]))
			{
				continue;
			}
			string[] array2 = array[i].Split('-');
			if (array2.Length != 2)
			{
				continue;
			}
			string value = array2[0];
			string text = array2[1];
			if (!Enum.IsDefined(typeFromHandle, value))
			{
				continue;
			}
			T key = (T)Enum.Parse(typeFromHandle, value, true);
			string[] array3 = text.Split(',');
			int[] array4 = new int[array3.Length];
			for (int j = 0; j < array3.Length; j++)
			{
				int result;
				if (int.TryParse(array3[j], out result))
				{
					array4[j] = result;
				}
				else
				{
					LogError("Source string could not parse int");
				}
			}
			dictionary[key] = array4;
		}
		return dictionary;
	}

	public static string convertEnumIntArrayDictionaryToString<T>(Dictionary<T, int[]> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, int[]> item in sourceDict)
		{
			string name = Enum.GetName(typeof(T), item.Key);
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < item.Value.Length; i++)
			{
				stringBuilder2.AppendFormat("{0},", item.Value[i]);
			}
			stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
			stringBuilder.AppendFormat("{0}-{1};", name, stringBuilder2.ToString());
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static int[] convertAllStringToInt(string[] array)
	{
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = int.Parse(array[i]);
		}
		return array2;
	}

	public static bool[] convertAllStringToBool(string[] array)
	{
		bool[] array2 = new bool[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = bool.Parse(array[i]);
		}
		return array2;
	}

	public static string[] convertAllIntToString(int[] array)
	{
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].ToString();
		}
		return array2;
	}

	public static string[] convertAllBoolToString(bool[] array)
	{
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].ToString();
		}
		return array2;
	}

	public static bool TryAddAnimationEvent(Animation animation, string clipName, AnimationEvent aniEvent)
	{
		foreach (AddedAnimationEventInfo addedAnimEvent in addedAnimEvents)
		{
			if (addedAnimEvent.animation == animation && addedAnimEvent.clipName == clipName && addedAnimEvent.time == aniEvent.time && addedAnimEvent.functionName == aniEvent.functionName)
			{
				return false;
			}
		}
		addedAnimEvents.Add(new AddedAnimationEventInfo(animation, clipName, aniEvent.time, aniEvent.functionName));
		animation[clipName].clip.AddEvent(aniEvent);
		return true;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError("GlobalDebug: " + msg, context);
	}
}
