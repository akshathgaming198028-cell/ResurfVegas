using System;
using System.Collections.Generic;
using PathologicalGames;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Subway.Managers
{
	[PrefabPath("Prefabs/Managers/SpawnPoolManager")]
	public class SpawnPoolManager : SingletonMonobehavior<SpawnPoolManager>
	{
		[Serializable]
		public class PrefabPoolSettingsGroup
		{
			public SpawnPoolType Pool;

			public List<PrefabPoolSettings> GenericPrefabPoolSettings;
		}

		public const string SPAWN_POOL_MANAGER_PREFAB_PATH = "Assets/Resources/Prefabs/Managers/SpawnPoolManager.prefab";

		public static readonly Vector3 DEFAULT_LOCAL_POSITION = new Vector3(-499.9f, -499.9f, -499.9f);

		public static readonly Vector3 DEFAULT_GROUP_LOCAL_POSITION = SpawnPool.DEFAULT_GROUP_LOCAL_POSITION;

		[SerializeField]
		private List<PrefabPoolSettingsGroup> _genericPrefabPoolSettingsGroups = new List<PrefabPoolSettingsGroup>();

		private ThemeManager _themeManager;

		private IDictionary<string, SpawnPool> _pools;

		private Dictionary<byte, string> _spawnPoolNames = new Dictionary<byte, string>();

		protected override void SingletonMonobehavior_OnInstantiateCompleted()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			base.transform.localPosition = DEFAULT_LOCAL_POSITION;
			_pools = PoolManager.Pools;
			_themeManager = ThemeManager.Instance;
			SpawnPoolType[] array = (SpawnPoolType[])Enum.GetValues(typeof(SpawnPoolType));
			SpawnPoolType[] array2 = array;
			foreach (SpawnPoolType spawnPoolType in array2)
			{
				string text = spawnPoolType.ToString();
				byte key = (byte)spawnPoolType;
				_spawnPoolNames[key] = text;
				SpawnPool value;
				if (!_pools.TryGetValue(text, out value))
				{
					CreateSpawnPool(text);
				}
			}
			CreateGenericPrefabPools();
		}

		public SpawnPool GetSpawnPool(SpawnPoolType spawnPoolType)
		{
			byte key = (byte)spawnPoolType;
			return _pools[_spawnPoolNames[key]];
		}

		public void CreatePrefabPool(GameObject prefab, PrefabPoolSettings prefabPoolSettings)
		{
			SpawnPool spawnPool = GetSpawnPool(prefabPoolSettings.SpawnPoolType);
			PrefabPool prefabPool = new PrefabPool(prefab, prefabPoolSettings);
			spawnPool.CreatePrefabPool(prefabPool);
		}

		public bool HasSpawnPool(SpawnPoolType spawnPoolType)
		{
			byte key = (byte)spawnPoolType;
			return _pools.ContainsKey(_spawnPoolNames[key]);
		}

		private SpawnPool CreateSpawnPool(string poolName)
		{
			GameObject gameObject = new GameObject(poolName + "Pool");
			Transform transform = gameObject.transform;
			transform.parent = base.transform;
			transform.localPosition = Vector3.zero;
			return gameObject.AddComponent<SpawnPool>();
		}

		private void CreateGenericPrefabPools()
		{
			for (int i = 0; i < _genericPrefabPoolSettingsGroups.Count; i++)
			{
				PrefabPoolSettingsGroup prefabPoolSettingsGroup = _genericPrefabPoolSettingsGroups[i];
				for (int j = 0; j < prefabPoolSettingsGroup.GenericPrefabPoolSettings.Count; j++)
				{
					PrefabPoolSettings prefabPoolSettings = prefabPoolSettingsGroup.GenericPrefabPoolSettings[j];
					GameObject themePrefab;
					if (!_themeManager.Theme.TryGetThemePrefabFromGeneric(prefabPoolSettings.gameObject, out themePrefab))
					{
						themePrefab = prefabPoolSettings.gameObject;
					}
					CreatePrefabPool(themePrefab, prefabPoolSettings);
				}
			}
		}
	}
}
