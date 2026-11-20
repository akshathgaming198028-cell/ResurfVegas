using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using UnityEngine;

namespace SYBO.Core.Managers
{
	public class TimeScaleManager : SingletonMonobehavior<TimeScaleManager>
	{
		private const float TimeScaleDefault = 1f;

		private float _timeScaleInMenuDefault = 1f;

		private float _timeScaleInGameDefault = 1f;

		public float TimeScale
		{
			get
			{
				return Time.timeScale;
			}
			set
			{
				DebugLog("TimeScale from:" + Time.timeScale + " to:" + value);
				Time.timeScale = value;
			}
		}

		public float TimeScaleInMenuDefault
		{
			get
			{
				return _timeScaleInMenuDefault;
			}
			set
			{
				DebugLog("DefaultTimeScaleForMenu from:" + _timeScaleInMenuDefault + " to:" + value);
				_timeScaleInMenuDefault = value;
			}
		}

		public float TimeScaleInGameDefault
		{
			get
			{
				return _timeScaleInGameDefault;
			}
			set
			{
				DebugLog("DefaultTimeScaleForInGame from:" + _timeScaleInGameDefault + " to:" + value);
				_timeScaleInGameDefault = value;
			}
		}

		protected override void SingletonMonobehavior_OnInstantiateCompleted()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		private void TakeKeys()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				TimeScale = 0f;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				TimeScale = Mathf.Clamp01(TimeScale - 0.1f);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				TimeScale += 0.1f;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				TimeScale = 1f;
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				TimeScale = Mathf.Clamp01(TimeScale * 0.9f);
			}
			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				TimeScale = TimeScale * 1f / 0.9f;
			}
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				TimeScale = 20f;
			}
		}

		private static void DebugLog(string message)
		{
		}
	}
}
