using System;
using SYBO.Subway;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class PogoStickPickup : MonoBehaviour
{
	private Game game;

	private bool canPickup;

	private void Awake()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
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
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
			canPickup = true;
		}
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		if (canPickup && !game.IsInJetpackMode)
		{
			game.PickupPogostick();
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}

	public static void DoReward()
	{
		Game.Instance.PickupPogostick();
	}
}
