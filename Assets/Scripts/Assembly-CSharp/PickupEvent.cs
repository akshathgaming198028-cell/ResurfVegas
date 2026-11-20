using UnityEngine;

public class PickupEvent : MonoBehaviour
{
	public delegate void OnPickupDelegate(CharacterPickupParticles pickupParticles);

	public OnPickupDelegate OnPickup;

	public bool CanBeSpawned = true;

	public void NotifyPickup(CharacterPickupParticles pickupParticles)
	{
		if (OnPickup != null)
		{
			OnPickup(pickupParticles);
		}
	}
}
