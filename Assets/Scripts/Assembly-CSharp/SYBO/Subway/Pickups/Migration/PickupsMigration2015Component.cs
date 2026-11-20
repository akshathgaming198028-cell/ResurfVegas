using UnityEngine;

namespace SYBO.Subway.Pickups.Migration
{
	public class PickupsMigration2015Component : MonoBehaviour
	{
		protected void Awake()
		{
			if (!PickupsMigration.Migration_IsActivePickupSystemVersion2015)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
