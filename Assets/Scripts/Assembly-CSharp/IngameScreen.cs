using System;
using System.Collections;
using Kiloo.Common;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Managers;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Characters.States;
using TopRun;
using UnityEngine;

public class IngameScreen : UIScreen
{
	private enum PowerUpType
	{
		_notset = 0,
		headstart = 1,
		scorebooster = 2
	}

	private enum SpeedometerState
	{
		inactive = 0,
		active = 1,
		hidden = 2
	}

	private const float HUNT_BG_EXTRA_SCALE = 3f;

	private const float ANIMATE_TO_NEXT_GEAR_SPEED_FACTOR = 1.2f;

	private const float ANIMATE_SPEEDOMETER_PANEL_ALPHA_SPEED_FACTOR = 3f;

	private const float ANIMATE_IN_SPEEDOMETER_GLOW_SPEED_FACTOR = 100f;

	private const float ANIMATE_OUT_SPEEDOMETER_GLOW_SPEED_FACTOR = 1.5f;

	private const float INACTIVE_SPEEDOMETER_ALPHA = 0.35f;

	private const float ACTIVE_SPEEDOMETER_ALPHA = 1f;

	private const float HIDDEN_SPEEDOMETER_ALPHA = 0f;

	private const int GEAR_GLOW_INITIAL_WIDTH = 40;

	private const int GEAR_GLOW_FINAL_WIDTH = 61;

	private const int GEAR_GLOW_INITIAL_HEIGHT = 46;

	private const int GEAR_GLOW_FINAL_HEIGHT = 70;

	private const int SCORE_ADJUST_GEAR_1_AMOUNT = 1000;

	private const int SCORE_ADJUST_GEAR_2_AMOUNT = 400;

	private const int SCORE_ADJUST_GEAR_3_AMOUNT = 250;

	private const float SCOREBOOSTER_TIMEPERIOD_THIRD_TIER = 3f;

	private const int DIGITS_THAT_FIT_WITHOUT_CHANGEING_THE_BOX_SIZE = 6;

	private const int DIST_PR_DIGIT = 11;

	[SerializeField]
	private UILabel scoreLabel;

	private Vector3 _originalScoreLabelPosition;

	public UILabel multiplierLabel;

	public UILabel coinLabel;

	public UISprite scoreBG;

	private int _originalWidthForScoreBG;

	[SerializeField]
	private Transform _multiplier;

	private Vector3 _originalMultiplierPosition;

	public UISprite coinBG;

	public SlideinPowerupHelper slideinPowerupHelper;

	public UILabel countdownStartingLabel;

	public UILabel countdownLabel;

	private float _countdownSeconds;

	private bool _countingDown;

	private Vector3 _cachedCountdownLabelScale = Vector3.zero;

	private bool _isMultiplierLabelUpdated;

	public Color SCOREBOOSTER_ACTIVE_COLOR = new Color(0.7490196f, 0.9137255f, 0.99215686f);

	private Color SCOREBOOSTER_ELECTRICBLUE = new Color(0f, 52f / 85f, 1f);

	private Color SCOREBOOSTER_WHITE = new Color(1f, 1f, 1f);

	public Color MULTIPLIER_LABEL_ORIGINAL_COLOR = new Color(1f, 73f / 85f, 0f);

	[SerializeField]
	private GameObject itemHuntCounter;

	[SerializeField]
	private GameObject pauseButton;

	[SerializeField]
	private GameObject debugPauseButton;

	[SerializeField]
	private UILabel _huntAmountLabel;

	[SerializeField]
	private UISprite _huntLabelBg;

	[SerializeField]
	private Transform _huntIcon;

	[SerializeField]
	private WeeklyHuntIconPositioning _huntBgPositioning;

	[SerializeField]
	private GameObject _turboHeadstartUI;

	[SerializeField]
	private Transform _turboHeadstartIndicator;

	[SerializeField]
	private UISprite[] _turboHeadstartGearBackgrounds = new UISprite[3];

	[SerializeField]
	private UILabel[] _turboHeadstartGearLabels = new UILabel[3];

	[SerializeField]
	private UISprite[] _turboHeadstartGearsGlow = new UISprite[3];

	[SerializeField]
	private UISprite _turboHeadstartBase;

	[SerializeField]
	private UIPanel _turboHeadstartPanel;

	private int _currentHeadstartGear;

	private float _currentHeadstartIndicatorZRotation;

	private readonly int[] _turboHeadstartGearZRotations = new int[4] { 74, 69, 41, 12 };

