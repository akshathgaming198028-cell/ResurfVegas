using System;
using System.Collections;
using SYBO.Subway;
using SYBO.Subway.Characters.States;
using UnityEngine;

public class SlideinPowerupHelper : MonoBehaviour
{
	private enum SlideinPowerup
	{
		_notset = 0,
		powerup1 = 1,
		powerup2 = 2
	}

	private const float _DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS = 5f;

	private const float ANIMATE_GLOW_SPEED = 5f;

	private const float SHORT_TIME_BETWEEN_ANIMATION = 0.1f;

	private const float LONG_TIME_BETWEEN_ANIMATION = 0.5f;

	private const float PAUSE_BETWEEN_ANIMATIONs = 1.4f;

	private const float DELAY_ANIMATION_AMOUNT_ON_PRESS_POWERUP_BUTTON = 1.5f;

	private const int MAX_AMOUNT_OF_SCOREBOOSTER_GEARS = 3;

	[SerializeField]
	private SlideinPowerupButton powerup1Button;

	[SerializeField]
	private SlideinPowerupButton powerup2Button;

	[SerializeField]
	private UILabel powerup1AmountLabel;

	[SerializeField]
	private UILabel powerup2AmountLabel;

	[SerializeField]
	private UISprite _powerup1ButtonGlow;

	[SerializeField]
	private UISprite _powerup2ButtonGlow;

	private SpringPosition powerup1Spring;

	private SpringPosition powerup2Spring;

	private Vector3 powerup1PositionOff = new Vector3(-100f, 160f, 0f);

	private Vector3 powerup2PositionOff = new Vector3(-100f, 60f, 0f);

	private Vector3 powerup1PositionOn = new Vector3(50f, 160f, 0f);

	private Vector3 powerup2PositionOn = new Vector3(50f, 60f, 0f);

	private static SlideinPowerupHelper instance;

	private bool _hasInited;

	private bool _initialButtonGlowHasStarted;

	private int _currentHeadstartGear;

	private int _currentScoreboosterGear;

	public float DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS
	{
		get
		{
			return 5f;
		}
	}

