using System;
using System.Collections;
using System.Diagnostics;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Subway;
using SYBO.Subway.Info;
using TopRun;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameOverScreen : RunStartScreen
{
	public enum ActiveTabState
	{
		leaderBoard = 0,
		topRun = 1
	}

	private const uint DEFAULT_COUNTER_EXPIRE_TIME = 72000u;

	private const uint DEFAULT_TOTAL_SHOW_SLOTS = 5u;

	private const uint DEFAULT_ACTIVE_SHOW_SLOT = 1u;

	private const string INTERSTITIAL_GAMEOVER_COUNTER_EXPIRE_TIME = "interstitial_game_over_counter_expire_time";

	private const string INTERSTITIAL_GAMEOVER_TOTAL_SHOW_SLOTS = "interstitial_game_over_total_show_slots";

	private const string INTERSTITIAL_GAMEOVER_ACTIVE_SHOW_SLOT = "interstitial_game_over_active_show_slot";

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel collectedCoinLabel;

	[SerializeField]
	private UISprite doubleCoinSprite;

	private CoinBoxSizer coinBoxSizer;

	private UILabel coinboxLabel;

	private Friend[] _friends;

	private int scoreFrom;

	private int scoreTo;

	private int coinboxFrom;

	private int coinboxTo;

	private int collectedCoinsFrom;

	private int collectedCoinsTo;

	private bool countingUpCoins;

	private ScoreCounterSoundPlayer scoreCounterSoundPlayer;

	private int _counter;

	private int _facebookShowCounter = FacebookRewardsInfo.POPUP_DISPLAY_RUN_LIMIT;

	private DateTime _lastCountUpTimeStamp = DateTime.MinValue;

	[SerializeField]
	private GameObject discountSticker;

	[SerializeField]
	private UISprite discountStickerSprite;

	[SerializeField]
	private UILabel discountLabel;

	[SerializeField]
	private UILabel discountLabelSingleLine;

	[SerializeField]
	private UISprite playButtonFill;

	[SerializeField]
	private UIButtonOverlayOff playButtonOverlayOff;

	[SerializeField]
	private UILabel homeButtonLabel;

	[SerializeField]
	private UILabel boostsButtonLabel;

	[SerializeField]
	private UILabel boostsButtonCoolDealLabel;

	[SerializeField]
	private UILabel playButtonLabel;

	[SerializeField]
	private UILabel topScoreLabel;

	[SerializeField]
	private GameObject iconsParrent;

	[SerializeField]
	private GameOverLeaderboardHandler leaderboardHandler;

	[SerializeField]
	private GameObject _leaderboardContent;

	[SerializeField]
	private GameObject _topRunContent;

	[SerializeField]
	private TopRunOnlineGameOverScreen _topRunContentScript;

	[SerializeField]
	private BreadCrumbHelper _breadcrumb;

	[SerializeField]
	private GameObject _eventRibbon;

	[SerializeField]
	private UILabel[] _ribbonLabels;

	[SerializeField]
	private GameObject _characterDayParticleSystem;

	private bool _leaderboardTabActive = true;

	private Color darkBlue = new Color(0.043137256f, 14f / 85f, 0.3254902f);

	private Color yellow = new Color(1f, 0.8745098f, 4f / 85f);

	private bool hasBeenSetupAfterAGame;

	private bool cleanUpDone;

	public static Action OnGameOverScreenShown;

	private ActiveTabState _currnetActiveTab;

	public static Action onTabClicked;

	private void Awake()
	{
		homeButtonLabel.text = Strings.Get(StringID.GAMEOVER_UI_HOME);
		boostsButtonLabel.text = Strings.Get(StringID.GAMEOVER_UI_BOOSTS);
		discountLabelSingleLine.text = Strings.Get(StringID.GAMEOVER_UI_COOL_DEAL_SINGLE_LINE);
		playButtonLabel.text = Strings.Get(StringID.GAMEOVER_UI_PLAY);
		topScoreLabel.text = Strings.Get(StringID.GAMEOVER_UI_SCORE);
		UIScreenController.Instance.OnLastPopupClosed += OnLastPopupClosed;
	}

	public override void Init()
	{
		base.Init();
		scoreCounterSoundPlayer = GetComponent<ScoreCounterSoundPlayer>();
		playButtonFill.color = GlobalColors.FOOTER_BIG_PLAY_RESUME_COLOR;
		GameObject gameObject = InitializeCoinbox(null, true, true, true);
		coinBoxSizer = gameObject.GetComponentInChildren<CoinBoxSizer>();
		coinboxLabel = coinBoxSizer.coinAmountLabel;
	}

	public override void Show()
	{
		base.Show();
		UIScreenController.Instance.OnPopupShown += OnPopupShown;
		CharacterScreenManager.OnCharacterSelected += OnCharacterSelected;
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Combine(instance.onCoinsChanged, new Action(OnCoinsChanged));
		MissionsManager instance2 = MissionsManager.Instance;
		instance2.onMissionComplete = (MissionsManager.MissionCompleteHandler)Delegate.Combine(instance2.onMissionComplete, new MissionsManager.MissionCompleteHandler(OnMissionComplete));
		SocialManager.instance.AddFriendsChangedHandler(ReloadFriends);
		InAppManager.instance.onPurchaseSuccess += UpdateUpgradeSticker;
		InAppManager.instance.onPurchaseSuccess += UpdateDoubleCoinLabels;
		SSEventsManagerController.OnSSEventStartedHandler += SetUpEventUIElements;
		SSEventsManagerController.OnSSEventFinishedHandler += SetUpEventUIElements;
		UIScreenController.OnApplicationResumed += SetUpEventUIElements;
		UIModelController.Instance.ActivateGameOverModel();
		UpdateUpgradeSticker(string.Empty);
		Action onGameOverScreenShown = OnGameOverScreenShown;
		if (onGameOverScreenShown != null)
		{
			onGameOverScreenShown();
		}
		if (Globals.DEBUG_PRINT_SPAWNED_POWER_UPS)
		{
			SpawnedObjects.endPowerUpCount();
			SpawnedObjects.printInConsole();
		}
		int score = GameStats.Instance.score;
		int num = score + GameStats.CoinToScoreConversion(GameStats.Instance.coins);
		bool flag = num > PlayerInfo.Instance.highestScore;
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.SubmitScore(PlayerInfo.Instance.rawMultiplier, num, Game.Instance.runStartTime);
		}
		ChangePlayButtonState();
		if (flag)
		{
			PlayerInfo.Instance.highestScore = num;
			PushNewHighScoreCelebrationScreen(num);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BeatOwnHighscore);
		}
		if (SocialManager.instance != null)
		{
			SocialManager.instance.ReportScore(num, flag, Mathf.Max(1, Mathf.RoundToInt(GameStats.Instance.MetersInCurrentGame)));
		}
		_breadcrumb.SetupBreadCrumbState();
		SetUpEventUIElements();
		SSEventsManagerController.instance.RestoreBoardAfterEventIfNeeded();
		HideEventStarsIfNecessary();
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

	private void HideEventStarsIfNecessary()
	{
		if (UIScreenController.Instance.isShowingPopup && _characterDayParticleSystem.activeSelf)
		{
			_characterDayParticleSystem.SetActive(false);
		}
	}

	private void OnLastPopupClosed()
	{
		if (base.gameObject.activeInHierarchy)
		{
			UIModelController.Instance.ActivateGameOverModel();
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterDay);
			if (activeEventManager != null)
			{
				_characterDayParticleSystem.SetActive(ShouldEnableCharacterDayEventElements(activeEventManager));
			}
		}
	}

	private bool ShouldEnableCharacterDayEventElements(IEventManager activeEventManager)
	{
		bool result = false;
		Characters.CharacterType currentCharacter = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		bool found = false;
		if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterDay)
		{
			Characters.CharacterType eventValue = activeEventManager.GetEventValue<Characters.CharacterType>(out found);
			result = found && currentCharacter == eventValue;
		}
		return result;
	}

	private void SetUpEventUIElements()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		bool flag = ShouldEnableCharacterDayEventElements(activeEventManager);
		ActivateEventRibbon(flag, activeEventManager);
		_characterDayParticleSystem.SetActive(flag && !UIScreenController.Instance.isShowingPopup);
	}

	public bool IsCountingUpCoins()
	{
		return countingUpCoins;
	}

	private void ActivateEventRibbon(bool activate, IEventManager activeEventManager)
	{
		_eventRibbon.SetActive(activate);
		if (activate)
		{
			string firstLine = string.Empty;
			string secondLine = string.Empty;
			string thirdLine = string.Empty;
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterDay)
			{
				activeEventManager.GetRibbonDescription(out firstLine, out secondLine, out thirdLine);
			}
			if (!string.IsNullOrEmpty(firstLine) && !string.IsNullOrEmpty(secondLine) && !string.IsNullOrEmpty(thirdLine))
			{
				_ribbonLabels[0].text = firstLine;
				_ribbonLabels[1].text = secondLine;
				_ribbonLabels[2].text = thirdLine;
			}
		}
	}

	private void ChangePlayButtonState()
	{
		iconsParrent.SetActive(true);
		playButtonLabel.text = Strings.Get(StringID.GAMEOVER_UI_PLAY);
		playButtonOverlayOff.GoToNormalState();
		playButtonOverlayOff.ResetTweens();
	}

	public override void Hide()
	{
		base.Hide();
		UIScreenController.Instance.OnPopupShown -= OnPopupShown;
		CharacterScreenManager.OnCharacterSelected -= OnCharacterSelected;
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Remove(instance.onCoinsChanged, new Action(OnCoinsChanged));
		MissionsManager instance2 = MissionsManager.Instance;
		instance2.onMissionComplete = (MissionsManager.MissionCompleteHandler)Delegate.Remove(instance2.onMissionComplete, new MissionsManager.MissionCompleteHandler(OnMissionComplete));
		SocialManager.instance.RemoveFriendsChangedHandler(ReloadFriends);
		InAppManager.instance.onPurchaseSuccess -= UpdateUpgradeSticker;
		InAppManager.instance.onPurchaseSuccess -= UpdateDoubleCoinLabels;
		SSEventsManagerController.OnSSEventFinishedHandler += SetUpEventUIElements;
		SSEventsManagerController.OnSSEventStartedHandler += SetUpEventUIElements;
		UIScreenController.OnApplicationResumed -= SetUpEventUIElements;
		UIModelController.Instance.ClearModels();
		ScreenAdsManager.instance.EnableShowLocation(InterstitialLocations.GetLocation(LocationID.gameover_after_countup), false);
	}

	private void OnCharacterSelected()
	{
		UIModelController.Instance.ActivateGameOverModel();
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && !countingUpCoins)
		{
			ReloadFriends();
		}
	}

	public void SetupBeforeMysteryBox()
	{
		if (Globals.DEBUG_USE_CELEBRATION_TESTER)
		{
			RewardManager.AddRewardToUnlock(CelebrationTesterPopup.GetDebugCelebrationReward());
		}
		hasBeenSetupAfterAGame = true;
		scoreLabel.text = string.Empty + GameStats.Instance.score;
		scoreFrom = GameStats.Instance.score;
	}

	public void SetupAfterMysteryBox()
	{
		if (hasBeenSetupAfterAGame)
		{
			int coins = GameStats.Instance.coins;
			coinBoxSizer.updateAutomatically = false;
			collectedCoinsFrom = 0;
			collectedCoinsTo = coins;
			scoreTo = scoreFrom + GameStats.CoinToScoreConversion(coins);
			coinboxFrom = PlayerInfo.Instance.amountOfCoins;
			coinboxTo = coinboxFrom + coins;
			PlayerInfo.Instance.amountOfCoins = coinboxTo;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.GetExactlyAmountOfCoins, coins);
			StartCoroutine("CountUpCoins");
			hasBeenSetupAfterAGame = false;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterName);
			if (activeEventManager != null)
			{
				CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
				if (typeOfActiveEventManager != null && typeOfActiveEventManager.HasCompletedCollectionForSelectedCharacter())
				{
					UIScreenController.Instance.QueuePopup("CharacterNameEventPopup");
				}
			}
			if (FacebookRewardsInfo.ShouldShowConnectPopupOnGameOver(_facebookShowCounter))
			{
				UIScreenController.Instance.QueuePopup("FacebookRewardsPopup");
				FacebookRewardsInfo.connectPopupLastShownOnGameOver = DateTime.Now;
				_facebookShowCounter = 1;
			}
			else
			{
				_facebookShowCounter++;
			}
			HideEventStarsIfNecessary();
		}
		if (UIScreenController.Instance.justPaidOutTopRun)
		{
			_topRunContentScript.AnimateMedalIfNeeded();
		}
	}

	private void PushNewHighScoreCelebrationScreen(int newHighSchore)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.NewHighScore;
		celebrationReward.rewardType = CelebrationRewardType.highscore;
		celebrationReward.characterType = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		celebrationReward.characterOutfitIndex = PlayerInfo.Instance.currentThemeIndex;
		RewardManager.AddRewardToUnlock(celebrationReward);
	}

	private void UpdateUpgradeSticker(string data = "")
	{
		discountSticker.SetActive(false);
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.BiggerJackpot);
		if (InAppManager.IsDoubleCoinsDiscountActive && activeEventManager != null)
		{
			discountSticker.SetActive(true);
			discountStickerSprite.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			discountLabelSingleLine.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
		}
	}

	private void OnCoinsChanged()
	{
		UpdateUpgradeSticker(string.Empty);
	}

	private void OnMissionComplete(string message)
	{
		UpdateUpgradeSticker(string.Empty);
	}

	private void ReloadFriends()
	{
		bool flag = true;
		StopCoroutine("CorouReloadFriends");
		StartCoroutine("CorouReloadFriends", flag);
	}

	private IEnumerator CorouReloadFriends(bool framedelay)
	{
		if ((UIScreenController.ShouldShowOnlineScreen() || CachedLeaderboard.IsAvailable()) && framedelay)
		{
			float framestamp = Time.frameCount;
			while ((float)Time.frameCount < framestamp + 2f)
			{
				yield return null;
			}
		}
		leaderboardHandler.Init();
		yield return null;
	}

	private void CountUpCompleted()
	{
		PlayerInfo.Instance.ratingAskCounter++;
		if (!PlayerInfo.Instance.neverAskForRating && PlayerInfo.Instance.ratingAskCounter > 0 && PlayerInfo.Instance.ratingAskCounter % 15 == 0 && PlayerInfo.Instance.lastVersionRated != DeviceUtility.GetBundleVersion() && !string.IsNullOrEmpty(Globals.RatingURL))
		{
			UIScreenController.Instance.QueuePopup("RatingPopup");
		}
		ReloadFriends();
		if (!UIScreenController.Instance.isShowingPopup && IsAllowedToShowInterstitial())
		{
			ScreenAdsManager.instance.EnableShowLocation(InterstitialLocations.GetLocation(LocationID.gameover_after_countup), true);
		}
	}

	public bool IsAllowedToShowInterstitial()
	{
		uint value = OnlineSettings.instance.GetValue("interstitial_game_over_counter_expire_time", 72000u);
		DateTime t = _lastCountUpTimeStamp + TimeSpan.FromSeconds(value);
		if (DateTime.Compare(DateTime.Now, t) > 0)
		{
			_counter = 0;
		}
		Flurry.LogEventWithAParameter("Interstitial Game Over Counter", "Value", _counter.ToString());
		uint value2 = OnlineSettings.instance.GetValue("interstitial_game_over_total_show_slots", 5u);
		uint value3 = OnlineSettings.instance.GetValue("interstitial_game_over_active_show_slot", 1u);
		uint num = 1u;
		if (value3 < value2)
		{
			num = value3;
		}
		bool result = _counter % value2 == num;
		_lastCountUpTimeStamp = DateTime.Now;
		_counter++;
		return result;
	}

	private void OnPopupShown()
	{
		HideEventStarsIfNecessary();
		DisableInterstitialShowLocation();
	}

	private void DisableInterstitialShowLocation()
	{
		switch (UIScreenController.Instance.GetTopScreenName())
		{
		case "GameoverUI":
			ScreenAdsManager.instance.EnableShowLocation(InterstitialLocations.GetLocation(LocationID.gameover_after_countup), false);
			break;
		}
	}

	public void FacebookLoggedIn()
	{
		ReloadFriends();
	}

	private void UpdateDoubleCoinLabels(string data = "")
	{
		collectedCoinLabel.color = Color.white;
		doubleCoinSprite.color = darkBlue;
		if (PlayerInfo.Instance.hasDoubleCoins)
		{
			doubleCoinSprite.spriteName = "scoreboard_doublecoins_overlay";
			collectedCoinLabel.color = yellow;
			doubleCoinSprite.color = yellow;
			if (doubleCoinSprite.GetComponent<Collider>() != null)
			{
				UnityEngine.Object.Destroy(doubleCoinSprite.GetComponent<Collider>());
			}
		}
		else
		{
			doubleCoinSprite.spriteName = "scoreboard_doublecoins_overlay_i";
		}
	}

	public void StopCoinsCountUp()
	{
		if (countingUpCoins)
		{
			StopCoroutine("CountUpCoins");
			scoreCounterSoundPlayer.StopScoreSound();
			scoreLabel.text = string.Empty + scoreTo;
			coinboxLabel.text = string.Empty + PlayerInfo.Instance.amountOfCoins;
			collectedCoinLabel.text = string.Empty + collectedCoinsTo;
			coinBoxSizer.updateAutomatically = true;
			countingUpCoins = false;
			CountUpCompleted();
		}
	}

	private IEnumerator CountUpCoins()
	{
		float countFactor = 0f;
		float countTime = Mathf.Lerp(0.3f, 2f, (float)collectedCoinsTo / 100f);
		countingUpCoins = true;
		UpdateDoubleCoinLabels(string.Empty);
		collectedCoinLabel.text = collectedCoinsFrom.ToString();
		yield return new WaitForSeconds(1.2f);
		coinBoxSizer.updateAutomatically = true;
		countFactor = 0f;
		while (countFactor < 1f)
		{
			scoreCounterSoundPlayer.PlayCoinSound(countFactor);
			countFactor += Time.deltaTime / countTime;
			scoreLabel.text = Mathf.RoundToInt(Mathf.SmoothStep(scoreFrom, scoreTo, countFactor)).ToString();
			coinboxLabel.text = Mathf.RoundToInt(Mathf.SmoothStep(coinboxFrom, coinboxTo, countFactor)).ToString();
			coinBoxSizer.AdjustCoinsSegmentSize();
			collectedCoinLabel.text = Mathf.Round(Mathf.SmoothStep(collectedCoinsFrom, collectedCoinsTo, countFactor)).ToString();
			yield return null;
		}
		scoreCounterSoundPlayer.StopScoreSound();
		scoreLabel.text = scoreTo.ToString();
		coinboxLabel.text = coinboxTo.ToString();
		collectedCoinLabel.text = collectedCoinsTo.ToString();
		countingUpCoins = false;
		CountUpCompleted();
	}

	private IEnumerator SplashLabelEffect(UILabel label, int expander)
	{
		label.gameObject.SetActive(true);
		float countFactor = 0f;
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime;
			label.cachedTransform.localScale += new Vector3((float)expander * Time.deltaTime * 34f, (float)(34 * expander) * Time.deltaTime, 0f);
			label.alpha = 0.1f - countFactor * 0.1f;
			yield return null;
		}
	}

	public void LeaderboardTabTapped()
	{
		if (!_leaderboardTabActive)
		{
			NGUITools.SetActive(_leaderboardContent, true);
			leaderboardHandler.Init();
			_leaderboardTabActive = true;
			NGUITools.SetActive(_topRunContent.gameObject, false);
			_currnetActiveTab = ActiveTabState.leaderBoard;
			if (onTabClicked != null)
			{
				onTabClicked();
			}
		}
	}

	public void TopRunTabTapped()
	{
		if (_leaderboardTabActive)
		{
			NGUITools.SetActive(_topRunContent.gameObject, true);
			_leaderboardTabActive = false;
			NGUITools.SetActive(_leaderboardContent, false);
			_currnetActiveTab = ActiveTabState.topRun;
			if (onTabClicked != null)
			{
				onTabClicked();
			}
		}
	}

	public ActiveTabState GetActiveTabState(Action onTabChanged)
	{
		onTabClicked = onTabChanged;
		return _currnetActiveTab;
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

	[Conditional("ENABLE_GAMEOVERSCREEN_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
