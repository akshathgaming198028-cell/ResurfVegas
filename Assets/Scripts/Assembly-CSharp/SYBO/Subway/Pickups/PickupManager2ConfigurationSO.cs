using System;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	[Serializable]
	public class PickupManager2ConfigurationSO : ScriptableObject
	{
		[SerializeField]
		public ActivePickupSystemVersion _activePickupSystemVersion;
	}
}
