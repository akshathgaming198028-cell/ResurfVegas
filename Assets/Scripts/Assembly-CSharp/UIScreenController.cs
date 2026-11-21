#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Managers;
using SYBO.Subway;
using SYBO.Subway.Info;
using SYBO.Subway.Level;
using TopRun;
using UnityEngine;
using Debug = UnityEngine.Debug;
using com.kiloo.awards;

public class UIScreenController : MonoBehaviour
{
	public enum SlideInType
	{
		TopRunTimer = 0,
		Mission = 1,
		MissionSet = 2,
		Letters = 3,
		LettersCompleteMysteryBox = 4,
		LettersCompleteCoins = 5,
		Unlock = 6,
		ErrorMessage = 7,
		ItemHunt = 8,
		OfferWall = 9,
		VideoForKeys = 10,
		Brag = 11,
		BundlesPack = 12,
		Awards = 13,
		Facebook = 14,
		CharacterEventWord = 15,
		CharacterEventWordReward = 16
	}

	public class SlideIn
	{
		private SlideInType _type;

		private string _payload = string.Empty;

		private string _extraInfo = string.Empty;

		private AwardTierType _awardTierType;

		private UISlideInFacebook.FacebookSlideInState _facebookState;

		private Action _onDidShowCallback;

		private int _payloadInt;

		private MysteryBox.Type _mysteryBoxType;

		public SlideInType type
		{
			get
			{
				return _type;
			}
		}

		public string payload
		{
			get
			{
				return _payload;
			}
		}

		public string extraInfo
		{
			get
			{
				return _extraInfo;
			}
		}

		public AwardTierType awardTierType
		{
			get
			{
				return _awardTierType;
			}
		}

		public UISlideInFacebook.FacebookSlideInState facebookState
		{
			get
			{
				return _facebookState;
			}
		}

		public Action onDidShowCallback
		{
			get
			{
				return _onDidShowCallback;
			}
		}

		public int payloadInt
		{
			get
			{
				return _payloadInt;
			}
		}

		public MysteryBox.Type mysteryBoxType
		{
			get
			{
				return _mysteryBoxType;
			}
		}

		public SlideIn(SlideInType type)
		{
			_type = type;
		}

		public SlideIn(SlideInType type, string payload)
		{
			_type = type;
			_payload = payload;
		}

		public SlideIn(SlideInType type, string payload, AwardTierType awardTierType)
		{
			_payload = payload;
			_type = type;
			_awardTierType = awardTierType;
		}

		public SlideIn(SlideInType type, int payload, string extraInfo)
		{
			_payloadInt = payload;
			_type = type;
			_extraInfo = extraInfo;
		}

		public SlideIn(SlideInType type, UISlideInFacebook.FacebookSlideInState state, Action onDidShowCallback = null)
		{
			_facebookState = state;
			_type = type;
			_onDidShowCallback = onDidShowCallback;
		}

		public SlideIn(SlideInType type, int PayLoadInt)
		{
			_type = type;
			_payloadInt = PayLoadInt;
		}

		public SlideIn(SlideInType type, MysteryBox.Type mysteryBoxType)
		{
			_type = type;
			_mysteryBoxType = mysteryBoxType;
		}
	}

	private const string SCREEN_RESOURCE_PATH = "Prefabs/Screens/";

	private const string POPUP_RESOURCE_PATH = "Prefabs/Popups/";

	private static UIScreenController _instance;

	public AnimationCurve guidelineAnimation;

	public GameObject backgroundAnchor;

	public GameObject screenAnchor;

	public GameObject popupAnchor;

	public GameObject superPopupAnchor;

	public UICamera nguiCamera;

	public Camera Camera3D;

	public Camera CameraOverlay2dClipping;

	public Camera CameraOverlay2d;

	public Camera CameraCelebration;

	public Camera Camera2DNormal;

	public Camera Camera3DOverlay;

	public Camera Camera3DClip;

	public UIRoot root;

	public GameObject MenuElements3D;

	public bool LoadMenuOnStart;

	public Font FloatingTextFont;

	private Camera mainCamera;

	public Action<string> OnChangedScreen;

	[SerializeField]
	private Font lilitaRegularFont;

	[SerializeField]
	private Font titanRegularFont;

	private bool _isApplicationFocused = true;

	private static readonly List<string> PAYOUT_DISALLOWED_SCREENS = new List<string> { "IngameUI" };

	private static bool _facebookPayoutPopupQueued = false;

	private static bool _gameCenterPayoutPopupQueued = false;

	private Dictionary<string, UIScreen> _cachedScreens = new Dictionary<string, UIScreen>();

	private List<string> _screenStack = new List<string>();

	private List<string> _popupQueue = new List<string>();

	private List<string> _delayedPopups = new List<string>();

	private bool _popupActive;

	private List<string> _screenNamesWithoutBackground = new List<string> { "FrontUI", "IngameUI" };

	public string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

	public int size = 16;

	public FontStyle style;

	private bool _isApplicationResuming;

	private bool _isApplicationPaused;

	public bool justPaidOutTopRun;

	public bool stoppingFromEditor;

	public bool doSaveAndDelayRemainingPopups;

	public bool ignoreLastPopupClosed;

	[SerializeField]
	private UISlideInMissionHelper missionSlideIn;

	[SerializeField]
	private UISlideInMissionSetHelper missionSetSlideIn;

	[SerializeField]
	private UISlideInLettersHelper lettersSlideIn;

	[SerializeField]
	private UISlideInUnlock unlockSlideIn;

	[SerializeField]
	private UISlideInErrorMessage errorSlideIn;

	[SerializeField]
	private UISlideInOfferPayoutHelper offerWallSlideIn;

	[SerializeField]
	private UISlideInItemHunt itemHuntSlideIn;

	[SerializeField]
	private UISlideInBrag bragSlideIn;

	[SerializeField]
	private UISlideInVideoPayoutHelper watchForKeysSlideIn;

	[SerializeField]
	private UISlideInBundlesPack bundlesPackSlideIn;

	[SerializeField]
	private UISlideInAwards awardsSlideIn;

	[SerializeField]
	private UISlideInFacebook facebookSlideIn;

	[SerializeField]
	private UISlideInTopRunTimer topRunTimerSlideIn;

	[SerializeField]
	private UISlideInCharacterEventHelper characterEventSlideIn;

	[SerializeField]
	private UISlideInCharacterEventRewardHelper characterEventRewardSlideIn;

	public UISlideInMissionSetHelper coinReward;

	private List<SlideIn> _slideInQueue = new List<SlideIn>(15);

	private List<SlideIn> _noOfMissionSetSlideIns = new List<SlideIn>();

	private bool slideInActive;

	private bool slideHasPreloaded;

	[SerializeField]
	private AudioClipInfo slideInSound;

	[SerializeField]
	private AudioClipInfo slideInFanfare;

	private List<SlideInType> _uniqueSlideInTypes = new List<SlideInType> { SlideInType.Facebook };

	public UIMessageHelper messageHelper;

	private Queue<string> _messageQueue = new Queue<string>();

	private bool messageShowing;

	public GameObject inAppPurchaseOverlay;

