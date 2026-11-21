#define ENABLE_ERROR_LOGS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using JetBrains.Annotations;
using Kiloo.HTTP;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Subway.Info;
using TopRun;
using UnityEngine;
using Debug = UnityEngine.Debug;
using com.kiloo.awards;

public class HouseKeeper : MonoBehaviour
{
	public delegate void OnLateUpdate();

	private const string KEY_FIRST_TIME_RUN = "SetShouldShowPopupToFalse";

	private const string APPLICATION_AUTHENTICITY_KEY = "applicationAuthenticity";

	private const string SOUND_BEFORE_APPLICATION_LOSTFOCUS = "soundBeforeLostFocus";

	private const string ANDROID_IN_APP_BILLING_GAMEOBJECT_NAME = "InAppBillingAndroidGameObject";

	private const float MIN_REFRESH_INTERVAL = 3600f;

	private const string REFRESH_INTERVAL_KEY = "refreshinterval";

	private int[] _dailyKeysBrackets = new int[15]
	{
		100, 120, 140, 160, 180, 200, 220, 240, 260, 280,
		300, 400, 1000, 1500, 2000
	};

	private int[] _consumableHoverboardBrackets = new int[15]
	{
		100, 120, 140, 160, 180, 200, 220, 240, 260, 280,
		300, 400, 1000, 1500, 2000
	};

	private int[] _headstartBrackets = new int[15]
	{
		100, 120, 140, 160, 180, 200, 220, 240, 260, 280,
		300, 400, 1000, 1500, 2000
	};

	private int[] _scoreBoosterBrackets = new int[15]
	{
		100, 120, 140, 160, 180, 200, 220, 240, 260, 280,
		300, 400, 1000, 1500, 2000
	};

	private static Action<bool> delegateFacebookPayoutPopupAction = QueueFacebookPayoutPopupIfNeeded;

	private static float _onlineSettingsLastDownloadTime = float.NegativeInfinity;

	public static event OnLateUpdate onLateUpdate;

	[UsedImplicitly]
	private void Awake()
	{
		DeviceUtility.Init();
		Application.targetFrameRate = 60;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Strings.Language = "english";
		Flurry.StartSession(AdConstants.instance.FlurryApiKey, PlayerInfo.Instance.flurryUserIdHash);
		InitHttp();
		InitTopRun();
		RefreshOnlineSettingsAndInappsIfNeeded();
		SSEventsManagerController.instance.Initialize();
		HookUpListenersToOnlineSettings();
		AudioListener.volume = 0f;
		TryRestoreSoundOptionAfterLostFocusIfNeeded();
		IncomingCallManager.Init();
		SocialManager.Init();
		SocialManager.instance.AddFriendsConsolidatedHandler(OnFriendsConsolidated);
		SocialManager.instance.onRegisterUserCompleted += OnRegisterUserCompleted;
		if (SocialManager.instance.consolidatedFriendsCompleted)
		{
			OnFriendsConsolidated(true);
		}
		TopRunNotificationSlideInManager.Init();
		InAppManager.Init();
		Layers.Init();
		ScreenAdsManager.instance.InitProviderListAndCache();
		CancellAllLocalNotifications();
		PlayerInfo.Instance.ConvertHeadstartsToMegaHeadstarts();
		if (!PlayerPrefs.HasKey("SetShouldShowPopupToFalse"))
		{
			PlayerPrefs.SetString("SetShouldShowPopupToFalse", "true");
			PlayerInfo.Instance.shouldShowCollectPopup = false;
			PlayerInfo.Instance.shouldShowDuelsBreadcrumb = false;
			PlayerInfo.Instance.lastTimeAppWasUpdatedOrInstalled = DateTime.Now;
			PlayerInfo.Instance.firstInstallDate = DateTime.Now;
		}
		PostFlurryEventsOnceADay();
		FixUnlockCherryBoardIfNeeded();
		LogApplicationAuthenticity();
		AwardsManager.Instance.InitTrophies();
		OfferWallManager.Instance.Initialize();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnKnownServerTimeChange = (Action)Delegate.Combine(instance.OnKnownServerTimeChange, new Action(FacebookWeeklyGiftManager.Instance.CheckAndUpdateNextWeeklyGiftDate));
		AddFpsCounter();
	}