	[SerializeField]
	private GameObject _scoreBoosterUI;

	[SerializeField]
	private Transform _scoreBoosterIndicator;

	[SerializeField]
	private UISprite[] _scoreBoosterGearBackgrounds = new UISprite[3];

	[SerializeField]
	private UILabel[] _scoreBoosterGearLabels = new UILabel[3];

	[SerializeField]
	private UISprite[] _scoreBoosterGearsGlow = new UISprite[3];

	[SerializeField]
	private UISprite _scoreBoosterBase;

	[SerializeField]
	private UIPanel _scoreBoosterPanel;

	private int _currentScoreBoosterGear;

	private float _currentScoreBoosterIndicatorZRotation;

	private readonly int[] _scoreBoosterGearZRotations = new int[4] { -74, -69, -41, -12 };

	private bool _scoreboosterTimePeriodIsActive;

	private bool _jetpackIsActive;

	private SpeedometerState _currentScoreboosterState = SpeedometerState.hidden;

	private SpeedometerState _currentHeadstartState = SpeedometerState.hidden;

	private WeeklyHuntManager.WeeklyHuntPeriodData? _currentWeekHuntPrize;

	private int _huntCollectedItems;

	private int _huntMaxItems;

	private bool _hasCalculatedHuntLabelOffsets;

	private float _huntLabelFrontOffset;

	private float _huntLabelBackOffset;

	private int score = -1;

	private float lerpFactor;

	private float lerpFactorCoins;

	private bool doLastIteration;

	private int lastKnowDigits;

	private int digits;

	private float mTimeStart;

	private float mTimeDelta;

	private float mActual;

	private bool mTimeStarted;

	private int extraNumberOfChars
	{
		get
		{
			return Mathf.Clamp(digits - 6, 0, 28);
		}
	}

	public override void Init()
	{
		base.Init();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onScoreMultiplierChanged = (Action)Delegate.Combine(instance.onScoreMultiplierChanged, new Action(resetMultiplierLabel));
		GameStats instance2 = GameStats.Instance;
		instance2.OnCoinsChanged = (Action)Delegate.Combine(instance2.OnCoinsChanged, new Action(OnCoinsChanged));
		Game.OnGameStarted = (Action)Delegate.Combine(Game.OnGameStarted, new Action(OnGameStarted));
		Game instance3 = Game.Instance;
		instance3.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(instance3.OnPauseChange, new Game.OnPauseChangeDelegate(OnPauseChange));
		resetMultiplierLabel();
		scoreLabel.text = GameStats.Instance.score.ToString();
		_originalMultiplierPosition = _multiplier.localPosition;
		_originalScoreLabelPosition = scoreLabel.cachedTransform.localPosition;
		_originalWidthForScoreBG = scoreBG.width;
		countdownStartingLabel.text = string.Empty;
		countdownLabel.text = string.Empty;
		_currentScoreBoosterIndicatorZRotation = _scoreBoosterGearZRotations[0];
		_currentHeadstartIndicatorZRotation = _turboHeadstartGearZRotations[0];
	}

	public void OnDestroy()
	{
		if (!UIScreenController.Instance.stoppingFromEditor)
		{
			PlayerInfo instance = PlayerInfo.Instance;
			instance.onScoreMultiplierChanged = (Action)Delegate.Remove(instance.onScoreMultiplierChanged, new Action(resetMultiplierLabel));
			GameStats instance2 = GameStats.Instance;
			instance2.OnCoinsChanged = (Action)Delegate.Remove(instance2.OnCoinsChanged, new Action(OnCoinsChanged));
			Game.OnGameStarted = (Action)Delegate.Remove(Game.OnGameStarted, new Action(OnGameStarted));
		}
	}

