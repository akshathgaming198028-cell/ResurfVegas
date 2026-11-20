using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public abstract class RandomizerTargetWeightedHold : RandomizerTargetWeighted
	{
		private int _currentRandomizedSectionIndex = -1;

		private GameObject _lastSelectedObject;

		private Track _track;

		protected override void Initialize()
		{
			base.Initialize();
			_track = Track.Instance;
		}

		public override void PerformSelection(List<GameObject> objectsToVisit)
		{
			InitializeCheck();
			int num = Mathf.FloorToInt(_transform.position.z / _track.EnvironmentRepeatDistance);
			if (_currentRandomizedSectionIndex != num)
			{
				_currentRandomizedSectionIndex = num;
				int count = objectsToVisit.Count;
				base.PerformSelection(objectsToVisit);
				if (count != objectsToVisit.Count)
				{
					_lastSelectedObject = objectsToVisit[objectsToVisit.Count - 1];
				}
			}
			else if (_lastSelectedObject != null)
			{
				objectsToVisit.Add(_lastSelectedObject);
			}
		}
	}
}
