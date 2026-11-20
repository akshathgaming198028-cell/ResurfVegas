using System;
using System.Collections.Generic;
using Kiloo.Common.UI;
using SYBO.Subway.Info;
using UnityEngine;
using com.kiloo.awards;

public class DebugSettingsPopup : UIScreen
{
	public class DebugSettingsListAdapter : ListAdapter<DebugSettingsCell>
	{
		private const int DEBUGSETTINGS_TOGGLE_PREFAB = 0;

		private const int DEBUGSETTINGS_BUTTON_PREFAB = 1;

		private const int DEBUGSETTINGS_SUBTOGGLE_PREFAB = 2;

		private const int DEBUGSETTINGS_SPACE_PREFAB = 3;

		private const int DEBUGSETTINGS_EXPANDABLE_PREFAB = 4;

		private const int DEBUGSETTINGS_TITLE_PREFAB = 5;

		private const int DEBUGSETTINGS_SUBBUTTON_PREFAB = 6;

		public DebugSettingsPopup debugSettingsPopup;

		public GameObject debugTitlePrefab;

		public GameObject debugTogglePrefab;

		public GameObject debugButtonPrefab;

		public GameObject debugSubButtonPrefab;

		public GameObject debugSubTogglePrefab;

		public GameObject debugExpandablePrefab;

		public GameObject debugSpacePrefab;

		public DebugSettingsListAdapter(List<DebugSettingsCell> contentList, GameObject debugSettingsTogglePrefab, GameObject debugSettingsButtonPrefab, GameObject debugSettingsSubTogglePrefab, GameObject debugSettingsSpacePrefab, GameObject debugSettingsExpandablePrefab, GameObject debugSettingsTitlePrefab, GameObject debugSettingsSubButtonPrefab, DebugSettingsPopup screenReference)
			: base(contentList)
		{
			debugTogglePrefab = debugSettingsTogglePrefab;
			debugSubTogglePrefab = debugSettingsSubTogglePrefab;
			debugButtonPrefab = debugSettingsButtonPrefab;
			debugSpacePrefab = debugSettingsSpacePrefab;
			debugExpandablePrefab = debugSettingsExpandablePrefab;
			debugTitlePrefab = debugSettingsTitlePrefab;
			debugSubButtonPrefab = debugSettingsSubButtonPrefab;
			debugSettingsPopup = screenReference;
		}

		public void setNewDataList(List<DebugSettingsCell> leaderboardListT)
		{
			base.dataList = leaderboardListT;
			NotifyDataSetChanged();
		}

