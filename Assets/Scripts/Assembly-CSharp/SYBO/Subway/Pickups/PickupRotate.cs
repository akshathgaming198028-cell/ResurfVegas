using System;
using PathologicalGames;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	public class PickupRotate : MonoBehaviour
	{
		public Transform target;

		public float speed = 180f;

		public float rotatePhase = 0.9f;

		[SerializeField]
		private bool _registerInCoinPool = true;

		private bool _willRotate = true;

		private float z;

		public bool WillRotate
		{
			get
			{
				return _willRotate;
			}
		}

		public void Awake()
		{
			TrackObject component = base.gameObject.GetComponent<TrackObject>();
			if (component != null)
			{
				component.OnActivate += TrackObject_OnActivate;
				component.OnDeactivate += TrackObject_OnDeactivate;
			}
			PickupController component2 = base.gameObject.GetComponent<PickupController>();
			if (component2 != null)
			{
				component2.OnActivate = (PickupController.OnActivateDelegate)Delegate.Combine(component2.OnActivate, new PickupController.OnActivateDelegate(PickupController_OnActivate));
			}
			VisibleObject componentInChildren = GetComponentInChildren<VisibleObject>();
			if (componentInChildren != null)
			{
				componentInChildren.OnVisibleChange = (VisibleObject.OnVisibleChangeDelegate)Delegate.Combine(componentInChildren.OnVisibleChange, new VisibleObject.OnVisibleChangeDelegate(VisibleObject_OnVisibleChange));
			}
			_willRotate = false;
		}

		protected void OnDestroy()
		{
			StopRotating();
		}

		public void PhasedRotate()
		{
			target.localRotation = Quaternion.AngleAxis(Time.time * speed + z * rotatePhase, Vector3.up);
		}

		public void OnSpawned(SpawnPool spawnPool)
		{
		}

		public void OnDespawned(SpawnPool spawnPool)
		{
		}

		private void Activate()
		{
			z = base.transform.position.z;
			if (_registerInCoinPool)
			{
				CoinPool.Instance.ActiveRotatingPickups.Add(this);
			}
			_willRotate = true;
		}

		private void StopRotating()
		{
			_willRotate = false;
			if (CoinPool.IsInstantiated() && _registerInCoinPool)
			{
				CoinPool.Instance.ActiveRotatingPickups.Remove(this);
			}
		}

		private void VisibleObject_OnVisibleChange(bool isVisible)
		{
		}

		private void PickupController_OnActivate()
		{
			Activate();
		}

		private void TrackObject_OnActivate()
		{
			Activate();
		}

		private void TrackObject_OnDeactivate()
		{
			StopRotating();
		}
	}
}
