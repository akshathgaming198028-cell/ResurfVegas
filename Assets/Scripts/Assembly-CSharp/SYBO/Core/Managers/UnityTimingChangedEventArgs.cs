using System;

namespace SYBO.Core.Managers
{
	public class UnityTimingChangedEventArgs : EventArgs
	{
		private UnityTiming _oldUnityTiming;

		private UnityTiming _newUnityTiming;

		public UnityTiming OldUnityTiming
		{
			get
			{
				return _oldUnityTiming;
			}
		}

		public UnityTiming NewUnityTiming
		{
			get
			{
				return _newUnityTiming;
			}
		}

		public UnityTimingChangedEventArgs(UnityTiming oldUnityTiming, UnityTiming newUnityTiming)
		{
			_oldUnityTiming = oldUnityTiming;
			_newUnityTiming = newUnityTiming;
		}
	}
}
