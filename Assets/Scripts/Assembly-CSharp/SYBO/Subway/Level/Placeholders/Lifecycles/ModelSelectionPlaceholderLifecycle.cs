using System.Collections;
using System.Collections.Generic;
using SYBO.Core;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Core.Visuals;
using SYBO.Subway.Level.Placeholders.PrefabProviders;
using UnityEngine;

namespace SYBO.Subway.Level.Placeholders.Lifecycles
{
	public class ModelSelectionPlaceholderLifecycle : SelectionPlaceholderLifecycle
	{
		[SerializeField]
		private bool _combineModelsInstantly;

		private MeshContainer _meshContainer;

		private MeshFilter _meshFilter;

		private MeshRenderer _meshRenderer;

		private bool _usingCombinedMesh;

		private CombineableMeshGroup _spawnedCombineableMeshGroup;

		private bool _isCombiningMeshes;

		private IEnumerator _meshCombiningRoutine;

		protected override void Initialize()
		{
			base.Initialize();
			_meshContainer = SingletonMonobehavior<MeshContainer>.Instance;
			_meshFilter = (MeshFilter)this.AddComponentIfMissing(typeof(MeshFilter));
			_meshRenderer = (MeshRenderer)this.AddComponentIfMissing(typeof(MeshRenderer));
		}

		protected override void SpawnInstances(PlaceholderPrefabProviderBase prefabProvider, bool spawnInChildrenLifecycles = false, bool spawningOnStart = false, bool locatedInIntro = false)
		{
			List<PrefabSpawnInfo> prefabsToSpawn = prefabProvider.PrefabsToSpawn;
			if (1 < prefabsToSpawn.Count && Application.isEditor)
			{
				Debug.LogError("ModelSelectionPlaceholderLifecycle: Multiple group combining not implemented");
			}
			_spawnedCombineableMeshGroup = prefabsToSpawn[0].PrefabTransform.GetComponent(typeof(CombineableMeshGroup)) as CombineableMeshGroup;
			if (_spawnedCombineableMeshGroup == null)
			{
				if (Application.isEditor)
				{
					Debug.LogError("ModelSelectionPlaceholderLifecycle: spawned object without CombineableMeshGroup");
				}
				base.SpawnInstances(prefabProvider, spawnInChildrenLifecycles, spawningOnStart, locatedInIntro);
			}
			else
			{
				if (!(_spawnedCombineableMeshGroup != null))
				{
					return;
				}
				ModelInfo combinedMeshInfo;
				if (_meshContainer.TryGetGroupMesh(_spawnedCombineableMeshGroup, out combinedMeshInfo))
				{
					_meshFilter.mesh = combinedMeshInfo.Mesh;
					_meshRenderer.materials = combinedMeshInfo.Materials;
					_meshRenderer.enabled = true;
					return;
				}
				if (_meshContainer.IsMeshGroupCombineInProgress(_spawnedCombineableMeshGroup))
				{
					_meshContainer.AddMeshGroupCombinedCallback(_spawnedCombineableMeshGroup, UpdateToCombinedMesh);
					return;
				}
				base.SpawnInstances(prefabProvider, true, spawningOnStart, locatedInIntro);
				CombineableMesh[] componentsInChildren = GetComponentsInChildren<CombineableMesh>(true);
				if (0 < componentsInChildren.Length)
				{
					if (spawningOnStart || _combineModelsInstantly)
					{
						CombineMeshes(componentsInChildren);
						return;
					}
					_meshContainer.NotifyMeshGroupCombineInProgress(_spawnedCombineableMeshGroup);
					_meshCombiningRoutine = CombineMeshesAsync(componentsInChildren);
					SingletonMonobehavior<WorkLoadQueue>.Instance.EnqueueWork(_meshCombiningRoutine);
				}
				else
				{
					Debug.LogWarning("No model infoes passed for combining.");
				}
			}
		}

		private void CombineMeshes(IList<CombineableMesh> combineableMeshes)
		{
			IList<ObjectModelInfo> meshInfos = MeshUtil.GetMeshInfos(combineableMeshes);
			ModelInfo? combinedMeshes = MeshUtil.CombineMeshes(base.gameObject, meshInfos);
			ReceiveCombinedMeshAndDespawn(combinedMeshes);
		}

		private IEnumerator CombineMeshesAsync(IList<CombineableMesh> combineableMeshes)
		{
			_isCombiningMeshes = true;
			yield return MeshUtil.CombineMeshesAsync(meshInfos: MeshUtil.GetMeshInfos(combineableMeshes), parentGO: base.gameObject, meshReceiveCallback: ReceiveCombinedMeshAndDespawn);
		}

		private void UpdateToCombinedMesh(ModelInfo? combinedMeshes)
		{
			if (combinedMeshes.HasValue)
			{
				combinedMeshes.Value.Mesh.name = base.name + "_combined";
				_meshFilter.mesh = combinedMeshes.Value.Mesh;
				_meshRenderer.materials = combinedMeshes.Value.Materials;
				_meshRenderer.enabled = true;
				_usingCombinedMesh = true;
			}
			else
			{
				Debug.LogErrorFormat(this, "Did not receive a combined mesh. No child meshes to combine");
			}
		}

		private void ReceiveCombinedMeshAndDespawn(ModelInfo? combinedMeshes)
		{
			UpdateToCombinedMesh(combinedMeshes);
			if (combinedMeshes.HasValue)
			{
				_meshContainer.AddGroup(_spawnedCombineableMeshGroup, combinedMeshes.Value);
				base.DespawnInstances();
			}
			_isCombiningMeshes = false;
		}

		protected override void GetSpawnedObjects(List<GameObject> objectsToVisit)
		{
		}

		protected override void DespawnInstances()
		{
			if (_isCombiningMeshes)
			{
				_meshContainer.NotifyMeshGroupCombineCanceled(_spawnedCombineableMeshGroup);
				_isCombiningMeshes = false;
				_spawnedCombineableMeshGroup = null;
				SingletonMonobehavior<WorkLoadQueue>.Instance.CancelWork(_meshCombiningRoutine);
			}
			if (_usingCombinedMesh)
			{
				_meshFilter.mesh = null;
				_meshRenderer.enabled = false;
				_usingCombinedMesh = false;
			}
			else
			{
				_placeholder.DespawnInstances();
			}
		}

		private void OnValidate()
		{
			PlaceholderPrefabProviderBase component = GetComponent<PlaceholderPrefabProviderBase>();
			if (component == null)
			{
				Debug.LogError("ModelSelectionPlaceholderLifecycle: Missing a PlaceholderPrefabProvider at " + Utils.GetLongName(base.transform), this);
				return;
			}
			for (int i = 0; i < component.PrefabCount; i++)
			{
				Transform prefab = component.GetPrefab(i);
				if (prefab == null)
				{
					Debug.LogError("ModelSelectionPlaceholderLifecycle: NULL prefabs at index " + i + " in PlaceholderPrefabProvider at " + Utils.GetLongName(prefab.transform), component);
				}
				else
				{
					CombineableMeshGroup component2 = prefab.GetComponent<CombineableMeshGroup>();
					if (component2 == null)
					{
						Debug.LogError("ModelSelectionPlaceholderLifecycle: Model prefab missing CombineableMeshGroup component " + Utils.GetLongName(prefab.transform), component);
					}
				}
			}
		}
	}
}
