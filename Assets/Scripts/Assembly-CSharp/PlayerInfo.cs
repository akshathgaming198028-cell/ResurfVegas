using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Subway.Info;
using SYBO.Subway.Pickups;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerInfo
{
	public enum StickerType
	{
		noSticker = 0,
		Discount = 1,
		Gift = 2
	}

	private enum Key
	{
		AmountOfCoins = 0,
		HighestScore = 1,
		DailyWord = 2,
		DailyWordUnlockMask = 3,
		DailyWordExpireTime = 4,
		DailyWordPayedOut = 5,
		CurrentCharacter = 6,
		CurrentMissionSet = 7,
		CurrentMissionSetProgress = 8,
		CollectedCharacterTokens = 9,
		AmountOfMysteryBoxesOpened = 10,
		TutorialCompleted = 11,
		InAppPurchaseCount = 12,
		InAppConsumablePurchaseCount = 13,
		InAppNonconsumablePurchaseCount = 14,
		InAppRestoredPurchaseCount = 15,
		EarnCurrencyData = 16,
		PayBonusFacebook = 17,
		PayBonusFacebookPack = 18,
		PayBonusFacebookCharacter = 19,
		PayBonusGameCenter = 20,
		UnlockedTrophies = 21,
		AchievementProgress = 22,
		DoubleCoins = 23,
		DailyWordInRow = 24,
		MissionCompletedSum = 25,
		NumberOfRunsSinceLastGuideline = 26,
		HasShownHoverboardPopup = 27,
		HasShownAwardsIntroductionPopup = 28,
		HasShownTopRunIntroductionPopup = 29,
		LastShownTopRunIntroPopupVersionNumber = 30,
		HasShownHoverboardCustomizationPopup = 31,
		HasShownTurboHeadstartIntroductionPopup = 32,
		ShouldShowHoverboardPopup = 33,
		HasShownFacebookPopup = 34,
		ShouldShowFacebookPopup = 35,
		HasShownDuelIntroductionPopup = 36,
		ShouldShowDuelIntroductionPopup = 37,
		ShouldShowDuelsBreadcrumb = 38,
		HasShownMission1Popup = 39,
		ShouldShowMission1Popup = 40,
		HasShownCollectPopup = 41,
		ShouldShowCollectPopup = 42,
		HasShownMission2Popup = 43,
		ShouldShowMission2Popup = 44,
		DummyFriendCollected = 45,
		DummyFriendShouldShow = 46,
		DailyWordLastPayoutDayOfYear = 47,
		AmountOfSuperMysteryBoxesOpened = 48,
		HasHoverboardsBeenSeen = 49,
		UnlockedHoverboardTypes = 50,
		CurrentHoverboard = 51,
		PreviousHoverboard = 52,
		ShouldRestoreAfterEvent = 53,
		BreadCrumbsShownOnFrontPage = 54,
		UnreportedGames = 55,
		UnreportedGamesTimestamp = 56,
		HistoryOfInapp = 57,
		HasCharacterBeenSeen = 58,
		NeverAskForRating = 59,
		LastVersionRated = 60,
		NumberOfTotalRuns = 61,
		AmountOfSeasonalEggs = 62,
		HasCollectedFromFriends = 63,
		AmountOfKeys = 64,
		HasFirstMBKey = 65,
		HasFirstSMBKey = 66,
		LastTopRunResultsAwardedID = 67,
		PendingTopRunResultsID = 68,
		PendingTopRunResultScore = 69,
		PendingTopRunResultRank = 70,
		PendingTopRunResultWeek = 71,
		LastWeekMedalWasShown = 72,
		LastPlayDate = 73,
		LastQuitDate = 74,
		LastDailyOnlineLog = 75,
		HasShownKeysPopup = 76,
		MissionsVersionLastUpdate = 77,
		SpecialChallengeData = 78,
		CharacterThemesUnlocked = 79,
		CharacterLastSelectedThemes = 80,
		CharacterThemesSeen = 81,
		PendingRewards = 82,
		RemoteNotificationsEnabled = 83,
		IsFreshInstall = 84,
		LastEventID = 85,
		LastTimeAnEventHasBeenTried = 86,
		LastTimeAnEventWasStarted = 87,
		LastTimeEventPopupHasBeenForcedShown = 88,
		LastEventItemID = 89,
		DailyInterstitialExpireTime = 90,
		LastTimeInterstitialWasSeen = 91,
		LastTimeInterstitialProviderHasChanged = 92,
		HourlyInterstitialExpireTime = 93,
		NumberOfInterstitialsPerDay = 94,
		NumberOfInterstitialsPerHour = 95,
		InterstitialProviderListVersion = 96,
		IsSaveMePopupEnabled = 97,
		NumberOfRuns = 98,
		LastCharacterSentToServer = 99,
		LastCharacterThemeSentToServer = 100,
		LastKnownServerTime = 101,
		LastTimeAVideoForKeysWasSeen = 102,
		LastTimeAWeeklyFacebookGiftWasCollected = 103,
		LastPurchasedBundle = 104,
		LastTimeAppWasUpdatedOrInstalled = 105,
		FirstInstallDate = 106,
		Stats = 107,
		HoverboardCustomizationData = 108,
		CollectCoinsDummyData = 109,
		AwardStatus = 110,
		CharacterNameEventData = 111,
		HasCollectedFirstAwardReward = 112,
		HasCollectedFacebookWeeklyGiftThisWeek = 113,
		AmountOfMiniMysteryBoxesOpened = 114,
		FlurryUserId = 115,
		LatestAvailableBundlesVersion = 116,
		AllBundlesDownloaded = 117
	}

	public const string ONLINESETTINGS_PICKUP_SYSTEM_VERSION_KEY = "pickup_system_version";

	private const string SECRET = "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn";

	private const int VERSION = 1;

	private static readonly CultureInfo DATETIME_CULTUREINFO = new CultureInfo("da-DK");

	private bool _dirty;

	public Action onCoinsChanged;

	public Action onKeysChanged;

	private int _xoredAmountOfCoins;

	private int _xoredAmountOfKeys;

	private int _highestScore;

	private int _oldHighestScore;

	public Action onHighScoreChanged;

	private int _highestMeters;

	private int _amountOfMysteryBoxesOpened;

	private int _amountOfMiniMysteryBoxesOpened;

	private bool _willShowSMBAfterDailyChallenge;

	private int _amountOfSuperMysteryBoxesOpened;

	private int _amountOfSeasonalEggs;

	private string _weeklyChallangeData = string.Empty;

	private string _lastPlayDate = string.Empty;

	private string _lastQuitDate = string.Empty;

	private string _lastDailyOnlineLog = string.Empty;

	private bool _hasReceivedFirstMBKey;

	private bool _hasReceivedFirstSMBKey;

	private List<CelebrationReward> _pendingRewards = new List<CelebrationReward>();

	private CelebrationReward _lastAddedReward;

	private string _lastTopRunResultAwardedID = string.Empty;

	private string _pendingTopRunResultID = string.Empty;

	private int _pendingTopRunResultsScore = -1;

	private int _pendingTopRunResultsRank = -1;

	private int _pendingTopRunResultsWeek = -1;

	private int _pendingTopRunBeatenFriendsAward;

	private int _lastWeekMedalWasShown = -1;

	private int _lastMissionCompleted = -1;

	private int _currentMissionSet = -1;

	private int[] _currentMissionProgress;

	private int _missionCompletedSum;

	public Action onScoreMultiplierChanged;

	private Dictionary<Characters.CharacterType, int> _lastSelectedThemes = new Dictionary<Characters.CharacterType, int>();

	private Dictionary<Characters.CharacterType, int[]> _characterThemesSeen = new Dictionary<Characters.CharacterType, int[]>();

	public Action<Characters.CharacterType> OnCharacterOutfitUnlocked;

	private Dictionary<Characters.CharacterType, int[]> _characterThemesUnlocked = new Dictionary<Characters.CharacterType, int[]>();

	private int _lastCharacterSentToServer;

	private int _lastCharacterThemeSentToServer;

	private DateTime _lastTimeInterstitialProviderHasChanged;

	private DateTime _lastTimeInterstitialWasSeen;

	private DateTime _dailyInterstitialExpireTime;

	private DateTime _hourlyInterstitialExpireTime;

	private int _numberOfInterstitialsPerDay;

	private int _numberOfInterstitialsPerHour;

	private int _interstitialProviderListVersion;

	private int _currentCharacter;

	public Action<Characters.CharacterType> OnTokenCollected;

	private int[] _collectedCharacterTokens = new int[Characters.characterData.Count];

	private bool _isNew_tasha = true;

	private bool _isNew_zoe = true;

	private bool _isNew_brody = true;

	private bool _isNew_princek = true;

	private Dictionary<Characters.CharacterType, bool> _hasCharacterBeenSeen = new Dictionary<Characters.CharacterType, bool>();

	private Hoverboards.BoardType _currentHoverboard;

	private Hoverboards.BoardType _previousHoverboard;

	private Dictionary<Hoverboards.BoardType, bool> _hoverboardUnlockStatus = new Dictionary<Hoverboards.BoardType, bool>();

	public Action<Hoverboards.BoardType> OnBoardUnlocked;

	private Dictionary<Hoverboards.BoardType, bool> _hasHoverboardBeenSeen = new Dictionary<Hoverboards.BoardType, bool>();

	private int _unreportedGames;

	private DateTime _unreportedGamesTimestamp = new DateTime(0L);

	public Action<OldTrophies.Trophy> OnTrophyUnlocked;

	private bool[] _unlockedTrophies = new bool[Enum.GetValues(typeof(OldTrophies.Trophy)).Length];

	private int[] _achievementProgress = new int[41];

	private bool _hasCollectedFirstAwardReward;

	private bool _hasCollectedFacebookWeeklyGiftThisWeek;

	private bool _hasPayedOutFacebook;

	private bool _hasPayedOutFacebookPack;

	private bool _hasPayedOutFacebookCharacter;

	private bool _hasPayedOutGameCenter;

	private string _dailyWord = string.Empty;

	private int _dailyWordInRow;

	private int _dailyWordLastPayoutDayOfYear;

	private IntMask _dailyWordUnlockedMask;

	private DateTime _dailyWordExpireTime;

	private DateTime _dailyWordExpireDeviceTime;

	private bool _dailyWordPayedOut;

	public Action OnPickedUpLetter;

	public Action OnDailyWordInRowChanged;

	private ActivePickupSystemVersion _currentpickupSystemVersion = ActivePickupSystemVersion.Version2015;

	private bool _hasSetCurrentPickupSystemVersion;

	private bool _isTutorialCompleted;

	private int _numberOfBreadCrumbsShownOnFrontPage;

	private bool _shouldRestoreAfterEvent;

	private long _lastEventID;

	private string _lastEventItemID = string.Empty;

	private long _lastTimeAnEventWasStarted;

	private long _lastTimeAnEventHasBeenTried;

	private long _lastTimeEventPopupHasBeenForcedShown;

	public Action OnKnownServerTimeChange;

	private DateTime _lastKnownServerTime;

	private DateTime _lastTimeAVideoForKeysWasSeen;

	private DateTime _weeklyGiftEndTime;

	private DateTime _lastTimeAppWasUpdatedOrInstalled;

	private DateTime _firstInstallDate;

	private int _numberOfRunsSinceLastGuideline;

	private bool _hasShownHoverboardPopup;

	private bool _shouldShowHoverboardPopup;

	private bool _hasShownFacebookPopup;

	private bool _shouldShowFacebookPopup;

	private bool _hasShownDuelIntroductionPopup;

	private bool _shouldShowDuelIntroductionPopup;

	private bool _shouldShowDuelsBreadcrumb = true;

	private bool _hasShownMission1Popup;

	private bool _shouldShowMission1Popup;

	private bool _hasShownCollectPopup;

	private bool _shouldShowCollectPopup;

	private bool _hasShownMission2Popup;

	private bool _shouldShowMission2Popup;

	private bool _hasShownAwardsIntroductionPopup;

	private bool _hasShownTopRunIntroductionPopup;

	private bool _hasShownHoverboardCustmoizationPopup;

	private bool _hasShownTurboHeadstartIntroductionPopup;

	private int _lastShownTopRunIntroPopupVersionNumber;

	private bool _hasCollectedFromFriends;

	private int _numberOfRuns;

	private bool _isSaveMePopupEnabled;

	private int _latestAvailableAssetBundlesVersion;

	private bool _allAssetBundlesDownloaded;

	private bool _dummyFriendCollected;

	private bool _dummyFriendShouldShow;

	private int _inAppLegacyPurchaseCount;

	private int _inAppConsumablePurchaseCount;

	private int _inAppNonconsumablePurchaseCount;

	private int _inAppRestoredPurchaseCount;

	private string _earnCurrenyData = string.Empty;

	public Action<PowerupType, int> onPowerupAmountChanged;

	private Dictionary<PowerupType, int> _upgradeAmounts = new Dictionary<PowerupType, int>
	{
		{
			PowerupType.hoverboard,
			0
		},
		{
			PowerupType.headstart500,
			0
		},
		{
			PowerupType.headstart2000,
			0
		},
		{
			PowerupType.mysterybox,
			0
		},
		{
			PowerupType.scorebooster,
			0
		}
	};

	private Dictionary<PowerupType, int> _upgradeTiers = new Dictionary<PowerupType, int>
	{
		{
			PowerupType.jetpack,
			0
		},
		{
			PowerupType.supersneakers,
			0
		},
		{
			PowerupType.coinmagnet,
			0
		},
		{
			PowerupType.letters,
			0
		},
		{
			PowerupType.doubleMultiplier,
			0
		}
	};

	private Dictionary<string, string> _inappHistory = new Dictionary<string, string>();

	private bool _hasDoubleCoins;

	private InAppData.BundleTier _lastPurchasedBundle;

	private bool _hasShownKeysPopup;

	private Statistics _stats;

	private string _trophyMissionData = string.Empty;

	private string _characterNameEventData = string.Empty;

	private HoverboardCustomizationData _hoverboardCustomizationData;

	private CollectCoinsDummyData _collectCoinsDummyData;

	private bool _neverAskForRating;

	private string _lastVersionRated = string.Empty;

	private int _ratingAskCounter;

	private int _missionsVersionLastUpdate;

	private bool _remoteNotificationsEnabled = true;

	private bool _isFreshInstall = true;

	private string _flurryUserId;

	private string _flurryUserIdHash;

	private Key _flurry_nextKeyToRead;

	private bool _doubleScore;

	private static PlayerInfo _instance;

	public bool dirty
	{
		get
		{
			return _dirty;
		}
	}

	public int amountOfCoins
	{
		get
		{
			return Utils.EncryptDecryptXORValue(_xoredAmountOfCoins);
		}
		set
		{
			int num = Utils.EncryptDecryptXORValue(_xoredAmountOfCoins);
			if (num != value)
			{
				_xoredAmountOfCoins = Utils.EncryptDecryptXORValue(value);
				_dirty = true;
				Action action = onCoinsChanged;
				if (action != null)
				{
					action();
				}
			}
		}
	}

	public int amountOfKeys
	{
		get
		{
			return Utils.EncryptDecryptXORValue(_xoredAmountOfKeys);
		}
		set
		{
			int num = Utils.EncryptDecryptXORValue(_xoredAmountOfKeys);
			if (num != value)
			{
				_xoredAmountOfKeys = Utils.EncryptDecryptXORValue(value);
				_dirty = true;
				Action action = onKeysChanged;
				if (action != null)
				{
					action();
				}
			}
		}
	}

	public int highestScore
	{
		get
		{
			return _highestScore;
		}
		set
		{
			if (value > _highestScore)
			{
				_oldHighestScore = _highestScore;
				_highestScore = value;
				_dirty = true;
				Action action = onHighScoreChanged;
				if (action != null)
				{
					action();
				}
			}
		}
	}

	public int oldHighestScore
	{
		get
		{
			return _oldHighestScore;
		}
	}

	public int highestMeters
	{
		get
		{
			return _highestMeters;
		}
		set
		{
			if (_highestMeters != value)
			{
				_highestMeters = value;
				_dirty = true;
			}
		}
	}

	public int amountOfMysteryBoxesOpened
	{
		get
		{
			return _amountOfMysteryBoxesOpened;
		}
		set
		{
			if (_amountOfMysteryBoxesOpened != value)
			{
				_amountOfMysteryBoxesOpened = value;
				_dirty = true;
			}
		}
	}

	public int amountOfMiniMysteryBoxesOpened
	{
		get
		{
			return _amountOfMiniMysteryBoxesOpened;
		}
		set
		{
			if (_amountOfMiniMysteryBoxesOpened != value)
			{
				_amountOfMiniMysteryBoxesOpened = value;
				_dirty = true;
			}
		}
	}

	public bool willShowSMBAfterDailyChallenge
	{
		get
		{
			return _willShowSMBAfterDailyChallenge;
		}
		set
		{
			_willShowSMBAfterDailyChallenge = value;
		}
	}

	public int amountOfSuperMysteryBoxesOpened
	{
		get
		{
			return _amountOfSuperMysteryBoxesOpened;
		}
		set
		{
			if (_amountOfSuperMysteryBoxesOpened != value)
			{
				_amountOfSuperMysteryBoxesOpened = value;
				_dirty = true;
			}
		}
	}

	public int amountOfSeasonalEggs
	{
		get
		{
			return _amountOfSeasonalEggs;
		}
		set
		{
			if (_amountOfSeasonalEggs != value)
			{
				_amountOfSeasonalEggs = value;
				_dirty = true;
			}
		}
	}

	public string weeklyChallangeData
	{
		get
		{
			return _weeklyChallangeData;
		}
		set
		{
			if (_weeklyChallangeData != value)
			{
				_weeklyChallangeData = value;
				_dirty = true;
			}
		}
	}

	public string lastPlayDate
	{
		get
		{
			return _lastPlayDate;
		}
		set
		{
			if (_lastPlayDate != value)
			{
				_lastPlayDate = value;
				_dirty = true;
			}
		}
	}

	public string lastQuitDate
	{
		get
		{
			return _lastQuitDate;
		}
		set
		{
			if (_lastQuitDate != value)
			{
				_lastQuitDate = value;
				_dirty = true;
			}
		}
	}

	public string lastDailyOnlineLog
	{
		get
		{
			return _lastDailyOnlineLog;
		}
		set
		{
			if (_lastDailyOnlineLog != value)
			{
				_lastDailyOnlineLog = value;
				_dirty = true;
			}
		}
	}

	public bool hasReceivedFirstMBKey
	{
		get
		{
			return _hasReceivedFirstMBKey;
		}
		set
		{
			if (_hasReceivedFirstMBKey != value)
			{
				_hasReceivedFirstMBKey = value;
				_dirty = true;
			}
		}
	}

	public bool hasReceivedFirstSMBKey
	{
		get
		{
			return _hasReceivedFirstSMBKey;
		}
		set
		{
			if (_hasReceivedFirstSMBKey != value)
			{
				_hasReceivedFirstSMBKey = value;
				_dirty = true;
			}
		}
	}

	public List<CelebrationReward> pendingRewards
	{
		get
		{
			return _pendingRewards;
		}
	}

	public CelebrationReward lastAddedReward
	{
		get
		{
			return _lastAddedReward;
		}
	}

	public string lastTopRunResultAwardedID
	{
		get
		{
			return _lastTopRunResultAwardedID;
		}
		set
		{
			if (_lastTopRunResultAwardedID != value)
			{
				_lastTopRunResultAwardedID = value;
				_dirty = true;
			}
		}
	}

	public string pendingTopRunResultID
	{
		get
		{
			return _pendingTopRunResultID;
		}
		set
		{
			if (_pendingTopRunResultID != value)
			{
				_pendingTopRunResultID = value;
				_dirty = true;
			}
		}
	}

	public int pendingTopRunResultScore
	{
		get
		{
			return _pendingTopRunResultsScore;
		}
		set
		{
			if (_pendingTopRunResultsScore != value)
			{
				_pendingTopRunResultsScore = value;
				_dirty = true;
			}
		}
	}

	public int pendingTopRunResultsRank
	{
		get
		{
			return _pendingTopRunResultsRank;
		}
		set
		{
			if (_pendingTopRunResultsRank != value)
			{
				_pendingTopRunResultsRank = value;
				_dirty = true;
			}
		}
	}

	public int pendingTopRunResultsWeek
	{
		get
		{
			return _pendingTopRunResultsWeek;
		}
		set
		{
			if (_pendingTopRunResultsWeek != value)
			{
				_pendingTopRunResultsWeek = value;
				_dirty = true;
			}
		}
	}

	public int pendingTopRunBeatenFriendsAward
	{
		get
		{
			return _pendingTopRunBeatenFriendsAward;
		}
		set
		{
			if (_pendingTopRunBeatenFriendsAward != value)
			{
				_pendingTopRunBeatenFriendsAward = value;
				_dirty = true;
			}
		}
	}

	public int lastWeekMedalWasShown
	{
		get
		{
			return _lastWeekMedalWasShown;
		}
		set
		{
			if (_lastWeekMedalWasShown != value)
			{
				_lastWeekMedalWasShown = value;
				_dirty = true;
			}
		}
	}

	public int currentMissionSet
	{
		get
		{
			return _currentMissionSet;
		}
	}

	public int lastMissionCompleted
	{
		get
		{
			return _lastMissionCompleted;
		}
	}

	public int missionCompletedSum
	{
		get
		{
			if (_missionCompletedSum == 0)
			{
				_missionCompletedSum = currentMissionSet + 1;
			}
			return _missionCompletedSum;
		}
		set
		{
			_missionCompletedSum = value;
		}
	}

	public int scoreMultiplier
	{
		get
		{
			int num = Mathf.Clamp(_currentMissionSet + 1, 1, 30);
			if (GameStats.Instance.characterDayEventActivated)
			{
				num += 5;
			}
			if (GameStats.Instance.scoreBoosterActivated)
			{
				switch (GameStats.Instance.scoreBoosterTier)
				{
				case 1:
					num += 5;
					break;
				case 2:
					num += 6;
					break;
				case 3:
					num += 7;
					break;
				default:
					num += 5;
					break;
				}
			}
			if (PlusMultiplier.plusMultiplierEnabled)
			{
				num += 10;
			}
			if (doubleScore)
			{
				num *= 2;
			}
			return num;
		}
	}

	public int rawMultiplier
	{
		get
		{
			return Mathf.Clamp(_currentMissionSet + 1, 1, 30);
		}
	}

	public int currentThemeIndex
	{
		get
		{
			return GetIndexForLastSelectedTheme((Characters.CharacterType)currentCharacter);
		}
		set
		{
			SetLastSelectedTheme((Characters.CharacterType)currentCharacter, value);
		}
	}

	public int lastCharacterSentToServer
	{
		get
		{
			return _lastCharacterSentToServer;
		}
	}

	public int lastCharacterThemeSentToServer
	{
		get
		{
			return _lastCharacterThemeSentToServer;
		}
	}

	public bool shouldSendCharacterToServer
	{
		get
		{
			return _lastCharacterSentToServer != currentCharacter || _lastCharacterThemeSentToServer != currentThemeIndex;
		}
	}

	public DateTime lastTimeInterstitialProviderHasChanged
	{
		get
		{
			return _lastTimeInterstitialProviderHasChanged;
		}
		set
		{
			if (_lastTimeInterstitialProviderHasChanged != value)
			{
				_lastTimeInterstitialProviderHasChanged = value;
				_dirty = true;
			}
		}
	}

	public DateTime lastTimeInterstitialWasSeen
	{
		get
		{
			return _lastTimeInterstitialWasSeen;
		}
		set
		{
			if (_lastTimeInterstitialWasSeen != value)
			{
				_lastTimeInterstitialWasSeen = value;
				_dirty = true;
			}
		}
	}

	public DateTime dailyInterstitialsTimeLeft
	{
		get
		{
			return _dailyInterstitialExpireTime;
		}
		set
		{
			if (_dailyInterstitialExpireTime != value)
			{
				_dailyInterstitialExpireTime = value;
				_dirty = true;
			}
		}
	}

	public DateTime hourlyInterstitialsTimeLeft
	{
		get
		{
			return _hourlyInterstitialExpireTime;
		}
		set
		{
			if (_hourlyInterstitialExpireTime != value)
			{
				_hourlyInterstitialExpireTime = value;
				_dirty = true;
			}
		}
	}

	public int numberOfInterstitialsPerDay
	{
		get
		{
			return _numberOfInterstitialsPerDay;
		}
		set
		{
			if (_numberOfInterstitialsPerDay != value)
			{
				_numberOfInterstitialsPerDay = value;
				_dirty = true;
			}
		}
	}

	public int numberOfInterstitialsPerHour
	{
		get
		{
			return _numberOfInterstitialsPerHour;
		}
		set
		{
			if (_numberOfInterstitialsPerHour != value)
			{
				_numberOfInterstitialsPerHour = value;
				_dirty = true;
			}
		}
	}

	public int interstitialProviderListVersion
	{
		get
		{
			return _interstitialProviderListVersion;
		}
		set
		{
			if (_interstitialProviderListVersion != value)
			{
				_interstitialProviderListVersion = value;
				_dirty = true;
			}
		}
	}

	public int currentCharacter
	{
		get
		{
			return _currentCharacter;
		}
		set
		{
			if (value != _currentCharacter)
			{
				_currentCharacter = value;
				_dirty = true;
			}
		}
	}

	public Hoverboards.BoardType currentHoverboard
	{
		get
		{
			return _currentHoverboard;
		}
		set
		{
			if (value != _currentHoverboard)
			{
				_currentHoverboard = value;
				_dirty = true;
			}
		}
	}

	public Hoverboards.BoardType previousHoverboard
	{
		get
		{
			return _previousHoverboard;
		}
		set
		{
			if (value != _previousHoverboard)
			{
				_previousHoverboard = value;
				_dirty = true;
			}
		}
	}

	public int[] achievementProgress
	{
		get
		{
			return _achievementProgress;
		}
	}

	public bool hasCollectedFirstAwardReward
	{
		get
		{
			return _hasCollectedFirstAwardReward;
		}
		set
		{
			if (_hasCollectedFirstAwardReward != value)
			{
				_hasCollectedFirstAwardReward = value;
				_dirty = true;
			}
		}
	}

	public bool hasCollectedFacebookWeeklyGiftThisWeek
	{
		get
		{
			return _hasCollectedFacebookWeeklyGiftThisWeek;
		}
		set
		{
			if (_hasCollectedFacebookWeeklyGiftThisWeek != value)
			{
				_hasCollectedFacebookWeeklyGiftThisWeek = value;
				_dirty = true;
			}
		}
	}

	public bool hasPayedOutFacebook
	{
		get
		{
			return _hasPayedOutFacebook;
		}
		set
		{
			if (_hasPayedOutFacebook != value)
			{
				_hasPayedOutFacebook = value;
				_dirty = true;
			}
		}
	}

	public bool hasPayedOutFacebookPack
	{
		get
		{
			return _hasPayedOutFacebookPack;
		}
		set
		{
			if (_hasPayedOutFacebookPack != value)
			{
				_hasPayedOutFacebookPack = value;
				_dirty = true;
			}
		}
	}

	public bool hasPayedOutFacebookCharacter
	{
		get
		{
			return _hasPayedOutFacebookCharacter;
		}
		set
		{
			if (_hasPayedOutFacebookCharacter != value)
			{
				_hasPayedOutFacebookCharacter = value;
				_dirty = true;
			}
		}
	}

	public bool hasPayedOutGameCenter
	{
		get
		{
			return _hasPayedOutGameCenter;
		}
		set
		{
			if (_hasPayedOutGameCenter != value)
			{
				_hasPayedOutGameCenter = value;
				_dirty = true;
			}
		}
	}

	public string dailyWord
	{
		get
		{
			return _dailyWord;
		}
	}

	public int dailyWordInRow
	{
		get
		{
			return _dailyWordInRow;
		}
	}

	public int dailyWordLastPayoutDayOfYear
	{
		get
		{
			return _dailyWordLastPayoutDayOfYear;
		}
	}

	public IntMask dailyWordUnlockedMask
	{
		get
		{
			return _dailyWordUnlockedMask;
		}
	}

	public TimeSpan dailyWordTimeLeft
	{
		get
		{
			return _dailyWordExpireTime - DateTime.UtcNow;
		}
	}

	public TimeSpan dailyWordDeviceTiemLeft
	{
		get
		{
			return _dailyWordExpireDeviceTime - DateTime.UtcNow;
		}
	}

	public ActivePickupSystemVersion CurrentPickupSystemVersion
	{
		get
		{
			if (!_hasSetCurrentPickupSystemVersion)
			{
				_currentpickupSystemVersion = GetActivePickupSystemVersion();
				_hasSetCurrentPickupSystemVersion = true;
			}
			return _currentpickupSystemVersion;
		}
	}

	public bool IsTutorialCompleted
	{
		get
		{
			return _isTutorialCompleted;
		}
		set
		{
			if (_isTutorialCompleted != value)
			{
				_isTutorialCompleted = value;
				_dirty = true;
			}
		}
	}

	public int numberOfBreadCrumbsShownOnFrontPage
	{
		get
		{
			return _numberOfBreadCrumbsShownOnFrontPage;
		}
	}

	public bool shouldRestoreAfterEvent
	{
		get
		{
			return _shouldRestoreAfterEvent;
		}
		set
		{
			if (_shouldRestoreAfterEvent != value)
			{
				_shouldRestoreAfterEvent = value;
				_dirty = true;
			}
		}
	}

	public long lastEventID
	{
		get
		{
			return _lastEventID;
		}
		set
		{
			if (_lastEventID != value)
			{
				_lastEventID = value;
				_dirty = true;
			}
		}
	}

	public string lastEventItemID
	{
		get
		{
			return _lastEventItemID;
		}
		set
		{
			if (_lastEventItemID != value)
			{
				_lastEventItemID = value;
				_dirty = true;
			}
		}
	}

	public long lastTimeAnEventWasStarted
	{
		get
		{
			return _lastTimeAnEventWasStarted;
		}
		set
		{
			if (_lastTimeAnEventWasStarted != value)
			{
				_lastTimeAnEventWasStarted = value;
				_dirty = true;
			}
		}
	}

	public long lastTimeAnEventHasBeenTried
	{
		get
		{
			return _lastTimeAnEventHasBeenTried;
		}
		set
		{
			if (_lastTimeAnEventHasBeenTried != value)
			{
				_lastTimeAnEventHasBeenTried = value;
				_dirty = true;
			}
		}
	}

	public long lastTimeEventPopupHasBeenForcedShown
	{
		get
		{
			return _lastTimeEventPopupHasBeenForcedShown;
		}
		set
		{
			if (_lastTimeEventPopupHasBeenForcedShown != value)
			{
				_lastTimeEventPopupHasBeenForcedShown = value;
				_dirty = true;
			}
		}
	}

	public DateTime lastKnownServerTime
	{
		get
		{
			return _lastKnownServerTime;
		}
		set
		{
			_lastKnownServerTime = value;
			_dirty = true;
			Action onKnownServerTimeChange = OnKnownServerTimeChange;
			if (onKnownServerTimeChange != null)
			{
				onKnownServerTimeChange();
			}
		}
	}

	public DateTime lastTimeAVideoForKeysWasSeen
	{
		get
		{
			return _lastTimeAVideoForKeysWasSeen;
		}
		set
		{
			_lastTimeAVideoForKeysWasSeen = value;
			_dirty = true;
		}
	}

	public DateTime weeklyGiftEndTime
	{
		get
		{
			return _weeklyGiftEndTime;
		}
		set
		{
			_weeklyGiftEndTime = value;
			_dirty = true;
		}
	}

	public DateTime lastTimeAppWasUpdatedOrInstalled
	{
		get
		{
			return _lastTimeAppWasUpdatedOrInstalled;
		}
		set
		{
			_lastTimeAppWasUpdatedOrInstalled = value;
			_dirty = true;
		}
	}

	public DateTime firstInstallDate
	{
		get
		{
			return _firstInstallDate;
		}
		set
		{
			_firstInstallDate = value;
			_dirty = true;
		}
	}

	public bool hasShownHoverboardPopup
	{
		get
		{
			return _hasShownHoverboardPopup;
		}
		set
		{
			if (_hasShownHoverboardPopup != value)
			{
				_hasShownHoverboardPopup = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowHoverboardPopup
	{
		get
		{
			return _shouldShowHoverboardPopup;
		}
		set
		{
			if (_shouldShowHoverboardPopup != value)
			{
				_shouldShowHoverboardPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownFacebookPopup
	{
		get
		{
			return _hasShownFacebookPopup;
		}
		set
		{
			if (_hasShownFacebookPopup != value)
			{
				_hasShownFacebookPopup = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowFacebookPopup
	{
		get
		{
			return _shouldShowFacebookPopup;
		}
		set
		{
			if (_shouldShowFacebookPopup != value)
			{
				_shouldShowFacebookPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownDuelIntroductionPopup
	{
		get
		{
			return _hasShownDuelIntroductionPopup;
		}
		set
		{
			if (_hasShownDuelIntroductionPopup != value)
			{
				_hasShownDuelIntroductionPopup = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowDuelIntroductionPopup
	{
		get
		{
			return _shouldShowDuelIntroductionPopup;
		}
		set
		{
			if (_shouldShowDuelIntroductionPopup != value)
			{
				_shouldShowDuelIntroductionPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownAwardsIntroductionPopup
	{
		get
		{
			return _hasShownAwardsIntroductionPopup;
		}
		set
		{
			if (_hasShownAwardsIntroductionPopup != value)
			{
				_hasShownAwardsIntroductionPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownHoverboardCustmoizationPopup
	{
		get
		{
			return _hasShownHoverboardCustmoizationPopup;
		}
		set
		{
			if (_hasShownHoverboardCustmoizationPopup != value)
			{
				_hasShownHoverboardCustmoizationPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownTurboHeadstartIntroductionPopup
	{
		get
		{
			return _hasShownTurboHeadstartIntroductionPopup;
		}
		set
		{
			if (_hasShownTurboHeadstartIntroductionPopup != value)
			{
				_hasShownTurboHeadstartIntroductionPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownTopRunIntroductionPopup
	{
		get
		{
			return _hasShownTopRunIntroductionPopup;
		}
		set
		{
			if (_hasShownTopRunIntroductionPopup != value)
			{
				_hasShownTopRunIntroductionPopup = value;
				_dirty = true;
			}
		}
	}

	public int lastShownTopRunIntroPopupVersionNumber
	{
		get
		{
			return _lastShownTopRunIntroPopupVersionNumber;
		}
		set
		{
			if (_lastShownTopRunIntroPopupVersionNumber != value)
			{
				_lastShownTopRunIntroPopupVersionNumber = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowDuelsBreadcrumb
	{
		get
		{
			return _shouldShowDuelsBreadcrumb;
		}
		set
		{
			if (_shouldShowDuelsBreadcrumb != value)
			{
				_shouldShowDuelsBreadcrumb = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownMission1Popup
	{
		get
		{
			return _hasShownMission1Popup;
		}
		set
		{
			if (_hasShownMission1Popup != value)
			{
				_hasShownMission1Popup = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowMission1Popup
	{
		get
		{
			return _shouldShowMission1Popup;
		}
		set
		{
			if (_shouldShowMission1Popup != value)
			{
				_shouldShowMission1Popup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownCollectPopup
	{
		get
		{
			return _hasShownCollectPopup;
		}
		set
		{
			if (_hasShownCollectPopup != value)
			{
				_hasShownCollectPopup = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowCollectPopup
	{
		get
		{
			return _shouldShowCollectPopup;
		}
		set
		{
			if (_shouldShowCollectPopup != value)
			{
				_shouldShowCollectPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownMission2Popup
	{
		get
		{
			return _hasShownMission2Popup;
		}
		set
		{
			if (_hasShownMission2Popup != value)
			{
				_hasShownMission2Popup = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowMission2Popup
	{
		get
		{
			return _shouldShowMission2Popup;
		}
		set
		{
			if (shouldShowMission2Popup != value)
			{
				_shouldShowMission2Popup = value;
				_dirty = true;
			}
		}
	}

	public bool hasCollectedFromFriends
	{
		get
		{
			return _hasCollectedFromFriends;
		}
		set
		{
			if (_hasCollectedFromFriends != value)
			{
				_hasCollectedFromFriends = value;
				_dirty = true;
			}
		}
	}

	public int numberOfRuns
	{
		get
		{
			return _numberOfRuns;
		}
		set
		{
			if (_numberOfRuns != value)
			{
				_numberOfRuns = value;
				_dirty = true;
			}
		}
	}

	public bool isSaveMePopupEnabled
	{
		get
		{
			return _isSaveMePopupEnabled;
		}
		set
		{
			if (_isSaveMePopupEnabled != value)
			{
				_isSaveMePopupEnabled = value;
				_dirty = true;
			}
		}
	}

	public int LatestAvailableAssetBundlesVersion
	{
		get
		{
			return _latestAvailableAssetBundlesVersion;
		}
		set
		{
			_latestAvailableAssetBundlesVersion = value;
			_dirty = true;
		}
	}

	public bool AllAssetBundlesDownloaded
	{
		get
		{
			return _allAssetBundlesDownloaded;
		}
		set
		{
			_allAssetBundlesDownloaded = value;
			_dirty = true;
		}
	}

	public bool dummyFriendCollected
	{
		get
		{
			return _dummyFriendCollected;
		}
		set
		{
			_dummyFriendCollected = value;
			_dirty = true;
		}
	}

	public bool dummyFriendShouldShow
	{
		get
		{
			return _dummyFriendShouldShow;
		}
		set
		{
			_dummyFriendShouldShow = value;
			_dirty = true;
		}
	}

	public int inAppLegacyPurchaseCount
	{
		get
		{
			return _inAppLegacyPurchaseCount;
		}
		set
		{
			_inAppLegacyPurchaseCount = value;
			_dirty = true;
		}
	}

	public int inAppConsumablePurchaseCount
	{
		get
		{
			return _inAppConsumablePurchaseCount;
		}
		set
		{
			_inAppConsumablePurchaseCount = value;
			_dirty = true;
		}
	}

	public int inAppNonconsumablePurchaseCount
	{
		get
		{
			return _inAppNonconsumablePurchaseCount;
		}
		set
		{
			_inAppNonconsumablePurchaseCount = value;
			_dirty = true;
		}
	}

	public int inAppRestoredPurchaseCount
	{
		get
		{
			return _inAppRestoredPurchaseCount;
		}
		set
		{
			_inAppRestoredPurchaseCount = value;
			_dirty = true;
		}
	}

	public string earnCurrenyData
	{
		get
		{
			return _earnCurrenyData;
		}
		set
		{
			_earnCurrenyData = value;
			_dirty = true;
		}
	}

	public bool hasDoubleCoins
	{
		get
		{
			return _hasDoubleCoins;
		}
		set
		{
			if (_hasDoubleCoins != value)
			{
				_hasDoubleCoins = value;
				_dirty = true;
			}
		}
	}

	public InAppData.BundleTier lastPurchasedBundle
	{
		get
		{
			return _lastPurchasedBundle;
		}
		private set
		{
			if (_lastPurchasedBundle != value)
			{
				_lastPurchasedBundle = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownKeysPopup
	{
		get
		{
			return _hasShownKeysPopup;
		}
		set
		{
			if (_hasShownKeysPopup != value)
			{
				_hasShownKeysPopup = value;
				_dirty = true;
			}
		}
	}

	public Statistics stats
	{
		get
		{
			return _stats;
		}
	}

	public string trophyMissionData
	{
		get
		{
			return _trophyMissionData;
		}
		set
		{
			if (_trophyMissionData != value)
			{
				_trophyMissionData = value;
				_dirty = true;
			}
		}
	}

	public string characterNameEventData
	{
		get
		{
			return _characterNameEventData;
		}
		set
		{
			if (_characterNameEventData != value)
			{
				_characterNameEventData = value;
				_dirty = true;
			}
		}
	}

	public HoverboardCustomizationData HoverboardCustomizationData
	{
		get
		{
			return _hoverboardCustomizationData;
		}
		private set
		{
			_hoverboardCustomizationData = value;
		}
	}

	public CollectCoinsDummyData CollectCoinsDummyData
	{
		get
		{
			return _collectCoinsDummyData;
		}
		private set
		{
			_collectCoinsDummyData = value;
		}
	}

	public bool neverAskForRating
	{
		get
		{
			return _neverAskForRating;
		}
		set
		{
			if (_neverAskForRating != value)
			{
				_neverAskForRating = value;
				_dirty = true;
			}
		}
	}

	public string lastVersionRated
	{
		get
		{
			return _lastVersionRated;
		}
		set
		{
			if (_lastVersionRated != value)
			{
				_lastVersionRated = value;
				_dirty = true;
			}
		}
	}

	public int ratingAskCounter
	{
		get
		{
			return _ratingAskCounter;
		}
		set
		{
			if (_ratingAskCounter != value)
			{
				_ratingAskCounter = value;
				_dirty = true;
			}
		}
	}

	public int missionsVersionLastUpdate
	{
		get
		{
			return _missionsVersionLastUpdate;
		}
		set
		{
			if (_missionsVersionLastUpdate != value)
			{
				_missionsVersionLastUpdate = value;
				_dirty = true;
			}
		}
	}

	public bool remoteNotificationsEnabled
	{
		get
		{
			return _remoteNotificationsEnabled;
		}
		set
		{
			if (_remoteNotificationsEnabled != value)
			{
				_remoteNotificationsEnabled = value;
				_dirty = true;
			}
		}
	}

	public bool isFreshInstall
	{
		get
		{
			return _isFreshInstall;
		}
		set
		{
			if (_isFreshInstall != value)
			{
				_isFreshInstall = value;
				_dirty = true;
			}
		}
	}

	public string flurryUserIdHash
	{
		get
		{
			if (string.IsNullOrEmpty(_flurryUserId))
			{
				_flurryUserId = Guid.NewGuid().ToString();
			}
			if (string.IsNullOrEmpty(_flurryUserIdHash))
			{
				_flurryUserIdHash = StringUtility.ToHexString(Checksum.ComputeSHA256(_flurryUserId));
			}
			return _flurryUserIdHash;
		}
	}

	public float doubleScoreMultiplierDuration
	{
		get
		{
			return GetPowerupDuration(PowerupType.doubleMultiplier);
		}
	}

	public bool doubleScore
	{
		get
		{
			return _doubleScore;
		}
		set
		{
			if (value != _doubleScore)
			{
				_doubleScore = value;
				TriggerOnScoreMultiplierChanged();
			}
		}
	}

	public static PlayerInfo Instance
	{
		get
		{
			return _instance ?? (_instance = new PlayerInfo());
		}
	}

	private PlayerInfo()
	{
		Load();
	}

	public void ResetHighestScoreTo(int score)
	{
		_oldHighestScore = score;
		if (score != _highestScore)
		{
			_highestScore = score;
			_dirty = true;
			Action action = onHighScoreChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public void BragCompleted()
	{
		ResetHighestScoreTo(highestScore);
	}

	public void TriggerOnScoreMultiplierChanged()
	{
		Action action = onScoreMultiplierChanged;
		if (action != null)
		{
			action();
		}
	}

	public bool IsNewUser()
	{
		return rawMultiplier <= 1;
	}

	public void AddSaveMeTokenToUnlock()
	{
		amountOfKeys++;
	}

	public void AddPendingReward(CelebrationReward reward)
	{
		_lastAddedReward = reward;
		bool flag = false;
		CelebrationRewardOrigin celebrationRewardOrigin = CelebrationRewardOrigin.CharacterEventSuperMysteryBox;
		CelebrationRewardOrigin celebrationRewardOrigin2 = CelebrationRewardOrigin.NewHighScore;
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < _pendingRewards.Count; i++)
		{
			if (_pendingRewards[i].Uid == reward.Uid)
			{
				flag = true;
			}
			if (_pendingRewards[i].CelebrationRewardOrigin == celebrationRewardOrigin && num == -1)
			{
				num = i;
			}
			if (_pendingRewards[i].CelebrationRewardOrigin == celebrationRewardOrigin2 && num2 == -1)
			{
				num2 = i;
			}
		}
		if (flag)
		{
			return;
		}
		if (num2 > -1 && reward.CelebrationRewardOrigin != celebrationRewardOrigin)
		{
			_pendingRewards.Insert(num2, reward);
		}
		else if (num > -1)
		{
			if (reward.CelebrationRewardOrigin == celebrationRewardOrigin)
			{
				_pendingRewards.Add(reward);
			}
			else
			{
				_pendingRewards.Insert(num, reward);
			}
		}
		else
		{
			_pendingRewards.Add(reward);
		}
		_dirty = true;
	}

	public void RemovePendingReward(CelebrationReward reward)
	{
		CelebrationReward celebrationReward = _pendingRewards.Find((CelebrationReward mbr) => mbr.Uid == reward.Uid);
		if (celebrationReward == null)
		{
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox)
			{
				celebrationReward = _pendingRewards.Find((CelebrationReward mbr) => mbr.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox);
			}
			else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox)
			{
				celebrationReward = _pendingRewards.Find((CelebrationReward mbr) => mbr.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox);
			}
		}
		if (celebrationReward != null)
		{
			_pendingRewards.Remove(celebrationReward);
			if (_lastAddedReward != null && celebrationReward.Uid == _lastAddedReward.Uid)
			{
				_lastAddedReward = null;
			}
			_dirty = true;
		}
	}

	public bool IsCurrentMissionSetInited()
	{
		return false;
	}

	public void InitCurrentMissionSet(int missionSet, int missionCount, bool resetProgress)
	{
		if (missionSet == _currentMissionSet)
		{
			return;
		}
		_currentMissionSet = missionSet;
		if (resetProgress)
		{
			_currentMissionProgress = new int[missionCount];
			for (int i = 0; i < missionCount; i++)
			{
				_currentMissionProgress[i] = 0;
			}
		}
		_dirty = true;
		TriggerOnScoreMultiplierChanged();
	}

	public int GetCurrentMissionProgress(int mission)
	{
		if (mission >= 3)
		{
			return _achievementProgress[mission - 3];
		}
		if (_currentMissionProgress == null)
		{
			return 0;
		}
		if (mission < _currentMissionProgress.Length)
		{
			return _currentMissionProgress[mission];
		}
		return 0;
	}

	public void SetCurrentMissionProgress(int mission, int progress)
	{
		if (mission >= 3)
		{
			_achievementProgress[mission - 3] = progress;
		}
		else if (_currentMissionProgress[mission] != progress)
		{
			_currentMissionProgress[mission] = progress;
			_dirty = true;
		}
	}

	public bool IncrementCurrentMissionProgress(int mission, int target)
	{
		if (_currentMissionProgress[mission] < target)
		{
			_currentMissionProgress[mission]++;
			_dirty = true;
			return _currentMissionProgress[mission] == target;
		}
		return false;
	}

	public int GetCurrentRank()
	{
		return currentMissionSet / MissionsManager.Instance.GetNumberOfBasicMission();
	}

	public int GetIndexForLastSelectedTheme(Characters.CharacterType character)
	{
		int value;
		if (_lastSelectedThemes.TryGetValue(character, out value))
		{
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(character);
			if (list != null && list.Count >= value)
			{
				return value;
			}
		}
		return 0;
	}

	public void SetLastSelectedTheme(Characters.CharacterType character, int themeIndex)
	{
		if (Application.isEditor)
		{
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(character);
			if (list != null && (list.Count < themeIndex || themeIndex < 0))
			{
				Debug.LogError("The theme index is too great " + themeIndex + " for character: " + character.ToString());
			}
		}
		if (_lastSelectedThemes.ContainsKey(character))
		{
			_lastSelectedThemes.Remove(character);
		}
		if (themeIndex > 0)
		{
			_lastSelectedThemes.Add(character, themeIndex);
		}
		_dirty = true;
	}

	public void ThemeSeen(Characters.CharacterType character, int index)
	{
		int[] value;
		_characterThemesSeen.TryGetValue(character, out value);
		if (value == null)
		{
			value = new int[1] { index };
		}
		else
		{
			int[] array = new int[value.Length + 1];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = value[i];
			}
			array[value.Length] = index;
			value = array;
		}
		_characterThemesSeen.Remove(character);
		_characterThemesSeen.Add(character, value);
		_dirty = true;
	}

	public bool IsThemeSeenForCharacter(Characters.CharacterType character, int index)
	{
		if (index == 0)
		{
			return false;
		}
		int[] value;
		_characterThemesSeen.TryGetValue(character, out value);
		if (value != null)
		{
			bool result = false;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == index)
				{
					result = true;
					break;
				}
			}
			return result;
		}
		return false;
	}

	public void UnlockTheme(Characters.CharacterType character, int index)
	{
		int[] value;
		_characterThemesUnlocked.TryGetValue(character, out value);
		if (value == null)
		{
			value = new int[1] { index };
		}
		else
		{
			int[] array = new int[value.Length + 1];
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == index)
				{
					return;
				}
				array[i] = value[i];
			}
			array[value.Length] = index;
			value = array;
		}
		_characterThemesUnlocked.Remove(character);
		_characterThemesUnlocked.Add(character, value);
		_dirty = true;
		Action<Characters.CharacterType> onCharacterOutfitUnlocked = OnCharacterOutfitUnlocked;
		if (onCharacterOutfitUnlocked != null)
		{
			onCharacterOutfitUnlocked(character);
		}
	}

	public bool IsThemeUnlockedForCharacter(Characters.CharacterType character, int index)
	{
		if (Globals.DEBUG_ALL_CHARS)
		{
			return true;
		}
		if (index == 0)
		{
			return true;
		}
		int[] value;
		_characterThemesUnlocked.TryGetValue(character, out value);
		if (value != null)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == index)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetLastCharacterSentToServer(int character, int characterTheme)
	{
		if (character != _lastCharacterSentToServer || characterTheme != _lastCharacterThemeSentToServer)
		{
			_lastCharacterSentToServer = character;
			_lastCharacterThemeSentToServer = characterTheme;
			_dirty = true;
		}
	}

	public void CollectToken(Characters.CharacterType characterType, int amount)
	{
		_collectedCharacterTokens[(int)characterType] += amount;
		_dirty = true;
		Action<Characters.CharacterType> onTokenCollected = OnTokenCollected;
		if (onTokenCollected != null)
		{
			onTokenCollected(characterType);
		}
	}

	public void DecreaseToken(Characters.CharacterType characterType, int amount)
	{
		_collectedCharacterTokens[(int)characterType] -= amount;
		if (_collectedCharacterTokens[(int)characterType] < 0)
		{
			_collectedCharacterTokens[(int)characterType] = 0;
		}
	}

	public bool IsCollectionComplete(Characters.CharacterType characterType)
	{
		if (Globals.DEBUG_ALL_CHARS)
		{
			return true;
		}
		Characters.Model model = Characters.characterData[characterType];
		if (model.unlockType == Characters.UnlockType.free)
		{
			return true;
		}
		return GetCollectedTokens(characterType) >= model.Price;
	}

	public int GetCollectedTokens(Characters.CharacterType ModelType)
	{
		return _collectedCharacterTokens[(int)ModelType];
	}

	public bool IsTokenUseful(Characters.CharacterType characterType, CelebrationReward currentReward = null, bool isDoubling = false)
	{
		int num = 0;
		for (int i = 0; i < _pendingRewards.Count; i++)
		{
			if (_pendingRewards[i].rewardType == CelebrationRewardType.token && _pendingRewards[i].characterType == characterType)
			{
				num += _pendingRewards[i].amount;
			}
		}
		int price = Characters.characterData[characterType].Price;
		int num2 = ((!isDoubling) ? (GetCollectedTokens(characterType) + ((currentReward != null) ? currentReward.amount : 0)) : GetCollectedTokens(characterType));
		int num3 = price - num2;
		if (num3 <= num)
		{
			return false;
		}
		if (num2 < price)
		{
			return true;
		}
		return false;
	}

	public bool IsCharacterNew(Characters.CharacterType characterType)
	{
		if (Characters.characterData[characterType].isNewInThisUpdate)
		{
			switch (characterType)
			{
			case Characters.CharacterType.tasha:
				return _isNew_tasha;
			case Characters.CharacterType.zoe:
				return _isNew_zoe;
			case Characters.CharacterType.brody:
				return _isNew_brody;
			case Characters.CharacterType.princek:
				return _isNew_princek;
			}
		}
		return false;
	}

	public bool HasCharacterBeenSeen(Characters.CharacterType characterType)
	{
		if (!Characters.characterData[characterType].isNewInThisUpdate)
		{
			return true;
		}
		if (_hasCharacterBeenSeen.ContainsKey(characterType))
		{
			return _hasCharacterBeenSeen[characterType];
		}
		return false;
	}

	public void MarkCharacterAsSeen(Characters.CharacterType characterType)
	{
		if (_hasCharacterBeenSeen.ContainsKey(characterType))
		{
			_hasCharacterBeenSeen[characterType] = true;
		}
		else
		{
			_hasCharacterBeenSeen.Add(characterType, true);
		}
		_dirty = true;
	}

	private DateTime DefaultExpirationDateForCharacter(Characters.CharacterType cType)
	{
		if (Characters.characterData[cType].defaultExpirationDate != DateTime.MinValue)
		{
			return Characters.characterData[cType].defaultExpirationDate;
		}
		return DateTime.MaxValue;
	}

	public DateTime CharacterExpirationDate(Characters.CharacterType characterType)
	{
		string key = "character_expiretime_" + characterType;
		DateTime result = DateTime.MinValue;
		string valueString;
		if (!OnlineSettings.instance.HasValue(key))
		{
			result = DefaultExpirationDateForCharacter(characterType);
		}
		else if (OnlineSettings.instance.TryGetValue(key, out valueString))
		{
			try
			{
				result = DateTime.Parse(valueString, DATETIME_CULTUREINFO);
			}
			catch (FormatException)
			{
				result = DefaultExpirationDateForCharacter(characterType);
				LogWarning(string.Format("Unable to convert '{0}'.", valueString));
				LogWarning("Reading from online failed ");
			}
		}
		return result;
	}

	public DateTime CharacterStartAvailabilityDate(Characters.CharacterType characterType)
	{
		string key = "character_starttime_" + characterType;
		DateTime result = DateTime.MinValue;
		string valueString;
		if (!OnlineSettings.instance.HasValue(key))
		{
			result = ((!(Characters.characterData[characterType].defaultStartDate != DateTime.MinValue)) ? DateTime.MinValue : Characters.characterData[characterType].defaultStartDate);
		}
		else if (OnlineSettings.instance.TryGetValue(key, out valueString))
		{
			try
			{
				result = DateTime.Parse(valueString, DATETIME_CULTUREINFO);
			}
			catch (FormatException)
			{
				LogWarning(string.Format("Unable to convert '{0}'.", valueString));
				LogWarning("Reading from online failed ");
			}
		}
		return result;
	}

	public bool isCharacterActive(Characters.CharacterType characterType)
	{
		if (Globals.DEBUG_ALL_CHARS)
		{
			return true;
		}
		if (!Characters.characterData[characterType].hasOnlineSettings)
		{
			return true;
		}
		string key = "character_expiretime_" + characterType;
		string valueString;
		if (OnlineSettings.instance.HasValue(key) && OnlineSettings.instance.TryGetValue(key, out valueString) && valueString == "disabled")
		{
			return false;
		}
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.ExpiredLimitedCharacter || eventType == SSEventType.CharacterDay)
			{
				bool found = false;
				Characters.CharacterType eventValue = activeEventManager.GetEventValue<Characters.CharacterType>(out found);
				if (eventValue == characterType)
				{
					return true;
				}
			}
		}
		DateTime t = CharacterStartAvailabilityDate(characterType);
		DateTime t2 = CharacterExpirationDate(characterType);
		if (DateTime.Compare(t2, DateTime.Now) > 0 && DateTime.Compare(t, DateTime.Now) < 0)
		{
			return true;
		}
		return false;
	}

	public TimeSpan CharacterExpirationTimespan(Characters.CharacterType characterType)
	{
		string key = "character_expiretime_" + characterType;
		DateTime dateTime = DateTime.Now;
		string valueString;
		if (!OnlineSettings.instance.HasValue(key))
		{
			dateTime = ((!(Characters.characterData[characterType].defaultExpirationDate != DateTime.MinValue)) ? DateTime.MaxValue : Characters.characterData[characterType].defaultExpirationDate);
		}
		else if (OnlineSettings.instance.TryGetValue(key, out valueString))
		{
			dateTime = ((!(valueString == "disabled")) ? CharacterExpirationDate(characterType) : DateTime.Now);
		}
		if (dateTime == DateTime.MinValue)
		{
			dateTime = DateTime.Now;
		}
		return dateTime - DateTime.Now;
	}

	private DateTime DefaultExpirationDateForHoverboard(Hoverboards.BoardType bType)
	{
		if (Hoverboards.boardData[bType].defaultExpirationTime != DateTime.MinValue)
		{
			return Hoverboards.boardData[bType].defaultExpirationTime;
		}
		return DateTime.MaxValue;
	}

	public DateTime BoardExpirationDate(Hoverboards.BoardType boardType)
	{
		string key = "hoverboard_expiretime_" + boardType;
		DateTime result = DateTime.MinValue;
		string valueString;
		if (!OnlineSettings.instance.HasValue(key))
		{
			result = DefaultExpirationDateForHoverboard(boardType);
		}
		else if (OnlineSettings.instance.TryGetValue(key, out valueString))
		{
			try
			{
				result = DateTime.Parse(valueString, DATETIME_CULTUREINFO);
			}
			catch (FormatException)
			{
				result = DefaultExpirationDateForHoverboard(boardType);
				LogWarning(string.Format("Unable to convert '{0}'.", valueString));
				LogWarning("Reading from online failed ");
			}
		}
		return result;
	}

	public DateTime BoardStartAvailabilityDate(Hoverboards.BoardType boardType)
	{
		string key = "hoverboard_starttime_" + boardType;
		DateTime result = DateTime.MinValue;
		string valueString;
		if (!OnlineSettings.instance.HasValue(key))
		{
			result = ((!(Hoverboards.boardData[boardType].defaultStartDate != DateTime.MinValue)) ? DateTime.MinValue : Hoverboards.boardData[boardType].defaultStartDate);
		}
		else if (OnlineSettings.instance.TryGetValue(key, out valueString))
		{
			try
			{
				result = DateTime.Parse(valueString, DATETIME_CULTUREINFO);
			}
			catch (FormatException)
			{
				LogWarning(string.Format("Unable to convert '{0}'.", valueString));
				LogWarning("Reading from online failed ");
			}
		}
		return result;
	}

	public bool isBoardActive(Hoverboards.BoardType boardType)
	{
		if (Globals.DEBUG_ALL_BOARDS)
		{
			return true;
		}
		Hoverboards.Board board = Hoverboards.boardData[boardType];
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.ExpiredLimitedBoard);
		if (activeEventManager != null)
		{
			bool found = false;
			Hoverboards.BoardType eventValue = activeEventManager.GetEventValue<Hoverboards.BoardType>(out found);
			if (found && eventValue == boardType)
			{
				return true;
			}
		}
		if (board.unlockType == Hoverboards.UnlockType.hiddenUntillUnlocked)
		{
			return false;
		}
		if (board.unlockType == Hoverboards.UnlockType.itemHunt && !WeeklyHuntManager.IsBoardAwardedInCurrentPeriod(boardType))
		{
			return false;
		}
		if (!board.hasOnlineTimeLimit)
		{
			return true;
		}
		string key = "hoverboard_expiretime_" + boardType;
		string valueString;
		if (OnlineSettings.instance.HasValue(key) && OnlineSettings.instance.TryGetValue(key, out valueString) && valueString == "disabled")
		{
			return false;
		}
		DateTime t = BoardExpirationDate(boardType);
		DateTime t2 = BoardStartAvailabilityDate(boardType);
		if (DateTime.Compare(t, DateTime.Now) > 0 && DateTime.Compare(t2, DateTime.Now) < 0)
		{
			return true;
		}
		return false;
	}

	public TimeSpan BoardExpirationTimespan(Hoverboards.BoardType boardType)
	{
		string key = "hoverboard_expiretime_" + boardType;
		DateTime dateTime = DateTime.Now;
		string valueString;
		if (!OnlineSettings.instance.HasValue(key))
		{
			dateTime = ((!(Hoverboards.boardData[boardType].defaultExpirationTime != DateTime.MinValue)) ? DateTime.MaxValue : Hoverboards.boardData[boardType].defaultExpirationTime);
		}
		else if (OnlineSettings.instance.TryGetValue(key, out valueString))
		{
			dateTime = ((!(valueString == "disabled")) ? BoardExpirationDate(boardType) : DateTime.Now);
		}
		if (dateTime == DateTime.MinValue)
		{
			dateTime = DateTime.Now;
		}
		return dateTime - DateTime.Now;
	}

	public void UnlockHoverboard(Hoverboards.BoardType boardType)
	{
		if (_hoverboardUnlockStatus.ContainsKey(boardType))
		{
			_hoverboardUnlockStatus[boardType] = true;
		}
		else
		{
			_hoverboardUnlockStatus.Add(boardType, true);
		}
		Action<Hoverboards.BoardType> onBoardUnlocked = OnBoardUnlocked;
		if (onBoardUnlocked != null)
		{
			onBoardUnlocked(boardType);
		}
		_dirty = true;
	}

	public void LockHoverboard(Hoverboards.BoardType boardType)
	{
		if (_hoverboardUnlockStatus.ContainsKey(boardType))
		{
			_hoverboardUnlockStatus[boardType] = false;
			_dirty = true;
		}
	}

	public bool isHoverboardUnlocked(Hoverboards.BoardType boardType)
	{
		if (Globals.DEBUG_ALL_BOARDS)
		{
			return true;
		}
		if (Hoverboards.boardData[boardType].unlockType == Hoverboards.UnlockType.alwaysUnlocked)
		{
			return true;
		}
		if (_hoverboardUnlockStatus.ContainsKey(boardType))
		{
			return _hoverboardUnlockStatus[boardType];
		}
		return false;
	}

	public bool HasHoverboardBeenSeen(Hoverboards.BoardType boardType)
	{
		if (!Hoverboards.boardData[boardType].isNewInThisUpdate)
		{
			return true;
		}
		if (_hasHoverboardBeenSeen.ContainsKey(boardType))
		{
			return _hasHoverboardBeenSeen[boardType];
		}
		return false;
	}

	public void MarkHoverboardAsSeen(Hoverboards.BoardType boardType)
	{
		if (_hasHoverboardBeenSeen.ContainsKey(boardType))
		{
			_hasHoverboardBeenSeen[boardType] = true;
		}
		else
		{
			_hasHoverboardBeenSeen.Add(boardType, true);
		}
		_dirty = true;
	}

	public float GetHoverBoardCoolDown()
	{
		return 5f;
	}

	public int IncrementUnreportedGames()
	{
		_unreportedGames++;
		if (_unreportedGames == 1)
		{
			_unreportedGamesTimestamp = DateTime.Now;
		}
		_dirty = true;
		return _unreportedGames;
	}

	public DateTime GetFirstUnreportedGameTimestamp()
	{
		if (_unreportedGames < 1)
		{
			LogWarning("PlayerInfo.GetFirstUnreportedGameTimestamp should not be called when number of unreported games is < 1");
		}
		return _unreportedGamesTimestamp;
	}

	public void ClearUnreportedGames()
	{
		_unreportedGames = 0;
		_dirty = true;
	}

	public bool[] GetUnlockedTrophies()
	{
		return _unlockedTrophies;
	}

	public int NumberOfUnlockedTrophyForFirstAchievement()
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			if (_unlockedTrophies[i])
			{
				num++;
			}
		}
		return num;
	}

	public void UnlockTrophy(OldTrophies.Trophy trophy)
	{
		if (!_unlockedTrophies[(int)trophy])
		{
			_unlockedTrophies[(int)trophy] = true;
			_dirty = true;
			Action<OldTrophies.Trophy> onTrophyUnlocked = OnTrophyUnlocked;
			if (onTrophyUnlocked != null)
			{
				onTrophyUnlocked(trophy);
			}
		}
	}

	private Stat StatForTrophyType(OldTrophies.Trophy trophy)
	{
		switch (trophy)
		{
		case OldTrophies.Trophy.diamond:
			return Stat.Diamond;
		case OldTrophies.Trophy.goldbar:
			return Stat.Goldbar;
		case OldTrophies.Trophy.goldChainClock:
			return Stat.GoldChainClock;
		case OldTrophies.Trophy.goldChainDollar:
			return Stat.GoldChainDollar;
		case OldTrophies.Trophy.goldSkull:
			return Stat.GoldSkull;
		case OldTrophies.Trophy.headphones:
			return Stat.Headphones;
		case OldTrophies.Trophy.lpBlack:
			return Stat.LpBlack;
		case OldTrophies.Trophy.tapeBlack:
			return Stat.TapeBlack;
		default:
			return Stat.NotSet;
		}
	}

	public void OldTrophySpawned(OldTrophies.Trophy trophy)
	{
		Stat stat = StatForTrophyType(trophy);
		Statistics statistics2;
		Statistics statistics = (statistics2 = _stats);
		Stat stat3;
		Stat stat2 = (stat3 = stat);
		int num = statistics2[stat3];
		statistics[stat2] = num + 1;
	}

	public bool shouldSpawnTrophy(OldTrophies.Trophy trophy)
	{
		Stat stat = StatForTrophyType(trophy);
		return AwardsManager.Instance.ShouldSpawnOldTrophyForStat(stat);
	}

	public int GetDailyWordDaysInARow(out bool mostRecentIsToday)
	{
		int dayOfYear = _dailyWordExpireTime.DayOfYear;
		if (_dailyWordLastPayoutDayOfYear == dayOfYear)
		{
			mostRecentIsToday = true;
			return _dailyWordInRow;
		}
		if (_dailyWordLastPayoutDayOfYear == dayOfYear - 1 || (dayOfYear == 1 && _dailyWordLastPayoutDayOfYear >= 365))
		{
			mostRecentIsToday = false;
			return _dailyWordInRow;
		}
		mostRecentIsToday = false;
		return 0;
	}

	public void CheckIfWeShouldRemoveProgressForDailyQuestInRow()
	{
		bool mostRecentIsToday;
		if (Instance.GetDailyWordDaysInARow(out mostRecentIsToday) == 0 && !mostRecentIsToday)
		{
			MissionsManager.Instance.RemoveProgressForThis(MissionTarget.DailyQuestInRow);
		}
	}

	public void InitDailyWord(string word, int secondsLeft)
	{
		if (Globals.DEBUG_USE_DEBUG_DAILYWORD)
		{
			word = "Day";
			secondsLeft++;
		}
		if (!_dailyWord.Equals(word))
		{
			_dailyWord = word;
			_dailyWordPayedOut = false;
			_dailyWordUnlockedMask = 0;
			_dirty = true;
		}
		DateTime dateTime = _lastKnownServerTime.AddSeconds(secondsLeft);
		DateTime dailyWordExpireDeviceTime = DateTime.UtcNow.AddSeconds(secondsLeft);
		if (!_dailyWordExpireTime.Equals(dateTime))
		{
			_dailyWordExpireTime = dateTime;
			_dailyWordExpireDeviceTime = dailyWordExpireDeviceTime;
			_dirty = true;
		}
		if (DailyLetterPickupManager.Instance != null)
		{
			DailyLetterPickupManager.Instance.UpdateLetter();
		}
	}

	public bool IsActiveDailyLetterEvent()
	{
		return GetNewDailyLetter() != '\0';
	}

	public bool HasDailyWordTimeLeft()
	{
		return IsActiveDailyLetterEvent() && dailyWordTimeLeft.TotalSeconds > 0.0;
	}

	public void PickedupLetter(char letter)
	{
		for (int i = 0; i < _dailyWord.Length; i++)
		{
			if (_dailyWord[i] == letter && !_dailyWordUnlockedMask[i])
			{
				_dailyWordUnlockedMask[i] = true;
				Action onPickedUpLetter = OnPickedUpLetter;
				if (onPickedUpLetter != null)
				{
					onPickedUpLetter();
				}
				_dirty = true;
				break;
			}
		}
		if (!isDailyWordComplete() || _dailyWordPayedOut)
		{
			return;
		}
		bool mostRecentIsToday;
		_dailyWordInRow = GetDailyWordDaysInARow(out mostRecentIsToday) + 1;
		_dailyWordLastPayoutDayOfYear = _dailyWordExpireTime.DayOfYear;
		Action onDailyWordInRowChanged = OnDailyWordInRowChanged;
		if (onDailyWordInRowChanged != null)
		{
			onDailyWordInRowChanged();
		}
		MissionsManager.Instance.PlayerDidThis(MissionTarget.DailyQuests);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.DailyQuestInRow);
		DailyWord.DailyWordReward rewardForDay = DailyWord.GetRewardForDay(_dailyWordInRow - 1);
		if (rewardForDay.type == DailyWord.DailyWordRewardType.Coins)
		{
			if (rewardForDay.coins > 0)
			{
				amountOfCoins += rewardForDay.coins;
				UIScreenController.Instance.coinReward.NewCoinAmount(rewardForDay.coins);
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.LettersCompleteCoins));
			}
		}
		else if (rewardForDay.type == DailyWord.DailyWordRewardType.MysteryBox)
		{
			if (rewardForDay.mysteryBoxType == MysteryBox.Type.Normal)
			{
				RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox);
			}
			else
			{
				RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox);
			}
			UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.LettersCompleteMysteryBox, rewardForDay.mysteryBoxType));
			Instance.willShowSMBAfterDailyChallenge = true;
		}
		else
		{
			LogError("PlayerInfo Unhandled DailyWordRewardType: " + rewardForDay.type);
		}
		_dailyWordPayedOut = true;
		_dirty = true;
		Flurry.LogEventWithAParameter("Daily Challenge completed", "Id", _dailyWord);
	}

	public char GetNewDailyLetter()
	{
		for (int i = 0; i < _dailyWord.Length; i++)
		{
			if (!_dailyWordUnlockedMask[i])
			{
				return _dailyWord[i];
			}
		}
		return '\0';
	}

	public bool isDailyWordComplete()
	{
		if (string.IsNullOrEmpty(_dailyWord))
		{
			return false;
		}
		return (1 << _dailyWord.Length) - 1 == (int)_dailyWordUnlockedMask;
	}

	public ActivePickupSystemVersion GetActivePickupSystemVersion()
	{
		ActivePickupSystemVersion result = ActivePickupSystemVersion.Version2015;
		string valueString;
		if (OnlineSettings.instance.TryGetValue("pickup_system_version", out valueString))
		{
			switch (valueString)
			{
			default:
			{
				int num = 0;
				if (num == 1)
				{
					result = ActivePickupSystemVersion.Version2015;
					break;
				}
				result = ActivePickupSystemVersion.Version2015;
				LogWarning("Incorrect formatting of PickupSystemVersion! - Defaulting.");
				break;
			}
			case "2014":
				result = ActivePickupSystemVersion.Version2014;
				break;
			}
		}
		return result;
	}

	public bool ShouldShowStickersForCharacterThemes()
	{
		bool flag = true;
		if (_hasShownHoverboardPopup && _hasShownFacebookPopup && _hasShownMission1Popup)
		{
			return true;
		}
		return false;
	}

	public bool ShouldShowStickersForHoverboars()
	{
		bool flag = false;
		if (_numberOfBreadCrumbsShownOnFrontPage < 30)
		{
			flag = true;
		}
		if (_hasShownHoverboardPopup || _hasShownCollectPopup || _hasShownFacebookPopup || _hasShownMission1Popup)
		{
			return true;
		}
		return false;
	}

	public void BreadCrumbShownOnFrontPage()
	{
		_numberOfBreadCrumbsShownOnFrontPage++;
		_dirty = true;
	}

	public void RunCompleted()
	{
		_dirty = true;
		_numberOfRunsSinceLastGuideline++;
		_numberOfRuns++;
		if (numberOfRuns >= 4)
		{
			isSaveMePopupEnabled = true;
		}
		if (_numberOfRunsSinceLastGuideline <= 4)
		{
			return;
		}
		_numberOfRunsSinceLastGuideline = 0;
		if (!_hasShownHoverboardPopup && _shouldShowHoverboardPopup)
		{
			return;
		}
		if (!_hasShownHoverboardPopup)
		{
			if (Achievements.Instance.GetNumberOfHoverboardsUsed() == 0 || _instance.GetUpgradeAmount(PowerupType.hoverboard) == 3 - Achievements.Instance.GetNumberOfHoverboardsUsed())
			{
				_shouldShowHoverboardPopup = true;
				_shouldShowFacebookPopup = false;
				_shouldShowCollectPopup = false;
				_shouldShowMission1Popup = false;
				_shouldShowMission2Popup = false;
				_dirty = true;
				return;
			}
			_hasShownHoverboardPopup = true;
			_shouldShowHoverboardPopup = false;
			_dirty = true;
		}
		if (!_hasShownFacebookPopup && !SocialManager.instance.facebookIsLoggedIn && !_instance.hasPayedOutFacebook)
		{
			_shouldShowFacebookPopup = true;
			_dirty = true;
			return;
		}
		if (!_hasShownMission1Popup)
		{
			if (rawMultiplier < 5)
			{
				_shouldShowMission1Popup = true;
				_dirty = true;
				return;
			}
			_hasShownMission1Popup = true;
			_shouldShowMission1Popup = false;
			_dirty = true;
		}
		if (!_hasShownCollectPopup)
		{
			if (!_hasCollectedFromFriends && (SocialManager.instance.facebookIsLoggedIn || SocialManager.instance.gamecenterIsLoggedIn))
			{
				_shouldShowCollectPopup = true;
				_dirty = true;
			}
			else if (_hasCollectedFromFriends)
			{
				_hasShownCollectPopup = true;
				_shouldShowCollectPopup = false;
				_dummyFriendShouldShow = false;
				_dirty = true;
			}
		}
	}

	public bool AddTransactionToHistory(string orderId, string itemId)
	{
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WP8Player)
		{
			return true;
		}
		if (!_inappHistory.ContainsKey(orderId))
		{
			_inappHistory.Add(orderId, itemId);
			_dirty = true;
			return true;
		}
		return false;
	}

	public bool RemoveTransactionFromHistory(string orderId)
	{
		if (_inappHistory.ContainsKey(orderId))
		{
			_inappHistory.Remove(orderId);
			_dirty = true;
			return true;
		}
		return false;
	}

	public bool HasTransactionHistoryItemId(string itemId)
	{
		bool result = false;
		foreach (KeyValuePair<string, string> item in _inappHistory)
		{
			string key = item.Key;
			string value = item.Value;
			if (value == itemId)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public bool IsTransactionInHistory(string orderId)
	{
		return _inappHistory.ContainsKey(orderId);
	}

	public void PrintAllInappHistory()
	{
		string text = Globals.convertStringStringDictionaryToString(_inappHistory);
	}

	public int GetUpgradeTierSum()
	{
		return GetCurrentTier(PowerupType.jetpack) + GetCurrentTier(PowerupType.doubleMultiplier) + GetCurrentTier(PowerupType.coinmagnet) + GetCurrentTier(PowerupType.supersneakers);
	}

	public bool IsUpgradeStackable(PowerupType type)
	{
		return _upgradeAmounts.ContainsKey(type);
	}

	public bool IsUpgradeTiered(PowerupType type)
	{
		return _upgradeTiers.ContainsKey(type);
	}

	public int GetUpgradeAmount(PowerupType type)
	{
		return _upgradeAmounts[type];
	}

	public int GetCurrentTier(PowerupType type)
	{
		if (!_upgradeTiers.ContainsKey(type))
		{
			return 0;
		}
		return _upgradeTiers[type];
	}

	public float GetPowerupDuration(PowerupType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		if (type == PowerupType.hoverboard && Hoverboard.Instance.IsARewardFromMysteryPowerup)
		{
			return MysteryPowerup.GetRandomHoverboardDuration();
		}
		return Upgrades.upgrades[type].durations[GetCurrentTier(type)];
	}

	public float GetPowerupSpeed(PowerupType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		return Upgrades.upgrades[type].speed;
	}

	public float GetPowerupLandSpeed(PowerupType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		return Upgrades.upgrades[type].landSpeed;
	}

	public void IncreasePowerupTier(PowerupType type)
	{
		if (_upgradeTiers.ContainsKey(type))
		{
			_upgradeTiers[type] += 1;
			_dirty = true;
		}
		else
		{
			LogError("Trying to increase tier for a non-tiered upgrade");
		}
	}

	public void UseUpgrade(PowerupType type)
	{
		if (_upgradeAmounts.ContainsKey(type) && _upgradeAmounts[type] > 0)
		{
			Dictionary<PowerupType, int> upgradeAmounts;
			Dictionary<PowerupType, int> dictionary = (upgradeAmounts = _upgradeAmounts);
			PowerupType key2;
			PowerupType key = (key2 = type);
			int num = upgradeAmounts[key2];
			dictionary[key] = num - 1;
			_dirty = true;
			Action<PowerupType, int> action = onPowerupAmountChanged;
			if (action != null)
			{
				action(type, _upgradeAmounts[type]);
			}
			if (type != PowerupType.hoverboard)
			{
			}
		}
		else
		{
			LogError(string.Concat("You are using more upgrades than you have available for the UpgradeType: ", type, ". Or the type of upgrade is not available."));
		}
	}

	public void IncreaseUpgradeAmount(PowerupType type, int amount = 1)
	{
		if (_upgradeAmounts.ContainsKey(type))
		{
			Dictionary<PowerupType, int> upgradeAmounts;
			Dictionary<PowerupType, int> dictionary = (upgradeAmounts = _upgradeAmounts);
			PowerupType key2;
			PowerupType key = (key2 = type);
			int num = upgradeAmounts[key2];
			dictionary[key] = num + amount;
			_dirty = true;
			Action<PowerupType, int> action = onPowerupAmountChanged;
			if (action != null)
			{
				action(type, _upgradeAmounts[type]);
			}
		}
		else
		{
			LogError("Trying to increase upgrade amount for a non-consumable");
		}
	}

	public void ConvertHeadstartsToMegaHeadstarts()
	{
		if (_upgradeAmounts[PowerupType.headstart500] > 0)
		{
			IncreaseUpgradeAmount(PowerupType.headstart2000, _upgradeAmounts[PowerupType.headstart500]);
			_upgradeAmounts[PowerupType.headstart500] = 0;
		}
	}

	public void UpdateLastPurchasedBundle()
	{
		switch (lastPurchasedBundle)
		{
		case InAppData.BundleTier.None:
			lastPurchasedBundle = InAppData.BundleTier.BundleTier1;
			break;
		case InAppData.BundleTier.BundleTier1:
			lastPurchasedBundle = InAppData.BundleTier.BundleTier2;
			break;
		case InAppData.BundleTier.BundleTier2:
			lastPurchasedBundle = InAppData.BundleTier.BundleTier3;
			break;
		case InAppData.BundleTier.BundleTier3:
			lastPurchasedBundle = InAppData.BundleTier.BundleTier4;
			break;
		case InAppData.BundleTier.BundleTier4:
			lastPurchasedBundle = InAppData.BundleTier.BundleTier5;
			break;
		case InAppData.BundleTier.BundleTier5:
			Debug.LogError("Trying to update last purchased bundle when there are no more tiers. Last purchased bundle is " + lastPurchasedBundle);
			break;
		default:
			Debug.LogError("No know bundle tier: " + lastPurchasedBundle);
			break;
		}
	}

	private static string GetSavePath()
	{
		return DeviceUtility.GetUserDataPath() + "/playerdata";
	}

	private static bool TryGetLoadPaths(out string path)
	{
		string userDataPath = DeviceUtility.GetUserDataPath();
		if (string.IsNullOrEmpty(userDataPath))
		{
			path = null;
			return false;
		}
		path = userDataPath + "/playerdata";
		return true;
	}

	public void SaveIfDirty()
	{
		if (_dirty || _stats.dirty || _hoverboardCustomizationData.IsDirty || _collectCoinsDummyData.IsDirty)
		{
			Save();
		}
	}

	private void Save()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(1);
			Dictionary<Key, string> dict = new Dictionary<Key, string>();
			SaveInt(dict, Key.AmountOfCoins, amountOfCoins);
			SaveInt(dict, Key.HighestScore, _highestScore);
			SaveString(dict, Key.DailyWord, _dailyWord);
			SaveInt(dict, Key.DailyWordUnlockMask, _dailyWordUnlockedMask);
			SaveDateTime(dict, Key.DailyWordExpireTime, _dailyWordExpireTime);
			SaveBool(dict, Key.DailyWordPayedOut, _dailyWordPayedOut);
			SaveInt(dict, Key.CurrentCharacter, _currentCharacter);
			SaveInt(dict, Key.CurrentMissionSet, _currentMissionSet);
			SaveInt(dict, Key.AmountOfMysteryBoxesOpened, _amountOfMysteryBoxesOpened);
			SaveInt(dict, Key.AmountOfSuperMysteryBoxesOpened, _amountOfSuperMysteryBoxesOpened);
			SaveBool(dict, Key.HasFirstMBKey, _hasReceivedFirstMBKey);
			SaveBool(dict, Key.HasFirstSMBKey, _hasReceivedFirstSMBKey);
			SaveBool(dict, Key.TutorialCompleted, _isTutorialCompleted);
			SaveInt(dict, Key.InAppPurchaseCount, _inAppLegacyPurchaseCount);
			SaveInt(dict, Key.InAppConsumablePurchaseCount, _inAppConsumablePurchaseCount);
			SaveInt(dict, Key.InAppNonconsumablePurchaseCount, _inAppNonconsumablePurchaseCount);
			SaveInt(dict, Key.InAppRestoredPurchaseCount, _inAppRestoredPurchaseCount);
			SaveString(dict, Key.EarnCurrencyData, _earnCurrenyData);
			SaveBool(dict, Key.PayBonusFacebook, _hasPayedOutFacebook);
			SaveBool(dict, Key.PayBonusFacebookPack, _hasPayedOutFacebookPack);
			SaveBool(dict, Key.PayBonusFacebookCharacter, _hasPayedOutFacebookCharacter);
			SaveBool(dict, Key.PayBonusGameCenter, _hasPayedOutGameCenter);
			SaveBool(dict, Key.DoubleCoins, _hasDoubleCoins);
			SaveEnum(dict, Key.LastPurchasedBundle, _lastPurchasedBundle);
			SaveString(dict, Key.LastTopRunResultsAwardedID, _lastTopRunResultAwardedID);
			SaveString(dict, Key.PendingTopRunResultsID, _pendingTopRunResultID);
			SaveInt(dict, Key.PendingTopRunResultScore, _pendingTopRunResultsScore);
			SaveInt(dict, Key.PendingTopRunResultRank, _pendingTopRunResultsRank);
			SaveInt(dict, Key.PendingTopRunResultWeek, _pendingTopRunResultsWeek);
			SaveInt(dict, Key.LastWeekMedalWasShown, _lastWeekMedalWasShown);
			SaveInt(dict, Key.NumberOfRunsSinceLastGuideline, _numberOfRunsSinceLastGuideline);
			SaveBool(dict, Key.HasShownHoverboardPopup, _hasShownHoverboardPopup);
			SaveBool(dict, Key.HasShownAwardsIntroductionPopup, _hasShownAwardsIntroductionPopup);
			SaveBool(dict, Key.HasShownHoverboardCustomizationPopup, _hasShownHoverboardCustmoizationPopup);
			SaveBool(dict, Key.HasShownTurboHeadstartIntroductionPopup, _hasShownTurboHeadstartIntroductionPopup);
			SaveBool(dict, Key.HasShownTopRunIntroductionPopup, _hasShownTopRunIntroductionPopup);
			SaveInt(dict, Key.LastShownTopRunIntroPopupVersionNumber, _lastShownTopRunIntroPopupVersionNumber);
			SaveBool(dict, Key.ShouldShowHoverboardPopup, _shouldShowHoverboardPopup);
			SaveBool(dict, Key.HasShownFacebookPopup, _hasShownFacebookPopup);
			SaveBool(dict, Key.ShouldShowFacebookPopup, _shouldShowFacebookPopup);
			SaveBool(dict, Key.HasShownDuelIntroductionPopup, _hasShownDuelIntroductionPopup);
			SaveBool(dict, Key.ShouldShowDuelIntroductionPopup, _shouldShowDuelIntroductionPopup);
			SaveBool(dict, Key.ShouldShowDuelsBreadcrumb, _shouldShowDuelsBreadcrumb);
			SaveBool(dict, Key.HasShownMission1Popup, _hasShownMission1Popup);
			SaveBool(dict, Key.ShouldShowMission1Popup, _shouldShowMission1Popup);
			SaveBool(dict, Key.HasShownCollectPopup, _hasShownCollectPopup);
			SaveBool(dict, Key.ShouldShowCollectPopup, _shouldShowCollectPopup);
			SaveBool(dict, Key.HasShownMission2Popup, _hasShownMission2Popup);
			SaveBool(dict, Key.ShouldShowMission2Popup, _shouldShowMission2Popup);
			SaveBool(dict, Key.HasShownKeysPopup, _hasShownKeysPopup);
			SaveInt(dict, Key.LatestAvailableBundlesVersion, _latestAvailableAssetBundlesVersion);
			SaveBool(dict, Key.AllBundlesDownloaded, _allAssetBundlesDownloaded);
			SaveBool(dict, Key.DummyFriendCollected, _dummyFriendCollected);
			SaveBool(dict, Key.DummyFriendShouldShow, _dummyFriendShouldShow);
			SaveInt(dict, Key.NumberOfTotalRuns, _ratingAskCounter);
			if (_currentMissionSet >= 0)
			{
				SaveIntArray(dict, Key.CurrentMissionSetProgress, _currentMissionProgress);
			}
			SaveIntArray(dict, Key.CollectedCharacterTokens, _collectedCharacterTokens);
			SaveBoolArray(dict, Key.UnlockedTrophies, _unlockedTrophies);
			SaveIntArray(dict, Key.AchievementProgress, _achievementProgress);
			SaveInt(dict, Key.DailyWordInRow, _dailyWordInRow);
			SaveInt(dict, Key.DailyWordLastPayoutDayOfYear, _dailyWordLastPayoutDayOfYear);
			SaveInt(dict, Key.MissionCompletedSum, _missionCompletedSum);
			SaveString(dict, Key.HasHoverboardsBeenSeen, Globals.convertEnumBoolDictionaryToString(_hasHoverboardBeenSeen));
			SaveString(dict, Key.UnlockedHoverboardTypes, Globals.convertEnumBoolDictionaryToString(_hoverboardUnlockStatus));
			SaveEnum(dict, Key.CurrentHoverboard, _currentHoverboard);
			SaveEnum(dict, Key.PreviousHoverboard, _previousHoverboard);
			SaveInt(dict, Key.BreadCrumbsShownOnFrontPage, _numberOfBreadCrumbsShownOnFrontPage);
			SaveInt(dict, Key.UnreportedGames, _unreportedGames);
			SaveLong(dict, Key.UnreportedGamesTimestamp, _unreportedGamesTimestamp.Ticks);
			SaveString(dict, Key.HistoryOfInapp, Globals.convertStringStringDictionaryToString(_inappHistory));
			SaveString(dict, Key.HasCharacterBeenSeen, Globals.convertEnumBoolDictionaryToString(_hasCharacterBeenSeen));
			SaveBool(dict, Key.NeverAskForRating, _neverAskForRating);
			SaveString(dict, Key.LastVersionRated, _lastVersionRated);
			SaveInt(dict, Key.AmountOfSeasonalEggs, _amountOfSeasonalEggs);
			SaveString(dict, Key.SpecialChallengeData, _weeklyChallangeData);
			SaveBool(dict, Key.HasCollectedFromFriends, _hasCollectedFromFriends);
			SaveInt(dict, Key.AmountOfKeys, amountOfKeys);
			SaveString(dict, Key.LastPlayDate, _lastPlayDate);
			SaveString(dict, Key.LastQuitDate, _lastQuitDate);
			SaveString(dict, Key.LastDailyOnlineLog, _lastDailyOnlineLog);
			SaveInt(dict, Key.MissionsVersionLastUpdate, _missionsVersionLastUpdate);
			SaveString(dict, Key.CharacterThemesUnlocked, Globals.convertEnumIntArrayDictionaryToString(_characterThemesUnlocked));
			SaveString(dict, Key.CharacterLastSelectedThemes, Globals.convertEnumIntDictionaryToString(_lastSelectedThemes));
			SaveString(dict, Key.CharacterThemesSeen, Globals.convertEnumIntArrayDictionaryToString(_characterThemesSeen));
			SaveString(dict, Key.PendingRewards, GetStringFromRewards(_pendingRewards));
			SaveBool(dict, Key.RemoteNotificationsEnabled, _remoteNotificationsEnabled);
			SaveBool(dict, Key.IsFreshInstall, _isFreshInstall);
			SaveLong(dict, Key.LastEventID, _lastEventID);
			SaveString(dict, Key.LastEventItemID, _lastEventItemID);
			SaveBool(dict, Key.ShouldRestoreAfterEvent, _shouldRestoreAfterEvent);
			SaveLong(dict, Key.LastTimeAnEventHasBeenTried, _lastTimeAnEventHasBeenTried);
			SaveLong(dict, Key.LastTimeAnEventWasStarted, _lastTimeAnEventWasStarted);
			SaveLong(dict, Key.LastTimeEventPopupHasBeenForcedShown, _lastTimeEventPopupHasBeenForcedShown);
			SaveDateTime(dict, Key.DailyInterstitialExpireTime, _dailyInterstitialExpireTime);
			SaveDateTime(dict, Key.LastTimeInterstitialWasSeen, _lastTimeInterstitialWasSeen);
			SaveDateTime(dict, Key.LastTimeInterstitialProviderHasChanged, _lastTimeInterstitialProviderHasChanged);
			SaveDateTime(dict, Key.HourlyInterstitialExpireTime, _hourlyInterstitialExpireTime);
			SaveInt(dict, Key.NumberOfInterstitialsPerDay, _numberOfInterstitialsPerDay);
			SaveInt(dict, Key.NumberOfInterstitialsPerHour, _numberOfInterstitialsPerHour);
			SaveInt(dict, Key.InterstitialProviderListVersion, _interstitialProviderListVersion);
			SaveBool(dict, Key.IsSaveMePopupEnabled, _isSaveMePopupEnabled);
			SaveInt(dict, Key.NumberOfRuns, _numberOfRuns);
			SaveInt(dict, Key.LastCharacterSentToServer, _lastCharacterSentToServer);
			SaveInt(dict, Key.LastCharacterThemeSentToServer, _lastCharacterThemeSentToServer);
			SaveDateTime(dict, Key.LastKnownServerTime, _lastKnownServerTime);
			SaveDateTime(dict, Key.LastTimeAVideoForKeysWasSeen, _lastTimeAVideoForKeysWasSeen);
			SaveDateTime(dict, Key.LastTimeAWeeklyFacebookGiftWasCollected, _weeklyGiftEndTime);
			SaveDateTime(dict, Key.LastTimeAppWasUpdatedOrInstalled, _lastTimeAppWasUpdatedOrInstalled);
			SaveDateTime(dict, Key.FirstInstallDate, _firstInstallDate);
			SaveString(dict, Key.Stats, _stats.ToString());
			SaveString(dict, Key.HoverboardCustomizationData, _hoverboardCustomizationData.ToSerializedString());
			SaveString(dict, Key.CollectCoinsDummyData, _collectCoinsDummyData.ToSerializedString());
			SaveString(dict, Key.AwardStatus, _trophyMissionData);
			SaveString(dict, Key.CharacterNameEventData, _characterNameEventData);
			SaveBool(dict, Key.HasCollectedFirstAwardReward, _hasCollectedFirstAwardReward);
			SaveBool(dict, Key.HasCollectedFacebookWeeklyGiftThisWeek, _hasCollectedFacebookWeeklyGiftThisWeek);
			SaveInt(dict, Key.AmountOfMiniMysteryBoxesOpened, _amountOfMiniMysteryBoxesOpened);
			if (!string.IsNullOrEmpty(_flurryUserId))
			{
				SaveString(dict, Key.FlurryUserId, _flurryUserId);
			}
			FileUtil.WriteEnumStringDictionary(binaryWriter, dict);
			FileUtil.WriteEnumIntDictionary(binaryWriter, _upgradeAmounts);
			FileUtil.WriteEnumIntDictionary(binaryWriter, _upgradeTiers);
			FileUtil.Save(GetSavePath(), memoryStream.GetBuffer(), "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn", 0, (int)memoryStream.Length, 1, 5);
			memoryStream.Close();
			_dirty = false;
		}
		catch (Exception ex)
		{
			LogError("Error saving player info: " + ex);
		}
	}

	private void SaveString(Dictionary<Key, string> dict, Key key, string value)
	{
		dict[key] = value;
	}

	private string LoadString(Dictionary<Key, string> dict, Key key, string defaultValue)
	{
		_flurry_nextKeyToRead = key;
		string value;
		if (dict.TryGetValue(key, out value))
		{
			return value;
		}
		return defaultValue;
	}

	private void SaveInt(Dictionary<Key, string> dict, Key key, int value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private int LoadInt(Dictionary<Key, string> dict, Key key, int defaultValue)
	{
		_flurry_nextKeyToRead = key;
		string value;
		int result;
		if (dict.TryGetValue(key, out value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private void SaveLong(Dictionary<Key, string> dict, Key key, long value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private long LoadLong(Dictionary<Key, string> dict, Key key, long defaultValue)
	{
		_flurry_nextKeyToRead = key;
		string value;
		long result;
		if (dict.TryGetValue(key, out value) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private void SaveBool(Dictionary<Key, string> dict, Key key, bool value)
	{
		dict[key] = value.ToString();
	}

	private bool LoadBool(Dictionary<Key, string> dict, Key key, bool defaultValue)
	{
		_flurry_nextKeyToRead = key;
		string value;
		bool result;
		if (dict.TryGetValue(key, out value) && bool.TryParse(value, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private void SaveDateTime(Dictionary<Key, string> dict, Key key, DateTime value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private DateTime LoadDateTime(Dictionary<Key, string> dict, Key key, DateTime defaultValue)
	{
		_flurry_nextKeyToRead = key;
		string value;
		if (dict.TryGetValue(key, out value))
		{
			return Utils.StringToDateTime(value, defaultValue);
		}
		return defaultValue;
	}

	private void SaveEnum<T>(Dictionary<Key, string> dict, Key key, T value)
	{
		dict[key] = value.ToString();
	}

	private T LoadEnum<T>(Dictionary<Key, string> dict, Key key, T defaultValue)
	{
		_flurry_nextKeyToRead = key;
		string value;
		if (dict.TryGetValue(key, out value))
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value);
			}
			catch (Exception)
			{
			}
		}
		return defaultValue;
	}

	private void SaveIntArray(Dictionary<Key, string> dict, Key key, int[] value)
	{
		dict[key] = string.Join(",", Globals.convertAllIntToString(value));
	}

	private int[] LoadIntArray(Dictionary<Key, string> dict, Key key, int[] defaultValue)
	{
		_flurry_nextKeyToRead = key;
		string value;
		if (dict.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
		{
			return Globals.convertAllStringToInt(value.Split(','));
		}
		return defaultValue;
	}

	private void SaveBoolArray(Dictionary<Key, string> dict, Key key, bool[] value)
	{
		dict[key] = string.Join(",", Globals.convertAllBoolToString(value));
	}

	private bool[] LoadBoolArray(Dictionary<Key, string> dict, Key key, bool[] defaultValue)
	{
		_flurry_nextKeyToRead = key;
		string value;
		if (dict.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
		{
			return Globals.convertAllStringToBool(value.Split(','));
		}
		return defaultValue;
	}

	public void Load()
	{
		if (false)
		{
			InitNew();
			return;
		}
		try
		{
			string path;
			if (!TryGetLoadPaths(out path))
			{
				Flurry.LogEventWithAParameter("PlayerInfo.Load Empty path", "Stacktrace", new StackTrace().ToString());
			}
			byte[] buffer = FileUtil.Load(path, "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn");
			MemoryStream memoryStream = new MemoryStream(buffer);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			binaryReader.ReadInt32();
			Dictionary<Key, string> dict = FileUtil.ReadEnumStringDictionary<Key>(binaryReader);
			amountOfCoins = LoadInt(dict, Key.AmountOfCoins, 0);
			ResetHighestScoreTo(LoadInt(dict, Key.HighestScore, 0));
			_dailyWord = LoadString(dict, Key.DailyWord, string.Empty);
			_dailyWordUnlockedMask = LoadInt(dict, Key.DailyWordUnlockMask, 0);
			_dailyWordExpireTime = LoadDateTime(dict, Key.DailyWordExpireTime, DateTime.UtcNow);
			_dailyWordPayedOut = LoadBool(dict, Key.DailyWordPayedOut, false);
			_currentCharacter = LoadInt(dict, Key.CurrentCharacter, 0);
			_currentMissionSet = LoadInt(dict, Key.CurrentMissionSet, -1);
			_amountOfMysteryBoxesOpened = LoadInt(dict, Key.AmountOfMysteryBoxesOpened, 0);
			_amountOfSuperMysteryBoxesOpened = LoadInt(dict, Key.AmountOfSuperMysteryBoxesOpened, 0);
			_hasReceivedFirstMBKey = LoadBool(dict, Key.HasFirstMBKey, false);
			_hasReceivedFirstSMBKey = LoadBool(dict, Key.HasFirstSMBKey, false);
			_lastTopRunResultAwardedID = LoadString(dict, Key.LastTopRunResultsAwardedID, string.Empty);
			_pendingTopRunResultID = LoadString(dict, Key.PendingTopRunResultsID, string.Empty);
			_pendingTopRunResultsScore = LoadInt(dict, Key.PendingTopRunResultScore, -1);
			_pendingTopRunResultsRank = LoadInt(dict, Key.PendingTopRunResultRank, -1);
			_pendingTopRunResultsWeek = LoadInt(dict, Key.PendingTopRunResultWeek, -1);
			_lastWeekMedalWasShown = LoadInt(dict, Key.LastWeekMedalWasShown, -1);
			_isTutorialCompleted = LoadBool(dict, Key.TutorialCompleted, false);
			_inAppLegacyPurchaseCount = LoadInt(dict, Key.InAppPurchaseCount, 0);
			_inAppConsumablePurchaseCount = LoadInt(dict, Key.InAppConsumablePurchaseCount, 0);
			_inAppNonconsumablePurchaseCount = LoadInt(dict, Key.InAppNonconsumablePurchaseCount, 0);
			_inAppRestoredPurchaseCount = LoadInt(dict, Key.InAppRestoredPurchaseCount, 0);
			_earnCurrenyData = LoadString(dict, Key.EarnCurrencyData, string.Empty);
			_hasPayedOutFacebook = LoadBool(dict, Key.PayBonusFacebook, false);
			_hasPayedOutFacebookPack = LoadBool(dict, Key.PayBonusFacebookPack, false);
			_hasPayedOutFacebookCharacter = LoadBool(dict, Key.PayBonusFacebookCharacter, false);
			_hasPayedOutGameCenter = LoadBool(dict, Key.PayBonusGameCenter, false);
			_hasDoubleCoins = LoadBool(dict, Key.DoubleCoins, false);
			_lastPurchasedBundle = LoadEnum(dict, Key.LastPurchasedBundle, InAppData.BundleTier.None);
			_numberOfRunsSinceLastGuideline = LoadInt(dict, Key.NumberOfRunsSinceLastGuideline, 0);
			_ratingAskCounter = LoadInt(dict, Key.NumberOfTotalRuns, 0);
			_hasShownHoverboardPopup = LoadBool(dict, Key.HasShownHoverboardPopup, false);
			_hasShownAwardsIntroductionPopup = LoadBool(dict, Key.HasShownAwardsIntroductionPopup, false);
			_hasShownTopRunIntroductionPopup = LoadBool(dict, Key.HasShownTopRunIntroductionPopup, false);
			_lastShownTopRunIntroPopupVersionNumber = LoadInt(dict, Key.LastShownTopRunIntroPopupVersionNumber, 0);
			_hasShownHoverboardCustmoizationPopup = LoadBool(dict, Key.HasShownHoverboardCustomizationPopup, false);
			_hasShownTurboHeadstartIntroductionPopup = LoadBool(dict, Key.HasShownTurboHeadstartIntroductionPopup, false);
			_shouldShowHoverboardPopup = LoadBool(dict, Key.ShouldShowHoverboardPopup, false);
			_hasShownFacebookPopup = LoadBool(dict, Key.HasShownFacebookPopup, false);
			_shouldShowFacebookPopup = LoadBool(dict, Key.ShouldShowFacebookPopup, false);
			_hasShownDuelIntroductionPopup = LoadBool(dict, Key.HasShownDuelIntroductionPopup, false);
			_shouldShowDuelIntroductionPopup = LoadBool(dict, Key.ShouldShowDuelIntroductionPopup, false);
			_shouldShowDuelsBreadcrumb = LoadBool(dict, Key.ShouldShowDuelsBreadcrumb, false);
			_hasShownMission1Popup = LoadBool(dict, Key.HasShownMission1Popup, false);
			_shouldShowMission1Popup = LoadBool(dict, Key.ShouldShowMission1Popup, false);
			_hasShownCollectPopup = LoadBool(dict, Key.HasShownCollectPopup, false);
			_shouldShowCollectPopup = LoadBool(dict, Key.ShouldShowCollectPopup, false);
			_hasShownMission2Popup = LoadBool(dict, Key.HasShownMission2Popup, false);
			_shouldShowMission2Popup = LoadBool(dict, Key.ShouldShowMission2Popup, false);
			_hasShownKeysPopup = LoadBool(dict, Key.HasShownKeysPopup, false);
			_hasCollectedFromFriends = LoadBool(dict, Key.HasCollectedFromFriends, false);
			_latestAvailableAssetBundlesVersion = LoadInt(dict, Key.LatestAvailableBundlesVersion, 0);
			_allAssetBundlesDownloaded = LoadBool(dict, Key.AllBundlesDownloaded, false);
			_dummyFriendCollected = LoadBool(dict, Key.DummyFriendCollected, false);
			_dummyFriendShouldShow = LoadBool(dict, Key.DummyFriendShouldShow, false);
			_amountOfSeasonalEggs = LoadInt(dict, Key.AmountOfSeasonalEggs, 0);
			_weeklyChallangeData = LoadString(dict, Key.SpecialChallengeData, string.Empty);
			amountOfKeys = LoadInt(dict, Key.AmountOfKeys, 5);
			_lastPlayDate = LoadString(dict, Key.LastPlayDate, string.Empty);
			_lastQuitDate = LoadString(dict, Key.LastQuitDate, string.Empty);
			_lastDailyOnlineLog = LoadString(dict, Key.LastDailyOnlineLog, string.Empty);
			_missionsVersionLastUpdate = LoadInt(dict, Key.MissionsVersionLastUpdate, 0);
			_pendingRewards = GetRewardsFromString(LoadString(dict, Key.PendingRewards, string.Empty));
			_lastEventID = LoadLong(dict, Key.LastEventID, 0L);
			_lastEventItemID = LoadString(dict, Key.LastEventItemID, string.Empty);
			_shouldRestoreAfterEvent = LoadBool(dict, Key.ShouldRestoreAfterEvent, false);
			_lastTimeAnEventHasBeenTried = LoadLong(dict, Key.LastTimeAnEventHasBeenTried, 0L);
			_lastTimeAnEventWasStarted = LoadLong(dict, Key.LastTimeAnEventWasStarted, 0L);
			_lastTimeEventPopupHasBeenForcedShown = LoadLong(dict, Key.LastTimeEventPopupHasBeenForcedShown, 0L);
			_remoteNotificationsEnabled = LoadBool(dict, Key.RemoteNotificationsEnabled, true);
			_isFreshInstall = LoadBool(dict, Key.IsFreshInstall, true);
			_dailyInterstitialExpireTime = LoadDateTime(dict, Key.DailyInterstitialExpireTime, DateTime.UtcNow.AddHours(24.0));
			_lastTimeInterstitialWasSeen = LoadDateTime(dict, Key.LastTimeInterstitialWasSeen, DateTime.MinValue);
			_lastTimeInterstitialProviderHasChanged = LoadDateTime(dict, Key.LastTimeInterstitialProviderHasChanged, DateTime.MinValue);
			_hourlyInterstitialExpireTime = LoadDateTime(dict, Key.HourlyInterstitialExpireTime, DateTime.UtcNow.AddHours(1.0));
			_numberOfInterstitialsPerDay = LoadInt(dict, Key.NumberOfInterstitialsPerDay, 0);
			_numberOfInterstitialsPerHour = LoadInt(dict, Key.NumberOfInterstitialsPerHour, 0);
			_interstitialProviderListVersion = LoadInt(dict, Key.InterstitialProviderListVersion, 0);
			_isSaveMePopupEnabled = LoadBool(dict, Key.IsSaveMePopupEnabled, _isTutorialCompleted);
			_numberOfRuns = LoadInt(dict, Key.NumberOfRuns, 0);
			_lastCharacterSentToServer = LoadInt(dict, Key.LastCharacterSentToServer, 0);
			_lastCharacterThemeSentToServer = LoadInt(dict, Key.LastCharacterThemeSentToServer, 0);
			_lastKnownServerTime = LoadDateTime(dict, Key.LastKnownServerTime, DateTime.MinValue);
			_lastTimeAVideoForKeysWasSeen = LoadDateTime(dict, Key.LastTimeAVideoForKeysWasSeen, DateTime.MinValue);
			_weeklyGiftEndTime = LoadDateTime(dict, Key.LastTimeAWeeklyFacebookGiftWasCollected, DateTime.MinValue);
			_lastTimeAppWasUpdatedOrInstalled = LoadDateTime(dict, Key.LastTimeAppWasUpdatedOrInstalled, DateTime.Now);
			_firstInstallDate = LoadDateTime(dict, Key.FirstInstallDate, DateTime.MinValue);
			_stats = Statistics.Parse(LoadString(dict, Key.Stats, string.Empty));
			_hoverboardCustomizationData = new HoverboardCustomizationData(LoadString(dict, Key.HoverboardCustomizationData, string.Empty));
			_collectCoinsDummyData = new CollectCoinsDummyData(LoadString(dict, Key.CollectCoinsDummyData, string.Empty));
			_trophyMissionData = LoadString(dict, Key.AwardStatus, string.Empty);
			_characterNameEventData = LoadString(dict, Key.CharacterNameEventData, string.Empty);
			_hasCollectedFirstAwardReward = LoadBool(dict, Key.HasCollectedFirstAwardReward, false);
			_hasCollectedFacebookWeeklyGiftThisWeek = LoadBool(dict, Key.HasCollectedFacebookWeeklyGiftThisWeek, false);
			_currentMissionProgress = LoadIntArray(dict, Key.CurrentMissionSetProgress, null);
			_amountOfMiniMysteryBoxesOpened = LoadInt(dict, Key.AmountOfMiniMysteryBoxesOpened, 0);
			_flurryUserId = LoadString(dict, Key.FlurryUserId, string.Empty);
			int[] array = LoadIntArray(dict, Key.CollectedCharacterTokens, null);
			for (int i = 0; i < _collectedCharacterTokens.Length; i++)
			{
				_collectedCharacterTokens[i] = 0;
			}
			if (array != null)
			{
				int length = Mathf.Min(array.Length, _collectedCharacterTokens.Length);
				Array.Copy(array, _collectedCharacterTokens, length);
			}
			bool[] array2 = LoadBoolArray(dict, Key.UnlockedTrophies, null);
			for (int j = 0; j < _unlockedTrophies.Length; j++)
			{
				_unlockedTrophies[j] = false;
			}
			if (array2 != null)
			{
				int length2 = Mathf.Min(array2.Length, _unlockedTrophies.Length);
				Array.Copy(array2, _unlockedTrophies, length2);
			}
			int[] array3 = LoadIntArray(dict, Key.AchievementProgress, null);
			for (int k = 0; k < _achievementProgress.Length; k++)
			{
				_achievementProgress[k] = 0;
			}
			if (array3 != null)
			{
				int length3 = Mathf.Min(array3.Length, _achievementProgress.Length);
				Array.Copy(array3, _achievementProgress, length3);
			}
			_dailyWordInRow = LoadInt(dict, Key.DailyWordInRow, 0);
			_dailyWordLastPayoutDayOfYear = LoadInt(dict, Key.DailyWordLastPayoutDayOfYear, 0);
			_missionCompletedSum = LoadInt(dict, Key.MissionCompletedSum, 0);
			_hasHoverboardBeenSeen = Globals.convertStringToEnumBoolDictionary<Hoverboards.BoardType>(LoadString(dict, Key.HasHoverboardsBeenSeen, string.Empty));
			string text = LoadString(dict, Key.UnlockedHoverboardTypes, string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				if (!text.Contains(Hoverboards.BoardType.kitty.ToString()))
				{
					text = text.Replace("fortune", Hoverboards.BoardType.kitty.ToString());
				}
				if (!text.Contains(Hoverboards.BoardType.flamingo.ToString()))
				{
					text = text.Replace("miaminewboard", Hoverboards.BoardType.flamingo.ToString());
				}
			}
			_hoverboardUnlockStatus = Globals.convertStringToEnumBoolDictionary<Hoverboards.BoardType>(text);
			_inappHistory = Globals.convertStringToStringStringDictionary(LoadString(dict, Key.HistoryOfInapp, string.Empty));
			_currentHoverboard = LoadEnum(dict, Key.CurrentHoverboard, Hoverboards.BoardType.normal);
			_previousHoverboard = LoadEnum(dict, Key.PreviousHoverboard, Hoverboards.BoardType.normal);
			_numberOfBreadCrumbsShownOnFrontPage = LoadInt(dict, Key.BreadCrumbsShownOnFrontPage, 0);
			_unreportedGames = LoadInt(dict, Key.UnreportedGames, 0);
			_unreportedGamesTimestamp = new DateTime(LoadLong(dict, Key.UnreportedGamesTimestamp, 0L));
			_hasCharacterBeenSeen = Globals.convertStringToEnumBoolDictionary<Characters.CharacterType>(LoadString(dict, Key.HasCharacterBeenSeen, string.Empty));
			_characterThemesUnlocked = Globals.convertStringToEnumIntArrayDictionary<Characters.CharacterType>(LoadString(dict, Key.CharacterThemesUnlocked, string.Empty));
			_lastSelectedThemes = Globals.convertStringToEnumIntDictionary<Characters.CharacterType>(LoadString(dict, Key.CharacterLastSelectedThemes, string.Empty));
			_neverAskForRating = LoadBool(dict, Key.NeverAskForRating, false);
			_lastVersionRated = LoadString(dict, Key.LastVersionRated, string.Empty);
			_characterThemesSeen = Globals.convertStringToEnumIntArrayDictionary<Characters.CharacterType>(LoadString(dict, Key.CharacterThemesSeen, string.Empty));
			Dictionary<PowerupType, int> dictionary = FileUtil.ReadEnumIntDictionary<PowerupType>(binaryReader);
			foreach (KeyValuePair<PowerupType, int> item in dictionary)
			{
				_upgradeAmounts[item.Key] = item.Value;
			}
			Dictionary<PowerupType, int> dictionary2 = FileUtil.ReadEnumIntDictionary<PowerupType>(binaryReader);
			foreach (KeyValuePair<PowerupType, int> item2 in dictionary2)
			{
				_upgradeTiers[item2.Key] = item2.Value;
			}
			memoryStream.Close();
			_dirty = false;
		}
		catch (Exception ex)
		{
			LogWarning("Error loading player info: " + ex);
			InitNew();
			if (ex is FileNotFoundException)
			{
				if (PlayerPrefs.HasKey("playerinfo_filenotfound_once"))
				{
					Flurry.LogEventWithAParameter("PlayerInfo.Load FileNotFound", "Exception", ex.Message + " \n Stacktrace: " + new StackTrace());
				}
				else
				{
					PlayerPrefs.SetInt("playerinfo_filenotfound_once", 1);
				}
				return;
			}
			Flurry.LogEventWithAParameter("PlayerInfo.Load Exception", "Exception", string.Concat("Key '", _flurry_nextKeyToRead, "': ", ex.Message, " \n Stacktrace: ", new StackTrace()));
		}
	}

	private static string GetStringFromRewards(List<CelebrationReward> rewards)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (CelebrationReward reward in rewards)
		{
			stringBuilder.Append(reward).Append(";");
		}
		int num = stringBuilder.ToString().LastIndexOf(';');
		if (num >= 0)
		{
			stringBuilder.Remove(num, 1);
		}
		return stringBuilder.ToString();
	}

	private static List<CelebrationReward> GetRewardsFromString(string rewardsAsString)
	{
		List<CelebrationReward> list = new List<CelebrationReward>();
		if (!string.IsNullOrEmpty(rewardsAsString))
		{
			string[] array = rewardsAsString.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					CelebrationReward celebrationReward = new CelebrationReward();
					celebrationReward.PopulateFromString(text);
					list.Add(celebrationReward);
				}
			}
		}
		return list;
	}

	public void InitNew()
	{
		amountOfCoins = 0;
		_highestScore = 0;
		_oldHighestScore = _highestScore;
		_dailyWord = string.Empty;
		_dailyWordUnlockedMask = 0;
		_dailyWordExpireTime = DateTime.UtcNow;
		_dailyWordPayedOut = false;
		_dailyWordInRow = 0;
		_dailyWordLastPayoutDayOfYear = 0;
		_amountOfMysteryBoxesOpened = 0;
		_amountOfMiniMysteryBoxesOpened = 0;
		_amountOfSuperMysteryBoxesOpened = 0;
		_hasReceivedFirstMBKey = false;
		_hasReceivedFirstSMBKey = false;
		_lastTopRunResultAwardedID = string.Empty;
		_pendingTopRunResultID = string.Empty;
		_pendingTopRunResultsScore = -1;
		_pendingTopRunResultsRank = -1;
		_pendingTopRunResultsWeek = -1;
		_lastWeekMedalWasShown = -1;
		_amountOfSeasonalEggs = 0;
		_weeklyChallangeData = string.Empty;
		amountOfKeys = 5;
		_currentCharacter = 0;
		_currentMissionSet = -1;
		_currentMissionProgress = null;
		IsTutorialCompleted = false;
		_inAppLegacyPurchaseCount = 0;
		_inAppConsumablePurchaseCount = 0;
		_inAppNonconsumablePurchaseCount = 0;
		_inAppRestoredPurchaseCount = 0;
		_earnCurrenyData = string.Empty;
		_hasPayedOutFacebook = false;
		_hasPayedOutFacebookPack = false;
		_hasPayedOutFacebookCharacter = false;
		_hasPayedOutGameCenter = false;
		_hasDoubleCoins = false;
		_lastPurchasedBundle = InAppData.BundleTier.None;
		_numberOfRunsSinceLastGuideline = 0;
		_ratingAskCounter = 0;
		_hasCollectedFromFriends = false;
		_hasShownHoverboardPopup = false;
		_hasShownAwardsIntroductionPopup = false;
		_hasShownHoverboardCustmoizationPopup = false;
		_hasShownTurboHeadstartIntroductionPopup = false;
		_hasShownTopRunIntroductionPopup = false;
		_lastShownTopRunIntroPopupVersionNumber = 0;
		_shouldShowHoverboardPopup = false;
		_hasShownFacebookPopup = false;
		_shouldShowFacebookPopup = false;
		_hasShownDuelIntroductionPopup = false;
		_shouldShowDuelIntroductionPopup = true;
		_shouldShowDuelsBreadcrumb = true;
		_hasShownMission1Popup = false;
		_shouldShowMission1Popup = false;
		_hasShownCollectPopup = false;
		_shouldShowCollectPopup = false;
		_hasShownMission2Popup = false;
		_shouldShowMission2Popup = false;
		_latestAvailableAssetBundlesVersion = 0;
		_allAssetBundlesDownloaded = false;
		_dummyFriendCollected = false;
		_dummyFriendShouldShow = false;
		_missionsVersionLastUpdate = 0;
		_isSaveMePopupEnabled = false;
		_numberOfRuns = 0;
		_lastCharacterSentToServer = 0;
		_lastCharacterThemeSentToServer = 0;
		_lastKnownServerTime = DateTime.MinValue;
		_lastTimeAVideoForKeysWasSeen = DateTime.MinValue;
		_weeklyGiftEndTime = DateTime.MinValue;
		_lastTimeAppWasUpdatedOrInstalled = DateTime.Now;
		_firstInstallDate = DateTime.MinValue;
		for (int i = 0; i < _collectedCharacterTokens.Length; i++)
		{
			_collectedCharacterTokens[i] = 0;
		}
		for (int j = 0; j < _unlockedTrophies.Length; j++)
		{
			_unlockedTrophies[j] = false;
		}
		for (int k = 0; k < _achievementProgress.Length; k++)
		{
			_achievementProgress[k] = 0;
		}
		Dictionary<PowerupType, int> dictionary = new Dictionary<PowerupType, int>(_upgradeAmounts.Count);
		foreach (PowerupType key in _upgradeAmounts.Keys)
		{
			if (key == PowerupType.hoverboard)
			{
				dictionary[key] = 3;
			}
			else
			{
				dictionary[key] = 0;
			}
		}
		_upgradeAmounts = dictionary;
		dictionary = new Dictionary<PowerupType, int>(_upgradeTiers.Count);
		foreach (PowerupType key2 in _upgradeTiers.Keys)
		{
			dictionary[key2] = 0;
		}
		_upgradeTiers = dictionary;
		_isNew_tasha = true;
		_isNew_zoe = true;
		_isNew_brody = true;
		_isNew_princek = true;
		_missionCompletedSum = 0;
		_hasHoverboardBeenSeen = new Dictionary<Hoverboards.BoardType, bool>();
		_hoverboardUnlockStatus = new Dictionary<Hoverboards.BoardType, bool>();
		_inappHistory = new Dictionary<string, string>();
		_currentHoverboard = Hoverboards.BoardType.normal;
		_previousHoverboard = Hoverboards.BoardType.normal;
		_numberOfBreadCrumbsShownOnFrontPage = 0;
		_unreportedGames = 0;
		_unreportedGamesTimestamp = new DateTime(0L);
		_hasCharacterBeenSeen = new Dictionary<Characters.CharacterType, bool>();
		_neverAskForRating = false;
		_lastVersionRated = string.Empty;
		_characterThemesUnlocked = new Dictionary<Characters.CharacterType, int[]>();
		_lastSelectedThemes = new Dictionary<Characters.CharacterType, int>();
		_characterThemesSeen = new Dictionary<Characters.CharacterType, int[]>();
		_pendingRewards = new List<CelebrationReward>();
		_shouldRestoreAfterEvent = false;
		_remoteNotificationsEnabled = true;
		_isFreshInstall = true;
		_lastEventID = 0L;
		_lastEventItemID = string.Empty;
		_lastTimeAnEventHasBeenTried = 0L;
		_lastTimeAnEventWasStarted = 0L;
		_lastTimeEventPopupHasBeenForcedShown = 0L;
		_dailyInterstitialExpireTime = DateTime.UtcNow.AddHours(24.0);
		_lastTimeInterstitialWasSeen = DateTime.MinValue;
		_lastTimeInterstitialProviderHasChanged = DateTime.MinValue;
		_hourlyInterstitialExpireTime = DateTime.UtcNow.AddHours(1.0);
		_numberOfInterstitialsPerDay = 0;
		_numberOfInterstitialsPerHour = 0;
		_interstitialProviderListVersion = 0;
		_stats = new Statistics();
		_hoverboardCustomizationData = new HoverboardCustomizationData(string.Empty);
		_collectCoinsDummyData = new CollectCoinsDummyData(string.Empty);
		_trophyMissionData = string.Empty;
		_characterNameEventData = string.Empty;
		_hasCollectedFirstAwardReward = false;
		_hasCollectedFacebookWeeklyGiftThisWeek = false;
		_flurryUserId = string.Empty;
	}

	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("PLAYERINFO_DEBUG_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
