using System.Collections.Generic;
using PathologicalGames;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Level.Placeholders.Lifecycles;
using SYBO.Subway.Level.Placeholders.PrefabProviders;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Level.Placeholders
{
	[DisallowMultipleComponent]
	public class Placeholder : MonoBehaviour
	{
		protected Transform _transform;

		private bool _initialized;

		private SpawnPoolManager _poolManager;

		private List<TrackObject> _childTrackObjects = new List<TrackObject>();

		private List<PrefabSpawnInfo> _prefabsToSpawn = new List<PrefabSpawnInfo>();

		private Dictionary<Transform, PrefabSpawnInfo> _spawnedToSpawnInfoMap = new Dictionary<Transform, PrefabSpawnInfo>();

		public bool HasSpawnedInstances
		{
			get
			{
				return _transform != null && _transform.childCount != 0;
			}
		}

		protected virtual void Initialize()
		{
			_transform = base.transform;
			if (Application.isPlaying)
			{
				_poolManager = SingletonMonobehavior<SpawnPoolManager>.Instance;
			}
			_initialized = true;
		}

		protected void InitializeCheck()
		{
			if (!_initialized)
			{
				Initialize();
			}
		}

		private void OnValidate()
		{
			PlaceholderLifecycle component = base.gameObject.GetComponent<PlaceholderLifecycle>();
			if (component == null)
			{
				Debug.LogError("No lifecycle for placeholder " + this.GetLongName());
			}
		}

		public void GetSpawnedObjects(List<GameObject> spawnedObjects)
		{
			InitializeCheck();
			for (int i = 0; i < _transform.childCount; i++)
			{
				spawnedObjects.Add(_transform.GetChild(i).gameObject);
			}
		}

		internal virtual GameObject SpawnInstance(PrefabSpawnInfo spawnInfo)
		{
			if (spawnInfo.PrefabTransform == null)
			{
				Debug.LogError("Placeholder cannot spawn object - no prefab reference specified " + Utils.GetLongName(_transform));
				return null;
			}
			Transform transform = null;
			if (Application.isPlaying)
			{
				SpawnPool spawnPool = _poolManager.GetSpawnPool(spawnInfo.SpawnPoolType);
				transform = spawnPool.Spawn(spawnInfo.PrefabTransform);
				if (transform == null)
				{
					Debug.LogError("Placeholder on [" + base.name + "] failed to get object");
					return null;
				}
				_spawnedToSpawnInfoMap[transform] = spawnInfo;
			}
			else if (Application.isEditor)
			{
				GameObject gameObject = Object.Instantiate(spawnInfo.PrefabTransform.gameObject);
				transform = gameObject.transform;
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
				Transform[] array = componentsInChildren;
				foreach (Transform transform2 in array)
				{
					transform2.gameObject.hideFlags = HideFlags.DontSave;
				}
			}
			InitializeSpawnedInstance(transform, spawnInfo.PrefabTransform.name);
			if (!transform.gameObject.activeSelf)
			{
				Debug.LogWarning("Placeholder spawned inactive object (it's Awake() won't be called): '" + Utils.GetLongName(transform));
			}
			transform.gameObject.SetActive(true);
			GetTrackObjects(transform);
			return transform.gameObject;
		}

		protected virtual void InitializeSpawnedInstance(Transform objectTransform, string newName)
		{
			objectTransform.name = newName;
			Vector3 localScale = objectTransform.localScale;
			objectTransform.parent = _transform;
			objectTransform.localScale = localScale;
			objectTransform.localPosition = Vector3.zero;
			objectTransform.localRotation = Quaternion.identity;
		}

		protected void GetTrackObjects(Transform t, bool addRootTrackObject = true)
		{
			if (addRootTrackObject)
			{
				TrackObject component = t.GetComponent<TrackObject>();
				if (component != null)
				{
					_childTrackObjects.Add(component);
					component.OnDestroy += TrackChunk_OnDestroy;
				}
			}
			for (int i = 0; i < t.childCount; i++)
			{
				Transform child = t.GetChild(i);
				TrackObject component2 = child.GetComponent<TrackObject>();
				if (component2 != null)
				{
					_childTrackObjects.Add(component2);
					component2.OnDestroy += TrackChunk_OnDestroy;
				}
				if (child.GetComponent<Placeholder>() == null)
				{
					GetTrackObjects(child, false);
				}
			}
		}

		internal void SpawnInstances(PlaceholderPrefabProviderBase prefabProvider)
		{
			InitializeCheck();
			if (HasSpawnedInstances)
			{
				if (Application.isEditor)
				{
					Debug.LogWarningFormat(this, "Placeholder already populated at '{0}'", Utils.GetLongName(_transform));
				}
				return;
			}
			List<PrefabSpawnInfo> prefabsToSpawn = prefabProvider.PrefabsToSpawn;
			for (int i = 0; i < prefabsToSpawn.Count; i++)
			{
				GameObject gameObject = SpawnInstance(prefabsToSpawn[i]);
				if (Application.isEditor && !Application.isPlaying)
				{
					PlaceholderLifecycle[] componentsInChildren = gameObject.GetComponentsInChildren<PlaceholderLifecycle>();
					PlaceholderLifecycle[] array = componentsInChildren;
					foreach (PlaceholderLifecycle placeholderLifecycle in array)
					{
					}
				}
			}
		}

		internal void DespawnInstances()
		{
			InitializeCheck();
			if (Application.isPlaying)
			{
				int num = _transform.childCount - 1;
				while (0 <= num)
				{
					Transform child = _transform.GetChild(num);
					PrefabSpawnInfo value;
					if (_spawnedToSpawnInfoMap.TryGetValue(child, out value))
					{
						value = _spawnedToSpawnInfoMap[child];
						SpawnPool spawnPool = _poolManager.GetSpawnPool(value.SpawnPoolType);
						spawnPool.Despawn(child);
						_spawnedToSpawnInfoMap.Remove(child);
					}
					else
					{
						child.parent = null;
						Object.Destroy(child.gameObject);
					}
					num--;
				}
			}
			else if (Application.isEditor)
			{
				while (_transform.childCount != 0)
				{
					Object.DestroyImmediate(_transform.GetChild(0).gameObject);
				}
			}
			_childTrackObjects.Clear();
			_prefabsToSpawn.Clear();
		}

		internal void Activate()
		{
			int num = 0;
			while (num < _childTrackObjects.Count)
			{
				TrackObject trackObject = _childTrackObjects[num];
				if (trackObject != null)
				{
					num++;
					if (trackObject.gameObject.activeInHierarchy)
					{
						trackObject.Activate();
					}
				}
				else
				{
					if (Application.isEditor)
					{
						Debug.LogWarning("Destroyed TrackObject removed from list of " + Utils.GetLongName(_transform));
					}
					_childTrackObjects.RemoveAt(num);
				}
			}
		}

		internal void Deactivate()
		{
			for (int i = 0; i < _childTrackObjects.Count; i++)
			{
				TrackObject trackObject = _childTrackObjects[i];
				if (trackObject.Active)
				{
					trackObject.Deactivate();
				}
			}
		}

		private void TrackChunk_OnDestroy(TrackObject obj)
		{
			_childTrackObjects.Remove(obj);
			obj.OnDestroy -= TrackChunk_OnDestroy;
		}
	}
}
