using System;
using UnityEngine;

public class TrackObject : MonoBehaviour
{
	private bool _initialized;

	private Transform _transform;

	private bool _active;

	private bool _propagateActivationStateToChildren;

	public Transform Transform
	{
		get
		{
			InitializeCheck();
			return _transform;
		}
	}

	public bool Active
	{
		get
		{
			return _active;
		}
	}

	public bool PropagateActivationStateToChildren
	{
		get
		{
			return _propagateActivationStateToChildren;
		}
		set
		{
			if (_propagateActivationStateToChildren != value)
			{
				_propagateActivationStateToChildren = value;
				if (_active && _propagateActivationStateToChildren)
				{
					ActivateChildren();
				}
			}
		}
	}

	public event Action OnActivate;

	public event Action OnDeactivate;

	public event Action<TrackObject> OnDestroy;

	public void Hack_PropagateActivationStateToChildrenImmediately()
	{
		PropagateActivationStateToChildren = false;
		PropagateActivationStateToChildren = true;
	}

	private void Awake()
	{
		InitializeCheck();
	}

	private void InitializeCheck()
	{
		if (!_initialized)
		{
			_transform = base.transform;
			_initialized = true;
		}
	}

	private void Destroy()
	{
		if (this.OnDestroy != null)
		{
			this.OnDestroy(this);
		}
	}

	private void ActivateChildren()
	{
		TrackObject[] componentsInChildren = GetComponentsInChildren<TrackObject>(true);
		foreach (TrackObject trackObject in componentsInChildren)
		{
			if (trackObject != this)
			{
				trackObject.Activate();
			}
		}
	}

	private void DeactivateChildren()
	{
		TrackObject[] componentsInChildren = GetComponentsInChildren<TrackObject>(true);
		foreach (TrackObject trackObject in componentsInChildren)
		{
			if (trackObject != this)
			{
				trackObject.Deactivate();
			}
		}
	}

	public void Activate()
	{
		if (!_active)
		{
			if (this.OnActivate != null)
			{
				this.OnActivate();
			}
			if (_propagateActivationStateToChildren)
			{
				ActivateChildren();
			}
			_active = true;
		}
	}

	public void Deactivate()
	{
		if (_active)
		{
			if (this.OnDeactivate != null)
			{
				this.OnDeactivate();
			}
			if (_propagateActivationStateToChildren)
			{
				DeactivateChildren();
			}
			_active = false;
		}
	}
}
