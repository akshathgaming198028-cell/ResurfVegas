using System.Collections;
using UnityEngine;

public class PlusMultiplier : CharacterModifier
{
	public float ratio;

	public ActivePowerup Powerup;

	public AudioClipInfo powerDownSound;

	public static bool plusMultiplierEnabled;

	private static PlusMultiplier instance;

	public static PlusMultiplier Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(PlusMultiplier)) as PlusMultiplier);
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.pickedUpPowerups++;
		base.Paused = false;
		stop = StopSignal.DONT_STOP;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.plusMultiplier);
		float duration = Powerup.timeLeft;
		StartPlusMultiplier();
		while (Powerup.timeLeft >= 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = Powerup.timeLeft / duration;
			yield return null;
		}
		StopPlusMultiplier();
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
		plusMultiplierEnabled = false;
	}

	public void StartPlusMultiplier()
	{
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.plusMultiplier);
		UIScreenController.Instance.messageHelper.ShowMessage(Strings.Get(Upgrades.upgrades[PowerupType.plusMultiplier].name), true);
		plusMultiplierEnabled = true;
		ChangeTheMultiplier();
	}

	public void StopPlusMultiplier()
	{
		plusMultiplierEnabled = false;
		ResetLabelColour();
		ChangeTheMultiplier();
	}

	private void ResetLabelColour()
	{
		UIScreenController uIScreenController = UIScreenController.Instance;
		IngameScreen ingameScreen = uIScreenController.GetScreenFromCache(uIScreenController.GetTopScreenName()) as IngameScreen;
		if (ingameScreen != null)
		{
			ingameScreen.ResetMultiplierLabelColour();
		}
	}

	private void ChangeTheMultiplier()
	{
		PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
	}
}
