using System.Collections.Generic;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Level.Placeholders.PrefabProviders
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public abstract class PlaceholderPrefabProviderBase : MonoBehaviour
	{
		protected bool _initialized;

		private ThemeManager _themeManager;

		protected List<PrefabSpawnInfo> _prefabsToSpawn = new List<PrefabSpawnInfo>(1);

		internal List<PrefabSpawnInfo> PrefabsToSpawn
		{
			get
			{
				return _prefabsToSpawn;
			}
		}

		internal abstract int PrefabCount { get; }

		private void Awake()
		{
			InitializeCheck();
		}

		protected virtual void InitializeCheck()
		{
			if (!_initialized)
			{
				Initialize();
				_initialized = true;
			}
		}

		protected virtual void Initialize()
		{
			_themeManager = ThemeManager.Instance;
		}

		internal abstract void Evaluate();

		internal abstract Transform GetPrefab(int prefabIndex);

		internal PrefabSpawnInfo? GetPrefabSpawnInfo(Transform prefabTransform)
		{
			InitializeCheck();
			GameObject genericPrefab = prefabTransform.gameObject;
			GameObject themePrefab;
			_themeManager.Theme.TryGetThemePrefabFromGeneric(genericPrefab, out themePrefab);
			PrefabSpawnInfo value = new PrefabSpawnInfo
			{
				PrefabTransform = ((!(themePrefab != null)) ? prefabTransform : themePrefab.transform),
				SpawnPoolType = SpawnPoolType.Default
			};
			PrefabPoolSettings component = prefabTransform.GetComponent<PrefabPoolSettings>();
			if (component != null)
			{
				value.SpawnPoolType = component.SpawnPoolType;
			}
			return value;
		}
	}
}
