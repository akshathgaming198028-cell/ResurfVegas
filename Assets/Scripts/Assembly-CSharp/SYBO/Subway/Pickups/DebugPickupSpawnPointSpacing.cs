using SYBO.Core.Components.Singleton;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	public class DebugPickupSpawnPointSpacing : MonoBehaviour
	{
		private bool _isTestingZSpacing;

		private PickupSpawnPoint2 _pickupSpawnPoint;

		private bool _isInitialized;

		private bool _hasAlreadyLoggedOneError;

		protected void Awake()
		{
			if (_isTestingZSpacing)
			{
				Initialize();
			}
			else
			{
				Object.Destroy(this);
			}
		}

		protected void Update()
		{
			if (_isInitialized && _isTestingZSpacing)
			{
				TestZSpacing_OnUpdate();
			}
		}

		internal void Initialize()
		{
			if (!_isInitialized)
			{
				_hasAlreadyLoggedOneError = false;
				_isInitialized = true;
				_pickupSpawnPoint = GetComponent<PickupSpawnPoint2>();
			}
		}

		private void TestZSpacing_OnUpdate()
		{
			if (_pickupSpawnPoint.SpawnPointState != SpawnPointState.Spawned)
			{
				return;
			}
			foreach (PickupSpawnPoint2 pickupSpawnPoint in SingletonMonobehavior<PickupManager2>.Instance.PickupSpawnPointList)
			{
				if (pickupSpawnPoint != _pickupSpawnPoint && pickupSpawnPoint.SpawnPointState == SpawnPointState.Spawned && _pickupSpawnPoint.gameObject.activeInHierarchy && pickupSpawnPoint.gameObject.activeInHierarchy)
				{
					float num = Mathf.Abs(_pickupSpawnPoint.transform.position.z - pickupSpawnPoint.transform.position.z);
					if (num <= 1000f && _pickupSpawnPoint.transform.position.z < pickupSpawnPoint.transform.position.z && !_hasAlreadyLoggedOneError)
					{
						_hasAlreadyLoggedOneError = true;
						Debug.LogError("PickupSpawnPoints are TOO z-close (" + num + ")!");
						Debug.Log("\t1: " + _pickupSpawnPoint.PickupData.PickupType, _pickupSpawnPoint.gameObject);
						Debug.Log("\t2: " + pickupSpawnPoint.PickupData.PickupType, pickupSpawnPoint.gameObject);
					}
				}
			}
		}
	}
}