	private void OnPauseChange(bool pause)
	{
		if (!pause && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
		{
			if (_currentHeadstartState == SpeedometerState.active)
			{
				SpeedometerPauseHandle(_turboHeadstartPanel, PowerUpType.headstart);
			}
			if (_currentScoreboosterState == SpeedometerState.active)
			{
				SpeedometerPauseHandle(_scoreBoosterPanel, PowerUpType.scorebooster);
			}
		}
	}

	public override void Show()
	{
		base.Show();
		if (Game.Instance == null)
		{
			Debug.LogError("You must be running the wrong scene");
			return;
		}
		if (Game.Instance.IsPaused && UIScreenController.Instance.IsApplicationFocused)
		{
			_countdownSeconds = 3f;
			_countingDown = true;
		}
		if (!_countingDown)
		{
			MissionsManager.Instance.inRun = true;
			if (multiplierLabel.color != MULTIPLIER_LABEL_ORIGINAL_COLOR)
			{
				multiplierLabel.color = MULTIPLIER_LABEL_ORIGINAL_COLOR;
			}
		}
		SocialManager.instance.pausePhotoDownloading = true;
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.pauseRefreshing = true;
		}
		SetPauseButtonVisibility(true);
		debugPauseButton.SetActive(Globals.DEBUG_USE_TIMESCALE_PAUSE_BUTTON);
		if (itemHuntCounter != null)
		{
			itemHuntCounter.SetActive(WeeklyHuntManager.IsHuntStillActive());
		}
		SlideinPowerupHelper.OnHeadstartActivated += OnHeadstartActivated;
		JetpackState.Instance.OnDeactivateTurboHeadstart += OnHeadstartDeactivated;
		SlideinPowerupHelper.OnScoreBoostActivated += OnScoreboosterActivated;
		Game.OnRunEnded = (Action)Delegate.Combine(Game.OnRunEnded, new Action(OnRunEnded));
	}

