using System;
using SYBO.Subway;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class SuperSneakersPickup : MonoBehaviour
{
	private Game game;

	private bool canPickup;

	private void Awake()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
			game = Game.Instance;
			PickupDefault component = base.gameObject.GetComponent<PickupDefault>();
			component.ActivatePickup = (PickupDefault.ActivateDelegate)Delegate.Combine(component.ActivatePickup, new PickupDefault.ActivateDelegate(OnActivate));
			Pickup component2 = GetComponent<Pickup>();
			component2.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(component2.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
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
		if (canPickup)
		{
			game.Modifiers.Add(game.Modifiers.SuperSneakes);
			GameStats.Instance.superSneakerPickups++;
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}

	public static void DoReward()
	{
		Game.Instance.Modifiers.Add(Game.Instance.Modifiers.SuperSneakes);
		GameStats.Instance.superSneakerPickups++;
	}
}
