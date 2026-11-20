using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Common
{
	public class MonoBehaviourMessageHelper : MonoBehaviour
	{
		public event Action<bool> applicationPause;

		public event Action<bool> applicationFocus;

		public event Action applicationQuit;

		[UsedImplicitly]
		private void OnApplicationPause(bool paused)
		{
			Action<bool> action = this.applicationPause;
			if (action != null)
			{
				action(paused);
			}
		}

		[UsedImplicitly]
		private void OnApplicationFocus(bool inFocus)
		{
			Action<bool> action = this.applicationFocus;
			if (action != null)
			{
				action(inFocus);
			}
		}

		[UsedImplicitly]
		private void OnApplicationQuit()
		{
			Action action = this.applicationQuit;
			if (action != null)
			{
				action();
			}
		}
	}
}
