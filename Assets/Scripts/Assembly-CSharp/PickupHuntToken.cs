using System;
using UnityEngine;

public class PickupHuntToken : MonoBehaviour
{
	[SerializeField]
	private Collider _collider;

	[SerializeField]
	private MeshRenderer _meshRenderer;

	[SerializeField]
	private Glow _glow;

	private bool canPickup;

	private void Awake()
	{
		PickupController component = base.gameObject.GetComponent<PickupController>();
		component.OnActivate = (PickupController.OnActivateDelegate)Delegate.Combine(component.OnActivate, new PickupController.OnActivateDelegate(OnActivate));
		PickupEvent component2 = GetComponent<PickupEvent>();
		component2.OnPickup = (PickupEvent.OnPickupDelegate)Delegate.Combine(component2.OnPickup, new PickupEvent.OnPickupDelegate(PickupEvent_OnPickup));
	}

	private void SetVisible(bool visible)
	{
		_collider.enabled = visible;
		_meshRenderer.enabled = visible;
		_glow.SetVisible(visible);
	}

	private void DeactivatePickupIfEventHasCompleted()
	{
		if (!WeeklyHuntManager.CanSpawnToken())
		{
			OnDeactivate();
		}
	}

	private void CleanUpAfterUsage()
	{
		canPickup = false;
		WeeklyHuntManager.ItemCountUpdated -= WeeklyHuntManager_OnItemCountUpdated;
	}

	private void OnActivate()
	{
		canPickup = true;
		SetVisible(true);
		WeeklyHuntManager.ItemCountUpdated += WeeklyHuntManager_OnItemCountUpdated;
	}

	private void OnDeactivate()
	{
		SetVisible(false);
		CleanUpAfterUsage();
	}

	private void WeeklyHuntManager_OnItemCountUpdated()
	{
		DeactivatePickupIfEventHasCompleted();
	}

	private void PickupEvent_OnPickup(CharacterPickupParticles particles)
	{
		if (canPickup)
		{
			WeeklyHuntManager.ItemPickedUp();
			particles.PickedUpPowerUp();
			CleanUpAfterUsage();
		}
	}

	public static void DoReward()
	{
		WeeklyHuntManager.ItemPickedUp();
	}
}
