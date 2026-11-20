using System;
using SYBO.Subway;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

[RequireComponent(typeof(Pickup))]
public class CoinMagnetPickup : MonoBehaviour
{
	private Game game;

	private Pickup pickup;

	private bool canPickup;

	private void Awake()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
			game = Game.Instance;
			PickupDefault component = base.gameObject.GetComponent<PickupDefault>();
			component.ActivatePickup = (PickupDefault.ActivateDelegate)Delegate.Combine(component.ActivatePickup, new PickupDefault.ActivateDelegate(OnActivate));
			pickup = GetComponent<Pickup>();
			Pickup obj = pickup;
			obj.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(obj.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
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
			game.Modifiers.Add(game.Modifiers.CoinMagnet);
			GameStats.Instance.coinMagnetsPickups++;
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}

	public static void DoReward()
	{
		Game.Instance.Modifiers.Add(Game.Instance.Modifiers.CoinMagnet);
		GameStats.Instance.coinMagnetsPickups++;
	}
}
