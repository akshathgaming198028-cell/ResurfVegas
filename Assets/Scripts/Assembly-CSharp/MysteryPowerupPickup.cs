using System;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

public class MysteryPowerupPickup : MonoBehaviour
{
	private Game game;

	private bool canPickup;

	private void Awake()
	{
		if (Game.IsInstantiated())
		{
			game = Game.Instance;
			TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
			trackObject.OnActivate += OnActivate;
			Pickup component = GetComponent<Pickup>();
			component.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(component.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
		}
	}

	private void OnActivate()
	{
		canPickup = true;
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		if (canPickup)
		{
			CharacterModifier randomModifier = MysteryPowerup.GetRandomModifier();
			game.Modifiers.Add(randomModifier);
			if (randomModifier is SwapCharacter || randomModifier is Hoverboard)
			{
				Character.Instance.mysteryPowerupParticleSystem.Play();
			}
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}

	public static void DoReward()
	{
		CharacterModifier randomModifier = MysteryPowerup.GetRandomModifier();
		Game.Instance.Modifiers.Add(randomModifier);
		if (randomModifier is SwapCharacter || randomModifier is Hoverboard)
		{
			Character.Instance.mysteryPowerupParticleSystem.Play();
		}
	}
}
