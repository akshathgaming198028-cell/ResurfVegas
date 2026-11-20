using System;
using SYBO.Subway;
using UnityEngine;

public class UIPowerupHelper : MonoBehaviour
{
	private enum FadeTarget
	{
		white = 0,
		green = 1,
		darkgreen = 2,
		none = 3
	}

	private const float AMOUNT_LABEL_HIGH_RES_OFFSET = 0.5f;

	private const float COLOR_CHANGE_SPEED = 8.2343f;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UISprite sliderBG;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UISprite iconBG;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite foreground;

	[SerializeField]
	private GameObject container;

	private Color originalColor;

	private float sliderSteps;

	private bool _amountLabelRepositioned;

	private IngameScreen _ingameScreen;

	private bool inverse;

	private ActivePowerup _powerup;

	private Color32[] colorTargets = new Color32[3]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(149, 192, 34, byte.MaxValue),
		new Color32(84, 118, 64, byte.MaxValue)
	};

	private FadeTarget currentFadeTarget = FadeTarget.none;

	private FadeTarget oldFadeTarget = FadeTarget.none;

	private Color storedColor;

	private float lerpTime = 1f;

	private IngameScreen ingameScreen
	{
		get
		{
			if (_ingameScreen == null)
			{
				_ingameScreen = UnityEngine.Object.FindObjectOfType(typeof(IngameScreen)) as IngameScreen;
			}
			return _ingameScreen;
		}
	}

	private void Awake()
	{
		originalColor = ingameScreen.MULTIPLIER_LABEL_ORIGINAL_COLOR;
	}

	private void OnDisable()
	{
		ReturnToNormal();
	}

	private void OnEnable()
	{
		sliderSteps = (int)slider.foregroundWidget.localSize.x;
	}

	private void ReturnToNormal()
	{
		lerpTime = 1f;
		inverse = false;
		if (GameStats.Instance.scoreBoosterActivated || GameStats.Instance.characterDayEventActivated)
		{
			ingameScreen.multiplierLabel.color = ingameScreen.SCOREBOOSTER_ACTIVE_COLOR;
		}
		else if (ingameScreen.multiplierLabel.color != originalColor)
		{
			ingameScreen.multiplierLabel.color = originalColor;
		}
	}

	public void HidePowerupSlot()
	{
		if (_powerup != null && _powerup.type == PowerupType.doubleMultiplier)
		{
			ReturnToNormal();
		}
	}

	public void setContainerPosition(float x, float y, float z)
	{
		container.transform.localPosition = new Vector3(x, y, z);
	}

	public int getSliderWidth()
	{
		return sliderBG.width + iconBG.width - 5;
	}

	public float getSliderHeight()
	{
		return iconBG.height;
	}

	public void SetPowerupSlot(ActivePowerup powerup)
	{
		_powerup = powerup;
		if (_powerup.type == PowerupType.swapCharacter || _powerup.type == PowerupType.plusMultiplier || _powerup.type == PowerupType.doubleJump || _powerup.type == PowerupType.doubleCoinsMysteryPowerup || (_powerup.type == PowerupType.hoverboard && Hoverboard.Instance.IsARewardFromMysteryPowerup))
		{
			iconBG.spriteName = "icon_background_violet";
		}
		else
		{
			iconBG.spriteName = "icon_background_normal";
		}
		if (powerup.type == PowerupType.hoverboard && Hoverboard.Instance.IsARewardFromMysteryPowerup)
		{
			icon.spriteName = MysteryPowerup.GetRandomBoardIconName();
		}
		else
		{
			icon.spriteName = Upgrades.upgrades[powerup.type].iconName;
		}
		icon.MakePixelPerfect();
		float num = ((powerup.type != PowerupType.jetpack) ? (powerup.timeLeft / PlayerInfo.Instance.GetPowerupDuration(powerup.type)) : (powerup.timeLeft / Game.Instance.Jetpack.PowerupTimeLeftMax));
		if (sliderSteps != 0f)
		{
			slider.value = num * sliderSteps / sliderSteps;
		}
		else
		{
			sliderSteps = slider.foregroundWidget.localSize.x;
		}
		if (powerup.type == PowerupType.hoverboard && !Hoverboard.Instance.IsARewardFromMysteryPowerup)
		{
			amountLabel.text = PlayerInfo.Instance.GetUpgradeAmount(powerup.type).ToString();
			amountLabel.gameObject.SetActive(true);
			if (DeviceInfo.Instance.isHighres && !_amountLabelRepositioned)
			{
				amountLabel.transform.localPosition = new Vector3(amountLabel.transform.localPosition.x + 0.5f, amountLabel.transform.localPosition.y, amountLabel.transform.localPosition.z);
				_amountLabelRepositioned = true;
			}
		}
		else
		{
			amountLabel.gameObject.SetActive(false);
		}
		if (powerup.type == PowerupType.doubleMultiplier)
		{
			FadingToWhiteToGreenToDarkGreen(inverse);
		}
		if (powerup.timeLeft < 0f)
		{
			if (slider.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(slider.gameObject, false);
			}
			icon.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * (float)Math.PI * 4f));
			iconBG.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * (float)Math.PI * 4f));
		}
		else
		{
			if (!slider.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(slider.gameObject, true);
			}
			icon.color = Color.white;
			iconBG.color = Color.white;
		}
	}

	private void FadingToWhiteToGreenToDarkGreen(bool sendInverse)
	{
		if (sendInverse)
		{
			if (lerpTime <= 1f)
			{
				currentFadeTarget = FadeTarget.green;
				if (currentFadeTarget != oldFadeTarget)
				{
					storedColor = ingameScreen.multiplierLabel.color;
				}
				ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, colorTargets[(int)currentFadeTarget], lerpTime);
			}
			else if (lerpTime <= 2f)
			{
				currentFadeTarget = FadeTarget.white;
				if (currentFadeTarget != oldFadeTarget)
				{
					storedColor = ingameScreen.multiplierLabel.color;
				}
				ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, colorTargets[(int)currentFadeTarget], lerpTime - 1f);
			}
			else
			{
				currentFadeTarget = FadeTarget.none;
				inverse = false;
				lerpTime = 0f;
			}
		}
		else if (lerpTime <= 1f)
		{
			currentFadeTarget = FadeTarget.green;
			if (currentFadeTarget != oldFadeTarget)
			{
				storedColor = ingameScreen.multiplierLabel.color;
			}
			ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, colorTargets[(int)currentFadeTarget], lerpTime);
		}
		else if (lerpTime <= 2f)
		{
			currentFadeTarget = FadeTarget.darkgreen;
			if (currentFadeTarget != oldFadeTarget)
			{
				storedColor = ingameScreen.multiplierLabel.color;
			}
			ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, colorTargets[(int)currentFadeTarget], lerpTime - 1f);
		}
		else
		{
			currentFadeTarget = FadeTarget.none;
			inverse = true;
			lerpTime = 0f;
		}
		lerpTime += Time.deltaTime * 8.2343f;
		oldFadeTarget = currentFadeTarget;
	}
}
