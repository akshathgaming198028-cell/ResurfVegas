using System;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class SuperMysteryBoxPickup : MonoBehaviour
{
	private bool canPickup;

	private void Awake()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
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
		if (canPickup)
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox);
			GameStats.Instance.superMysteryBoxPickups++;
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}

	public static void DoReward()
	{
		RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox);
		GameStats.Instance.superMysteryBoxPickups++;
	}
}
