using System;
using System.Collections;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using TopRun;
using UnityEngine;

public class FrontScreen : RunStartScreen
{
	[SerializeField]
	private UIAnchor topLeft;

	[SerializeField]
	private Transform topRight;

	[SerializeField]
	private Transform settingsGo;

	[SerializeField]
	private UIAnchor bottomLeft;

	[SerializeField]
	private UIAnchor bottomCenter;

	[SerializeField]
	private UIAnchor bottomRight;

	private bool _hasTriggeredTweens;

	[SerializeField]
	private GameObject[] gameobjectsToTween;

	[SerializeField]
	private GameObject discountSticker;

	[SerializeField]
	private UISprite sticker;

	[SerializeField]
	private BoxCollider startButtonCollider;

	private static bool buttonsHaveTweened;

	[SerializeField]
	private UILabel missionsButtonLabel;

	[SerializeField]
	private UILabel dailyChallengeLabel;

	[SerializeField]
	private UILabel meTopLabel;

	[SerializeField]
	private UILabel meBottomLabel;

	[SerializeField]
	private UILabel shopDiscountLabel1;

	[SerializeField]
	private UILabel shopDiscountLabel2;

	[SerializeField]
	private UILabel shopDiscountLabelSingleLine;

	[SerializeField]
	private GuidelineButton _guidelineButtonShop;

	[SerializeField]
	private UILabel tapToPlayLabel;

	[SerializeField]
	private GameObject _showEventsPopupBtn;

	[SerializeField]
	private UILabel _huntAmountLabel;

	[SerializeField]
	private Transform _huntButtonOutlineTrans;

	[SerializeField]
	private GameObject _limitedCharacterButton;

	[SerializeField]
	private GameObject _limitedBoardButton;

	[SerializeField]
	private QuickVideoButtonHelper _quickVideoBtnHelper;

	private bool _videoReadyToShow;

	private bool _isEventActive;

	private readonly Vector3 QUICKVIDEO_DEFAULT_POSITION_END = new Vector3(-28f, -132f, 0f);

	private readonly Vector3 QUICKVIDEO_DEFAULT_POSITION_START = new Vector3(28f, -132f, 0f);

	private readonly Vector3 QUICKVIDEO_NEW_POSITION_END = new Vector3(-28f, -74f, 0f);

	private readonly Vector3 QUICKVIDEO_NEW_POSITION_START = new Vector3(28f, -74f, 0f);

	private readonly Vector3 LIMITED_CHARACTER_POPUP_POSITION_END = new Vector3(30f, -132f, 0f);

	private readonly Vector3 LIMITED_CHARACTER_POPUP_POSITION_START = new Vector3(-30f, -132f, 0f);

	private WeeklyHuntManager.WeeklyHuntPeriodData? _currentWeekHuntPrize;

	private int _huntCollectedItems;

	private int _huntMaxItems;

	private bool _isApplicationResuming;

	public static event Action tweensHaveFinishedAnimating;

