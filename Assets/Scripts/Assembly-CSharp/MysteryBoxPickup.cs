using System;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class MysteryBoxPickup : MonoBehaviour
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
			GameStats.Instance.mysteryBoxPickups++;
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox);
			particles.PickedUpPowerUp();
			GameStats.Instance.AddScoreForPickup(PowerupType.mysterybox);
			canPickup = false;
		}
	}

	public static void DoReward()
	{
		GameStats.Instance.mysteryBoxPickups++;
		RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox);
		GameStats.Instance.AddScoreForPickup(PowerupType.mysterybox);
	}
}
