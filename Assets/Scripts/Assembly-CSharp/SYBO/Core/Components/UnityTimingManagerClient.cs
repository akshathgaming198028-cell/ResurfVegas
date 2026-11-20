using System;
using SYBO.Core.Managers;
using UnityEngine;

namespace SYBO.Core.Components
{
	public class UnityTimingManagerClient : MonoBehaviour
	{
		public event EventHandler<UnityTimingChangedEventArgs> OnUnityTimingChanged;

		public void Register()
		{
		}

		public void Deregister()
		{
		}

		public void OnDestroy()
		{
		}

		private void RaiseOnUnityTimingChanged(UnityTiming oldUnityTiming, UnityTiming newUnityTiming)
		{
			UnityTimingChangedEventArgs e = new UnityTimingChangedEventArgs(oldUnityTiming, newUnityTiming);
			if (base.gameObject != null)
			{
				if (this.OnUnityTimingChanged != null)
				{
					this.OnUnityTimingChanged(this, e);
				}
				DuplicateMeComponent component = base.gameObject.GetComponent<DuplicateMeComponent>();
				if (component != null)
				{
					component.UnityTimingManagerClient_OnUnityTimingChanged(this, e);
				}
			}
		}

		public void UnityTimingManagerEditor_OnUnityTimingChanged(object sender, UnityTimingChangedEventArgs unityTimingChangedEventArgs)
		{
			RaiseOnUnityTimingChanged(unityTimingChangedEventArgs.OldUnityTiming, unityTimingChangedEventArgs.NewUnityTiming);
		}
	}
}