	public static SlideinPowerupHelper Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(SlideinPowerupHelper)) as SlideinPowerupHelper);
		}
	}

	public static event Action<int> OnHeadstartActivated;

	public static event Action<int> OnScoreBoostActivated;

	private void Start()
	{
		if (!_hasInited)
		{
			InitHelper();
		}
	}

	private void OnEnable()
	{
		_initialButtonGlowHasStarted = false;
		UIScreenController uIScreenController = UIScreenController.Instance;
		uIScreenController.OnChangedScreen = (Action<string>)Delegate.Combine(uIScreenController.OnChangedScreen, new Action<string>(ScreenDidChange));
	}

	private void OnDisable()
	{
		UIScreenController uIScreenController = UIScreenController.Instance;
		uIScreenController.OnChangedScreen = (Action<string>)Delegate.Remove(uIScreenController.OnChangedScreen, new Action<string>(ScreenDidChange));
		ResetHeadstartButtonGlowColor();
		ResetScoreboosterButtonGlowColor();
	}

	private void ScreenDidChange(string screenOrPopupName)
	{
		if (screenOrPopupName.Equals("SaveMePopup"))
		{
			HidePowerups();
		}
	}

	private void InitHelper()
	{
		powerup1Button.InitSlideinButton(this, 1);
		powerup2Button.InitSlideinButton(this, 2);
		powerup1Button.GetComponent<Collider>().enabled = false;
		powerup2Button.GetComponent<Collider>().enabled = false;
		powerup1Button.transform.localPosition = powerup1PositionOff;
		powerup2Button.transform.localPosition = powerup2PositionOff;
		powerup1AmountLabel.text = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.scorebooster).ToString();
		powerup2AmountLabel.text = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.headstart2000).ToString();
		PlayerInfo playerInfo = PlayerInfo.Instance;
		playerInfo.onPowerupAmountChanged = (Action<PowerupType, int>)Delegate.Combine(playerInfo.onPowerupAmountChanged, new Action<PowerupType, int>(OnUpgradeUsed));
		Game game = Game.Instance;
		game.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(game.OnPauseChange, new Game.OnPauseChangeDelegate(OnPauseChange));
		_hasInited = true;
	}

	private void OnPauseChange(bool pause)
	{
		if (!pause)
		{
			ResumeButtonsGlowColor();
		}
	}

	private void OnDestroy()
	{
		PlayerInfo playerInfo = PlayerInfo.Instance;
		playerInfo.onPowerupAmountChanged = (Action<PowerupType, int>)Delegate.Remove(playerInfo.onPowerupAmountChanged, new Action<PowerupType, int>(OnUpgradeUsed));
	}

	private void OnUpgradeUsed(PowerupType powerupType, int newAmount)
	{
		switch (powerupType)
		{
		case PowerupType.scorebooster:
			powerup1AmountLabel.text = newAmount.ToString();
			if (newAmount == 0)
			{
				HidePowerup1();
			}
			break;
		case PowerupType.headstart2000:
			powerup2AmountLabel.text = newAmount.ToString();
			if (newAmount == 0)
			{
				HidePowerup2();
			}
			break;
		}
	}

	public void ShowPowerups()
	{
		if (!_hasInited)
		{
			InitHelper();
		}
		_currentHeadstartGear = 0;
		ResetHeadstartButtonGlowColor();
		_currentScoreboosterGear = 0;
		ResetScoreboosterButtonGlowColor();
		ShowPowerup1();
		ShowPowerup2();
		CancelInvoke("HidePowerups");
		CancelInvoke("HideScoreboosterDelayed");
		JetpackState.Instance.OnHidTurboHeadstartButtons += HideHeadstart;
		Game.OnRunEnded = (Action)Delegate.Combine(Game.OnRunEnded, new Action(OnRunEnded));
		Invoke("HidePowerups", 5f);
	}

	private void HideScoreboosterDelayed()
	{
		HidePowerup1();
	}

	private void HideHeadstart()
	{
		JetpackState.Instance.OnHidTurboHeadstartButtons -= HideHeadstart;
		HidePowerup2();
	}

	private void ResetHeadstartButtonGlowColor()
	{
		StopCoroutine("AnimateButtonBackground");
		_powerup2ButtonGlow.color = GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR;
	}

	private void ResetScoreboosterButtonGlowColor()
	{
		StopCoroutine("AnimateButtonBackground");
		_powerup1ButtonGlow.color = GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR;
	}

	private void OnRunEnded()
	{
		HidePowerups();
	}

	private void ResetButtonGlowInvokes()
	{
		StopCoroutine("DelayResumeButtonsGlow");
		if (_currentScoreboosterGear < 3 || _currentHeadstartGear < 3)
		{
			StartCoroutine("DelayResumeButtonsGlow");
		}
	}

	private IEnumerator DelayResumeButtonsGlow()
	{
		yield return new WaitForSeconds(1.5f);
		ResumeButtonsGlowColor();
	}

	private void ResumeButtonsGlowColor()
	{
		if ((powerup2Button.GetComponent<Collider>().enabled || powerup1Button.GetComponent<Collider>().enabled) && base.gameObject.activeInHierarchy && base.gameObject.activeSelf)
		{
			_powerup2ButtonGlow.color = GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR;
			_powerup1ButtonGlow.color = GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR;
			StartCoroutine("AnimateButtonBackground");
		}
	}

	public void SlideinPowerupClicked(int powerupAsInt)
	{
		if (Game.Instance.IsPaused)
		{
			return;
		}
		if (powerupAsInt == 1 && powerup1Button.GetComponent<Collider>().enabled)
		{
			CancelInvoke("HidePowerups");
			CancelInvoke("HideScoreboosterDelayed");
			if (_currentHeadstartGear > 0)
			{
				Invoke("HideScoreboosterDelayed", 5f);
			}
			else
			{
				Invoke("HidePowerups", 5f);
			}
			ResetScoreboosterButtonGlowColor();
			ResetButtonGlowInvokes();
			int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.scorebooster);
			if (upgradeAmount > 0)
			{
				_currentScoreboosterGear++;
				if (_currentScoreboosterGear <= 3)
				{
					Action<int> onScoreBoostActivated = SlideinPowerupHelper.OnScoreBoostActivated;
					if (onScoreBoostActivated != null)
					{
						onScoreBoostActivated(_currentScoreboosterGear);
					}
					GameStats.Instance.scoreBoosterTier = _currentScoreboosterGear;
					GameStats.Instance.scoreBoosterActivated = true;
					PlayerInfo.Instance.UseUpgrade(PowerupType.scorebooster);
					MissionsManager.Instance.PlayerDidThis(MissionTarget.ScoreBooster);
				}
				if (_currentScoreboosterGear >= 3)
				{
					HidePowerup1();
				}
			}
			else
			{
				HidePowerup1();
			}
		}
		else
		{
			if (powerupAsInt != 2 || !powerup2Button.GetComponent<Collider>().enabled)
			{
				return;
			}
			CancelInvoke("HidePowerups");
			CancelInvoke("HideScoreboosterDelayed");
			Invoke("HideScoreboosterDelayed", 5f);
			ResetHeadstartButtonGlowColor();
			ResetButtonGlowInvokes();
			int upgradeAmount2 = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.headstart2000);
			if (upgradeAmount2 > 0)
			{
				_currentHeadstartGear++;
				if (_currentHeadstartGear <= 3)
				{
					Action<int> onHeadstartActivated = SlideinPowerupHelper.OnHeadstartActivated;
					if (onHeadstartActivated != null)
					{
						onHeadstartActivated(_currentHeadstartGear);
					}
					Game.Instance.Megaheadstart();
					PlayerInfo.Instance.UseUpgrade(PowerupType.headstart2000);
					MissionsManager.Instance.PlayerDidThis(MissionTarget.Headstart);
				}
				if (_currentHeadstartGear >= 3)
				{
					HidePowerup2();
				}
			}
			else
			{
				HidePowerup2();
			}
		}
	}

	public void HidePowerups()
	{
		CancelInvoke("HidePowerups");
		HidePowerups(false);
	}

	public void HidePowerups(bool instant)
	{
		Game.OnRunEnded = (Action)Delegate.Remove(Game.OnRunEnded, new Action(OnRunEnded));
		HidePowerup1(instant);
		HidePowerup2(instant);
	}

	public void ShowPowerup1()
	{
		powerup1Button.ResetPowerupButtonColorToInitial();
		if (PlayerInfo.Instance.GetUpgradeAmount(PowerupType.scorebooster) > 0)
		{
			powerup1Button.transform.localPosition = powerup1PositionOff;
			SpringPosition springPosition = SpringPosition.Begin(powerup1Button.gameObject, powerup1PositionOn, 10f);
			springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(OnPowerupButtonAnimatedIn));
			powerup1Button.GetComponent<Collider>().enabled = true;
		}
	}

	public void ShowPowerup2()
	{
		powerup2Button.ResetPowerupButtonColorToInitial();
		if (PlayerInfo.Instance.GetUpgradeAmount(PowerupType.headstart2000) > 0)
		{
			powerup2Button.transform.localPosition = powerup2PositionOff;
			SpringPosition springPosition = SpringPosition.Begin(powerup2Button.gameObject, powerup2PositionOn, 10f);
			springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(OnPowerupButtonAnimatedIn));
			powerup2Button.GetComponent<Collider>().enabled = true;
		}
	}

	public void HidePowerup1(bool instant = false)
	{
		powerup1Button.GetComponent<Collider>().enabled = false;
		if (!_hasInited)
		{
			InitHelper();
		}
		if (instant)
		{
			powerup1Button.transform.position = powerup1PositionOff;
			ResetScoreboosterButtonGlowColor();
		}
		else
		{
			SpringPosition springPosition = SpringPosition.Begin(powerup1Button.gameObject, powerup1PositionOff, 10f);
			springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(OnPowerup1ButtonAnimatedOut));
		}
		_currentScoreboosterGear = 0;
	}

	public void HidePowerup2(bool instant = false)
	{
		powerup2Button.GetComponent<Collider>().enabled = false;
		if (!_hasInited)
		{
			InitHelper();
		}
		if (instant)
		{
			powerup2Button.transform.position = powerup2PositionOff;
			ResetHeadstartButtonGlowColor();
		}
		else
		{
			SpringPosition springPosition = SpringPosition.Begin(powerup2Button.gameObject, powerup2PositionOff, 10f);
			springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(OnPowerup2ButtonAnimatedOut));
		}
		_currentHeadstartGear = 0;
	}

	private void OnPowerup1ButtonAnimatedOut()
	{
		ResetScoreboosterButtonGlowColor();
	}

	private void OnPowerup2ButtonAnimatedOut()
	{
		ResetHeadstartButtonGlowColor();
	}

	private void OnPowerupButtonAnimatedIn()
	{
		if (_currentHeadstartGear != 0 || _currentScoreboosterGear != 0)
		{
			StopCoroutine("AnimateButtonBackground");
		}
		if (!_initialButtonGlowHasStarted)
		{
			StartCoroutine("AnimateButtonBackground");
			_initialButtonGlowHasStarted = true;
		}
	}

	private IEnumerator AnimateButtonBackground()
	{
		while (true)
		{
			if (ScoreboosterButtonIsActive())
			{
				yield return StartCoroutine(AnimateButtonBackgrounGlow(_powerup1ButtonGlow));
				yield return new WaitForSeconds(0.1f);
				yield return StartCoroutine(AnimateButtonBackgrounGlow(_powerup1ButtonGlow));
			}
			if (ScoreboosterButtonIsActive() && HeadstartButtonIsActive())
			{
				yield return new WaitForSeconds(0.5f);
			}
			if (HeadstartButtonIsActive())
			{
				yield return StartCoroutine(AnimateButtonBackgrounGlow(_powerup2ButtonGlow));
				yield return new WaitForSeconds(0.1f);
				yield return StartCoroutine(AnimateButtonBackgrounGlow(_powerup2ButtonGlow));
			}
			yield return new WaitForSeconds(1.4f);
		}
	}

	public bool ScoreboosterButtonIsActive()
	{
		return powerup1Button.GetComponent<Collider>().enabled;
	}

	public bool HeadstartButtonIsActive()
	{
		return powerup2Button.GetComponent<Collider>().enabled;
	}

	private IEnumerator AnimateButtonBackgrounGlow(UISprite sprite)
	{
		yield return StartCoroutine(AnimateSpriteColor(sprite, GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR, GlobalColors.FINAL_HEADSTART_BUTTON_GLOW_COLOR, 5f));
		StartCoroutine(AnimateSpriteColor(sprite, GlobalColors.FINAL_HEADSTART_BUTTON_GLOW_COLOR, GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR, 5f));
	}

	private IEnumerator AnimateSpriteColor(UISprite spriteToAnimate, Color32 startValue, Color32 endValue, float speedFactor)
	{
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.color = Color.Lerp(startValue, endValue, animationLerpFactor);
			yield return null;
		}
	}
}
