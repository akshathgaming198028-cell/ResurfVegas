using System.Collections;
using UnityEngine;

public class DoubleCoins : CharacterModifier
{
	public float ratio;

	public ActivePowerup Powerup;

	public AudioClipInfo powerDownSound;

	public static bool mysteryDoubleCoinsEnabled;

	public static int numberOfAnimationBlinks;

	private static DoubleCoins instance;

	public static DoubleCoins Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(DoubleCoins)) as DoubleCoins);
		}
	}

	public void Awake()
	{
		instance = this;
	}

	public override IEnumerator Begin()
	{
		numberOfAnimationBlinks = 0;
		GameStats.Instance.pickedUpPowerups++;
		base.Paused = false;
		stop = StopSignal.DONT_STOP;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.doubleCoinsMysteryPowerup);
		float duration = Powerup.timeLeft;
		StartDoubleCoins();
		while (Powerup.timeLeft >= 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = Powerup.timeLeft / duration;
			yield return null;
		}
		StopDoubleCoins();
		if (Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public override void ExpireSoon()
	{
		Powerup.timeLeft = CharacterModifierCollection.TIME_LEFT_FOR_EXPIRE_SOON;
	}

	public override void Reset()
	{
		StopDoubleCoins();
	}

	public void StartDoubleCoins()
	{
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.doubleCoinsMysteryPowerup);
		UIScreenController.Instance.messageHelper.ShowMessage(Strings.Get(Upgrades.upgrades[PowerupType.doubleCoinsMysteryPowerup].name), true);
		mysteryDoubleCoinsEnabled = true;
	}

	public void StopDoubleCoins()
	{
		mysteryDoubleCoinsEnabled = false;
		numberOfAnimationBlinks = 0;
	}
}
