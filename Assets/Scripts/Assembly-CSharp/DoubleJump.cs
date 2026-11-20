using System.Collections;
using SYBO.Subway.Characters;
using UnityEngine;

public class DoubleJump : CharacterModifier
{
	public float ratio;

	public ActivePowerup Powerup;

	public AudioClipInfo powerDownSound;

	public static bool doubleJumpEnabled;

	private static DoubleJump instance;

	private Character character;

	public override bool ShouldPauseInJetpack
	{
		get
		{
			return true;
		}
	}

	public static DoubleJump Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(DoubleJump)) as DoubleJump);
		}
	}

	private void Awake()
	{
		instance = this;
		character = Character.Instance;
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.pickedUpPowerups++;
		base.Paused = false;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.doubleJump);
		float duration = Powerup.timeLeft;
		StartDoubleJump();
		stop = StopSignal.DONT_STOP;
		while (Powerup.timeLeft >= 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = Powerup.timeLeft / duration;
			yield return null;
		}
		StopDoubleJump();
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
		StopDoubleJump();
	}

	public void StartDoubleJump()
	{
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.doubleJump);
		UIScreenController.Instance.messageHelper.ShowMessage(Strings.Get(Upgrades.upgrades[PowerupType.doubleJump].name), true);
		doubleJumpEnabled = true;
		character.IsDoubleJumping = true;
		if (character.characterModel.DoubleJumpers != null)
		{
			character.characterModel.DoubleJumpers.enabled = true;
		}
	}

	public void StopDoubleJump()
	{
		base.Paused = false;
		doubleJumpEnabled = false;
		character.IsDoubleJumping = false;
		if (character.characterModel.DoubleJumpers != null)
		{
			character.characterModel.DoubleJumpers.enabled = false;
		}
	}
}
