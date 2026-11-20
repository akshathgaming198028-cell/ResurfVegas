using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public abstract class RandomizerHold : Selector
	{
		[SerializeField]
		private GameObject[] children;

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
			base.PerformSelection(objectsToVisit);
			int num = Mathf.FloorToInt(_transform.position.z / _track.EnvironmentRepeatDistance);
			if (_currentRandomizedSectionIndex != num)
			{
				_currentRandomizedSectionIndex = num;
				int num2 = Random.Range(0, children.Length);
				_lastSelectedObject = children[num2];
				objectsToVisit.Add(_lastSelectedObject);
			}
			else if (_lastSelectedObject != null)
			{
				objectsToVisit.Add(_lastSelectedObject);
			}
		}
	}
}
