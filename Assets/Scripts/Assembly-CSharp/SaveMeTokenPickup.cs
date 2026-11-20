using System;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class SaveMeTokenPickup : MonoBehaviour
{
	private bool canPickup;

	private void Awake()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
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
			PlayerInfo.Instance.AddSaveMeTokenToUnlock();
			GameStats.Instance.saveMeTokenPickup++;
			particles.PickedUpPowerUp();
			GameStats.Instance.AddScoreForPickup(PowerupType.saveMeToken);
			Statistics stats;
			Statistics statistics = (stats = PlayerInfo.Instance.stats);
			Stat stat2;
			Stat stat = (stat2 = Stat.KeysCollected);
			int num = stats[stat2];
			statistics[stat] = num + 1;
			canPickup = false;
		}
	}

	public static void DoReward()
	{
		PlayerInfo.Instance.AddSaveMeTokenToUnlock();
		GameStats.Instance.saveMeTokenPickup++;
		GameStats.Instance.AddScoreForPickup(PowerupType.saveMeToken);
		Statistics stats;
		Statistics statistics = (stats = PlayerInfo.Instance.stats);
		Stat stat2;
		Stat stat = (stat2 = Stat.KeysCollected);
		int num = stats[stat2];
		statistics[stat] = num + 1;
	}
}
