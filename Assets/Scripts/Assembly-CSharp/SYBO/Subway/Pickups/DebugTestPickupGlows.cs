using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	public class DebugTestPickupGlows : MonoBehaviour
	{
		[SerializeField]
		private GameObject _parentOfPickupsGO;

		private bool isDebugGlowEnabled;

		protected void Awake()
		{
		}

		public void ToggleGlows()
		{
			isDebugGlowEnabled = !isDebugGlowEnabled;
			List<Glow> list = _parentOfPickupsGO.GetComponentsInChildren<Glow>().ToList();
			foreach (Glow item in list)
			{
				item.IsDebugGlowEnabled = isDebugGlowEnabled;
			}
			Debug.Log("ToggleGlow() isGlowEnabled: " + isDebugGlowEnabled, base.gameObject);
		}
	}
}
