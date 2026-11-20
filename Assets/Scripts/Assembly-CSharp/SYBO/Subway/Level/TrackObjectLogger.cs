using UnityEngine;

namespace SYBO.Subway.Level
{
	[RequireComponent(typeof(TrackObject))]
	public class TrackObjectLogger : MonoBehaviour
	{
		private TrackObject _trackObject;

		public void Awake()
		{
			_trackObject = base.gameObject.GetComponent<TrackObject>();
			_trackObject.OnActivate += TrackObject_OnActivate;
			_trackObject.OnDeactivate += TrackObject_OnDeactivate;
		}

		private void Log()
		{
			Debug.LogWarning("_trackObject.Active: " + _trackObject.Active + " for " + Utils.GetLongName(base.transform), base.gameObject);
		}

		private void TrackObject_OnActivate()
		{
			Log();
		}

		private void TrackObject_OnDeactivate()
		{
			Log();
		}
	}
}