		public override int GetModelType(int index)
		{
			switch (GetItem(index).cellType)
			{
			case DebugSettingsCellType.Toggle:
				return 0;
			case DebugSettingsCellType.SubToggle:
				return 2;
			case DebugSettingsCellType.Button:
				return 1;
			case DebugSettingsCellType.Space:
				return 3;
			case DebugSettingsCellType.Expandable:
				return 4;
			case DebugSettingsCellType.Title:
				return 5;
			case DebugSettingsCellType.SubButton:
				return 6;
			default:
				return 0;
			}
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			DebugSettingsCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				DebugSettingsCellReferences debugSettingsCellReferences = new DebugSettingsCellReferences();
				switch (item.cellType)
				{
				case DebugSettingsCellType.Title:
				{
					container2 = new Container(debugTitlePrefab);
					DebugSettingsHelper component9 = container2.model.GetComponent<DebugSettingsHelper>();
					debugSettingsCellReferences.debugSettingsHelper = component9;
					container2.tag = debugSettingsCellReferences;
					break;
				}
				case DebugSettingsCellType.Toggle:
				{
					container2 = new Container(debugTogglePrefab);
					DebugSettingsHelper component7 = container2.model.GetComponent<DebugSettingsHelper>();
					debugSettingsCellReferences.debugSettingsHelper = component7;
					UIButtonMessage component8 = container2.model.GetComponent<UIButtonMessage>();
					debugSettingsCellReferences.uIButtonMessage = component8;
					container2.tag = debugSettingsCellReferences;
					break;
				}
				case DebugSettingsCellType.SubToggle:
				{
					container2 = new Container(debugSubTogglePrefab);
					DebugSettingsHelper component5 = container2.model.GetComponent<DebugSettingsHelper>();
					debugSettingsCellReferences.debugSettingsHelper = component5;
					UIButtonMessage component6 = container2.model.GetComponent<UIButtonMessage>();
					debugSettingsCellReferences.uIButtonMessage = component6;
					container2.tag = debugSettingsCellReferences;
					break;
				}
				case DebugSettingsCellType.Button:
				{
					container2 = new Container(debugButtonPrefab);
					DebugSettingsHelper component3 = container2.model.GetComponent<DebugSettingsHelper>();
					debugSettingsCellReferences.debugSettingsHelper = component3;
					UIButtonMessage component4 = container2.model.GetComponent<UIButtonMessage>();
					debugSettingsCellReferences.uIButtonMessage = component4;
					container2.tag = debugSettingsCellReferences;
					break;
				}
				case DebugSettingsCellType.SubButton:
				{
					container2 = new Container(debugSubButtonPrefab);
					DebugSettingsHelper component2 = container2.model.GetComponent<DebugSettingsHelper>();
					debugSettingsCellReferences.debugSettingsHelper = component2;
					UIButtonMessage componentInChildren = container2.model.GetComponentInChildren<UIButtonMessage>();
					debugSettingsCellReferences.uIButtonMessage = componentInChildren;
					container2.tag = debugSettingsCellReferences;
					break;
				}
				case DebugSettingsCellType.Expandable:
				{
					container2 = new Container(debugExpandablePrefab);
					DebugSettingsHelper component = container2.model.GetComponent<DebugSettingsHelper>();
					debugSettingsCellReferences.debugSettingsHelper = component;
					container2.tag = debugSettingsCellReferences;
					break;
				}
				case DebugSettingsCellType.Space:
					container2 = new Container(debugSpacePrefab);
					container2.tag = debugSettingsCellReferences;
					break;
				}
			}
			DebugSettingsCellReferences debugSettingsCellReferences2 = (DebugSettingsCellReferences)container2.tag;
			switch (item.cellType)
			{
			case DebugSettingsCellType.Title:
				debugSettingsCellReferences2.debugSettingsHelper.SetTesterIcon(item.cellButtonIcon);
				debugSettingsCellReferences2.debugSettingsHelper.UpdateTitle(item.cellTitle);
				debugSettingsCellReferences2.debugSettingsHelper.UpdateDescription(item.cellDescription);
				break;
			case DebugSettingsCellType.Toggle:
				debugSettingsCellReferences2.debugSettingsHelper.UpdateTitle(item.cellTitle);
				debugSettingsCellReferences2.debugSettingsHelper.UpdateDescription(item.cellDescription);
				debugSettingsCellReferences2.debugSettingsHelper.SetToggleIcon(item.settingType);
				debugSettingsCellReferences2.uIButtonMessage.target = debugSettingsPopup.gameObject;
				debugSettingsCellReferences2.uIButtonMessage.functionName = item.cellFunctionName;
				break;
			case DebugSettingsCellType.SubToggle:
				debugSettingsCellReferences2.debugSettingsHelper.UpdateTitle(item.cellTitle);
				debugSettingsCellReferences2.debugSettingsHelper.SetToggleIcon(item.settingType);
				debugSettingsCellReferences2.uIButtonMessage.target = debugSettingsPopup.gameObject;
				debugSettingsCellReferences2.uIButtonMessage.functionName = item.cellFunctionName;
				break;
			case DebugSettingsCellType.Button:
				debugSettingsCellReferences2.debugSettingsHelper.SetTesterIcon(item.cellButtonIcon);
				debugSettingsCellReferences2.debugSettingsHelper.UpdateTitle(item.cellTitle);
				debugSettingsCellReferences2.debugSettingsHelper.UpdateDescription(item.cellDescription);
				debugSettingsCellReferences2.uIButtonMessage.target = debugSettingsPopup.gameObject;
				debugSettingsCellReferences2.uIButtonMessage.functionName = item.cellFunctionName;
				break;
			case DebugSettingsCellType.SubButton:
				debugSettingsCellReferences2.debugSettingsHelper.UpdateTitle(item.cellTitle);
				debugSettingsCellReferences2.debugSettingsHelper.UpdateDescription(item.cellDescription);
				debugSettingsCellReferences2.uIButtonMessage.target = debugSettingsPopup.gameObject;
				debugSettingsCellReferences2.uIButtonMessage.functionName = item.cellFunctionName;
				break;
			case DebugSettingsCellType.Expandable:
				debugSettingsCellReferences2.debugSettingsHelper.UpdateTitle(item.cellTitle);
				debugSettingsCellReferences2.debugSettingsHelper.multiSelect = item.multiSelect;
				debugSettingsCellReferences2.debugSettingsHelper.SetExpandableList(item.settingType);
				break;
			default:
				Debug.LogError("No cell type match for: " + item.cellType);
				break;
			case DebugSettingsCellType.Space:
				break;
			}
			return container2;
		}
	}

	[SerializeField]
	private GameObject DebugSettingsTogglePrefab;

	[SerializeField]
	private GameObject DebugSettingsSubTogglePrefab;

	[SerializeField]
	private GameObject DebugSettingsExpandablePrefab;

	[SerializeField]
	private GameObject DebugSettingsButtonPrefab;

	[SerializeField]
	private GameObject DebugSettingsSpacePrefab;

	[SerializeField]
	private GameObject DebugSettingsTitlePrefab;

	[SerializeField]
	private GameObject DebugSettingsSubButtonPrefab;

	private static bool _isShowingFps = false;

	public static Dictionary<string, string> completeAwardsTiersSelectedItems = new Dictionary<string, string>();

	public static int completeAwardsTiersSelectedTiers;

	public static float TimeScaleInGameOverrideSelectedValue = -1f;

	public static float TimeScaleInMenuOverrideSelectedValue = -1f;

	private List<DebugSettingsCell> _contentList = new List<DebugSettingsCell>();

	private DebugSettingsListAdapter _adapter;

	[SerializeField]
	private ScrollView _scrollView;

	public static bool DownloadNewOS = false;

	public static bool RefreshInterstitials = false;

	public static bool RefreshVideoAds = false;

	private GameObject fpsObject;

	public static bool IsShowingFPS
	{
		get
		{
			return _isShowingFps;
		}
	}

	public override void Init()
	{
		base.Init();
		if (_contentList != null)
		{
			_adapter = new DebugSettingsListAdapter(_contentList, DebugSettingsTogglePrefab, DebugSettingsButtonPrefab, DebugSettingsSubTogglePrefab, DebugSettingsSpacePrefab, DebugSettingsExpandablePrefab, DebugSettingsTitlePrefab, DebugSettingsSubButtonPrefab, this);
		}
		if (Globals.DEBUG_POPUP_TESTOLOGY)
		{
			SetContentListTestology();
		}
		else
		{
			SetContentList();
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (DownloadNewOS)
		{
			DownloadNewOS = false;
			OnlineSettings.instance.DownloadNow();
		}
		if (RefreshInterstitials)
		{
			RefreshInterstitials = false;
			ScreenAdsManager.instance.InitProviderListAndCache();
		}
		if (RefreshVideoAds)
		{
			RefreshVideoAds = false;
			VideoAdsManager.instance.TryCachingVideosAgain();
		}
	}

	private void SetContentListTestology()
	{
		ResetList();
		InsertButton("Memory Benchmark", "Runs a memory benchmark", "StartMemoryBenchmark", ButtonIcon.None);
		InsertButton("FPS Test", "Runs a fps test", "StartAutoFPSTest", ButtonIcon.None);
		if (_scrollView != null && _adapter != null)
		{
			_scrollView.SetAdapter(_adapter);
		}
	}

	private void SetContentList()
	{
		ResetList();
		InsertToggle("All Characters", "Enables all chars before opening screen", "ToggleAllChars", DebugSetting.Characters);
		InsertToggle("All Boards", "Enables all boards before opening screen", "ToggleAllBoards", DebugSetting.Boards);
		InsertToggle("Free Purchases", "Buy anything anywhere for free", "ToggleFreePurchases", DebugSetting.Purchases);
		InsertToggle("Skip Daily Letters", "Daily letters will not be spawned", "ToggleSkipDailyLetters", DebugSetting.Letters);
		InsertToggle("Print Spawned Objects", "Prints a list of all spawned objects in a game", "TogglePrintPowerups", DebugSetting.Powerups);
		InsertToggle("Show Front Screen", "Enables the FrontScreen UI for a new user", "ToggleShowFrontScreen", DebugSetting.FrontScreen);
		InsertToggle("TimeScale Pausebutton", "Enables the TimesSale pausebutton", "DisplayTimescalePauseButton", DebugSetting.TimescaleButton);
		InsertGroupTitle("Override TimeScale", "Play in slow/fast motion");
		InsertGroupExpandable("InMenu", DebugSetting.TimeScaleInMenuOverride);
		InsertGroupExpandable("InGame", DebugSetting.TimeScaleInGameOverride);
		InsertGroupBottom();
		InsertGroupTop("Online Settings Set", "Enables a specific set of OnlineSettings", "ToggleUseOnlineSettingsSet", DebugSetting.UseOSSet);
		InsertGroupExpandable("Set", DebugSetting.OSSet);
		InsertGroupToggle("Use test time", "ToggleOnlineSettingsTestTime", DebugSetting.OSTesttime);
		InsertGroupBottom();
		InsertGroupTop("Interstitials", "Allows debugging of specific providers", "ToggleDebugInterstitialProviders", DebugSetting.Interstitials);
		InsertGroupExpandable("Provider", DebugSetting.InterstitialProvider, true);
		InsertGroupToggle("Ignore time limit", "ToggleDebugInterstitialTimelimit", DebugSetting.InterstitialTimelimit);
		InsertGroupToggle("Ignore in-app purchases", "ToggleDebugInterstitialInApp", DebugSetting.InterstitialInApp);
		InsertGroupBottom();
		InsertGroupTop("Video Ads", "Allows debugging of specific providers", "ToggleDebugVideoAdProviders", DebugSetting.VideoAds);
		InsertGroupExpandable("Provider", DebugSetting.VideoAdsProvider, true);
		InsertGroupToggle("Skip Video Ads", "SkipVideoAds", DebugSetting.SkipVideoAds);
		InsertGroupBottom();
		InsertToggle("Character Shader Debug", "Toggles global shader debug flag", "ToggleCharacterShaderDebug", DebugSetting.CharacterShaderDebug);
		InsertToggle("Character No Hit Collision", "Enables characters to pass through trains", "ToggleCharacterNoHitCollisions", DebugSetting.CharacterNoHitCollission);
		InsertToggle("Display FPS", "Enables the FPS counter", "DisplayFpsCounter", DebugSetting.FPS);
		InsertGroupTop("Record FPS", "Enables the FPS recorder", "RecordFPS", DebugSetting.RecordFPS);
		InsertGroupToggle("Upload FPS to Google", "ToggleUploadFPS", DebugSetting.UploadFPS);
		InsertGroupExpandable("Target FrameRate", DebugSetting.FPSTargetFramerate);
		InsertGroupSubButton("Auto Run", "Start", "StartAutoFPSTest");
		InsertGroupBottom();
		InsertToggle("Show Performance Info", "Shows performance info in the dev screen", "ToggleShowEndPerformanceInDevScreen", DebugSetting.PerformanceInfo);
		InsertGroupTitle("Awards Test", "Complete tiers on selected awards");
		InsertGroupExpandable("Award", DebugSetting.AwardTestAward, true);
		InsertGroupExpandable("Nr of Tiers", DebugSetting.AwardTestTier);
		InsertGroupSubButton(string.Empty, "Enable", "CompleteAwardsTiers");
		InsertGroupBottom();
		InsertGroupTop("Override Character Popup", "Overrides character shown in the popup", "ToggleDebugLimitedCharacterPopup", DebugSetting.LimitedCharacterPopup);
		InsertGroupToggle("Ignore expire time", "ToggleLimitedCharacterPopupIgnoreExpire", DebugSetting.LimitedCharacterPopupIgnoreExpire);
		InsertGroupExpandable("Character", DebugSetting.LimitedCharacterPopupType);
		InsertGroupBottom();
		InsertGroupTop("Override Board Popup", "Overrides board shown in the popup", "ToggleDebugLimitedBoardPopup", DebugSetting.LimitedBoardPopup);
		InsertGroupToggle("Ignore expire time", "ToggleLimitedBoardPopupIgnoreExpire", DebugSetting.LimitedBoardPopupIgnoreExpire);
		InsertGroupExpandable("Board", DebugSetting.LimitedBoardPopupType);
		InsertGroupBottom();
		InsertButton("Open Event Tester", "Use this for testing different events", "OpenEventTesterPopup", ButtonIcon.EventTester);
		InsertButton("Open Celebration Tester", "Use this for testing celebration screens", "OpenCelebrationTesterPopup", ButtonIcon.CelebrationTester);
		InsertButton("Memory Benchmark", "Runs a memory benchmark", "StartMemoryBenchmark", ButtonIcon.None);
		if (_scrollView != null && _adapter != null)
		{
			_scrollView.SetAdapter(_adapter);
		}
	}

	private void InsertGroupTitle(string title, string description)
	{
		_contentList.Add(new DebugSettingsCell
		{
			cellType = DebugSettingsCellType.Title,
			cellTitle = title,
			cellDescription = description,
			cellButtonIcon = ButtonIcon.None
		});
	}

	private void InsertGroupTop(string title, string description, string functionName, DebugSetting settingType)
	{
		_contentList.Add(new DebugSettingsCell
		{
			cellType = DebugSettingsCellType.Toggle,
			cellTitle = title,
			cellDescription = description,
			cellFunctionName = functionName,
			settingType = settingType
		});
	}

	private void InsertGroupBottom()
	{
		AddSpace();
	}

	private void InsertToggle(string title, string description, string functionName, DebugSetting settingType)
	{
		_contentList.Add(new DebugSettingsCell
		{
			cellType = DebugSettingsCellType.Toggle,
			cellTitle = title,
			cellDescription = description,
			cellFunctionName = functionName,
			settingType = settingType
		});
		AddSpace();
	}

	private void InsertButton(string title, string description, string functionName, ButtonIcon buttonIcon)
	{
		_contentList.Add(new DebugSettingsCell
		{
			cellType = DebugSettingsCellType.Button,
			cellTitle = title,
			cellDescription = description,
			cellFunctionName = functionName,
			cellButtonIcon = buttonIcon
		});
		AddSpace();
	}

	private void InsertGroupSubButton(string title, string buttonText, string functionName)
	{
		_contentList.Add(new DebugSettingsCell
		{
			cellType = DebugSettingsCellType.SubButton,
			cellTitle = title,
			cellDescription = buttonText,
			cellFunctionName = functionName
		});
	}

	private void InsertGroupToggle(string title, string functionName, DebugSetting settingType)
	{
		_contentList.Add(new DebugSettingsCell
		{
			cellType = DebugSettingsCellType.SubToggle,
			cellTitle = title,
			cellFunctionName = functionName,
			settingType = settingType
		});
	}

	private void AddSpace()
	{
		_contentList.Add(new DebugSettingsCell
		{
			cellType = DebugSettingsCellType.Space
		});
	}

	private void InsertGroupExpandable(string title, DebugSetting settingType, bool multiSelect = false)
	{
		_contentList.Add(new DebugSettingsCell
		{
			cellType = DebugSettingsCellType.Expandable,
			cellTitle = title,
			settingType = settingType,
			multiSelect = multiSelect
		});
	}

	private void ResetList()
	{
		_contentList.Clear();
	}

	private bool IsAllowingDebugging()
	{
		if (!Globals.DEBUG || !Globals.DEBUG_POPUP)
		{
			UIScreenController.Instance.ClosePopup();
		}
		return Globals.DEBUG && Globals.DEBUG_POPUP;
	}

	private void SkipVideoAds()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_SKIP_VIDEO_ADS = !Globals.DEBUG_SKIP_VIDEO_ADS;
		}
	}

	private void ToggleShowEndPerformanceInDevScreen()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_SHOW_PERFORMANCE_INFO = !Globals.DEBUG_SHOW_PERFORMANCE_INFO;
		}
	}

	private void ToggleCharacterNoHitCollisions()
	{
		if (IsAllowingDebugging())
		{
			bool ignoreLayerCollision = Physics.GetIgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit);
			Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, !ignoreLayerCollision);
			Globals.DEBUG_CHARACTER_MOVE_THROUGH_COLLIDERS = !ignoreLayerCollision;
		}
	}

	private void ToggleCharacterShaderDebug()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_SHADERS = !Globals.DEBUG_SHADERS;
			Shader.SetGlobalInt("_DebugShaders", Globals.DEBUG_SHADERS ? 1 : 0);
		}
	}

	private void ToggleAllChars()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_ALL_CHARS = !Globals.DEBUG_ALL_CHARS;
			if (!Globals.DEBUG_ALL_CHARS)
			{
				PlayerInfo.Instance.SetLastSelectedTheme(Characters.CharacterType.slick, 0);
				UIModelController.Instance.SelectCharacterForPlay(Characters.CharacterType.slick, 0);
			}
			CharacterScreen.forceRefreshOnCharacterScreen = true;
		}
	}

	private void ToggleAllBoards()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_ALL_BOARDS = !Globals.DEBUG_ALL_BOARDS;
			if (!Globals.DEBUG_ALL_BOARDS)
			{
				Hoverboards.BoardType boardType = Hoverboards.BoardType.starboard;
				UIModelController.Instance.SelectCurrentlyShownBoard(boardType);
			}
			BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
		}
	}

	private void ToggleFreePurchases()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_FREE_INAPP_PURCHASE = !Globals.DEBUG_FREE_INAPP_PURCHASE;
		}
	}

	private void ToggleSkipDailyLetters()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_SKIP_DAILY_LETTERS = !Globals.DEBUG_SKIP_DAILY_LETTERS;
		}
	}

	private void TogglePrintPowerups()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_PRINT_SPAWNED_POWER_UPS = !Globals.DEBUG_PRINT_SPAWNED_POWER_UPS;
		}
	}

	private void ToggleShowFrontScreen()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_SHOW_FRONT_SCREEN_IF_NEW_USER = !Globals.DEBUG_SHOW_FRONT_SCREEN_IF_NEW_USER;
			if (!Application.isEditor)
			{
				UIScreen screenFromCache = UIScreenController.Instance.GetScreenFromCache("FrontUI");
				screenFromCache.gameObject.SetActive(false);
				screenFromCache.gameObject.SetActive(true);
			}
		}
	}

	private void OpenEventTesterPopup()
	{
		if (IsAllowingDebugging())
		{
			UIScreenController.Instance.PushPopup("EventTesterPopup");
		}
	}

	private void OpenCelebrationTesterPopup()
	{
		if (IsAllowingDebugging())
		{
			UIScreenController.Instance.PushPopup("CelebrationTesterPopup");
		}
	}

	private void StartMemoryBenchmark()
	{
		if (IsAllowingDebugging())
		{
			MemoryLog.Instance.PromptStartMemoryBenchmark();
		}
	}

	private void ToggleDebugInterstitialProviders()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_INTERSTITIAL_PROVIDERS = !Globals.DEBUG_INTERSTITIAL_PROVIDERS;
			RefreshInterstitials = true;
		}
	}

	private void ToggleDebugVideoAdProviders()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_VIDEOADS_PROVIDERS = !Globals.DEBUG_VIDEOADS_PROVIDERS;
			RefreshVideoAds = true;
		}
	}

	private void ToggleDebugInterstitialTimelimit()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT = !Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT;
			RefreshInterstitials = true;
		}
	}

	private void ToggleDebugInterstitialInApp()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IN_APP_PURCHASES = !Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IN_APP_PURCHASES;
			RefreshInterstitials = true;
		}
	}

	private void ToggleUseOnlineSettingsSet()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_USE_ONLINESETTINGS_SET = !Globals.DEBUG_USE_ONLINESETTINGS_SET;
			DownloadNewOS = true;
		}
	}

	private void ToggleOnlineSettingsTestTime()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_USE_ONLINESETTINGS_TESTTIME = !Globals.DEBUG_USE_ONLINESETTINGS_TESTTIME;
		}
	}

	private void DisplayFpsCounter()
	{
		if (IsAllowingDebugging())
		{
			if (fpsObject == null)
			{
				fpsObject = new GameObject("FpsManager");
				fpsObject.name = "FpsManager";
				fpsObject.AddComponent<HUDFPS>();
				_isShowingFps = true;
			}
			else if (fpsObject.activeSelf)
			{
				fpsObject.SetActive(false);
				_isShowingFps = false;
			}
			else
			{
				fpsObject.SetActive(true);
				fpsObject.GetComponent<HUDFPS>().StartFpsCounter();
				_isShowingFps = true;
			}
		}
	}

	private void RecordFPS()
	{
		if (IsAllowingDebugging())
		{
			if (FPSLog.instance.IsLoggingFPS)
			{
				FPSLog.instance.SetFPSLogging(false);
			}
			else
			{
				FPSLog.instance.SetFPSLogging(true);
			}
		}
	}

	private void ToggleUploadFPS()
	{
		if (!IsAllowingDebugging())
		{
			return;
		}
		if (FPSLog.instance.IsUploadingFPS)
		{
			FPSLog.instance.SetFPSUpload(false);
		}
		else if (FPSLog.instance.TestGoogleConnection())
		{
			FPSLog.instance.SetFPSUpload(true);
			if (!FPSLog.instance.IsLoggingFPS)
			{
				FPSLog.instance.SetFPSLogging(true);
			}
			RefreshScrollList();
		}
		else
		{
			RefreshScrollList();
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.CONNECTION_TEST_FAILED_ERROR_MESSAGE));
		}
	}

	private void StartAutoFPSTest()
	{
		if (FPSLog.instance.TestGoogleConnection())
		{
			FPSLog.instance.StartAutoFPS();
		}
		else
		{
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.CONNECTION_TEST_FAILED_ERROR_MESSAGE));
		}
	}

	private void RefreshScrollList()
	{
		int dataIndex = _scrollView.GetFirstVisibleContainerOnScreen().dataIndex;
		SetContentList();
		_scrollView.SetSelection(dataIndex, ContainerPivot.Top, 0f);
	}

	private void DisplayTimescalePauseButton()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_USE_TIMESCALE_PAUSE_BUTTON = !Globals.DEBUG_USE_TIMESCALE_PAUSE_BUTTON;
		}
	}

	private void ToggleDebugLimitedCharacterPopup()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_LIMITED_CHARACTER_POPUP = !Globals.DEBUG_LIMITED_CHARACTER_POPUP;
		}
	}

	private void ToggleLimitedCharacterPopupIgnoreExpire()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_LIMITED_CHARACTER_POPUP_IGNORE_EXPIRE = !Globals.DEBUG_LIMITED_CHARACTER_POPUP_IGNORE_EXPIRE;
		}
	}

	private void ToggleDebugLimitedBoardPopup()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_LIMITED_BOARD_POPUP = !Globals.DEBUG_LIMITED_BOARD_POPUP;
		}
	}

	private void ToggleLimitedBoardPopupIgnoreExpire()
	{
		if (IsAllowingDebugging())
		{
			Globals.DEBUG_LIMITED_BOARD_POPUP_IGNORE_EXPIRE = !Globals.DEBUG_LIMITED_BOARD_POPUP_IGNORE_EXPIRE;
		}
	}

	private void CompleteAwardsTiers()
	{
		if (!Globals.DEBUG_POPUP)
		{
			return;
		}
		foreach (string key in completeAwardsTiersSelectedItems.Keys)
		{
			for (int i = 0; i < completeAwardsTiersSelectedTiers; i++)
			{
				AwardsManager.Instance.GetAwardForId((AwardId)(int)Enum.Parse(typeof(AwardId), key)).CompleteCurrentTierIfDebug();
			}
			AwardsManager.Instance.GetAwardForId((AwardId)(int)Enum.Parse(typeof(AwardId), key)).awardState = AwardState.Collectible;
		}
		UIScreenController.Instance.QueueErrorMessageSlidein("Award tier(s)\nare completed");
	}
}
