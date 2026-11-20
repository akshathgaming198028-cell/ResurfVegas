using UnityEngine;

namespace SYBO.Subway.Level.Placeholders.PrefabProviders
{
	public class PlaceholderPrefabProvider : PlaceholderPrefabProviderBase
	{
		[SerializeField]
		[Tooltip("Will spawn this prefab when track object activates. If prefab is used in Theme as generic and mapped to theme specific one, then theme specific will be spawned instead")]
		private Transform _prefab;

		[SerializeField]
		private DeviceInfo.PerformanceLevel MinDevicePerformance;

		private bool evaluated;

		public Transform Prefab
		{
			get
			{
				return _prefab;
			}
		}

		internal override int PrefabCount
		{
			get
			{
				return 1;
			}
		}

		internal override void Evaluate()
		{
			InitializeCheck();
			if (evaluated)
			{
				return;
			}
			if (DeviceInfo.Instance.IsDeviceCapablePerformance(MinDevicePerformance))
			{
				PrefabSpawnInfo? prefabSpawnInfo = GetPrefabSpawnInfo(_prefab);
				if (prefabSpawnInfo.HasValue)
				{
					_prefabsToSpawn.Add(prefabSpawnInfo.Value);
				}
			}
			evaluated = true;
		}

		internal override Transform GetPrefab(int prefabIndex)
		{
			return _prefab;
		}
	}
}