	[UsedImplicitly]
	private void LateUpdate()
	{
		if (HouseKeeper.onLateUpdate != null)
		{
			HouseKeeper.onLateUpdate();
		}
	}

	private void AddFpsCounter()
	{
		GameObject gameObject = new GameObject("FpsCounter");
		gameObject.name = "FpsCounter";
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		InRunAverageFPSCounter inRunAverageFPSCounter = gameObject.AddComponent<InRunAverageFPSCounter>();
		inRunAverageFPSCounter.Initialize();
	}

	private void LogApplicationAuthenticity()
	{
		if (!PlayerPrefs.HasKey("applicationAuthenticity"))
		{
			string argValue = "N/A";
			if (Application.genuineCheckAvailable)
			{
				argValue = ((!Application.genuine) ? "App is not authentic" : "App is authentic");
			}
			Flurry.LogEventWithAParameter("Application Authenticity", "Type", argValue);
			PlayerPrefs.SetString("applicationAuthenticity", "set");
		}
	}

	private void HookUpListenersToOnlineSettings()
	{
		OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsSetupFinished);
		OnlineSettings instance = OnlineSettings.instance;
		instance.onOnlineSettingsDownloadResult = (Action<bool>)Delegate.Combine(instance.onOnlineSettingsDownloadResult, new Action<bool>(OnOnlineSettingsDownloadedWithResult));
	}

	private void OnOnlineSettingsSetupFinished()
	{
		InitializeVideoAdsSystem();
	}

	private void OnOnlineSettingsDownloadedWithResult(bool result)
	{
		if (!IsDateToday(PlayerInfo.Instance.lastDailyOnlineLog))
		{
			Flurry.LogEventWithAParameter("Daily Check of Online Status", (!result) ? "Offline Country" : "Online Country", Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale()));
			PlayerInfo.Instance.lastDailyOnlineLog = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo);
		}
	}

	private void InitializeVideoAdsSystem()
	{
		VideoAdsManager.instance.Init();
		VideoAdsManager.instance.StartCachingVideo();
	}

	private void FixUnlockCherryBoardIfNeeded()
	{
		if (PlayerPrefs.HasKey("FixUnlockCherryBoardIfNeeded"))
		{
			return;
		}
		PlayerPrefs.SetInt("FixUnlockCherryBoardIfNeeded", 1);
		PlayerInfo instance = PlayerInfo.Instance;
		string weeklyChallangeData = instance.weeklyChallangeData;
		string[] array = weeklyChallangeData.Split(',');
		string text = null;
		if (array.Length >= 5)
		{
			text = array[4];
		}
		if (text != null)
		{
			int result;
			int.TryParse(text, out result);
			if (result == 60 && !instance.isHoverboardUnlocked(Hoverboards.BoardType.cherry))
			{
				instance.UnlockHoverboard(Hoverboards.BoardType.cherry);
			}
		}
	}

	private void InitHttp()
	{
		WebClient.timeout = 10f;
		WebClient.ClearCache(WebClient.ClearCacheFlags.Expired | WebClient.ClearCacheFlags.Old, 2419200f);
		KilooWebClient.InitAutomatically("subway_surfers");
	}

	private void InitTopRun()
	{
		IManager manager = TopRunRoot.manager;
		if (manager == null)
		{
			string defaultCountryCodeFromDevice = CountryManager.instance.GetDefaultCountryCodeFromDevice();
			TopRunRoot.Init(defaultCountryCodeFromDevice, PlayerInfo.Instance.rawMultiplier, PlayerInfo.Instance.highestScore);
		}
	}

	private void RefreshTopRun()
	{
		IManager manager = TopRunRoot.manager;
		if (manager == null)
		{
			LogError("TopRun Manager instance is null!");
		}
		else
		{
			manager.RefreshTournament();
		}
	}

	private void RegisterAllBundlesDownloaded()
	{
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
		{
			if (!IsDateToday(PlayerInfo.Instance.lastQuitDate))
			{
				string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
				bool allAssetBundlesDownloaded = PlayerInfo.Instance.AllAssetBundlesDownloaded;
				Flurry.LogEventWithAParameter("All Asset Bundle Downloads completed", "All Bundles Downloaded?;" + string.Format("All Bundles Downloaded {0} in Country", allAssetBundlesDownloaded), allAssetBundlesDownloaded + ";" + contryCodeFromLocale);
			}
			PlayerInfo.Instance.lastQuitDate = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo);
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	private void RegisterLocalNotificationStatus()
	{
	}

	private void RegisterReminderStatus()
	{
		Flurry.LogEventWithAParameter("State of reminders", "Enabled", Settings.optionReminder.ToString());
	}

	private void RegisterDailyActiveCharacter()
	{
		string argValue = ((Characters.CharacterType)PlayerInfo.Instance.currentCharacter).ToString() + "_Theme" + PlayerInfo.Instance.currentThemeIndex;
		Flurry.LogEventWithAParameter("Currently active character", "Id", argValue);
	}

	private void RegisterDailyActiveBoard()
	{
		Hoverboards.BoardType currentHoverboard = PlayerInfo.Instance.currentHoverboard;
		string text = PlayerInfo.Instance.currentHoverboard.ToString();
		if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(currentHoverboard))
		{
			foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[currentHoverboard])
			{
				if (PlayerInfo.Instance.HoverboardCustomizationData.IsThemeActive(currentHoverboard, item.identifier))
				{
					text = text + "_" + item.identifier;
				}
			}
		}
		Flurry.LogEventWithAParameter("Currently active board", "Id", text);
	}

	private void RegisterDailyCoinsAmount()
	{
		int amountOfCoins = PlayerInfo.Instance.amountOfCoins;
		string argValue = ((amountOfCoins <= 10000) ? RoundUpToTargetValue(amountOfCoins, 1000) : ((amountOfCoins <= 100000) ? RoundUpToTargetValue(amountOfCoins, 10000) : ((amountOfCoins <= 1000000) ? RoundUpToTargetValue(amountOfCoins, 100000) : ((amountOfCoins > 3000000) ? (3000000 + "+") : RoundUpToTargetValue(amountOfCoins, 500000)))));
		Flurry.LogEventWithAParameter("Daily report of coins", "Amount", argValue);
	}

	private void RegisterDailyKeysAmount()
	{
		int amountOfKeys = PlayerInfo.Instance.amountOfKeys;
		string bracketForValue = GetBracketForValue(amountOfKeys, _dailyKeysBrackets);
		Flurry.LogEventWithAParameter("Daily keys report", "Amount", bracketForValue);
	}

	private void RegisterFacebookStatus()
	{
		string argValue = ((!SocialManager.instance.facebookIsLoggedIn) ? "offline" : "online");
		Flurry.LogEventWithAParameter("Facebook status", "Status", argValue);
	}

	private string RoundUpToTargetValue(int total, int factor)
	{
		string result = string.Empty;
		for (int i = 1; i <= 10; i++)
		{
			if (total <= i * factor)
			{
				int num = i * factor;
				int num2 = num - factor;
				result = num2 + " - " + num;
				break;
			}
		}
		return result;
	}

	private void RegisterDevicePerfomanceLevel()
	{
		string argValue = ((DeviceInfo.Instance.performanceLevel != DeviceInfo.PerformanceLevel.High) ? "Low" : "High");
		Flurry.LogEventWithAParameter("Device performance", "Level", argValue);
	}

	private void RegisterDeviceMemory()
	{
		if (Application.platform == RuntimePlatform.WP8Player || Application.platform == RuntimePlatform.WindowsPlayer)
		{
			Flurry.LogEventWithAParameter("Device Memory", "Amount", SystemInfo.systemMemorySize.ToString());
		}
	}

	private void RegisterOwnedCharactersAndOutfits()
	{
		int num = 0;
		int regularCharactersOwned = 0;
		int limitedCharactersOwned = 0;
		int tokenCharactersOwned = 0;
		int facebookCharactersOwned = 0;
		int outfitsOwned = 0;
		foreach (Characters.CharacterType item in Characters.characterOrder)
		{
			IncrementTypeOfOwnedCharacter(item, ref regularCharactersOwned, ref limitedCharactersOwned, ref tokenCharactersOwned, ref facebookCharactersOwned);
			IncrementOwnedOutfits(item, ref outfitsOwned);
		}
		num = regularCharactersOwned + limitedCharactersOwned + tokenCharactersOwned + facebookCharactersOwned;
		Flurry.LogEventWithSeveralParameters("Characters owned", "total;regular;limited;token;facebook", num + ";" + regularCharactersOwned + ";" + limitedCharactersOwned + ";" + tokenCharactersOwned + ";" + facebookCharactersOwned);
		Flurry.LogEventWithAParameter("Character outfits owned", "Amount", outfitsOwned.ToString());
	}

	private void IncrementTypeOfOwnedCharacter(Characters.CharacterType character, ref int regularCharactersOwned, ref int limitedCharactersOwned, ref int tokenCharactersOwned, ref int facebookCharactersOwned)
	{
		if (!Characters.characterData.ContainsKey(character) || !PlayerInfo.Instance.IsCollectionComplete(character))
		{
			return;
		}
		Characters.Model model = Characters.characterData[character];
		if (model.hasOnlineSettings)
		{
			limitedCharactersOwned++;
			return;
		}
		switch (model.unlockType)
		{
		case Characters.UnlockType.facebook:
			if (SocialManager.instance.facebookIsLoggedIn)
			{
				facebookCharactersOwned++;
			}
			break;
		case Characters.UnlockType.tokens:
			tokenCharactersOwned++;
			break;
		default:
			regularCharactersOwned++;
			break;
		}
	}

	private void IncrementOwnedOutfits(Characters.CharacterType character, ref int outfitsOwned)
	{
		List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(character);
		if (list == null || list.Count <= 0)
		{
			return;
		}
		int num = 1;
		for (int i = 0; i < list.Count; i++)
		{
			if (PlayerInfo.Instance.IsThemeUnlockedForCharacter(character, num))
			{
				outfitsOwned++;
			}
			num++;
		}
	}

	public void RegisterOwnedHoverboardsAndAbilities()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			if (PlayerInfo.Instance.isHoverboardUnlocked(item) && Hoverboards.boardData.ContainsKey(item))
			{
				if (Hoverboards.boardData[item].hasOnlineTimeLimit)
				{
					num3++;
				}
				else
				{
					num2++;
				}
			}
		}
		foreach (KeyValuePair<Hoverboards.BoardType, List<HoverboardThemes.HoverboardTheme>> hoverboardCustomTheme in HoverboardThemes.hoverboardCustomThemes)
		{
			Hoverboards.BoardType key = hoverboardCustomTheme.Key;
			foreach (HoverboardThemes.HoverboardTheme item2 in HoverboardThemes.hoverboardCustomThemes[key])
			{
				if (PlayerInfo.Instance.HoverboardCustomizationData.IsThemeOwned(key, item2.identifier))
				{
					num4++;
				}
			}
		}
		num = num2 + num3;
		Flurry.LogEventWithSeveralParameters("Hoverboards owned", "total;regular;limited", num + ";" + num2 + ";" + num3);
		Flurry.LogEventWithAParameter("Hoverboard abilities owned", "Amount", num4.ToString());
	}

	private void RegisterOwnedConsumableHoverboards()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.hoverboard);
		string bracketForValue = GetBracketForValue(upgradeAmount, _consumableHoverboardBrackets);
		Flurry.LogEventWithAParameter("Daily report of consumable hoverboards", "Amount", bracketForValue);
	}

	private void RegisterOwnedHeadstarts()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.headstart2000);
		string bracketForValue = GetBracketForValue(upgradeAmount, _headstartBrackets);
		Flurry.LogEventWithAParameter("Daily report of headstarts owned", "Amount", bracketForValue);
	}

	private void RegisterOwnedScoreBoosters()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.scorebooster);
		string bracketForValue = GetBracketForValue(upgradeAmount, _scoreBoosterBrackets);
		Flurry.LogEventWithAParameter("Daily report of score boosters owned", "Amount", bracketForValue);
	}

	private void RegisterTotalAmountOfFinishedGoldAwards()
	{
		Flurry.LogEventWithAParameter("Total amount of gold awards finished", "Completed Gold Awards", AwardsManager.Instance.GetNumberOfFinishedAwards(AwardTierType.Gold).ToString());
	}

	private void RegisterTotalAmountOfFinishedSilverAwards()
	{
		Flurry.LogEventWithAParameter("Total amount of silver awards finished", "Completed Silver Awards", AwardsManager.Instance.GetNumberOfFinishedAwards(AwardTierType.Silver).ToString());
	}

	private void RegisterBundleIdentifier()
	{
		Flurry.LogEventWithAParameter("The current bundle identifier", "Daily bundle identifier", DeviceUtility.GetBundleIdentifier());
	}

	private string GetBracketForValue(int value, int[] bracket)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (value <= bracket[0])
		{
			return value.ToString();
		}
		for (int i = 1; i < bracket.Length; i++)
		{
			if (value <= bracket[i])
			{
				text = (bracket[i - 1] + 1).ToString();
				text2 = bracket[i].ToString();
				break;
			}
		}
		if (string.IsNullOrEmpty(text2))
		{
			int num = bracket.Length;
			string text3 = bracket[num - 1].ToString();
			return text3 + "+";
		}
		return text + "-" + text2;
	}

	private void PostFlurryEventsOnceADay()
	{
		if (!IsDateToday(PlayerInfo.Instance.lastPlayDate))
		{
			RegisterLocalNotificationStatus();
			RegisterReminderStatus();
			RegisterDailyCoinsAmount();
			RegisterDailyKeysAmount();
			RegisterDailyActiveCharacter();
			RegisterDailyActiveBoard();
			RegisterFacebookStatus();
			RegisterDevicePerfomanceLevel();
			RegisterDeviceMemory();
			RegisterOwnedCharactersAndOutfits();
			RegisterOwnedHoverboardsAndAbilities();
			RegisterOwnedConsumableHoverboards();
			RegisterOwnedHeadstarts();
			RegisterOwnedScoreBoosters();
			RegisterTotalAmountOfFinishedGoldAwards();
			RegisterTotalAmountOfFinishedSilverAwards();
			int numberOfDaysSinceLastPlay = getNumberOfDaysSinceLastPlay();
			if (numberOfDaysSinceLastPlay >= 1)
			{
				Flurry.LogEventWithAParameter("Days passed since last play", "Result", numberOfDaysSinceLastPlay.ToString(NumberFormatInfo.InvariantInfo));
			}
			PlayerInfo.Instance.lastPlayDate = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo);
		}
	}

	[UsedImplicitly]
	private void OnApplicationPause(bool pause)
	{
		VideoAdsManager.instance.OnApplicationPause(pause);
		if (pause)
		{
			Flurry.EndSession();
			PostFlurryEventsOnceADay();
			ScheduleLocalNotificationsIfNecessarry();
			RegisterAllBundlesDownloaded();
			return;
		}
		Flurry.StartSession(AdConstants.instance.FlurryApiKey, PlayerInfo.Instance.flurryUserIdHash);
		if (!ScreenAdsManager.instance.isPresentingInterstitial && !VideoAdsManager.instance.isVideoPlaying)
		{
			if (OfferWallManager.Instance.AreOffersAllowedOnDevice())
			{
				OfferWallManager.Instance.GetPendingRewards(true);
			}
			TryRestoreSoundOptionAfterLostFocusIfNeeded();
		}
		CancellAllLocalNotifications();
		if (DailyWord.Instance != null)
		{
			DailyWord.Instance.DownloadDaily();
		}
		RefreshOnlineSettingsAndInappsIfNeeded();
	}

	[UsedImplicitly]
	private void OnApplicationQuit()
	{
		ScheduleLocalNotificationsIfNecessarry();
		RegisterAllBundlesDownloaded();
	}

	private int getNumberOfDaysSinceLastPlay()
	{
		int result = 0;
		DateTime dateTime = Utils.StringToDateTime(PlayerInfo.Instance.lastPlayDate, DateTime.Now);
		DateTime now = DateTime.Now;
		if (now.Date > dateTime.Date)
		{
			result = (int)(now - dateTime).TotalDays;
		}
		return result;
	}

	private bool IsDateToday(string previousDate)
	{
		return Utils.StringToDateTime(previousDate, DateTime.MinValue).Date == DateTime.Now.Date;
	}

	private void CancellAllLocalNotifications()
	{
		LocalNotificationsManager.Instance.NotificationController.CancelAllNotifications();
	}

	private void ScheduleLocalNotificationsIfNecessarry()
	{
		bool flag = false;
		string valueString;
		if (OnlineSettings.instance.TryGetValue("enable_local_notifications", out valueString))
		{
			try
			{
				flag = !bool.Parse(valueString);
			}
			catch (FormatException)
			{
				LogWarning(string.Format("Unable to convert '{0}'.", valueString));
				LogWarning("Reading from online failed ");
			}
		}
		TimeSpan dailyWordTimeLeft = PlayerInfo.Instance.dailyWordTimeLeft;
		if (Settings.optionReminder && !flag && dailyWordTimeLeft > TimeSpan.Zero)
		{
			DateTime dateTime = DateTime.Today.AddHours(17.0);
			string title = Strings.Get(StringID.LOCAL_NOTIFICATION_TITLE);
			string localNotificationMessage = GetLocalNotificationMessage();
			TimeSpan timeSpan = dateTime - DateTime.Now;
			bool mostRecentIsToday;
			int dailyWordDaysInARow = PlayerInfo.Instance.GetDailyWordDaysInARow(out mostRecentIsToday);
			if (dailyWordDaysInARow > 0)
			{
				int hour = DateTime.Now.Add(dailyWordTimeLeft).AddMinutes(30.0).Hour;
				if (hour == 17)
				{
					if (mostRecentIsToday)
					{
						if (timeSpan > TimeSpan.Zero)
						{
							ScheduleNotification(dateTime.AddHours(1.0), title, localNotificationMessage);
						}
						else if (timeSpan <= TimeSpan.Zero)
						{
							ScheduleNotification(dateTime.AddDays(1.0).AddHours(1.0), title, localNotificationMessage);
						}
					}
					else if (timeSpan < TimeSpan.Zero && timeSpan + TimeSpan.FromHours(1.0) > TimeSpan.Zero)
					{
						ScheduleNotification(dateTime.AddHours(1.0), title, localNotificationMessage);
					}
				}
				else if (mostRecentIsToday)
				{
					if (dailyWordTimeLeft < timeSpan)
					{
						ScheduleNotification(dateTime, title, localNotificationMessage);
					}
					else if (dailyWordTimeLeft > timeSpan)
					{
						ScheduleNotification(dateTime.AddDays(1.0), title, localNotificationMessage);
					}
				}
				else if (dailyWordTimeLeft > timeSpan && timeSpan > TimeSpan.Zero)
				{
					ScheduleNotification(dateTime, title, localNotificationMessage);
				}
			}
		}
		if (Settings.optionReminder && !flag)
		{
			ScheduleDailyVideoNotificationIfNecessary();
			ScheduleTopRunNotificationIfNecessary();
		}
	}

	private void ScheduleTopRunNotificationIfNecessary()
	{
		IManager manager = TopRunRoot.manager;
		DateTime dateTime = DateTime.Today.AddHours(9.0);
		DateTime dateTime2 = DateTime.Today.AddHours(21.0);
		DateTime dateTime3 = DateTime.Today.AddDays(1.0).AddHours(9.0);
		DateTime dateTime4 = DateTime.Today.AddDays(1.0).AddHours(21.0);
		if (manager == null || manager.tournament == null)
		{
			return;
		}
		DateTime officialEndTimeUTC = manager.tournament.officialEndTimeUTC;
		DateTime dateTime5 = officialEndTimeUTC - new TimeSpan(0, 8, 0, 0);
		if (dateTime5.Date == DateTime.Today)
		{
			if (dateTime < dateTime5 && dateTime5 < dateTime2)
			{
				ScheduleNotification(dateTime5, Strings.Get(StringID.LOCAL_NOTIFICATION_TITLE), Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_TOPRUN_ENDING));
			}
			else if (dateTime5 < dateTime && dateTime < officialEndTimeUTC)
			{
				ScheduleNotification(dateTime, Strings.Get(StringID.LOCAL_NOTIFICATION_TITLE), Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_TOPRUN_ENDING));
			}
		}
		else if (dateTime5.Date == DateTime.Today.AddDays(1.0))
		{
			if (dateTime3 < dateTime5 && dateTime5 < dateTime4)
			{
				ScheduleNotification(dateTime5, Strings.Get(StringID.LOCAL_NOTIFICATION_TITLE), Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_TOPRUN_ENDING));
			}
			else if (dateTime5 < dateTime3 && dateTime3 < officialEndTimeUTC)
			{
				ScheduleNotification(dateTime3, Strings.Get(StringID.LOCAL_NOTIFICATION_TITLE), Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_TOPRUN_ENDING));
			}
		}
	}

	private void ScheduleDailyVideoNotificationIfNecessary()
	{
		DateTime lastTimeAVideoForKeysWasSeen = VideoAdsManager.instance.GetLastTimeAVideoForKeysWasSeen();
		DateTime dateTime = DateTime.Now.AddDays(-1.0);
		DateTime now = DateTime.Now;
		DateTime lastTimeAppWasUpdatedOrInstalled = PlayerInfo.Instance.lastTimeAppWasUpdatedOrInstalled;
		if (lastTimeAVideoForKeysWasSeen.Date == dateTime.Date || lastTimeAppWasUpdatedOrInstalled.Date == dateTime.Date)
		{
			ScheduleNotification(LocalNotificationsManager.Instance.GetNextNotificationTimeForVideos(), Strings.Get(StringID.LOCAL_NOTIFICATION_TITLE), Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_DAILY_VIDEO));
		}
		else if (lastTimeAVideoForKeysWasSeen.Date == now.Date || lastTimeAppWasUpdatedOrInstalled.Date == now.Date)
		{
			ScheduleNotification(LocalNotificationsManager.Instance.GetNextNotificationTimeForVideos().AddDays(1.0), Strings.Get(StringID.LOCAL_NOTIFICATION_TITLE), Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_DAILY_VIDEO));
		}
	}

	private void ScheduleNotification(DateTime time, string title, string message)
	{
		if (time.CompareTo(DateTime.Now) > 0)
		{
			ILocalNotificationController notificationController = LocalNotificationsManager.Instance.NotificationController;
			notificationController.ScheduleLocalNotification(time, title, message, "cpush.wav");
		}
	}

	private string GetLocalNotificationMessage()
	{
		DailyWord.DailyWordReward rewardForDay = DailyWord.GetRewardForDay(PlayerInfo.Instance.dailyWordInRow);
		if (rewardForDay.type == DailyWord.DailyWordRewardType.MysteryBox)
		{
			if (rewardForDay.mysteryBoxType == MysteryBox.Type.Super)
			{
				return Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_SMB);
			}
			return Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_MB);
		}
		int num = 0;
		if (rewardForDay.type == DailyWord.DailyWordRewardType.Coins)
		{
			num = rewardForDay.coins;
		}
		return string.Format(Strings.Get(StringID.LOCAL_NOTIFICATION_MESSAGE_COINS), num);
	}

	private void OnFriendsConsolidated(bool success)
	{
		SocialManager instance = SocialManager.instance;
		PlayerInfo instance2 = PlayerInfo.Instance;
		int num = 0;
		bool facebookIsLoggedIn = instance.facebookIsLoggedIn;
		if (facebookIsLoggedIn && !instance2.hasPayedOutFacebook)
		{
			num |= 1;
		}
		TriggerActionForFacebookPayout(facebookIsLoggedIn);
		if (instance.gamecenterIsLoggedIn && !instance2.hasPayedOutGameCenter)
		{
			instance2.amountOfCoins += 250;
			instance2.hasPayedOutGameCenter = true;
			UIScreenController.QueueGameCenterPayoutPopup();
		}
		PlayerInfo.Instance.SaveIfDirty();
		SocialManager.instance.SaveIfDirty();
		if (num != 0)
		{
			instance.AnnounceToFriends(num);
		}
		RefreshTopRun();
	}

	private static void TriggerActionForFacebookPayout(bool isLoggedInToFacebook)
	{
		Action<bool> action = delegateFacebookPayoutPopupAction;
		if (action != null)
		{
			action(isLoggedInToFacebook);
		}
	}

	private static void QueueFacebookPayoutPopupIfNeeded(bool isLoggedInToFacebook)
	{
		if (isLoggedInToFacebook && !FacebookRewardsInfo.HasReceivedAllAvailableRewards())
		{
			UIScreenController.QueueFacebookPayoutPopup();
		}
	}

	public static void SetFacebookPayoutAction(Action<bool> action)
	{
		delegateFacebookPayoutPopupAction = action;
	}

	public static void SetFacebookPayoutActionToDefault()
	{
		delegateFacebookPayoutPopupAction = QueueFacebookPayoutPopupIfNeeded;
	}

	private void OnRegisterUserCompleted(bool success)
	{
	}

	public static void SaveSoundOptionBeforeVideoAds(bool isEnabled)
	{
		PlayerPrefs.SetInt("soundBeforeLostFocus", isEnabled ? 1 : 0);
	}

	public static void DeleteSoundOptionBeforeVideoAds()
	{
		if (PlayerPrefs.HasKey("soundBeforeLostFocus"))
		{
			PlayerPrefs.DeleteKey("soundBeforeLostFocus");
		}
	}

	public static void TryRestoreSoundOptionAfterLostFocusIfNeeded()
	{
		if (!IncomingCallManager.IsInCall && PlayerPrefs.HasKey("soundBeforeLostFocus"))
		{
			Settings.optionSound = PlayerPrefs.GetInt("soundBeforeLostFocus", 1) != 0;
			DeleteSoundOptionBeforeVideoAds();
		}
	}

	public static void RefreshOnlineSettingsAndInappsIfNeeded()
	{
		float num = 3600f;
		string valueString;
		if (OnlineSettings.instance.TryGetValue("refreshinterval", out valueString))
		{
			float result;
			if (float.TryParse(valueString, out result))
			{
				if (result >= 10f)
				{
					num = result;
				}
				else
				{
					LogError("OnlineSettings refresh interval too small: " + num);
				}
			}
			else
			{
				LogError("Failed to parse Onlinesettings refresh interval from: " + valueString);
			}
		}
		if (Time.realtimeSinceStartup > _onlineSettingsLastDownloadTime + num)
		{
			_onlineSettingsLastDownloadTime = Time.realtimeSinceStartup;
			if (!OnlineSettings.instance.isDownloading)
			{
				OnlineSettings.instance.DownloadNow();
			}
			if (InAppManager.isInitialized)
			{
				InAppManager.instance.RequestIAPInfo();
			}
		}
		else if (InAppManager.isInitialized && !InAppManager.instance.ProductRequestSucceeded())
		{
			InAppManager.instance.RequestIAPInfo();
		}
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

	[Conditional("ENABLE_HOUSEKEEPER_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
