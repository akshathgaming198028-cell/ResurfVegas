using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[RequireComponent(typeof(TrackObject))]
	public class TrackObjectVisibleDuringActivation : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> _geometries;

		public void Awake()
		{
			TrackObject component = base.gameObject.GetComponent<TrackObject>();
			if (component != null)
			{
				component.OnActivate += TrackObject_OnActivate;
				component.OnDeactivate += TrackObject_OnDeactivate;
			}
			SetVisible(false);
		}

		private void SetVisible(bool isVisible)
		{
			foreach (GameObject geometry in _geometries)
			{
				geometry.SetActive(isVisible);
			}
		}

		private void TrackObject_OnActivate()
		{
			SetVisible(true);
		}

		private void TrackObject_OnDeactivate()
		{
			SetVisible(false);
		}
	}
}
