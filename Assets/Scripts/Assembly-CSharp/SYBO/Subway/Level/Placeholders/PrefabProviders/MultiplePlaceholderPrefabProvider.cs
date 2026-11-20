using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace SYBO.Subway.Level.Placeholders.PrefabProviders
{
	public class MultiplePlaceholderPrefabProvider : PlaceholderPrefabProviderBase
	{
		[Serializable]
		public struct GenericSpawnInfo
		{
			[SerializeField]
			internal GameObject Target;

			[SerializeField]
			internal float Weight;

			[SerializeField]
			internal DeviceInfo.PerformanceLevel MinDevicePerformance;
		}

		private static Dictionary<GameObject, GameObject> _nonRepeatableToLastPrefabDict = new Dictionary<GameObject, GameObject>();

		[Tooltip("Toggle to choose objects based on weight")]
		[SerializeField]
		private bool _spawnWeighted;

		[SerializeField]
		[Tooltip("Toggle to spawn all object.")]
		private bool _spawnAll;

		[SerializeField]
		protected List<GenericSpawnInfo> _prefabList = new List<GenericSpawnInfo>();

		private float _weightSum;

		private GameObject _lastSelectedObject;

		private List<GameObject> _nonRepeatablePrefabs;

		protected Track _track;

		protected List<GenericSpawnInfo> _prefabPickingList = new List<GenericSpawnInfo>();

		public ReadOnlyCollection<GenericSpawnInfo> PrefabList
		{
			get
			{
				return _prefabList.AsReadOnly();
			}
		}

		internal override int PrefabCount
		{
			get
			{
				return _prefabList.Count;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			_track = Track.Instance;
			DeviceInfo.PerformanceLevel performanceLevel = DeviceInfo.Instance.performanceLevel;
			if (Application.isPlaying)
			{
				int num = 0;
				while (num < _prefabList.Count)
				{
					GenericSpawnInfo genericSpawnInfo = _prefabList[num];
					if (performanceLevel < genericSpawnInfo.MinDevicePerformance)
					{
						_prefabList.RemoveAt(num);
						continue;
					}
					num++;
					SpawnSettings component = genericSpawnInfo.Target.GetComponent<SpawnSettings>();
					if (component != null && component.DontRepeatInMultiplePlaceholders)
					{
						if (_nonRepeatablePrefabs == null)
						{
							_nonRepeatablePrefabs = new List<GameObject>();
						}
						_nonRepeatablePrefabs.Add(genericSpawnInfo.Target);
						if (!_nonRepeatableToLastPrefabDict.ContainsKey(genericSpawnInfo.Target))
						{
							_nonRepeatableToLastPrefabDict[genericSpawnInfo.Target] = null;
						}
					}
				}
			}
			for (int i = 0; i < _prefabList.Count; i++)
			{
				_weightSum += _prefabList[i].Weight;
			}
			_prefabPickingList.AddRange(_prefabList);
		}

		internal override void Evaluate()
		{
			InitializeCheck();
			_prefabsToSpawn.Clear();
			RecomputeWeightAndPickingList();
			if (_prefabPickingList.Count == 0)
			{
				return;
			}
			if (_spawnAll)
			{
				for (int i = 0; i < _prefabList.Count; i++)
				{
					PrefabSpawnInfo? prefabSpawnInfo = GetPrefabSpawnInfo(_prefabList[i].Target.transform);
					if (prefabSpawnInfo.HasValue)
					{
						_prefabsToSpawn.Add(prefabSpawnInfo.Value);
					}
				}
				return;
			}
			_lastSelectedObject = SelectPrefab();
			if (_lastSelectedObject != null)
			{
				PrefabSpawnInfo? prefabSpawnInfo2 = GetPrefabSpawnInfo(_lastSelectedObject.transform);
				if (prefabSpawnInfo2.HasValue)
				{
					_prefabsToSpawn.Add(prefabSpawnInfo2.Value);
				}
			}
			if (!(_lastSelectedObject != null) || _nonRepeatablePrefabs == null || _nonRepeatablePrefabs.Count == 0)
			{
				return;
			}
			foreach (GameObject nonRepeatablePrefab in _nonRepeatablePrefabs)
			{
				_nonRepeatableToLastPrefabDict[nonRepeatablePrefab] = _lastSelectedObject;
			}
		}

		internal override Transform GetPrefab(int prefabIndex)
		{
			return _prefabList[prefabIndex].Target.transform;
		}

		private void RecomputeWeightAndPickingList()
		{
			RecomputePickingList();
			RecomupteWeights();
		}

		protected virtual void RecomputePickingList()
		{
			if (!NeedRecomputePickingList())
			{
				return;
			}
			_prefabPickingList.Clear();
			for (int i = 0; i < _prefabList.Count; i++)
			{
				if (CanPickPrefab(_prefabList[i].Target))
				{
					_prefabPickingList.Add(_prefabList[i]);
				}
			}
		}

		protected virtual bool NeedRecomputePickingList()
		{
			return (_nonRepeatablePrefabs != null && _nonRepeatablePrefabs.Count != 0) || Application.isEditor;
		}

		protected virtual bool CanPickPrefab(GameObject prefab)
		{
			return _nonRepeatablePrefabs == null || _nonRepeatablePrefabs.Count == 0 || !IsNonRepeatableSpawned(prefab);
		}

		private bool IsNonRepeatableSpawned(GameObject nonRepeatablePrefab)
		{
			foreach (GameObject nonRepeatablePrefab2 in _nonRepeatablePrefabs)
			{
				if (_nonRepeatableToLastPrefabDict[nonRepeatablePrefab2] == nonRepeatablePrefab)
				{
					return true;
				}
			}
			return false;
		}

		private void RecomupteWeights()
		{
			_weightSum = 0f;
			for (int i = 0; i < _prefabPickingList.Count; i++)
			{
				_weightSum += _prefabPickingList[i].Weight;
			}
		}

		protected virtual GameObject SelectPrefab()
		{
			GameObject gameObject = null;
			if (_spawnWeighted)
			{
				float num = UnityEngine.Random.Range(0f, _weightSum);
				float num2 = 0f;
				for (int i = 0; i < _prefabPickingList.Count; i++)
				{
					GenericSpawnInfo genericSpawnInfo = _prefabPickingList[i];
					num2 += genericSpawnInfo.Weight;
					if (num < num2)
					{
						gameObject = genericSpawnInfo.Target;
						break;
					}
				}
			}
			if (!_spawnWeighted || gameObject == null)
			{
				int index = UnityEngine.Random.Range(0, _prefabPickingList.Count);
				gameObject = _prefabPickingList[index].Target;
				if (_spawnWeighted)
				{
					Debug.LogError("Failed to pick prefab based on weight and resorted on uniform choice. Something wrong at " + Utils.GetLongName(base.transform));
				}
			}
			return gameObject;
		}
	}
}