	public GameObject adsOverlay;

	[SerializeField]
	private GameObject transitionAdsOverlay;

	public GameObject videoAdsOverlay;

	private bool _allowInputOnVideoAdsOverlay = true;

	[SerializeField]
	private GameObject _memoryBenchmarkOverlay;

	[SerializeField]
	private UIProgressBar _memoryBenchmarkProgress;

	public static Vector3 POPUP_DEFAULT_POSITION = new Vector3(-70f, -25f, 0f);

	public static Vector3 SCREEN_DEFAULT_POSITION = new Vector3(90f, -60f, 0f);

	public static Vector3 CELEBRATION_POPUP_POSITION = new Vector3(20f, 16f, -1f);

	public static Vector2 SAVEME_POPUP_POSITION = new Vector2(20f, 3.5f);

	public static bool IsInstanced
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController;
			}
			return _instance != null;
		}
	}

	public static UIScreenController Instance
	{
		get
		{
			return _instance ?? (_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController);
		}
	}

	public bool IsApplicationFocused
	{
		get
		{
			return _isApplicationFocused;
		}
	}

	public bool IsApplicationPaused
	{
		get
		{
			return _isApplicationPaused;
		}
	}

	public bool isShowingPopup
	{
		get
		{
			return _popupQueue != null && _popupQueue.Count > 0;
		}
	}

	public static event Action<string> OnNavigateToScreen;

	public static event Action OnApplicationResumed;

	public event Action OnPopupShown;

	public event Action OnLastPopupClosed;

	public event Action<string> OnPopupClosed;

	static UIScreenController()
	{
		UIScreenController.OnNavigateToScreen = null;
	}

	private void Awake()
	{
		MissionsManager instance = MissionsManager.Instance;
		instance.onMissionComplete = (MissionsManager.MissionCompleteHandler)Delegate.Combine(instance.onMissionComplete, new MissionsManager.MissionCompleteHandler(OnMissionCompleted));
		MissionsManager instance2 = MissionsManager.Instance;
		instance2.onMissionSetComplete = (MissionsManager.MissionSetCompleteHandler)Delegate.Combine(instance2.onMissionSetComplete, new MissionsManager.MissionSetCompleteHandler(OnMissionSetCompleted));
		PlayerInfo instance3 = PlayerInfo.Instance;
		instance3.OnPickedUpLetter = (Action)Delegate.Combine(instance3.OnPickedUpLetter, new Action(OnLetterPickedUp));
		PlayerInfo instance4 = PlayerInfo.Instance;
		instance4.OnTokenCollected = (Action<Characters.CharacterType>)Delegate.Combine(instance4.OnTokenCollected, new Action<Characters.CharacterType>(OnTokenPickUp));
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
		if (lilitaRegularFont != null)
		{
			lilitaRegularFont.RequestCharactersInTexture(text, size, style);
		}
		if (titanRegularFont != null)
		{
			titanRegularFont.RequestCharactersInTexture(text, size, style);
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void OnDestroy()
	{
		if (!stoppingFromEditor)
		{
			MissionsManager instance = MissionsManager.Instance;
			instance.onMissionComplete = (MissionsManager.MissionCompleteHandler)Delegate.Remove(instance.onMissionComplete, new MissionsManager.MissionCompleteHandler(OnMissionCompleted));
			MissionsManager instance2 = MissionsManager.Instance;
			instance2.onMissionSetComplete = (MissionsManager.MissionSetCompleteHandler)Delegate.Remove(instance2.onMissionSetComplete, new MissionsManager.MissionSetCompleteHandler(OnMissionSetCompleted));
			PlayerInfo instance3 = PlayerInfo.Instance;
			instance3.OnPickedUpLetter = (Action)Delegate.Remove(instance3.OnPickedUpLetter, new Action(OnLetterPickedUp));
			PlayerInfo instance4 = PlayerInfo.Instance;
			instance4.OnTokenCollected = (Action<Characters.CharacterType>)Delegate.Remove(instance4.OnTokenCollected, new Action<Characters.CharacterType>(OnTokenPickUp));
		}
	}

	private void Start()
	{
		HideInAppPurchaseOverlay();
		if (LoadMenuOnStart)
		{
			ShowMainMenu();
		}
		PlayerInfo.Instance.BragCompleted();
		MissionInfo[] missionInfo = MissionsManager.Instance.GetMissionInfo();
		if (missionInfo[0].complete && missionInfo[1].complete && missionInfo[2].complete)
		{
			PlayerInfo.Instance.missionCompletedSum++;
			MissionsManager.Instance.currentMissionSet++;
			LogWarning("you completed all missions but was not sent to next mission set, this should never happen, but this fixes it");
		}
		PreloadAllSlideIn();
		UpdateNguiTouchThresholds();
	}

	private void _SetApplicationPause(bool isApplicationPaused)
	{
		if (isApplicationPaused)
		{
			_isApplicationPaused = true;
			PlayerInfo.Instance.SaveIfDirty();
			UpdateApp.AllowAgainThisSession();
			LogFlurry();
		}
		else
		{
			_isApplicationPaused = false;
			_isApplicationResuming = true;
		}
		if (Application.platform == RuntimePlatform.WP8Player && isApplicationPaused && Peek(_screenStack) == "IngameUI" && !Game.Instance.isDead)
		{
			SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = 0f;
		}
	}

	private void _SetApplicationFocus(bool _isApplicationFocus)
	{
		_isApplicationFocused = _isApplicationFocus;
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
		{
			_SetApplicationPause(!_isApplicationFocus);
			if (Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice && !_isApplicationFocus && Peek(_screenStack) == "IngameUI" && !Game.Instance.isDead)
			{
				SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = 0f;
			}
		}
	}

	public void DebugSetApplicationPause(bool isApplicationPaused)
	{
		Debug.Log("UISC.DebugPauseApplication() isApplicationPaused: " + isApplicationPaused);
		_SetApplicationPause(isApplicationPaused);
	}

	public void DebugSetApplicationFocus(bool isApplicationFocused)
	{
		Debug.Log("UISC.DebugFocusApplication() isApplicationFocused: " + isApplicationFocused);
		_SetApplicationFocus(isApplicationFocused);
	}

	private void LateUpdate()
	{
		if (_isApplicationResuming)
		{
			_isApplicationResuming = false;
			if (_screenStack.Count > 0 && Peek(_screenStack) == "IngameUI" && (!(Game.Instance != null) || !Game.Instance.isDead))
			{
				PushScreen(null, "PauseUI");
			}
			if (UIScreenController.OnApplicationResumed != null)
			{
				UIScreenController.OnApplicationResumed();
			}
		}
	}

	private void OnApplicationPause(bool isApplicationPaused)
	{
		_SetApplicationPause(isApplicationPaused);
	}

	private void OnApplicationFocus(bool isFocused)
	{
		_SetApplicationFocus(isFocused);
	}

	private void UpdateNguiTouchThresholds()
	{
		float num = 0f;
		float num2 = 0f;
		num = ((!(Screen.dpi > 0f)) ? 30f : (0.1f * Screen.dpi));
		num2 = ((!(Screen.dpi > 0f)) ? 30f : (0.1f * Screen.dpi));
		nguiCamera.touchDragThreshold = num;
		nguiCamera.touchClickThreshold = num2;
	}

	private void LogFlurry()
	{
		string currentPopupName = GetCurrentPopupName();
		string topScreenName = GetTopScreenName();
		string eventName = ((Application.platform == RuntimePlatform.Android) ? "Home button pressed" : ((Application.platform != RuntimePlatform.IPhonePlayer) ? "Editor back btn pressed" : "Back button pressed"));
		if (!string.IsNullOrEmpty(currentPopupName))
		{
			if (!string.IsNullOrEmpty(currentPopupName) && !string.IsNullOrEmpty(topScreenName))
			{
				Flurry.LogEventWithAParameter(eventName, "Screen Name", topScreenName + "_" + currentPopupName);
			}
			else if (!string.IsNullOrEmpty(currentPopupName))
			{
				Flurry.LogEventWithAParameter(eventName, "Screen Name", currentPopupName);
			}
			else if (!string.IsNullOrEmpty(topScreenName))
			{
				Flurry.LogEventWithAParameter(eventName, "Screen Name", topScreenName);
			}
			else
			{
				Flurry.LogEventWithAParameter(eventName, "Screen Name", "NO SCREEN");
			}
		}
	}

	public void FacebookLogIn(bool loggedIn)
	{
		if (loggedIn)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.FacebookLoggedIn);
		}
	}

	public void ShowMainMenu()
	{
		StartCoroutine(ShowMainMenuCoroutine());
	}

	private IEnumerator ShowMainMenuCoroutine()
	{
		while (Time.realtimeSinceStartup < LoadLevelCtrl.continueTime)
		{
			yield return null;
		}
		ThemeManager.Instance.ForceRefresh();
		_ActivateScreen("FrontUI");
	}

	public void GameOverTriggered()
	{
		if (!GetTopScreenName().Equals("GameoverUI"))
		{
			PlayerInfo.Instance.RunCompleted();
			MissionsManager.Instance.inRun = false;
			_ActivateScreen("GameoverUI");
		}
	}

	public void QueueMessage(string message)
	{
		_QueueMessage(message);
	}

	public void GoToMainMenuFromGame(GameObject sender)
	{
		if (Game.Instance != null)
		{
			MissionsManager.Instance.inRun = false;
			Game.Instance.StartTopMenu();
			Game.Instance.TriggerPause(false);
		}
		PayWeeklyHuntIfNeeded();
		_ActivateScreen("FrontUI");
	}

	private void PayWeeklyHuntIfNeeded()
	{
		List<CelebrationReward> weeklyHuntRewardsToUnlock = RewardManager.GetWeeklyHuntRewardsToUnlock();
		foreach (CelebrationReward item in weeklyHuntRewardsToUnlock)
		{
			PayoutCelebrationReward(item);
		}
	}

	public void PayoutCelebrationReward(CelebrationReward reward)
	{
		if (reward.rewardType == CelebrationRewardType.topRun)
		{
			TopRunRewardHelper.TrackRewardStats(reward);
			TopRunEventsHandler.instance.TriggerOnTopRunChanged(this, new TournamentChangedEventArgs());
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.coins)
		{
			PlayerInfo.Instance.amountOfCoins += reward.amount;
		}
		else if (reward.rewardType == CelebrationRewardType.token)
		{
			PlayerInfo.Instance.CollectToken(reward.characterType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.trophy)
		{
			PlayerInfo.Instance.UnlockTrophy(reward.trophyType);
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			PlayerInfo.Instance.amountOfKeys += reward.amount;
		}
		RewardManager.RewardPayedOut(reward);
		PlayerInfo.Instance.SaveIfDirty();
	}

	public string GetTopScreenName()
	{
		if (_screenStack != null && _screenStack.Count > 0)
		{
			return Peek(_screenStack);
		}
		return null;
	}

	public UIScreen GetScreenFromCache(string screenName)
	{
		if (_cachedScreens.ContainsKey(screenName))
		{
			return _cachedScreens[screenName];
		}
		return null;
	}

	public void PushScreen(string screenName)
	{
		PushScreen(null, screenName);
	}

	public void PushScreen(GameObject sender)
	{
		PushScreen(sender, string.Empty);
	}

	private void PushScreen(GameObject sender, string screenOverride)
	{
		string text = string.Empty;
		if (screenOverride != string.Empty)
		{
			text = screenOverride;
		}
		else
		{
			UIButtonChangeScreen component = sender.GetComponent<UIButtonChangeScreen>();
			if (component != null)
			{
				text = component.ScreenNameToOpen;
			}
		}
		if (sender != null)
		{
			BackBtnBehaviour component2 = sender.GetComponent<BackBtnBehaviour>();
			if (component2 != null)
			{
				text = component2.ScreenNameToOpen;
			}
		}
		if (text.Equals("FrontUI") && GetTopScreenName().Equals("GameoverUI"))
		{
			GoToMainMenuFromGame(sender);
		}
		if ((text == "PauseUI" && (!(Game.Instance != null) || Game.Instance.isDead)) || (Peek(_screenStack) == "PauseUI" && text == "IngameUI" && _cachedScreens["PauseUI"].GetComponent<PauseScreen>().IsWaiting()))
		{
			return;
		}
		if (text == "FrontUI")
		{
			string topScreenName = GetTopScreenName();
			if (!string.IsNullOrEmpty(topScreenName))
			{
				switch (topScreenName)
				{
				case "FriendsUI":
					UIScreens.friendsMenu_lastScreen = topScreenName;
					break;
				case "LeaderboardUI":
					UIScreens.friendsMenu_lastScreen = topScreenName;
					break;
				case "TopRunUI":
					UIScreens.friendsMenu_lastScreen = "TopRunUI";
					break;
				case "CharacterScreen":
				case "BoardScreen":
				case "AwardsScreen":
					UIScreens.meMenu_lastScreen = topScreenName;
					break;
				case "UpgradesUI_shop":
				case "EarnCoinsScreen":
				case "CoinsUI_shop":
					UIScreens.shopMenu_lastScreen = topScreenName;
					break;
				}
			}
		}
		_ActivateScreen(text);
	}

	public void BackToPrevious()
	{
		_BackToPreviousScreen();
	}

	private UIScreen _ActivateScreen(string screenName)
	{
		bool flag = true;
		UIScreen uIScreen;
		if (_cachedScreens.ContainsKey(screenName))
		{
			if (_screenStack.Count > 0)
			{
				int num = _screenStack.LastIndexOf(screenName);
				if (num >= 0)
				{
					flag = false;
					if (num < _screenStack.Count - 1)
					{
						int num2 = num + 1;
						int num3 = _screenStack.Count - num2;
						for (int i = num2; i < num2 + num3; i++)
						{
							_cachedScreens[_screenStack[i]].Hide();
						}
						_screenStack.RemoveRange(num2, num3);
					}
				}
			}
			uIScreen = _cachedScreens[screenName];
		}
		else
		{
			uIScreen = _LoadScreenToCache(screenName);
		}
		if (DeviceUtility.hasPhysicalBackButton)
		{
			AddBackButtonBehavior(screenName);
		}
		if (flag)
		{
			if (_screenStack.Count > 0)
			{
				_cachedScreens[_screenStack[_screenStack.Count - 1]].Hide();
			}
			_screenStack.Add(screenName);
		}
		_ShowScreen(screenName, uIScreen);
		return uIScreen;
	}

	private void _ShowScreen(string screenName, UIScreen screen)
	{
		_SetBackground(!_screenNamesWithoutBackground.Contains(screenName));
		if (InterstitialLocations.ALLOWED_CACHE_SCREENS.Contains(screenName))
		{
			ScreenAdsManager.instance.SetAllowCache(true);
		}
		else
		{
			ScreenAdsManager.instance.SetAllowCache(false);
		}
		screen.Show();
		if (screenName == "GameoverUI")
		{
			_cachedScreens[screenName].GetComponent<GameOverScreen>().SetupBeforeMysteryBox();
			if (RewardManager.rewardsToUnlockCount > 0)
			{
				_QueuePopup("CelebrationPopup");
			}
			else
			{
				_cachedScreens[screenName].GetComponent<GameOverScreen>().SetupAfterMysteryBox();
			}
		}
		Action<string> onChangedScreen = OnChangedScreen;
		if (onChangedScreen != null)
		{
			onChangedScreen(screenName);
		}
		Action<string> onNavigateToScreen = UIScreenController.OnNavigateToScreen;
		if (onNavigateToScreen != null)
		{
			onNavigateToScreen(screenName);
		}
		ScreenDidChange(screenName);
	}

	private void _SwitchScreen(string screenName)
	{
		string key = Pop(_screenStack);
		_cachedScreens[key].Hide();
		_ActivateScreen(screenName);
	}

	public void SwitchScreen(GameObject sender)
	{
		string empty = string.Empty;
		empty = sender.GetComponent<UIButtonChangeScreen>().ScreenNameToOpen;
		_SwitchScreen(empty);
	}

	private void _BackToPreviousScreen()
	{
		if (_screenStack.Count > 1)
		{
			string key = Pop(_screenStack);
			_cachedScreens[key].Hide();
			key = Peek(_screenStack);
			_cachedScreens[key].Show();
			_SetBackground(!_screenNamesWithoutBackground.Contains(key));
			ScreenDidChange(key);
		}
		else
		{
			LogError("Tried to remove the only screen in the stack. You dun goofed.", this);
		}
	}

	private void ScreenDidChange(string newScreenName)
	{
		if (newScreenName != "IngameUI")
		{
			messageHelper.DisableShowLabel();
		}
		TryQueuePayoutPopups();
		if (newScreenName == "FrontUI" || newScreenName == "GameoverUI")
		{
			HouseKeeper.RefreshOnlineSettingsAndInappsIfNeeded();
		}
		PlayerInfo instance = PlayerInfo.Instance;
		if (newScreenName == "FrontUI" && FrontScreen.HaveButtonsTweened() && instance.shouldShowMission2Popup && !instance.hasShownMission2Popup)
		{
			instance.shouldShowMission2Popup = false;
			instance.hasShownMission2Popup = true;
			_QueuePopup("TutorialEndGameMissionsPopup");
		}
		instance.SaveIfDirty();
		SocialManager.instance.SaveIfDirty();
	}

	private void _SetBackground(bool state)
	{
		string text = "NotebookPanel2";
		if (state)
		{
			if (!_cachedScreens.ContainsKey(text))
			{
				GameObject prefab = Resources.Load("Prefabs/Screens/" + text, typeof(GameObject)) as GameObject;
				GameObject gameObject = NGUITools.AddChild(backgroundAnchor, prefab);
				_cachedScreens.Add(text, gameObject.GetComponent<UIScreen>());
				_cachedScreens[text].Init();
			}
			_cachedScreens[text].Show();
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			mainCamera.enabled = false;
		}
		else
		{
			if (_cachedScreens.ContainsKey(text))
			{
				_cachedScreens[text].Hide();
			}
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			mainCamera.enabled = true;
		}
	}

	private UIScreen _LoadScreenToCache(string screenName, bool isPopup = false)
	{
		GameObject gameObject;
		if (!isPopup)
		{
			GameObject prefab = Resources.Load("Prefabs/Screens/" + screenName, typeof(GameObject)) as GameObject;
			gameObject = NGUITools.AddChild(screenAnchor, prefab);
		}
		else
		{
			GameObject prefab2 = Resources.Load("Prefabs/Popups/" + screenName, typeof(GameObject)) as GameObject;
			gameObject = NGUITools.AddChild(popupAnchor, prefab2);
		}
		UIScreen component = gameObject.GetComponent<UIScreen>();
		_cachedScreens.Add(screenName, component);
		component.screenName = screenName;
		component.Init();
		return component;
	}

	public static bool ShouldShowOnlineScreen()
	{
		if (Globals.DEBUG_USE_DEBUG_LOGIN_STATE)
		{
			return true;
		}
		return SocialManager.instance.facebookIsLoggedIn || SocialManager.instance.facebookState == SocialManager.FacebookState.LoggingIn || SocialManagerIOS.instance.gameCenterState != SocialManagerIOS.GCState.LoggedOut;
	}

	private string Peek(List<string> list)
	{
		if (list.Count > 0)
		{
			return list[list.Count - 1];
		}
		return string.Empty;
	}

	private string Pop(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
		}
		return result;
	}

	private string QueuePeek(List<string> list)
	{
		string result = string.Empty;
		if (list != null && list.Count > 0)
		{
			result = list[0];
		}
		return result;
	}

	private string Dequeue(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[0];
			list.RemoveAt(0);
		}
		return result;
	}

	public void ShowUnlockAnimationForCharacter(Characters.CharacterType ctype, int themeIndex = 0)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock;
		celebrationReward.rewardType = CelebrationRewardType.character;
		celebrationReward.characterType = ctype;
		celebrationReward.characterOutfitIndex = themeIndex;
		RewardManager.AddRewardToUnlock(celebrationReward);
		QueueMysteryBox();
	}

	public void ShowUnlockAnimationForHoverboard(Hoverboards.BoardType boardType, Hoverboard.BoardAbility boardPower = Hoverboard.BoardAbility.None)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.BoardUnlock;
		celebrationReward.rewardType = CelebrationRewardType.specialBoard;
		celebrationReward.boardType = boardType;
		celebrationReward.boardPower = boardPower;
		RewardManager.AddRewardToUnlock(celebrationReward);
		QueueMysteryBox();
	}

	public string GetCurrentPopupName()
	{
		return QueuePeek(_popupQueue);
	}

	public bool IsPopupQueueEmpty()
	{
		return _popupQueue.Count <= 0;
	}

	public bool IsPopupAlreadyQueued(string popupName)
	{
		foreach (string item in _popupQueue)
		{
			if (item == popupName)
			{
				return true;
			}
		}
		return false;
	}

	public void PushPopup(string name)
	{
		_PushPopup(name);
	}

	public void PushPopup(GameObject sender)
	{
		string screenNameToOpen = sender.GetComponent<UIButtonChangeScreen>().ScreenNameToOpen;
		_PushPopup(screenNameToOpen);
	}

	private void _PushPopup(string name)
	{
		int num = -1;
		string text = QueuePeek(_popupQueue);
		if (!string.IsNullOrEmpty(text) && _cachedScreens.ContainsKey(text))
		{
			UIPanel[] componentsInChildren = _cachedScreens[text].GetComponentsInChildren<UIPanel>(true);
			UIPanel[] array = componentsInChildren;
			foreach (UIPanel uIPanel in array)
			{
				if (uIPanel.depth > num)
				{
					num = uIPanel.depth;
				}
			}
		}
		_popupQueue.Insert(0, name);
		_ActivateNextPopup(text);
		if (!_cachedScreens.ContainsKey(name))
		{
			return;
		}
		UIPanel[] componentsInChildren2 = _cachedScreens[name].GetComponentsInChildren<UIPanel>();
		int num2 = -1;
		if (componentsInChildren2 == null || componentsInChildren2.Length <= 0)
		{
			return;
		}
		UIPanel uIPanel2 = componentsInChildren2[0];
		UIPanel[] array2 = componentsInChildren2;
		foreach (UIPanel uIPanel3 in array2)
		{
			if (uIPanel3.depth < uIPanel2.depth)
			{
				uIPanel2 = uIPanel3;
			}
		}
		if (uIPanel2 != null && num > -1)
		{
			num2 = num + 1 - uIPanel2.depth;
		}
		if (num2 > -1)
		{
			UIPanel[] array3 = componentsInChildren2;
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k].depth += num2;
			}
		}
	}

	private void _QueuePopup(string name)
	{
		messageHelper.DisableShowLabel();
		_popupQueue.Add(name);
		if (!_popupActive)
		{
			_ActivateNextPopup(string.Empty);
		}
	}

	private void _ActivateNextPopup(string oldPopupName = "")
	{
		if (this.OnPopupShown != null)
		{
			this.OnPopupShown();
		}
		if (_popupQueue.Count > 0)
		{
			string text = QueuePeek(_popupQueue);
			foreach (string item in _popupQueue)
			{
				if (!_cachedScreens.ContainsKey(item))
				{
					continue;
				}
				UIScreen uIScreen = _cachedScreens[item];
				if (uIScreen.isActive)
				{
					if (item != text)
					{
						uIScreen.LooseFocus();
					}
					else
					{
						uIScreen.GainFocus();
					}
				}
			}
			ActivateAnchors(false);
			if (DeviceUtility.hasPhysicalBackButton)
			{
				string topScreenName = GetTopScreenName();
				if (!string.IsNullOrEmpty(topScreenName))
				{
					EnableBackButton(topScreenName, false);
				}
			}
			if (!_cachedScreens.ContainsKey(text))
			{
				_LoadScreenToCache(text, true);
			}
			if (text == "CelebrationPopup" && _screenStack.Count > 0)
			{
				if (_screenStack[_screenStack.Count - 1] == "GameoverUI")
				{
					RewardManager.canShowMultipleQueuedCelebrations = true;
				}
				if (_screenStack[_screenStack.Count - 1] != "GameoverUI")
				{
					_cachedScreens[_screenStack[_screenStack.Count - 1]].Hide();
				}
			}
			UIScreen uIScreen2 = _cachedScreens[text];
			if (uIScreen2 != null && !uIScreen2.isActive)
			{
				if (oldPopupName != string.Empty)
				{
					uIScreen2.parentScreen = oldPopupName;
				}
				uIScreen2.Show();
			}
			_popupActive = true;
			if (DeviceUtility.hasPhysicalBackButton && text != "CelebrationPopup" && _cachedScreens[text].gameObject.GetComponent<BackBtnBehaviour>() == null)
			{
				if (text == "SaveMePopup")
				{
					_cachedScreens[text].gameObject.AddComponent<BackBtnBehaviour>().screenChangeType = BackBtnBehaviour.ScreenChangeType.SaveMePopup;
				}
				else if (text == "NotEnoughCurencyPopup")
				{
					_cachedScreens[text].gameObject.AddComponent<BackBtnBehaviour>().screenChangeType = BackBtnBehaviour.ScreenChangeType.NotEnoughCurrency;
				}
				else
				{
					_cachedScreens[text].gameObject.AddComponent<BackBtnBehaviour>().screenChangeType = BackBtnBehaviour.ScreenChangeType.ClosePopup;
				}
			}
			Action<string> onChangedScreen = OnChangedScreen;
			if (onChangedScreen != null)
			{
				onChangedScreen(text);
			}
			return;
		}
		ActivateAnchors(true);
		if (DeviceUtility.hasPhysicalBackButton)
		{
			string topScreenName2 = GetTopScreenName();
			if (!string.IsNullOrEmpty(topScreenName2))
			{
				EnableBackButton(topScreenName2, true);
			}
		}
	}

	private void EnableBackButton(string screenName, bool enable)
	{
		BackBtnBehaviour backBtnBehaviour = null;
		UIScreen uIScreen = _cachedScreens[screenName];
		if (uIScreen != null)
		{
			backBtnBehaviour = uIScreen.gameObject.GetComponent<BackBtnBehaviour>();
			if (backBtnBehaviour != null)
			{
				backBtnBehaviour.enabled = enable;
			}
		}
	}

	private void ActivateAnchors(bool active)
	{
		NGUITools.SetActive(UIModelController.Instance.CharacterAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.GameOverAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.CelebrationPopupAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.PauseScreenAnchor, active);
		_PauseAnimations(!active, MenuElements3D.transform);
	}

	public static void QueueFacebookPayoutPopup()
	{
		_facebookPayoutPopupQueued = true;
		if (IsInstanced)
		{
			Instance.TryQueuePayoutPopups();
		}
	}

	public static void QueueGameCenterPayoutPopup()
	{
		_gameCenterPayoutPopupQueued = true;
		if (IsInstanced)
		{
			Instance.TryQueuePayoutPopups();
		}
	}

	private void TryQueuePayoutPopups()
	{
		if ((!_facebookPayoutPopupQueued && !_gameCenterPayoutPopupQueued) || _screenStack == null || _screenStack.Count <= 0)
		{
			return;
		}
		string item = Peek(_screenStack);
		if (PAYOUT_DISALLOWED_SCREENS.Contains(item))
		{
			return;
		}
		if (_facebookPayoutPopupQueued)
		{
			string popupName = "FacebookRewardsPopup";
			if (!IsPopupAlreadyQueued(popupName))
			{
				QueuePopup(popupName);
			}
			_facebookPayoutPopupQueued = false;
		}
		if (_gameCenterPayoutPopupQueued && PlayerInfo.Instance.IsTutorialCompleted)
		{
			string popupName2 = "GameCenterPayoutPopup";
			if (!IsPopupAlreadyQueued(popupName2))
			{
				QueuePopup(popupName2);
			}
			_gameCenterPayoutPopupQueued = false;
		}
	}

	public void QueuePopupRightAfterCurrentActivePopup(string popupName)
	{
		string value = QueuePeek(_popupQueue);
		if (!string.IsNullOrEmpty(value))
		{
			_popupQueue.Insert(1, popupName);
		}
		else
		{
			_QueuePopup(popupName);
		}
	}

	public void QueuePopup(string popupName)
	{
		_QueuePopup(popupName);
	}

	public void QueuePopup(GameObject sender)
	{
		string screenNameToOpen = sender.GetComponent<UIButtonChangeScreen>().ScreenNameToOpen;
		_QueuePopup(screenNameToOpen);
	}

	public void QueueMysteryBox()
	{
		string empty = string.Empty;
		if (_popupQueue.Count > 0)
		{
			empty = QueuePeek(_popupQueue);
			if (empty == "CelebrationPopup")
			{
				return;
			}
		}
		if (_popupQueue.Count > 0)
		{
			PushPopup("CelebrationPopup");
		}
		else
		{
			_QueuePopup("CelebrationPopup");
		}
	}

	public void CloseActivePopups(string popupToStopAt)
	{
		List<string> list = new List<string>();
		if (_popupQueue.Contains(popupToStopAt))
		{
			foreach (string item in _popupQueue)
			{
				UIScreen uIScreen = _cachedScreens[item];
				if (uIScreen.isActive)
				{
					list.Add(item);
					if (item == popupToStopAt)
					{
						break;
					}
				}
			}
		}
		foreach (string item2 in list)
		{
			_RemovePopup(item2);
		}
	}

	public void ClosePopup(GameObject go = null)
	{
		_RemovePopup(string.Empty);
	}

	private void _PauseAnimations(bool pause, Transform trans)
	{
		foreach (Transform tran in trans)
		{
			_PauseAnimations(pause, tran);
		}
		if (trans.GetComponent<CharacterModel>() != null)
		{
			if (pause)
			{
				trans.GetComponent<CharacterModel>().StopIdleAnimations();
			}
			else if (Peek(_screenStack) == "BoardScreen")
			{
				HoverboardModelPreviewFactory.Instance.ResetTeleportFX();
			}
			else
			{
				trans.GetComponent<CharacterModel>().StartIdleAnimations(UIModelController.ModelScreen.Character);
			}
		}
	}

	public void SaveAndDelayPopups()
	{
		doSaveAndDelayRemainingPopups = false;
		foreach (string item in _popupQueue)
		{
			_delayedPopups.Add(item);
		}
		_popupQueue.Clear();
	}

	public void RequeueDelayedPopups()
	{
		foreach (string delayedPopup in _delayedPopups)
		{
			QueuePopup(delayedPopup);
		}
		_delayedPopups.Clear();
	}

	private void _RemovePopup(string screenName = "")
	{
		if (_popupQueue.Count < 1)
		{
			return;
		}
		if (screenName == string.Empty)
		{
			screenName = Dequeue(_popupQueue);
		}
		else
		{
			_popupQueue.Remove(screenName);
		}
		if (screenName == "FacebookRewardsPopup")
		{
			FacebookRewardsPopup component = _cachedScreens[screenName].gameObject.GetComponent<FacebookRewardsPopup>();
			if (component != null)
			{
				component.OnBackBtnClicked();
			}
		}
		UIScreen uIScreen = _cachedScreens[screenName];
		uIScreen.Hide();
		uIScreen.parentScreen = string.Empty;
		if (_cachedScreens.ContainsKey("BoardScreen") && _popupQueue.Count == 0)
		{
			BoardScreenManager.Instance.willResetBoardAnimation = true;
		}
		Action<string> onPopupClosed = this.OnPopupClosed;
		if (onPopupClosed != null)
		{
			onPopupClosed(screenName);
		}
		_popupActive = false;
		if (_popupQueue.Count == 0)
		{
			Action<string> onChangedScreen = OnChangedScreen;
			if (onChangedScreen != null)
			{
				onChangedScreen(GetTopScreenName());
			}
		}
		if (doSaveAndDelayRemainingPopups)
		{
			SaveAndDelayPopups();
		}
		_ActivateNextPopup(string.Empty);
		if (screenName == "CelebrationPopup")
		{
			if (GetTopScreenName() == "CharacterScreen")
			{
				_cachedScreens["CharacterScreen"].GetComponent<CharacterScreen>().SetupAfterMysteryBox();
			}
			if (_screenStack.Count > 0 && GetTopScreenName() != "GameoverUI")
			{
				_cachedScreens[_screenStack[_screenStack.Count - 1]].Show();
			}
			if (GetTopScreenName() == "GameoverUI")
			{
				_cachedScreens["GameoverUI"].GetComponent<GameOverScreen>().SetupAfterMysteryBox();
			}
		}
		if (_popupQueue.Count == 0 && this.OnLastPopupClosed != null)
		{
			if (!ignoreLastPopupClosed)
			{
				this.OnLastPopupClosed();
			}
			else
			{
				ignoreLastPopupClosed = false;
			}
		}
	}

	public void SpawnCollectText(Vector3 startPosition, string text)
	{
		UILabel uILabel = NGUITools.AddWidget<UILabel>(superPopupAnchor);
		Utility.SetLayerRecursively(uILabel.gameObject.transform, superPopupAnchor.layer);
		uILabel.text = text;
		uILabel.transform.position = new Vector3(startPosition.x, startPosition.y, uILabel.cachedTransform.position.z);
		uILabel.trueTypeFont = FloatingTextFont;
		uILabel.fontSize = 20;
		uILabel.supportEncoding = false;
		uILabel.multiLine = false;
		uILabel.keepCrispWhenShrunk = UILabel.Crispness.Always;
		uILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
		uILabel.MakePixelPerfect();
		uILabel.color = new Color(50f / 51f, 66f / 85f, 0.23529412f, 0f);
		uILabel.gameObject.AddComponent<UIPanel>().depth = 11;
		StartCoroutine(AnimateCollectText(uILabel));
	}

	private IEnumerator AnimateCollectText(UILabel collectText)
	{
		Vector3 fromLocalPosition = collectText.transform.localPosition;
		Vector3 toLocalPosition = new Vector3(fromLocalPosition.x, fromLocalPosition.y + 50f, fromLocalPosition.z);
		yield return StartCoroutine(AnimateAlpha(collectText, 0.1f, 1f));
		StartCoroutine(MoveTransform(collectText.cachedTransform, 1f, toLocalPosition));
		yield return new WaitForSeconds(0.8f);
		StartCoroutine(AnimateAlpha(collectText, 0.2f, 0f));
		yield return new WaitForSeconds(0.25f);
		UnityEngine.Object.Destroy(collectText.gameObject);
	}

	private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
	}

	private IEnumerator MoveTransform(Transform trans, float duration, Vector3 toPos)
	{
		Vector3 fromPos = trans.localPosition;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			trans.localPosition = Vector3.Lerp(fromPos, toPos, factor);
			yield return null;
		}
	}

	public int NumberOfMissionSetSlideIns()
	{
		_noOfMissionSetSlideIns = _slideInQueue.FindAll((SlideIn item) => item.type == SlideInType.MissionSet);
		return _noOfMissionSetSlideIns.Count;
	}

	private void PreloadAllSlideIn()
	{
		if (!slideHasPreloaded)
		{
			slideHasPreloaded = true;
			StartCoroutine(missionSlideIn.PreloadSlideIn());
			StartCoroutine(missionSetSlideIn.PreloadSlideIn());
			StartCoroutine(lettersSlideIn.PreloadSlideIn());
			StartCoroutine(unlockSlideIn.PreloadSlideIn());
			StartCoroutine(errorSlideIn.PreloadSlideIn());
			StartCoroutine(itemHuntSlideIn.PreloadSlideIn());
			StartCoroutine(bragSlideIn.PreloadSlideIn());
			StartCoroutine(watchForKeysSlideIn.PreloadSlideIn());
			StartCoroutine(bundlesPackSlideIn.PreloadSlideIn());
			StartCoroutine(offerWallSlideIn.PreloadSlideIn());
			StartCoroutine(awardsSlideIn.PreloadSlideIn());
			StartCoroutine(facebookSlideIn.PreloadSlideIn());
			StartCoroutine(topRunTimerSlideIn.PreloadSlideIn());
			StartCoroutine(characterEventSlideIn.PreloadSlideIn());
			StartCoroutine(characterEventRewardSlideIn.PreloadSlideIn());
		}
	}

	private void OnMissionCompleted(string message)
	{
		QueueSlideIn(new SlideIn(SlideInType.Mission, message));
	}

	private void OnMissionSetCompleted()
	{
		MissionsManager.Instance.PlayerDidThis(MissionTarget.ReachMissionSet);
		QueueSlideIn(new SlideIn(SlideInType.MissionSet));
	}

	private void OnLetterPickedUp()
	{
		QueueSlideIn(new SlideIn(SlideInType.Letters));
	}

	private void OnTokenPickUp(Characters.CharacterType type)
	{
		if (Characters.characterData[type].Price <= PlayerInfo.Instance.GetCollectedTokens(type))
		{
			Flurry.LogEventWithAParameter("Character unlocked", "Id", Strings.Get(Characters.characterData[type].name).ToLower());
			Statistics stats;
			Statistics statistics = (stats = PlayerInfo.Instance.stats);
			Stat stat2;
			Stat stat = (stat2 = Stat.OwnCharacters);
			int num = stats[stat2];
			statistics[stat] = num + 1;
			if (Characters.characterData[type].unlockType == Characters.UnlockType.tokens)
			{
				RewardManager.canShowMultipleQueuedCelebrations = true;
				ShowUnlockAnimationForCharacter(type);
			}
		}
	}

	public void QueueErrorMessageSlidein(string message)
	{
		QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, message));
	}

	public void QueueSlideIn(SlideIn slideIn)
	{
		if (!UniqueAllreadyInQueue(slideIn.type))
		{
			_slideInQueue.Add(slideIn);
			if (!slideInActive)
			{
				_ShowSlideIn();
			}
		}
	}

	private bool UniqueAllreadyInQueue(SlideInType type)
	{
		if (_uniqueSlideInTypes.Contains(type))
		{
			foreach (SlideIn item in _slideInQueue)
			{
				if (item.type == type)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void ReadyForNextSlide()
	{
		slideInActive = false;
		if (!slideInActive)
		{
			_ShowSlideIn();
		}
	}

	private void _ShowSlideIn()
	{
		if (_slideInQueue.Count <= 0)
		{
			return;
		}
		SlideIn slideIn = _slideInQueue[0];
		_slideInQueue.RemoveAt(0);
		if (slideIn.type == SlideInType.CharacterEventWordReward)
		{
			So.Instance.playSound(slideInSound);
			characterEventRewardSlideIn.SetupCharacterEventReward(slideIn.payload);
		}
		else if (slideIn.type == SlideInType.CharacterEventWord)
		{
			So.Instance.playSound(slideInSound);
			characterEventSlideIn.SetupCharacterEventLetters(slideIn.payloadInt, slideIn.extraInfo);
		}
		else if (slideIn.type == SlideInType.TopRunTimer)
		{
			So.Instance.playSound(slideInSound);
			topRunTimerSlideIn.SetupSlideTopRunTimer(slideIn.payloadInt);
		}
		else if (slideIn.type == SlideInType.Mission)
		{
			So.Instance.playSound(slideInFanfare);
			missionSlideIn.SetupSlideInMission(slideIn.payload);
		}
		else if (slideIn.type == SlideInType.MissionSet)
		{
			So.Instance.playSound(slideInFanfare);
			missionSetSlideIn.SetupSlideInMissionSet(PlayerInfo.Instance.rawMultiplier);
		}
		else if (slideIn.type == SlideInType.Letters)
		{
			So.Instance.playSound(slideInSound);
			lettersSlideIn.SetupLetters();
		}
		else if (slideIn.type == SlideInType.LettersCompleteMysteryBox)
		{
			So.Instance.playSound(slideInFanfare);
			missionSetSlideIn.SetupMysteryBox(slideIn.mysteryBoxType);
		}
		else if (slideIn.type == SlideInType.LettersCompleteCoins)
		{
			So.Instance.playSound(slideInFanfare);
			missionSetSlideIn.SetupCoin();
		}
		else if (slideIn.type == SlideInType.Unlock)
		{
			So.Instance.playSound(slideInFanfare);
			unlockSlideIn.SetupSlideInUnlock(slideIn.payload);
		}
		else if (slideIn.type == SlideInType.ErrorMessage)
		{
			So.Instance.playSound(slideInSound);
			errorSlideIn.SetupErrorMessage(slideIn.payload);
		}
		else if (slideIn.type == SlideInType.OfferWall)
		{
			So.Instance.playSound(slideInFanfare);
			offerWallSlideIn.SetupSlideInOffer(slideIn.payload);
		}
		else if (slideIn.type == SlideInType.ItemHunt)
		{
			So.Instance.playSound(slideInFanfare);
			itemHuntSlideIn.SetupSlideInOffer(string.Empty);
		}
		else if (slideIn.type == SlideInType.Brag)
		{
			So.Instance.playSound(slideInFanfare);
			bragSlideIn.SetupSlideInBrag();
		}
		else if (slideIn.type == SlideInType.VideoForKeys)
		{
			So.Instance.playSound(slideInFanfare);
			watchForKeysSlideIn.SetupSlideInVideoPayout();
		}
		else if (slideIn.type == SlideInType.BundlesPack)
		{
			So.Instance.playSound(slideInFanfare);
			bundlesPackSlideIn.SetupSlideInOffer(slideIn.payload);
		}
		else if (slideIn.type == SlideInType.Awards)
		{
			So.Instance.playSound(slideInFanfare);
			awardsSlideIn.SetupSlideInAward(slideIn.payload, slideIn.awardTierType);
		}
		else if (slideIn.type == SlideInType.Facebook)
		{
			UISlideInFacebook.FacebookSlideInState facebookState = slideIn.facebookState;
			if (facebookState == UISlideInFacebook.FacebookSlideInState.ConnectSucceeded || facebookState == UISlideInFacebook.FacebookSlideInState.LoggedOut)
			{
				So.Instance.playSound(slideInFanfare);
			}
			else
			{
				So.Instance.playSound(slideInSound);
			}
			facebookSlideIn.SetupSlideFacebook(facebookState, slideIn.onDidShowCallback);
		}
		slideInActive = true;
	}

	public void ReadyForNextMessage()
	{
		messageShowing = false;
		_ShowNextMessage();
	}

	private void _QueueMessage(string message)
	{
		_messageQueue.Enqueue(message);
		if (!messageShowing)
		{
			_ShowNextMessage();
		}
		if (!slideInActive)
		{
			_ShowSlideIn();
		}
	}

	private void _ShowNextMessage()
	{
		if (_messageQueue.Count > 0)
		{
			string message = _messageQueue.Dequeue();
			messageHelper.ShowMessage(message);
			messageShowing = true;
		}
	}

	public void ShowInAppPurchaseOverlay(string text)
	{
		if (inAppPurchaseOverlay != null)
		{
			InAppPurchaseOverlay component = inAppPurchaseOverlay.GetComponent<InAppPurchaseOverlay>();
			if (component != null && component.titleLabel != null)
			{
				component.titleLabel.text = text;
			}
			inAppPurchaseOverlay.SetActive(true);
			Camera3D.enabled = false;
		}
	}

	public void HideInAppPurchaseOverlay()
	{
		inAppPurchaseOverlay.SetActive(false);
		Camera3D.enabled = true;
	}

	public bool IsInAppPurchaseOverlayVisible()
	{
		return inAppPurchaseOverlay.activeInHierarchy;
	}

	public void ShowAdsOverlay()
	{
		adsOverlay.SetActive(true);
	}

	public void HideAdsOverlay()
	{
		StartCoroutine(HideAdsOverlayWithDelay(0.3f));
	}

	private IEnumerator HideAdsOverlayWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		adsOverlay.SetActive(false);
	}

	public bool IsAdsOverlayVisible()
	{
		return adsOverlay.activeInHierarchy;
	}

	public void ShowTransitionAdsOverlay()
	{
		transitionAdsOverlay.SetActive(true);
		StartCoroutine(HideTransitionAdsOverlayWithDelay(0.5f));
	}

	private IEnumerator HideTransitionAdsOverlayWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		transitionAdsOverlay.SetActive(false);
	}

	public void ShowDiskSpaceErrorOverlay()
	{
		videoAdsOverlay.SetActive(true);
		_allowInputOnVideoAdsOverlay = false;
	}

	public void ShowVideoAdsOverlay()
	{
		videoAdsOverlay.SetActive(true);
		_allowInputOnVideoAdsOverlay = false;
		CoroutineUtil.StartCoroutine(DelayInputOnOverlay());
	}

	public void HideVideoAdsOverlay()
	{
		if (_allowInputOnVideoAdsOverlay || Globals.DEBUG_SKIP_VIDEO_ADS)
		{
			HouseKeeper.TryRestoreSoundOptionAfterLostFocusIfNeeded();
			videoAdsOverlay.SetActive(false);
		}
	}

	private IEnumerator DelayInputOnOverlay()
	{
		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(2f));
		_allowInputOnVideoAdsOverlay = true;
	}

	public bool IsVideoAdsOverlayVisible()
	{
		return videoAdsOverlay.activeInHierarchy;
	}

	public void ShowMemoryBenchmarkOverlay()
	{
		_memoryBenchmarkOverlay.SetActive(true);
	}

	public void HideMemoryBenchmarkOverlay()
	{
		_memoryBenchmarkOverlay.SetActive(false);
	}

	public void UpdateMemoryBenchmarkPercent(float percent)
	{
		_memoryBenchmarkProgress.value = percent;
	}

	private void AddBackButtonBehavior(string screenName)
	{
		BackBtnBehaviour component = _cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviour>();
		if (!(component == null))
		{
			return;
		}
		_cachedScreens[screenName].gameObject.AddComponent<BackBtnBehaviour>();
		if (screenName == "FrontUI")
		{
			_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviour>().screenChangeType = BackBtnBehaviour.ScreenChangeType.ExitGame;
			return;
		}
		if (!(screenName == "FriendsUI") && !(screenName == "FriendsUI") && !(screenName == "AwardsScreen") && !(screenName == "LeaderboardUI"))
		{
			switch (screenName)
			{
			default:
				return;
			case "LeaderboardUI":
			case "CharacterScreen":
			case "UpgradesUI_shop":
			case "CoinsUI_shop":
			case "GameoverUI":
			case "BoardScreen":
			case "EarnCoinsScreen":
			case "TopRunUI":
				break;
			case "PauseUI":
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviour>().screenChangeType = BackBtnBehaviour.ScreenChangeType.PushScreen;
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviour>().ScreenNameToOpen = "IngameUI";
				return;
			case "IngameUI":
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviour>().screenChangeType = BackBtnBehaviour.ScreenChangeType.PushScreen;
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviour>().ScreenNameToOpen = "PauseUI";
				return;
			}
		}
		_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviour>().screenChangeType = BackBtnBehaviour.ScreenChangeType.PushScreen;
		_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviour>().ScreenNameToOpen = "FrontUI";
	}

	public void SetViewportRectXForCamera(Camera camera, float x)
	{
		if (camera != null)
		{
			Rect rect = camera.rect;
			rect.x = x;
			camera.rect = rect;
		}
	}

	public void SetViewportRectYForCamera(Camera camera, float y)
	{
		if (camera != null)
		{
			Rect rect = camera.rect;
			rect.y = y;
			camera.rect = rect;
		}
	}

	public void SetViewportRectWidthForCamera(Camera camera, float width)
	{
		if (camera != null)
		{
			Rect rect = camera.rect;
			rect.width = width;
			camera.rect = rect;
		}
	}

	public void SetViewportRectHeightForCamera(Camera camera, float height)
	{
		if (camera != null)
		{
			Rect rect = camera.rect;
			rect.height = height;
			camera.rect = rect;
		}
	}

	public void RecalculateViewportRectForClipCamera(Camera camera, bool popupSizedClip)
	{
		if (camera != null)
		{
			UICameraScreenClipping component = camera.GetComponent<UICameraScreenClipping>();
			if (component != null)
			{
				component.CalculateClipping(popupSizedClip);
			}
		}
	}

	public void SetCameraToOrtographic(Camera camera, bool isOrtographic)
	{
		if (camera != null)
		{
			camera.orthographic = isOrtographic;
		}
	}

	public void CalculateCameraFieldOfView(Camera camera, float designedScreenWidth, float distanceToUI)
	{
		if (camera != null && !camera.orthographic)
		{
			float num = Mathf.Atan(designedScreenWidth / 2f / distanceToUI);
			float num2 = (float)camera.pixelWidth / (float)camera.pixelHeight;
			float fieldOfView = Mathf.Atan(Mathf.Tan(num / 2f) / num2) * 2f * 57.29578f;
			camera.fieldOfView = fieldOfView;
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

	[Conditional("ENABLE_SCREENCONTROLLER_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
