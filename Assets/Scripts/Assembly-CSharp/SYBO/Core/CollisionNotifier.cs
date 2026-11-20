using System;
using UnityEngine;

namespace SYBO.Core
{
	[RequireComponent(typeof(Collider))]
	public class CollisionNotifier : MonoBehaviour
	{
		private bool _initialized;

		private Collider _collider;

		public Collider Collider
		{
			get
			{
				Initialize();
				return _collider;
			}
		}

		public event Action<Collider> TriggerEnter;

		public event Action<Collider> TriggerExit;

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			if (!_initialized)
			{
				_collider = GetComponent<Collider>();
				_initialized = true;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (this.TriggerEnter != null)
			{
				this.TriggerEnter(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.TriggerExit != null)
			{
				this.TriggerExit(other);
			}
		}
	}
}
