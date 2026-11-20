using System;
using UnityEngine;

public class PickupSuperMysteryBox : MonoBehaviour
{
	private bool canPickup;

	private void Awake()
	{
		PickupController component = base.gameObject.GetComponent<PickupController>();
		component.OnActivate = (PickupController.OnActivateDelegate)Delegate.Combine(component.OnActivate, new PickupController.OnActivateDelegate(OnActivate));
		PickupEvent component2 = GetComponent<PickupEvent>();
		component2.OnPickup = (PickupEvent.OnPickupDelegate)Delegate.Combine(component2.OnPickup, new PickupEvent.OnPickupDelegate(OnPickup));
	}

	private void OnActivate()
	{
		canPickup = true;
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		if (canPickup)
		{
			GameStats.Instance.superMysteryBoxPickups++;
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox);
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}

	public static void DoReward()
	{
		GameStats.Instance.superMysteryBoxPickups++;
		RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox);
	}
}
