using System.Collections;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Core.Visuals
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class CombineableMesh : MonoBehaviour
	{
		[SerializeField]
		[Header("Will not combine submeshes if more than 4000 vertices")]
		private bool _combineSubmeshesLowEndDevice;

		private bool _initialized;

		private Mesh _originalMesh;

		private MeshRenderer _meshRenderer;

		private MeshFilter _meshFilter;

		private MeshContainer _meshContainer;

		private bool _startedMeshCombining;

		public MeshRenderer MeshRenderer
		{
			get
			{
				InitializeCheck();
				return _meshRenderer;
			}
		}

		public MeshFilter MeshFilter
		{
			get
			{
				InitializeCheck();
				return _meshFilter;
			}
		}

		public ObjectModelInfo MeshInfo
		{
			get
			{
				InitializeCheck();
				return new ObjectModelInfo(_meshRenderer, _meshFilter);
			}
		}

		private void Awake()
		{
			InitializeCheck();
		}

		private void InitializeCheck()
		{
			if (!_initialized)
			{
				_meshContainer = SingletonMonobehavior<MeshContainer>.Instance;
				_meshRenderer = GetComponent<Renderer>() as MeshRenderer;
				_meshFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
				_originalMesh = _meshFilter.sharedMesh;
				PerformanceLevelModelSwap performanceLevelModelSwap = GetComponent(typeof(PerformanceLevelModelSwap)) as PerformanceLevelModelSwap;
				if (performanceLevelModelSwap != null)
				{
					performanceLevelModelSwap.SwapMaterials();
				}
				if (_combineSubmeshesLowEndDevice && DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && _meshFilter.sharedMesh != null && _meshFilter.sharedMesh.vertexCount < 4000)
				{
					MergeSubmeshes();
				}
				_initialized = true;
			}
		}

		private void MergeSubmeshes()
		{
			if (_originalMesh != null && 1 < _originalMesh.subMeshCount)
			{
				ModelInfo combinedMeshInfo;
				if (_meshContainer.TryGetMesh(_originalMesh, out combinedMeshInfo))
				{
					UpdateToCombinedMesh(combinedMeshInfo);
					return;
				}
				if (_meshContainer.IsMeshCombineInProgress(_originalMesh))
				{
					_meshContainer.AddMeshGroupCombinedCallback(_originalMesh, UpdateToCombinedMesh);
					return;
				}
				_meshContainer.NotifyMeshCombineInProgress(_originalMesh);
				_startedMeshCombining = true;
				CombineableMesh[] combineableMeshes = new CombineableMesh[1] { this };
				IEnumerator workItem = CombineMeshesAsync(combineableMeshes);
				SingletonMonobehavior<WorkLoadQueue>.Instance.EnqueueWork(workItem);
			}
		}

		private IEnumerator CombineMeshesAsync(IList<CombineableMesh> combineableMeshes)
		{
			yield return MeshUtil.CombineMeshesAsync(meshInfos: MeshUtil.GetMeshInfos(combineableMeshes), parentGO: base.gameObject, meshReceiveCallback: UpdateToCombinedMesh);
		}

		private void UpdateToCombinedMesh(ModelInfo? combinedMesh)
		{
			if (combinedMesh.HasValue)
			{
				_meshFilter.mesh = combinedMesh.Value.Mesh;
				_meshRenderer.materials = combinedMesh.Value.Materials;
				if (_startedMeshCombining)
				{
					combinedMesh.Value.Mesh.name = _originalMesh.name + "_combined";
					_meshContainer.AddMesh(_originalMesh, combinedMesh.Value);
					_startedMeshCombining = false;
				}
			}
		}
	}
}
