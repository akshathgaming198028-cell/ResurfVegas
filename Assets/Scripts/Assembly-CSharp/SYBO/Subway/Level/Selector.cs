using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class Selector : MonoBehaviour
	{
		private bool _initialized;

		protected Transform _transform;

		public bool Initialized
		{
			get
			{
				return _initialized;
			}
		}

		internal event Action<List<GameObject>> PerformingSelection;

		protected virtual void Awake()
		{
			if (!_initialized)
			{
				Initialize();
			}
		}

		protected virtual void Start()
		{
		}

		protected virtual void Initialize()
		{
			_transform = base.transform;
			for (int i = 0; i < _transform.childCount; i++)
			{
				if (_transform.GetChild(i) != null)
				{
					_transform.GetChild(i).gameObject.SetActive(false);
				}
			}
			_initialized = true;
		}

		public virtual void PerformSelection(List<GameObject> objectsToVisit)
		{
			InitializeCheck();
			if (this.PerformingSelection != null)
			{
				this.PerformingSelection(objectsToVisit);
			}
		}

		protected void InitializeCheck()
		{
			if (!_initialized)
			{
				Initialize();
			}
		}
	}
}
