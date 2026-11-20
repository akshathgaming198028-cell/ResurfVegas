using UnityEngine;

namespace SYBO.Subway.Level.Placeholders.PrefabProviders
{
	public class EnvironmentPlaceholderPrefabProvider : MultiplePlaceholderPrefabProvider
	{
		protected Transform _transform;

		private EnvironmentType _requiredEnvironmentType;

		private EnvironmentType _lastSelectedEnvironmentType;

		private bool _hasPrefabWithRequiredEnvType;

		private bool _hasPrefabWithPreviousRequiredEnvType;

		protected override void Initialize()
		{
			base.Initialize();
			_transform = base.transform;
		}

		protected override bool NeedRecomputePickingList()
		{
			if (_track.EnvironmentRepeat)
			{
				_requiredEnvironmentType = _track.GetEnvironmentType(_transform.position.z);
				EnvironmentType previousEnvironmentType = _track.GetPreviousEnvironmentType(_transform.position.z);
				bool flag = _requiredEnvironmentType == EnvironmentType.None;
				_hasPrefabWithRequiredEnvType = false;
				_hasPrefabWithPreviousRequiredEnvType = false;
				for (int i = 0; i < _prefabList.Count; i++)
				{
					SpawnSettings component = _prefabList[i].Target.GetComponent<SpawnSettings>();
					_hasPrefabWithRequiredEnvType = _hasPrefabWithRequiredEnvType || component.EnvironmentType == _requiredEnvironmentType;
					_hasPrefabWithPreviousRequiredEnvType = _hasPrefabWithPreviousRequiredEnvType || component.EnvironmentType == previousEnvironmentType;
					if (_hasPrefabWithRequiredEnvType && _hasPrefabWithPreviousRequiredEnvType)
					{
						break;
					}
				}
				bool flag2 = flag && _hasPrefabWithPreviousRequiredEnvType && 2 < _prefabList.Count;
				bool flag3 = _hasPrefabWithRequiredEnvType && 1 < _prefabList.Count;
				bool flag4 = !flag && _prefabPickingList.Count != _prefabList.Count;
				return flag2 || flag3 || flag4;
			}
			return false;
		}

		protected override bool CanPickPrefab(GameObject prefab)
		{
			if (!_track.EnvironmentRepeat)
			{
				return true;
			}
			SpawnSettings component = prefab.GetComponent<SpawnSettings>();
			bool flag = _requiredEnvironmentType == EnvironmentType.None;
			EnvironmentType previousEnvironmentType = _track.GetPreviousEnvironmentType(_transform.position.z);
			if (flag)
			{
				return component != null && previousEnvironmentType != component.EnvironmentType;
			}
			bool flag2 = component != null && _requiredEnvironmentType == component.EnvironmentType;
			return !_hasPrefabWithRequiredEnvType || flag2;
		}

		protected override void RecomputePickingList()
		{
			base.RecomputePickingList();
			if (_prefabPickingList.Count == 0)
			{
				Debug.LogErrorFormat(this, "EnvironmentPlaceholderPrefabProvider computed empty environment picking list. Something broken, repopulate with all prefabs.");
				_prefabPickingList.AddRange(_prefabList);
			}
		}

		protected override GameObject SelectPrefab()
		{
			GameObject gameObject = base.SelectPrefab();
			if (_track.EnvironmentRepeat)
			{
				SpawnSettings component = gameObject.GetComponent<SpawnSettings>();
				_lastSelectedEnvironmentType = component.EnvironmentType;
				if (_requiredEnvironmentType == EnvironmentType.None && Application.isPlaying)
				{
					_track.SetEnvironmentType(_transform.position.z, _lastSelectedEnvironmentType);
				}
			}
			return gameObject;
		}
	}
}
