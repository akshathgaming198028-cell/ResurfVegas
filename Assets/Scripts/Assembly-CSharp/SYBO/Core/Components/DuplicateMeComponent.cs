using SYBO.Core.Managers;
using UnityEngine;

namespace SYBO.Core.Components
{
	[RequireComponent(typeof(UnityTimingManagerClient))]
	public class DuplicateMeComponent : MonoBehaviour
	{
		[SerializeField]
		private int _duplicationCount;

		[SerializeField]
		private bool _willBeDuplicated = true;

		[SerializeField]
		private bool _isDestroyable;

		public int DuplicationCount
		{
			get
			{
				return _duplicationCount;
			}
			set
			{
				_duplicationCount = value;
			}
		}

		public void Awake()
		{
		}

		public void OnEnable()
		{
		}

		public void DoDestroy()
		{
			if (base.gameObject != null)
			{
				Object.DestroyImmediate(base.gameObject);
			}
		}

		private void DuplicateMe()
		{
			_willBeDuplicated = false;
			_isDestroyable = true;
			for (int i = 0; i < _duplicationCount; i++)
			{
			}
			_willBeDuplicated = true;
			_isDestroyable = false;
		}

		public void UnityTimingManagerClient_OnUnityTimingChanged(object sender, UnityTimingChangedEventArgs unityTimingChangedEventArgs)
		{
			if (unityTimingChangedEventArgs.NewUnityTiming == UnityTiming.EditorBeforeChangeToRuntime && _willBeDuplicated)
			{
				DuplicateMe();
			}
			if (unityTimingChangedEventArgs.NewUnityTiming == UnityTiming.EditorAfterRuntimeOnDestroy && _isDestroyable)
			{
				DoDestroy();
			}
		}
	}
}
