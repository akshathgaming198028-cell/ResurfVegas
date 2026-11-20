using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class UpgradeScreen : UIScreen
{
	public GameObject DoubleCoinsPrefab;

	public GameObject BundlesPrefab;

	public GameObject FacebookRewardsButtonPrefab;

	public GameObject ConsumablePrefab;

	public GameObject PermanentPrefab;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIPanel _panel;

	[SerializeField]
	private UIScrollView _parentDragPanel;

	[SerializeField]
	private UIWidget _outline;

	[SerializeField]
	private ScrollViewHeightResizer _scrollViewHeightResizer;

	[SerializeField]
	private GameObject listTitleComponent;

	private GameObject[] skipMissions = new GameObject[3];

	private string[] skipMissionNames = new string[3];

	public List<UpgradeHelper> cachedUpgradeHelpers = new List<UpgradeHelper>(Upgrades.upgrades.Count);

	private UIScreenController screenController;

	private GameObject _bundlesButton;

	private bool _isReturningFromCelebration;

	private GameObject _facebookRewardsButton;

	private bool _facebookLoginRunning;

	[NonSerialized]
	public PowerupType[] powerupSingleUse = new PowerupType[4]
	{
		PowerupType.hoverboard,
		PowerupType.mysterybox,
		PowerupType.scorebooster,
		PowerupType.headstart2000
	};

	[NonSerialized]
	public PowerupType[] powerupSkipMission = new PowerupType[3]
	{
		PowerupType.skipmission1,
		PowerupType.skipmission2,
		PowerupType.skipmission3
	};

	[NonSerialized]
	public PowerupType[] powerupPermanent = new PowerupType[4]
	{
		PowerupType.jetpack,
		PowerupType.supersneakers,
		PowerupType.coinmagnet,
		PowerupType.doubleMultiplier
	};

	public static bool forceScrollRearange = false;

	private bool _isSubscribedToChangedScreen;

	private static readonly int HEIGHT_OF_DOUBLE_COINS_BUTTON = 70;

	private static readonly int HEIGHT_OF_BUNDLE_BUTTON = 80;

	private static readonly int HEIGHT_OF_FACEBOOK_BUTTON = 80;

	private static char[] SPLIT_CHARACTER = new char[1] { ';' };

	private IEventManager _activeEventManager;

	private float _uiPanelBoundsOffset;

	private float _scrollPanelStartY;

	private bool _hasRefreshedTable;

	private int numberOfObjects;

	private int _indexExpanded = -1;

	public int IndexExpanded
	{
		get
		{
			return _indexExpanded;
		}
		set
		{
			_indexExpanded = value;
		}
	}

	private void Start()
	{
		StartCoroutine(ReadYPanelPosAfterFrames(1));
		UIScreenController.OnNavigateToScreen += OnScreenChanged;
		OnScreenChanged("UpgradesUI_shop");
	}

	private void OnEnable()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onProductRequestDone += OnProductRequestDone;
		}
		_panel.onConstrainedTargetToBounds += AdjustForUIPanelBoundsOffset;
	}

	private void OnDisable()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onProductRequestDone -= OnProductRequestDone;
		}
		_panel.onConstrainedTargetToBounds -= AdjustForUIPanelBoundsOffset;
	}

	private IEnumerator ReadYPanelPosAfterFrames(int numberofFrames)
	{
		int frames = 0;
		while (frames < numberofFrames)
		{
			frames++;
			yield return null;
		}
		_scrollPanelStartY = _parentDragPanel.transform.localPosition.y;
		SetScrollPanelYForTableElements();
	}

	public override void Init()
	{
		base.Init();
		MissionsManager instance = MissionsManager.Instance;
		instance.onMissionComplete = (MissionsManager.MissionCompleteHandler)Delegate.Combine(instance.onMissionComplete, new MissionsManager.MissionCompleteHandler(OnMissionComplete));
		MissionsManager instance2 = MissionsManager.Instance;
		instance2.onMissionSetComplete = (MissionsManager.MissionSetCompleteHandler)Delegate.Combine(instance2.onMissionSetComplete, new MissionsManager.MissionSetCompleteHandler(OnMissionSetComplete));
		numberOfObjects = 0;
		FillTable();
		screenController = UIScreenController.Instance;
		InitializeCoinbox(_outline, true, true, true);
	}

	public override void Show()
	{
		base.Show();
		InAppData.BundleTier lastPurchasedBundle = PlayerInfo.Instance.lastPurchasedBundle;
		if ((lastPurchasedBundle == InAppData.BundleTier.BundleTier5 || !BundlesActiveInThisLocale()) && _bundlesButton != null)
		{
			UnityEngine.Object.Destroy(_bundlesButton);
			_bundlesButton = null;
		}
		if (_footerHandler != null)
		{
			_footerHandler.HandleStickers();
		}
		if (screenController.GetTopScreenName() == "GameoverUI")
		{
			GameOverScreen gameOverScreen = screenController.GetScreenFromCache(screenController.GetTopScreenName()) as GameOverScreen;
			gameOverScreen.StopCoinsCountUp();
		}
		_activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (IsJackpotEventActive())
		{
			forceScrollRearange = true;
		}
		if (forceScrollRearange && !_isReturningFromCelebration)
		{
			forceScrollRearange = false;
			if (_scrollViewHeightResizer != null)
			{
				_parentDragPanel.ResetPosition();
			}
		}
		StartCoroutine(RefreshTable(_isReturningFromCelebration));
		if (VideoAdsManager.instance.IsAnyVideoAvailable())
		{
			return;
		}
		VideoAdsManager.VideoState currentVideoState = VideoAdsManager.instance.GetCurrentVideoState();
		if (VideoAdsManager.instance.isInitialized)
		{
			VideoAdsManager.instance.ResumeProvider();
			if (currentVideoState == VideoAdsManager.VideoState.NoVideos || currentVideoState == VideoAdsManager.VideoState.Offline)
			{
				VideoAdsManager.instance.TryCachingVideosAgain();
			}
		}
	}

	private bool IsJackpotEventActive()
	{
		if (_activeEventManager != null && _activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
		{
			return true;
		}
		return false;
	}

	private IEnumerator RefreshTable(bool isReturningFromCelebration)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		if (IsJackpotEventActive() && !isReturningFromCelebration && screenController.GetTopScreenName() != "GameoverUI")
		{
			float lengthToScroll = HEIGHT_OF_DOUBLE_COINS_BUTTON;
			if (PlayerInfo.Instance.lastPurchasedBundle != InAppData.BundleTier.BundleTier5)
			{
				lengthToScroll += (float)HEIGHT_OF_BUNDLE_BUTTON;
			}
			if (!PlayerInfo.Instance.hasPayedOutFacebook)
			{
				lengthToScroll += (float)HEIGHT_OF_FACEBOOK_BUTTON;
			}
			UIPanel dragPanel = _parentDragPanel.GetComponent<UIPanel>();
			Vector3 pos = dragPanel.transform.localPosition;
			pos.y += lengthToScroll;
			dragPanel.transform.localPosition = pos;
			Vector2 pos2 = Vector2.zero;
			pos2.y = lengthToScroll;
			dragPanel.clipOffset -= pos2;
		}
		_table.Reposition();
		_hasRefreshedTable = true;
		if (isReturningFromCelebration)
		{
			SetScrollPanelYForTableElements();
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
	}

	public void OnScreenChanged(string screenName)
	{
		if (screenName != "UpgradesUI_shop")
		{
			if (_isSubscribedToChangedScreen)
			{
				UIScreenController.Instance.OnPopupClosed -= PopupClosed;
				_isReturningFromCelebration = false;
				_isSubscribedToChangedScreen = false;
			}
		}
		else if (!_isSubscribedToChangedScreen)
		{
			UIScreenController.Instance.OnPopupClosed += PopupClosed;
			_isSubscribedToChangedScreen = true;
		}
	}

	private void OnDestroy()
	{
		if (!screenController.stoppingFromEditor)
		{
			MissionsManager instance = MissionsManager.Instance;
			instance.onMissionComplete = (MissionsManager.MissionCompleteHandler)Delegate.Remove(instance.onMissionComplete, new MissionsManager.MissionCompleteHandler(OnMissionComplete));
			MissionsManager instance2 = MissionsManager.Instance;
			instance2.onMissionSetComplete = (MissionsManager.MissionSetCompleteHandler)Delegate.Remove(instance2.onMissionSetComplete, new MissionsManager.MissionSetCompleteHandler(OnMissionSetComplete));
			UIScreenController.OnNavigateToScreen -= OnScreenChanged;
		}
	}

	private void PopupClosed(string popupName)
	{
		if (popupName == "CelebrationPopup")
		{
			_isReturningFromCelebration = true;
		}
	}

	private void FillTable()
	{
		GameObject gameObject = NGUITools.AddChild(_table.gameObject, DoubleCoinsPrefab);
		gameObject.name = string.Format("{0:000}", numberOfObjects);
		gameObject.GetComponent<DoubleCoinUpgradeHelper>().Init();
		gameObject.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
		numberOfObjects++;
		_bundlesButton = NGUITools.AddChild(_table.gameObject, BundlesPrefab);
		_bundlesButton.name = string.Format("{0:000}", numberOfObjects);
		_bundlesButton.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
		numberOfObjects++;
		if (!PlayerInfo.Instance.hasPayedOutFacebookPack)
		{
			_facebookRewardsButton = NGUITools.AddChild(_table.gameObject, FacebookRewardsButtonPrefab);
			_facebookRewardsButton.name = string.Format("{0:000}", numberOfObjects);
			_facebookRewardsButton.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
			_facebookRewardsButton.GetComponent<FacebookRewardsButton>().upgradeScreen = this;
			numberOfObjects++;
		}
		GameObject gameObject2 = NGUITools.AddChild(_table.gameObject, listTitleComponent);
		ListTitleComponentHelper component = gameObject2.GetComponent<ListTitleComponentHelper>();
		gameObject2.name = string.Format("{0:000}", numberOfObjects);
		component.Setup(Strings.Get(StringID.UPGRADE_SCREEN_SINGLE_USE), new Color(0f, 0.2901961f, 0.5019608f, 1f), -145f);
		numberOfObjects++;
		PowerupType[] array = powerupSingleUse;
		foreach (PowerupType powerupType in array)
		{
			MakeBuyable(powerupType, false);
			numberOfObjects++;
		}
		for (int j = 0; j < powerupSkipMission.Length; j++)
		{
			string text = string.Format("{0:000}", numberOfObjects);
			skipMissionNames[j] = text;
			if (!MissionsManager.Instance.GetMissionInfo(j).complete)
			{
				skipMissions[j] = MakeBuyable(powerupSkipMission[j], false);
			}
			numberOfObjects++;
		}
		GameObject gameObject3 = NGUITools.AddChild(_table.gameObject, listTitleComponent);
		ListTitleComponentHelper component2 = gameObject3.GetComponent<ListTitleComponentHelper>();
		gameObject3.name = string.Format("{0:000}", numberOfObjects);
		component2.Setup(Strings.Get(StringID.UPGRADES_TITLE), new Color(0f, 0.2901961f, 0.5019608f, 1f), -142f);
		numberOfObjects++;
		PowerupType[] array2 = powerupPermanent;
		foreach (PowerupType powerupType2 in array2)
		{
			MakeBuyable(powerupType2, true);
			numberOfObjects++;
		}
	}

	private GameObject MakeBuyable(PowerupType powerupType, bool permanent)
	{
		GameObject gameObject;
		if (permanent)
		{
			gameObject = NGUITools.AddChild(_table.gameObject, PermanentPrefab);
			UIPanel component = _parentDragPanel.GetComponent<UIPanel>();
			gameObject.GetComponent<UpgradeHelper>().InitPermanent(powerupType, component);
		}
		else
		{
			gameObject = NGUITools.AddChild(_table.gameObject, ConsumablePrefab);
			UIPanel component2 = _parentDragPanel.GetComponent<UIPanel>();
			gameObject.GetComponent<UpgradeHelper>().InitSingle(powerupType, component2);
		}
		gameObject.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
		gameObject.name = string.Format("{0:000}", numberOfObjects);
		NGUITools.AddWidgetCollider(gameObject);
		cachedUpgradeHelpers.Add(gameObject.GetComponent<UpgradeHelper>());
		MakeTableElementAlwaysVisible(gameObject, numberOfObjects);
		return gameObject;
	}

	private void OnMissionComplete(string payload)
	{
		for (int i = 0; i < skipMissions.Length; i++)
		{
			if (skipMissions[i] != null && MissionsManager.Instance.GetMissionInfo(i).complete)
			{
				cachedUpgradeHelpers.Remove(skipMissions[i].GetComponent<UpgradeHelper>());
				NGUITools.SetActive(skipMissions[i], false);
				UnityEngine.Object.Destroy(skipMissions[i]);
			}
		}
		_table.repositionNow = true;
	}

	private void OnMissionSetComplete()
	{
		for (int i = 0; i < skipMissions.Length; i++)
		{
			if (skipMissions[i] != null)
			{
				cachedUpgradeHelpers.Remove(skipMissions[i].GetComponent<UpgradeHelper>());
				UnityEngine.Object.Destroy(skipMissions[i]);
			}
		}
		for (int j = 0; j < powerupSkipMission.Length; j++)
		{
			if (!MissionsManager.Instance.GetMissionInfo(j).complete)
			{
				GameObject gameObject = NGUITools.AddChild(_table.gameObject, ConsumablePrefab);
				gameObject.name = skipMissionNames[j];
				UIPanel component = _parentDragPanel.GetComponent<UIPanel>();
				gameObject.GetComponent<UpgradeHelper>().InitSingle(powerupSkipMission[j], component);
				gameObject.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
				NGUITools.AddWidgetCollider(gameObject);
				skipMissions[j] = gameObject;
				cachedUpgradeHelpers.Add(gameObject.GetComponent<UpgradeHelper>());
				MakeTableElementAlwaysVisible(gameObject, 8 + j);
			}
		}
		if (base.gameObject.activeInHierarchy)
		{
			SetScrollPanelYForTableElements();
			_table.repositionNow = true;
		}
	}

	private void AdjustForUIPanelBoundsOffset(Vector3 offset)
	{
		if (_hasRefreshedTable)
		{
			_uiPanelBoundsOffset -= offset.y;
			SetScrollPanelYForTableElements();
		}
	}

	private void SetScrollPanelYForTableElements()
	{
		TableElementAlwaysVisible[] componentsInChildren = _table.GetComponentsInChildren<TableElementAlwaysVisible>();
		foreach (TableElementAlwaysVisible tableElementAlwaysVisible in componentsInChildren)
		{
			tableElementAlwaysVisible.ScrollPanelStartY = _scrollPanelStartY + _uiPanelBoundsOffset;
		}
	}

	private void MakeTableElementAlwaysVisible(GameObject go, int index)
	{
		TableElementAlwaysVisible component = go.GetComponent<TableElementAlwaysVisible>();
		if (component != null)
		{
			component.Index = index;
			component.Parrent = this;
		}
	}

	public bool IsLastElementOpen()
	{
		bool result = false;
		TableElementAlwaysVisible[] componentsInChildren = _table.GetComponentsInChildren<TableElementAlwaysVisible>();
		foreach (TableElementAlwaysVisible tableElementAlwaysVisible in componentsInChildren)
		{
			if (tableElementAlwaysVisible.Index == _indexExpanded && tableElementAlwaysVisible.IsExpanded)
			{
				result = true;
			}
		}
		return result;
	}

	private bool BundlesActiveInThisLocale()
	{
		string key = "bundles_enabled_locales";
		string key2 = "bundles_disabled_locales";
		string text = "all";
		string text2 = "none";
		string locale = DeviceUtility.GetLocale();
		bool result = false;
		if (!string.IsNullOrEmpty(locale))
		{
			locale = locale.ToLower();
			locale = locale.Replace('-', '_');
			string valueString;
			if (OnlineSettings.instance.TryGetValue(key, out valueString))
			{
				if (string.IsNullOrEmpty(valueString))
				{
					valueString = text;
				}
			}
			else
			{
				valueString = text;
			}
			string valueString2;
			if (OnlineSettings.instance.TryGetValue(key2, out valueString2))
			{
				if (string.IsNullOrEmpty(valueString2))
				{
					valueString2 = text2;
				}
			}
			else
			{
				valueString2 = text2;
			}
			if (!valueString.Contains("all"))
			{
				valueString2 = "all";
			}
			valueString = valueString.ToLower();
			valueString = valueString.Replace('-', '_');
			valueString2 = valueString2.ToLower();
			valueString2 = valueString2.Replace('-', '_');
			string[] array = valueString.Split(SPLIT_CHARACTER);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			string[] array2 = valueString2.Split(SPLIT_CHARACTER);
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = array2[j].Trim();
			}
			if (valueString.Contains("all"))
			{
				for (int k = 0; k < array2.Length; k++)
				{
					if (!locale.Contains(array2[k]))
					{
						result = true;
						break;
					}
				}
			}
			else if (valueString2.Contains("all"))
			{
				for (int l = 0; l < array.Length; l++)
				{
					if (locale.Contains(array[l]))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		return result;
	}

	public void FacebookLoginClicked()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
	}

	public void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private void OnProductRequestDone()
	{
		Show();
	}

	private void QueueFacebookFailSlideIn(Action onFinishCallback)
	{
		UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectFailed, onFinishCallback));
	}

	private void OnFailSlideInFinish()
	{
		FacebookStatusChanged(false);
	}

	private void FacebookStatusChanged(bool success)
	{
		if (success)
		{
			if (!FacebookRewardsInfo.HasReceivedAllAvailableRewards())
			{
				UIScreenController.Instance.PushPopup("FacebookRewardsPopup");
			}
			else
			{
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectSucceeded));
			}
			UnityEngine.Object.Destroy(_facebookRewardsButton);
		}
		else
		{
			if (_facebookLoginRunning)
			{
				OnFacebookLoginResult(false);
			}
			_facebookRewardsButton.GetComponent<FacebookRewardsButton>().GoToActiveState();
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_facebookLoginRunning = false;
	}
}