	public override void Hide()
	{
		base.Hide();
		resetMultiplierLabel();
		_countingDown = false;
		countdownStartingLabel.text = string.Empty;
		countdownLabel.text = string.Empty;
		SocialManager.instance.pausePhotoDownloading = false;
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.pauseRefreshing = false;
		}
		WeeklyHuntManager.ItemCountUpdated -= UpdateHuntLabel;
		SlideinPowerupHelper.OnHeadstartActivated -= OnHeadstartActivated;
		JetpackState.Instance.OnDeactivateTurboHeadstart -= OnHeadstartDeactivated;
		SlideinPowerupHelper.OnScoreBoostActivated -= OnScoreboosterActivated;
		Game.OnRunEnded = (Action)Delegate.Remove(Game.OnRunEnded, new Action(OnRunEnded));
	}

	public void SetPauseButtonVisibility(bool isActive)
	{
		pauseButton.SetActive(isActive);
	}

	public void ToggleDebugPauseTimescale()
	{
		if (SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale == 1f)
		{
			SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = 0f;
		}
		else if (SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale == 0f)
		{
			SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
		}
		else
		{
			SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
		}
	}

	private void resetMultiplierLabel()
	{
		if ((GameStats.Instance.scoreBoosterActivated || GameStats.Instance.characterDayEventActivated || PlusMultiplier.plusMultiplierEnabled) && !PlayerInfo.Instance.doubleScore)
		{
			int num = int.Parse(multiplierLabel.text.Substring(multiplierLabel.text.IndexOf("x") + 1));
			if (num < PlayerInfo.Instance.scoreMultiplier)
			{
				int num2 = ((!(lerpFactor > 1f)) ? num : (num + 1));
				_isMultiplierLabelUpdated = false;
				doLastIteration = false;
				multiplierLabel.text = "x" + num2;
				return;
			}
			multiplierLabel.text = "x" + PlayerInfo.Instance.scoreMultiplier;
			if (doLastIteration)
			{
				_isMultiplierLabelUpdated = true;
			}
			doLastIteration = true;
		}
		else
		{
			multiplierLabel.text = "x" + PlayerInfo.Instance.scoreMultiplier;
		}
	}

	private void FadeFromWhiteToDarkBlue()
	{
		multiplierLabel.color = Color.Lerp(SCOREBOOSTER_WHITE, SCOREBOOSTER_ELECTRICBLUE, lerpFactor);
		if (multiplierLabel.text.Contains(string.Empty + PlayerInfo.Instance.scoreMultiplier) && doLastIteration)
		{
			multiplierLabel.color = Color.Lerp(SCOREBOOSTER_WHITE, SCOREBOOSTER_ACTIVE_COLOR, lerpFactor);
		}
	}

	public void ResetMultiplierLabelColour()
	{
		if (GameStats.Instance.scoreBoosterActivated || GameStats.Instance.characterDayEventActivated)
		{
			multiplierLabel.color = SCOREBOOSTER_ACTIVE_COLOR;
		}
		else if (multiplierLabel.color != MULTIPLIER_LABEL_ORIGINAL_COLOR)
		{
			multiplierLabel.color = MULTIPLIER_LABEL_ORIGINAL_COLOR;
		}
	}

	private void OnHeadstartActivated(int headstartTier)
	{
		_jetpackIsActive = true;
		_currentHeadstartGear = headstartTier;
		ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.active, PowerUpType.headstart, _turboHeadstartPanel);
		AnimateHeadstartIndicator();
		AnimateGearAndGlow(PowerUpType.headstart, _currentHeadstartGear, _turboHeadstartGearsGlow, _turboHeadstartGearBackgrounds, _turboHeadstartGearLabels);
		switch (_currentHeadstartGear)
		{
		case 1:
			GameStats.Instance.IncreaseAmountNeededToAdjustScoreBy(1000);
			break;
		case 2:
			GameStats.Instance.IncreaseAmountNeededToAdjustScoreBy(400);
			break;
		case 3:
			GameStats.Instance.IncreaseAmountNeededToAdjustScoreBy(250);
			break;
		}
		if (_currentScoreboosterState == SpeedometerState.hidden)
		{
			ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.inactive, PowerUpType.scorebooster, _scoreBoosterPanel);
		}
	}

	private void OnHeadstartDeactivated()
	{
		_jetpackIsActive = false;
		if (!_scoreboosterTimePeriodIsActive)
		{
			ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.hidden, PowerUpType.headstart, _turboHeadstartPanel);
			ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.hidden, PowerUpType.scorebooster, _scoreBoosterPanel);
		}
	}

	private void SetScoreboosterTimePeriodIsActive()
	{
		_scoreboosterTimePeriodIsActive = false;
	}

	private void OnScoreboosterActivated(int scoreboosterTier)
	{
		CancelInvoke("SetScoreboosterTimePeriodIsActive");
		_scoreboosterTimePeriodIsActive = true;
		CancelInvoke("DeactivateScoreboosterSpeedometer");
		if (!slideinPowerupHelper.ScoreboosterButtonIsActive() && !slideinPowerupHelper.HeadstartButtonIsActive())
		{
			Invoke("SetScoreboosterTimePeriodIsActive", 3f);
			Invoke("DeactivateScoreboosterSpeedometer", 3f);
		}
		else
		{
			Invoke("SetScoreboosterTimePeriodIsActive", slideinPowerupHelper.DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS);
			Invoke("DeactivateScoreboosterSpeedometer", slideinPowerupHelper.DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS);
		}
		_currentScoreBoosterGear = scoreboosterTier;
		ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.active, PowerUpType.scorebooster, _scoreBoosterPanel);
		AnimateScoreboosterIndicator();
		AnimateGearAndGlow(PowerUpType.scorebooster, _currentScoreBoosterGear, _scoreBoosterGearsGlow, _scoreBoosterGearBackgrounds, _scoreBoosterGearLabels);
		if (_currentHeadstartState == SpeedometerState.hidden)
		{
			ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.inactive, PowerUpType.headstart, _turboHeadstartPanel);
		}
	}

	private void DeactivateScoreboosterSpeedometer()
	{
		if (base.gameObject.activeSelf)
		{
			if (_currentScoreboosterState == SpeedometerState.active && !_jetpackIsActive)
			{
				ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.hidden, PowerUpType.scorebooster, _scoreBoosterPanel);
			}
			if ((_currentHeadstartState == SpeedometerState.active && !_jetpackIsActive) || _currentHeadstartState == SpeedometerState.inactive)
			{
				ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.hidden, PowerUpType.headstart, _turboHeadstartPanel);
			}
		}
		else
		{
			Debug.LogWarning("IngameScreen: Attempted to deactivate speedometer while ingamescreen not active.");
		}
	}

	private void ChangeSpeedometerState(SpeedometerState currentState, SpeedometerState nextState, PowerUpType powerUpType, UIPanel speedometerPanel)
	{
		if (nextState != SpeedometerState.hidden)
		{
			ToggleSpeedoMeterActive(nextState, powerUpType);
		}
		float correspondingAlphaValue = GetCorrespondingAlphaValue(currentState);
		float correspondingAlphaValue2 = GetCorrespondingAlphaValue(nextState);
		CoroutineUtil.StartCoroutine(AnimateSpeedoMeterPanel(powerUpType, nextState, correspondingAlphaValue, correspondingAlphaValue2, speedometerPanel));
	}

	private float GetCorrespondingAlphaValue(SpeedometerState state)
	{
		switch (state)
		{
		case SpeedometerState.hidden:
			return 0f;
		case SpeedometerState.active:
			return 1f;
		case SpeedometerState.inactive:
			return 0.35f;
		default:
			return 0f;
		}
	}

	private IEnumerator AnimateSpeedoMeterPanel(PowerUpType type, SpeedometerState nextState, float startValue, float endValue, UIPanel uiSpeedoMeterPanel)
	{
		yield return CoroutineUtil.StartCoroutine(AnimatePanelAlpha(startValue, endValue, uiSpeedoMeterPanel));
		if (nextState == SpeedometerState.hidden)
		{
			switch (type)
			{
			case PowerUpType.headstart:
				HideTurboHeadstartSpeedoMeter();
				break;
			case PowerUpType.scorebooster:
				HideScoreboosterSpeedoMeter();
				break;
			}
		}
	}

	private void ToggleSpeedoMeterActive(SpeedometerState state, PowerUpType type)
	{
		bool flag = state == SpeedometerState.active;
		switch (type)
		{
		case PowerUpType.headstart:
		{
			_turboHeadstartUI.SetActive(true);
			_turboHeadstartIndicator.gameObject.SetActive(flag);
			_turboHeadstartBase.color = ((!flag) ? GlobalColors.INACTIVE_GEAR_BACKGROUND_COLOR : GlobalColors.ACTIVE_HEADSTART_GEAR_COUNT_COLOR);
			UILabel[] turboHeadstartGearLabels = _turboHeadstartGearLabels;
			foreach (UILabel uILabel2 in turboHeadstartGearLabels)
			{
				uILabel2.enabled = flag;
			}
			_currentHeadstartState = (flag ? SpeedometerState.active : SpeedometerState.inactive);
			break;
		}
		case PowerUpType.scorebooster:
		{
			_scoreBoosterUI.SetActive(true);
			_scoreBoosterIndicator.gameObject.SetActive(flag);
			_scoreBoosterBase.color = ((!flag) ? GlobalColors.INACTIVE_GEAR_BACKGROUND_COLOR : GlobalColors.ACTIVE_SCOREBOOSTER_GEAR_COUNT_COLOR);
			UILabel[] scoreBoosterGearLabels = _scoreBoosterGearLabels;
			foreach (UILabel uILabel in scoreBoosterGearLabels)
			{
				uILabel.enabled = flag;
			}
			_currentScoreboosterState = (flag ? SpeedometerState.active : SpeedometerState.inactive);
			break;
		}
		}
	}

	private void OnRunEnded()
	{
		if (_currentScoreboosterState != SpeedometerState.hidden)
		{
			ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.hidden, PowerUpType.scorebooster, _scoreBoosterPanel);
		}
		if (_currentHeadstartState != SpeedometerState.hidden)
		{
			ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.hidden, PowerUpType.headstart, _turboHeadstartPanel);
		}
	}

	private void SpeedometerPauseHandle(UIPanel uiPanel, PowerUpType powerUpType)
	{
		if (uiPanel != null && uiPanel.alpha > 0f && uiPanel.gameObject.activeSelf)
		{
			switch (powerUpType)
			{
			case PowerUpType.headstart:
				AnimateHeadstartIndicator();
				break;
			case PowerUpType.scorebooster:
				AnimateScoreboosterIndicator();
				break;
			}
		}
	}

	private IEnumerator AnimatePanelAlpha(float startValue, float endValue, UIPanel uiSpeedoMeterPanel)
	{
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * 3f;
			float panelAlpha = Mathf.Lerp(startValue, endValue, animationLerpFactor);
			uiSpeedoMeterPanel.alpha = panelAlpha;
			yield return null;
		}
	}

	private IEnumerator AnimateSpriteColor(UISprite spriteToAnimate, Color32 startValue, Color32 endValue, float speedFactor)
	{
		yield return new WaitForSeconds(0.2f);
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.color = Color.Lerp(startValue, endValue, animationLerpFactor);
			yield return null;
		}
	}

	private IEnumerator AnimateSpriteSize(UISprite spriteToAnimate, int startHeightValue, int endHeightValue, int startWidthValue, int endWidthValue, float speedFactor)
	{
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.width = Mathf.CeilToInt(Mathf.Lerp(startWidthValue, endWidthValue, animationLerpFactor));
			spriteToAnimate.height = Mathf.CeilToInt(Mathf.Lerp(startHeightValue, endHeightValue, animationLerpFactor));
			yield return null;
		}
	}

	private void AnimateGearAndGlow(PowerUpType type, int currentGear, UISprite[] gearGlows, UISprite[] gearBackgrounds, UILabel[] gearLabels)
	{
		gearGlows[currentGear - 1].color = GlobalColors.FINAL_GEAR_GLOW_COLOR;
		ActivateGear(type, gearBackgrounds[currentGear - 1], gearLabels[currentGear - 1]);
		CoroutineUtil.StartCoroutine(AnimateSpriteColor(gearGlows[currentGear - 1], GlobalColors.FINAL_GEAR_GLOW_COLOR, GlobalColors.INITIAL_GEAR_GLOW_COLOR, 1.5f));
		CoroutineUtil.StartCoroutine(AnimateSpriteSize(gearGlows[currentGear - 1], 46, 70, 40, 61, 1.5f));
	}

	private void HideTurboHeadstartSpeedoMeter()
	{
		_turboHeadstartUI.SetActive(false);
		_turboHeadstartPanel.alpha = 0f;
		_currentHeadstartGear = 0;
		_currentHeadstartIndicatorZRotation = _turboHeadstartGearZRotations[0];
		_turboHeadstartIndicator.localRotation = Quaternion.Euler(new Vector3(0f, 0f, _turboHeadstartGearZRotations[0]));
		DeactivateGears(_turboHeadstartGearBackgrounds, _turboHeadstartGearLabels);
		ResetGlow(_turboHeadstartGearsGlow);
		_currentHeadstartState = SpeedometerState.hidden;
		_jetpackIsActive = false;
	}

	private void HideScoreboosterSpeedoMeter()
	{
		_scoreBoosterUI.SetActive(false);
		_scoreBoosterPanel.alpha = 0f;
		_currentScoreBoosterGear = 0;
		_currentScoreBoosterIndicatorZRotation = _scoreBoosterGearZRotations[0];
		_scoreBoosterIndicator.localRotation = Quaternion.Euler(new Vector3(0f, 0f, _scoreBoosterGearZRotations[0]));
		DeactivateGears(_scoreBoosterGearBackgrounds, _scoreBoosterGearLabels);
		ResetGlow(_scoreBoosterGearsGlow);
		_currentScoreboosterState = SpeedometerState.hidden;
	}

	private void ResetGlow(UISprite[] gearGlows)
	{
		foreach (UISprite uISprite in gearGlows)
		{
			uISprite.height = 46;
			uISprite.width = 40;
			uISprite.color = GlobalColors.INITIAL_GEAR_GLOW_COLOR;
		}
	}

	private void ActivateGear(PowerUpType type, UISprite background, UILabel count)
	{
		background.color = Color.white;
		switch (type)
		{
		case PowerUpType.headstart:
			CoroutineUtil.StartCoroutine(AnimateSpriteColor(background, Color.white, GlobalColors.ACTIVE_HEADSTART_GEAR_BACKGROUND_COLOR, 100f));
			count.color = GlobalColors.ACTIVE_HEADSTART_GEAR_COUNT_COLOR;
			break;
		case PowerUpType.scorebooster:
			CoroutineUtil.StartCoroutine(AnimateSpriteColor(background, Color.white, GlobalColors.ACTIVE_SCOREBOOSTER_GEAR_BACKGROUND_COLOR, 100f));
			count.color = GlobalColors.ACTIVE_SCOREBOOSTER_GEAR_COUNT_COLOR;
			break;
		}
		count.effectStyle = UILabel.Effect.None;
	}

	private void DeactivateGears(UISprite[] gearBackgrounds, UILabel[] gearLabels)
	{
		foreach (UISprite uISprite in gearBackgrounds)
		{
			uISprite.color = GlobalColors.INACTIVE_GEAR_BACKGROUND_COLOR;
		}
		foreach (UILabel uILabel in gearLabels)
		{
			uILabel.color = GlobalColors.INACTIVE_GEAR_COUNT_COLOR;
			uILabel.effectStyle = UILabel.Effect.Shadow;
		}
	}

	private void AnimateHeadstartIndicator()
	{
		StopCoroutine("AnimateHeadstartIndicatorGear");
		StartCoroutine("AnimateHeadstartIndicatorGear");
	}

	private IEnumerator AnimateHeadstartIndicatorGear()
	{
		float startIndicatorRotation = _currentHeadstartIndicatorZRotation;
		float endIndicatorRotation = _turboHeadstartGearZRotations[_currentHeadstartGear];
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * 1.2f;
			_currentHeadstartIndicatorZRotation = Mathf.Lerp(startIndicatorRotation, endIndicatorRotation, Mathf.Sin(animationLerpFactor * (float)Math.PI * 0.5f));
			Vector3 rotation = new Vector3(0f, 0f, _currentHeadstartIndicatorZRotation);
			_turboHeadstartIndicator.localRotation = Quaternion.Euler(rotation);
			yield return null;
		}
	}

	private void AnimateScoreboosterIndicator()
	{
		StopCoroutine("AnimateScoreboosterIndicatorGear");
		StartCoroutine("AnimateScoreboosterIndicatorGear");
	}

	private IEnumerator AnimateScoreboosterIndicatorGear()
	{
		float startIndicatorRotation = _currentScoreBoosterIndicatorZRotation;
		float endIndicatorRotation = _scoreBoosterGearZRotations[_currentScoreBoosterGear];
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * 1.2f;
			_currentScoreBoosterIndicatorZRotation = Mathf.Lerp(startIndicatorRotation, endIndicatorRotation, Mathf.Sin(animationLerpFactor * (float)Math.PI * 0.5f));
			Vector3 rotation = new Vector3(0f, 0f, _currentScoreBoosterIndicatorZRotation);
			_scoreBoosterIndicator.localRotation = Quaternion.Euler(rotation);
			yield return null;
		}
	}

	private void Update()
	{
		UpdateMultiplierLable();
		GameStats.Instance.CalculateScore();
		if (score != GameStats.Instance.score)
		{
			SetScoreLabel();
		}
		if (Game.Instance.isReadyForSlideinPowerups && !Game.Instance.track.IsRunningOnTutorialTrack)
		{
			Game.Instance.isReadyForSlideinPowerups = false;
			slideinPowerupHelper.ShowPowerups();
		}
		AnimateCoinsLabelIfDoubleCoins();
	}

	private void AnimateCoinsLabelIfDoubleCoins()
	{
		if (DoubleCoins.mysteryDoubleCoinsEnabled)
		{
			lerpFactorCoins += Time.deltaTime * 2f;
			if (lerpFactorCoins > 1f)
			{
				if (DoubleCoins.numberOfAnimationBlinks < 2)
				{
					DoubleCoins.numberOfAnimationBlinks++;
					lerpFactorCoins = 0f;
				}
				else
				{
					coinLabel.color = GlobalColors.COINS_LIGHT_PURPLE;
				}
			}
			else
			{
				coinLabel.color = Color.Lerp(GlobalColors.COINS_LIGHT_PURPLE, GlobalColors.COINS_DARK_PURPLE, lerpFactorCoins);
			}
		}
		else
		{
			coinLabel.color = GlobalColors.COINS_ORIGINAL_COLOR;
			lerpFactorCoins = 0f;
		}
	}

	private void UpdateMultiplierLable()
	{
		if ((GameStats.Instance.scoreBoosterActivated || GameStats.Instance.characterDayEventActivated || PlusMultiplier.plusMultiplierEnabled) && !_isMultiplierLabelUpdated && !PlayerInfo.Instance.doubleScore)
		{
			if (!PlusMultiplier.plusMultiplierEnabled)
			{
				lerpFactor += Time.deltaTime * 2f;
			}
			else
			{
				lerpFactor += Time.deltaTime * 4f;
			}
			if (lerpFactor > 1f)
			{
				resetMultiplierLabel();
				lerpFactor = 0f;
			}
			else if (!PlusMultiplier.plusMultiplierEnabled)
			{
				FadeFromWhiteToDarkBlue();
			}
			else if (!doLastIteration)
			{
				multiplierLabel.color = Color.Lerp(GlobalColors.MULTIPLIER_DARK_PURPLE, GlobalColors.MULTIPLIER_LIGHT_PURPLE, lerpFactor);
			}
		}
		if (!_countingDown || !UIScreenController.Instance.IsApplicationFocused)
		{
			return;
		}
		float num = UpdateRealTimeDelta();
		num *= 1.75f;
		_countdownSeconds -= num;
		countdownStartingLabel.text = Strings.Get(StringID.INGAME_UI_COUNTDOWN_STARTING);
		countdownLabel.text = Mathf.CeilToInt(_countdownSeconds).ToString();
		if (!countdownLabel.enabled)
		{
			countdownStartingLabel.enabled = true;
			countdownLabel.enabled = true;
		}
		if (_cachedCountdownLabelScale == Vector3.zero)
		{
			_cachedCountdownLabelScale = countdownLabel.cachedTransform.localScale;
		}
		countdownLabel.cachedTransform.localScale = _cachedCountdownLabelScale * ((1f - _countdownSeconds % 1f) * 0.5f + 1f);
		if (_countdownSeconds < 0f)
		{
			_countingDown = false;
			countdownStartingLabel.text = string.Empty;
			countdownLabel.text = string.Empty;
			countdownStartingLabel.enabled = false;
			countdownLabel.enabled = false;
			if (Game.Instance != null)
			{
				Game.Instance.TriggerPause(false);
				Character.Instance.sameLaneTimeStamp = Time.time;
			}
		}
	}

	private void OnCoinsChanged()
	{
		coinLabel.text = GameStats.Instance.coins.ToString();
		ResizeCoinBox();
	}

	private void OnGameStarted()
	{
		lastKnowDigits = 0;
		digits = 0;
		if (!Game.Instance.isReadyForSlideinPowerups)
		{
			slideinPowerupHelper.HidePowerups(true);
		}
		HideTurboHeadstartSpeedoMeter();
		HideScoreboosterSpeedoMeter();
	}

	private void SetScoreLabel()
	{
		score = GameStats.Instance.score;
		digits = Utility.NumberOfDigits(score);
		string text;
		switch (digits)
		{
		case 1:
			text = "00000";
			break;
		case 2:
			text = "0000";
			break;
		case 3:
			text = "000";
			break;
		case 4:
			text = "00";
			break;
		case 5:
			text = "0";
			break;
		default:
			text = string.Empty;
			break;
		}
		scoreLabel.text = text + score;
		if (lastKnowDigits != digits)
		{
			ResizeScoreBox();
			lastKnowDigits = digits;
		}
	}

	private void ResizeScoreBox()
	{
		_multiplier.localPosition = new Vector3(_originalMultiplierPosition.x - (float)(11 * extraNumberOfChars), _originalMultiplierPosition.y, 0f);
		scoreLabel.cachedTransform.localPosition = new Vector3(_originalScoreLabelPosition.x - (float)(11 * extraNumberOfChars), _originalScoreLabelPosition.y, 0f);
		scoreBG.width = _originalWidthForScoreBG + 11 * extraNumberOfChars;
	}

	private void ResizeCoinBox()
	{
		int length = coinLabel.text.Length;
		float num = 64f;
		if (length > 1)
		{
			num += (float)(12 * (length - 1));
		}
		if ((float)coinBG.width != num)
		{
			coinBG.width = Mathf.RoundToInt(num);
		}
	}

	private void ResizeMultiplierBox()
	{
		int length = multiplierLabel.text.Length;
		float num = 50f;
		if (length > 2)
		{
			num += (float)(10 * (length - 2));
		}
		if (_multiplier.transform.localScale.x != num)
		{
			_multiplier.transform.localScale = new Vector3(num, _multiplier.transform.localScale.y, _multiplier.transform.localScale.z);
		}
	}

	private void SincronizeHuntData()
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

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			UpdateHuntLabel();
		}
	}

	public void UpdateHuntLabel()
	{
		SincronizeHuntData();
		if (_huntAmountLabel != null)
		{
			_huntAmountLabel.enabled = WeeklyHuntManager.IsHuntStillActive();
			_huntAmountLabel.text = _huntCollectedItems + "/" + _huntMaxItems;
		}
		if (!_hasCalculatedHuntLabelOffsets)
		{
			_huntBgPositioning.ForceUpdate();
			_huntLabelFrontOffset = _huntLabelBg.transform.localPosition.x - _huntAmountLabel.transform.localPosition.x;
			_huntLabelBackOffset = _huntIcon.localPosition.x - _huntAmountLabel.transform.localPosition.x;
			_hasCalculatedHuntLabelOffsets = true;
		}
		_huntLabelBg.width = Mathf.RoundToInt((float)_huntAmountLabel.width + Mathf.Abs(_huntLabelFrontOffset) + Mathf.Abs(_huntLabelBackOffset) + 3f);
	}

	private void OnEnable()
	{
		mTimeStarted = true;
		mTimeDelta = 0f;
		mTimeStart = Time.realtimeSinceStartup;
		WeeklyHuntManager.ItemCountUpdated += UpdateHuntLabel;
		UpdateHuntLabel();
	}

	private float UpdateRealTimeDelta()
	{
		if (mTimeStarted)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float b = realtimeSinceStartup - mTimeStart;
			mActual += Mathf.Max(0f, b);
			mTimeDelta = 0.001f * Mathf.Round(mActual * 1000f);
			mActual -= mTimeDelta;
			mTimeStart = realtimeSinceStartup;
		}
		else
		{
			mTimeStarted = true;
			mTimeStart = Time.realtimeSinceStartup;
			mTimeDelta = 0f;
		}
		return mTimeDelta;
	}
}