	private void Awake()
	{
		missionsButtonLabel.text = Strings.Get(StringID.FRONT_UI_MISSIONS);
		dailyChallengeLabel.text = Strings.Get(StringID.FRONT_UI_DAILY_CHALLENGE);
		meTopLabel.text = Strings.Get(StringID.FRONT_UI_FREE);
		meBottomLabel.text = Strings.Get(StringID.FRONT_UI_GIFT);
		shopDiscountLabelSingleLine.text = Strings.Get(StringID.FRONT_UI_SALE);
		tapToPlayLabel.text = Strings.Get(StringID.FRONT_UI_TAP_TO_PLAY);
		if (TopRunRoot.manager != null)
		{
			TopRunRoot.manager.tournamentChanged += TournamentChanged;
		}
		GameObject[] array = gameobjectsToTween;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		bool flag = ShouldDisplayChangeLog();
		bool isTutorialCompleted = PlayerInfo.Instance.IsTutorialCompleted;
		VideoAdsManager.instance.AddOnVideoStateChangedHandler(CheckTheVideoButtonState);
		VideoAdsManager.instance.AddOnKeyVideoWatchedChangedHandler(CheckTheVideoButtonState);
		UIScreenController.Instance.OnPopupClosed += OnPopupClosed;
		LimitedCharacterPopupManager.OnLimitedCharacterExpiredHandler += CheckLimitedCharacterButtonState;
		LimitedCharacterPopupManager.OnLimitedCharacterStartedHandler += CheckLimitedCharacterButtonState;
		LimitedBoardPopupManager.OnLimitedBoardExpiredHandler += CheckLimitedBoardButtonState;
		LimitedBoardPopupManager.OnLimitedBoardStartedHandler += CheckLimitedBoardButtonState;
		AssetBundleManager.Instance.StartDownloadIfPossible();
		AssetBundleManager.Instance.PerformAvailabilityCheck();
		if (flag && isTutorialCompleted)
		{
			UIScreenController.Instance.QueuePopup("WelcomePopup");
			PlayerInfo.Instance.lastTimeAppWasUpdatedOrInstalled = DateTime.Now;
		}
		else
		{
			EnableScreenElements();
		}
		if (!PlayerInfo.Instance.IsTutorialCompleted)
		{
			topLeft.gameObject.SetActive(false);
			foreach (Transform item in topRight.transform)
			{
				if (item != settingsGo.transform)
				{
					item.gameObject.SetActive(false);
				}
			}
			bottomLeft.gameObject.SetActive(false);
			bottomCenter.gameObject.SetActive(false);
			bottomRight.gameObject.SetActive(false);
		}
		else
		{
			ActivateScreenElements();
		}
		if (Globals.DEBUG_SHOW_FRONT_SCREEN_IF_NEW_USER)
		{
			ActivateScreenElements();
		}
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

	private void OnDisable()
	{
		SSEventsManagerController.OnSSEventChangedHandler -= CheckTheEventsButtonState;
		SSEventsManagerController.OnSSEventChangedHandler -= SSEventsPopup.ShowIfNeeded;
		UIScreenController.Instance.OnLastPopupClosed -= ReEnableInterstitialShowLocation;
		UIScreenController.Instance.OnPopupShown -= DisableInterstitialShowLocation;
		ScreenAdsManager.instance.EnableShowLocation(InterstitialLocations.GetLocation(LocationID.front_screen_after_tweens), false);
		UIScreenController.Instance.OnPopupClosed -= OnPopupClosed;
		VideoAdsManager.instance.OnVideoCached -= CheckTheVideoButtonState;
		VideoAdsManager.instance.RemoveOnKeyVideoWatchedChangedHandler(CheckTheVideoButtonState);
		LimitedCharacterPopupManager.OnLimitedCharacterExpiredHandler -= CheckLimitedCharacterButtonState;
		LimitedCharacterPopupManager.OnLimitedCharacterStartedHandler -= CheckLimitedCharacterButtonState;
		LimitedBoardPopupManager.OnLimitedBoardExpiredHandler -= CheckLimitedBoardButtonState;
		LimitedBoardPopupManager.OnLimitedBoardStartedHandler -= CheckLimitedBoardButtonState;
	}

	private void ActivateScreenElements()
	{
		topLeft.gameObject.SetActive(true);
		topRight.gameObject.SetActive(true);
		bottomLeft.gameObject.SetActive(true);
		bottomCenter.gameObject.SetActive(true);
		bottomRight.gameObject.SetActive(true);
		foreach (Transform item in topRight.transform)
		{
			item.gameObject.SetActive(true);
		}
		CheckTheEventsButtonState();
		CheckTheVideoButtonState();
	}

	private void TournamentChanged(object sender, TournamentChangedEventArgs eventArg)
	{
		TopRunRewardHelper.RefreshPendingResults();
	}

	private static bool ShouldDisplayChangeLog()
	{
		bool result = false;
		string value = FormatVersionNumber(DeviceUtility.GetBundleVersion());
		string text = FormatVersionNumber(PlayerPrefs.GetString("lastSeenBundleVersionKey", string.Empty));
		if (!text.Equals(value) && !(text == string.Empty))
		{
			result = true;
			PlayerInfo.Instance.isFreshInstall = false;
		}
		if (!text.Equals(value))
		{
			PlayerPrefs.SetString("lastSeenBundleVersionKey", DeviceUtility.GetBundleVersion());
		}
		return result;
	}

	private static string FormatVersionNumber(string oldVersionNumber)
	{
		string result = oldVersionNumber;
		string[] array = oldVersionNumber.Split('.');
		if (array.Length >= 2)
		{
			result = string.Format("{0}.{1}", array[0], array[1]);
		}
		return result;
	}

	private void HookUpToEvents()
	{
		SSEventsManagerController.OnSSEventChangedHandler += CheckTheEventsButtonState;
		SSEventsManagerController.OnSSEventChangedHandler += SSEventsPopup.ShowIfNeeded;
		UIScreenController.Instance.OnLastPopupClosed += ReEnableInterstitialShowLocation;
		UIScreenController.Instance.OnPopupShown += DisableInterstitialShowLocation;
	}

	private void OnPopupClosed(string popupName)
	{
		switch (popupName)
		{
		case "WelcomePopup":
			EnableScreenElements();
			break;
		case "LimitedCharacterPopup":
		case "LimitedBoardPopup":
		case "DebugSettingsPopup":
			CheckLimitedCharacterButtonState();
			CheckLimitedBoardButtonState(0.5f);
			break;
		}
	}

	private void EnableScreenElements()
	{
		HookUpToEvents();
		UpdateHuntLabel();
		CheckTheVideoButtonState();
		CheckLimitedCharacterButtonState();
		CheckLimitedBoardButtonState();
		TweenAndHandleUI();
	}

	private void DisableInterstitialShowLocation()
	{
		switch (UIScreenController.Instance.GetTopScreenName())
		{
		case "FrontUI":
			ScreenAdsManager.instance.EnableShowLocation(InterstitialLocations.GetLocation(LocationID.front_screen_after_tweens), false);
			break;
		}
	}

	private void ReEnableInterstitialShowLocation()
	{
		switch (UIScreenController.Instance.GetTopScreenName())
		{
		case "FrontUI":
			ScreenAdsManager.instance.EnableShowLocation(InterstitialLocations.GetLocation(LocationID.front_screen_after_tweens), true);
			break;
		}
	}

	private void UpdateHuntLabel()
	{
		SincronizeData();
		if (_huntAmountLabel != null)
		{
			_huntAmountLabel.enabled = WeeklyHuntManager.IsHuntStillActive();
			_huntAmountLabel.text = _huntCollectedItems + "/" + _huntMaxItems;
		}
		PositionHuntLabel();
	}

	private void SincronizeData()
	{
		_currentWeekHuntPrize = WeeklyHuntManager.GetPrizeDataForPeriod(WeeklyHuntManager.GetActivePeriode());
		_huntMaxItems = _currentWeekHuntPrize.Value.itemsToColect;
		_huntCollectedItems = _currentWeekHuntPrize.Value.itemsCollected;
		if (_huntCollectedItems < 0)
		{
			_huntCollectedItems = 0;
		}
		else if (_huntCollectedItems > _huntMaxItems)
		{
			_huntCollectedItems = _huntMaxItems;
		}
	}

	private void PositionHuntLabel()
	{
		Vector3 localPosition = _huntButtonOutlineTrans.localPosition;
		Vector3 localPosition2 = _huntAmountLabel.transform.localPosition;
		localPosition2.x = localPosition.x;
		localPosition2.y = localPosition.y;
		_huntAmountLabel.transform.localPosition = localPosition2;
	}

	public static bool HaveButtonsTweened()
	{
		return buttonsHaveTweened;
	}

	private void TweenAndHandleUI()
	{
		if (!_hasTriggeredTweens)
		{
			Invoke("TriggerTween", 0.5f);
			_hasTriggeredTweens = true;
		}
		else
		{
			HandlePopups();
			ScreenAdsManager.instance.EnableShowLocation(InterstitialLocations.GetLocation(LocationID.front_screen_after_tweens), true);
		}
		CheckForDiscountButton();
		RewardManager.PayoutNonMysteryBoxRewards();
		CheckTheEventsButtonState();
	}

	private void CheckTheEventsButtonState()
	{
		_isEventActive = false;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (Application.isEditor)
		{
			_isEventActive = activeEventManager != null;
		}
		else
		{
			_isEventActive = (Globals.DEBUG_SHOW_FRONT_SCREEN_IF_NEW_USER ? (activeEventManager != null) : (activeEventManager != null && PlayerInfo.Instance.IsTutorialCompleted));
		}
		_showEventsPopupBtn.SetActive(_isEventActive);
		RepositionQuickVideoButton(_isEventActive);
	}

	private void CheckTheVideoButtonState()
	{
		if (PlayerInfo.Instance.IsTutorialCompleted && VideoAdsManager.instance.IsAnyVideoAvailable())
		{
			UpdateVideoButtonState();
			_videoReadyToShow = true;
			if (buttonsHaveTweened)
			{
				AnimateVideoButton();
			}
		}
		else
		{
			_quickVideoBtnHelper.gameObject.SetActive(false);
			_videoReadyToShow = false;
		}
	}

	private void CheckLimitedCharacterButtonState()
	{
		if (LimitedCharacterPopupManager.Instance.ShouldButtonBeVisible() && buttonsHaveTweened)
		{
			StartCoroutine(AnimateLimitedPopupButton(_limitedCharacterButton, 0f));
		}
		else
		{
			NGUITools.SetActive(_limitedCharacterButton, false);
		}
	}

	private void CheckLimitedBoardButtonState()
	{
		CheckLimitedBoardButtonState(0f);
	}

	private void CheckLimitedBoardButtonState(float activationDelay)
	{
		if (LimitedBoardPopupManager.Instance.ShouldButtonBeVisible() && buttonsHaveTweened)
		{
			CoroutineUtil.StartCoroutine(AnimateLimitedPopupButton(_limitedBoardButton, activationDelay));
		}
		else
		{
			NGUITools.SetActive(_limitedBoardButton, false);
		}
	}

	private void RepositionQuickVideoButton(bool isEventsButtonVisible)
	{
		if (isEventsButtonVisible)
		{
			_quickVideoBtnHelper.gameObject.transform.localPosition = QUICKVIDEO_DEFAULT_POSITION_END;
		}
		else
		{
			_quickVideoBtnHelper.gameObject.transform.localPosition = QUICKVIDEO_NEW_POSITION_END;
		}
	}

	private void UpdateVideoButtonState()
	{
		if (VideoAdsManager.instance.IsVideoForKeysAvailable())
		{
			_quickVideoBtnHelper.SetState(QuickVideoButtonHelper.State.Keys);
		}
		else
		{
			_quickVideoBtnHelper.SetState(QuickVideoButtonHelper.State.Minibox);
		}
	}

	private void AnimateVideoButton()
	{
		GameObject gameObject = _quickVideoBtnHelper.gameObject;
		if (_videoReadyToShow && !gameObject.activeSelf)
		{
			Vector3 zero = Vector3.zero;
			if (_isEventActive)
			{
				gameObject.transform.localPosition = QUICKVIDEO_DEFAULT_POSITION_START;
				zero = QUICKVIDEO_DEFAULT_POSITION_END;
			}
			else
			{
				gameObject.transform.localPosition = QUICKVIDEO_NEW_POSITION_START;
				zero = QUICKVIDEO_NEW_POSITION_END;
			}
			gameObject.SetActive(true);
			SpringPosition springPosition = SpringPosition.Begin(gameObject, zero, 5f);
			springPosition.ignoreTimeScale = true;
		}
	}

	private IEnumerator AnimateLimitedPopupButton(GameObject limitedButton, float delay)
	{
		if (!limitedButton.activeSelf)
		{
			limitedButton.transform.localPosition = LIMITED_CHARACTER_POPUP_POSITION_START;
			NGUITools.SetActive(limitedButton, true);
			yield return new WaitForSeconds(delay);
			SpringPosition sp = SpringPosition.Begin(limitedButton, LIMITED_CHARACTER_POPUP_POSITION_END, 5f);
			sp.ignoreTimeScale = true;
		}
	}

	private IEnumerator DelayTheActivationOfTheStartButton()
	{
		yield return null;
		startButtonCollider.enabled = true;
	}

	private void TriggerTween()
	{
		CheckTheEventsButtonState();
		int num = 0;
		GameObject[] array = gameobjectsToTween;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(true);
			SpringPosition springPosition = SpringPosition.Begin(gameObject, Vector3.zero, 5f);
			springPosition.ignoreTimeScale = true;
			if (num == 0)
			{
				springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(TweensHaveFinishedAnimating));
				num++;
			}
		}
		CheckForDiscountButton();
		StartCoroutine("DelayTheActivationOfTheStartButton");
	}

	private void TweensHaveFinishedAnimating()
	{
		buttonsHaveTweened = true;
		Action action = FrontScreen.tweensHaveFinishedAnimating;
		if (action != null)
		{
			FrontScreen.tweensHaveFinishedAnimating();
		}
		CheckTheVideoButtonState();
		CheckLimitedCharacterButtonState();
		CheckLimitedBoardButtonState();
		HandlePopups();
		if (!UIScreenController.Instance.isShowingPopup)
		{
			ScreenAdsManager.instance.EnableShowLocation(InterstitialLocations.GetLocation(LocationID.front_screen_after_tweens), true);
		}
	}

	private void CheckForDiscountButton()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.BiggerJackpot);
		if ((InAppManager.IsDoubleCoinsDiscountActive || InAppManager.IsAnyCurrencyDiscountActive) && !_guidelineButtonShop.ShouldInfoIconShow() && activeEventManager == null)
		{
			discountSticker.SetActive(true);
			sticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			shopDiscountLabel1.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			shopDiscountLabel2.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			shopDiscountLabelSingleLine.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
		}
		else
		{
			discountSticker.SetActive(false);
		}
	}

	private void _PauseApplication(bool paused)
	{
		if (!paused)
		{
			_isApplicationResuming = true;
			AssetBundleManager.Instance.StartDownloadIfPossible();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		_PauseApplication(paused);
	}

	private void OnApplicationFocus(bool focus)
	{
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
		{
			_PauseApplication(!focus);
		}
	}

	private void LateUpdate()
	{
		if (_isApplicationResuming)
		{
			_isApplicationResuming = false;
			UpdateHuntLabel();
			if (!ScreenAdsManager.instance.isPresentingInterstitial && !VideoAdsManager.instance.isVideoPlaying)
			{
				VideoAdsManager.instance.TryCachingVideosAgain();
				CheckTheVideoButtonState();
			}
			CheckForDiscountButton();
		}
	}

	private void HandlePopups()
	{
		UIScreenController.Instance.RequeueDelayedPopups();
		UpdateApp.ShowIfNeeded();
		SSEventsPopup.ShowIfNeeded();
		FacebookWeeklyGiftPopup.ShowIfNeeded();
	}

	public void EventsButtonClicked()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterName)
		{
			UIScreenController.Instance.QueuePopup("CharacterNameEventPopup");
		}
		else if (activeEventManager != null)
		{
			UIScreenController.Instance.QueuePopup("EventsPopup");
		}
	}

	public void LimitedCharacterButtonClicked()
	{
		UIScreenController.Instance.QueuePopup("LimitedCharacterPopup");
	}

	public void LimitedBoardButtonClicked()
	{
		UIScreenController.Instance.QueuePopup("LimitedBoardPopup");
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

	[Conditional("ENABLE_FRONTSCREEN_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
