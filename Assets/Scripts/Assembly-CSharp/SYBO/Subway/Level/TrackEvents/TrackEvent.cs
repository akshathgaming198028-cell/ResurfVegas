using System;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	[RequireComponent(typeof(TrackObject))]
	public abstract class TrackEvent : MonoBehaviour
	{
		[SerializeField]
		[Range(0f, 1f)]
		private float _probability = 1f;

		protected Transform _transform;

		private TrackObject _trackObject;

		protected bool _launched;

		public event Action Launching;

		public event Action Stopped;

		protected virtual void Initialize()
		{
			_transform = base.transform;
			_trackObject = GetComponent<TrackObject>();
			_trackObject.OnActivate += TrackObject_OnActivate;
			_trackObject.OnDeactivate += TrackObject_OnDeactivate;
		}

		private void Start()
		{
			Initialize();
		}

		protected virtual void OnDestroy()
		{
			if ((bool)_trackObject)
			{
				_trackObject.OnActivate -= TrackObject_OnActivate;
				_trackObject.OnDeactivate -= TrackObject_OnDeactivate;
			}
		}

		protected abstract void Launch();

		protected void EvaluateAndLaunchEvent()
		{
			if (_launched)
			{
				Debug.LogWarning("Attempted to trigger an event a second time!");
			}
			else if (!(_probability < UnityEngine.Random.value))
			{
				if (this.Launching != null)
				{
					this.Launching();
				}
				_launched = true;
				Launch();
			}
		}

		protected virtual void TrackObject_OnActivate()
		{
			EvaluateAndLaunchEvent();
		}

		protected virtual void TrackObject_OnDeactivate()
		{
			_launched = false;
			if (this.Stopped != null)
			{
				this.Stopped();
			}
		}
	}
}
