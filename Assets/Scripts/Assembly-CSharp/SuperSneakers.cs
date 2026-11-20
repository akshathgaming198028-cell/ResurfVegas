using System;
using System.Collections;
using SYBO.Subway.Characters;
using UnityEngine;

public class SuperSneakers : CharacterModifier
{
	public delegate void OnSwitchToSuperSneakersDelegate();

	public delegate void SuperSneakerOnStopDelegate();

	[HideInInspector]
	public bool isActive;

	private Character character;

	private CharacterRendering characterRendering;

	public AudioClipInfo powerDownSound;

	public ActivePowerup Powerup;

	private LongMagnet longMagnet;

	public float timeActiveInARow;

	public static Action OnSupersneakersTimeActiveChange;

	private static SuperSneakers instance;

	public override bool ShouldPauseInJetpack
	{
		get
		{
			return true;
		}
	}

	public static SuperSneakers Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(SuperSneakers)) as SuperSneakers);
		}
	}

	public event OnSwitchToSuperSneakersDelegate OnSwitchToSuperSneakers;

	public event SuperSneakerOnStopDelegate SuperSneakerOnStop;

	public void Awake()
	{
		instance = this;
		longMagnet = base.gameObject.GetComponent<LongMagnet>();
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
	}

	public override void ExpireSoon()
	{
		Powerup.timeLeft = CharacterModifierCollection.TIME_LEFT_FOR_EXPIRE_SOON;
	}

	public override void Reset()
	{
		base.Paused = false;
	}

	public void StopHack()
	{
		isActive = false;
		characterRendering.CharacterModel.MeshSuperSneaker.enabled = false;
	}

	public override void Prepare()
	{
		GameStats.Instance.pickedUpPowerups++;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.supersneakers);
	}

	public override IEnumerator Begin()
	{
		character.IsJumpingHigher = true;
		base.Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		characterRendering.CharacterModel.MeshSuperSneaker.enabled = true;
		isActive = true;
		longMagnet.Activate();
		if (this.OnSwitchToSuperSneakers != null)
		{
			this.OnSwitchToSuperSneakers();
		}
		stop = StopSignal.DONT_STOP;
		while (Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			if (!base.Paused)
			{
				timeActiveInARow += Time.deltaTime;
				Action handler = OnSupersneakersTimeActiveChange;
				if (handler != null)
				{
					handler();
				}
			}
			yield return null;
		}
		timeActiveInARow = 0f;
		characterRendering.CharacterModel.MeshSuperSneaker.enabled = false;
		isActive = false;
		if (!longMagnet.ShouldBeActive)
		{
			longMagnet.Deactivate();
		}
		if (Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
		character.IsJumpingHigher = false;
		if (this.SuperSneakerOnStop != null)
		{
			this.SuperSneakerOnStop();
		}
	}
}
