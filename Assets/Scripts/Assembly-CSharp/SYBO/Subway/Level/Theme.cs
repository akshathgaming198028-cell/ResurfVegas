using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class Theme : ScriptableObject
	{
		[Serializable]
		public struct GenericAndThemePrefab
		{
			public GameObject genericPrefab;

			public GameObject themePrefab;
		}

		[Serializable]
		public struct GenericAndThemePrefabGroup
		{
			public string name;

			public GenericAndThemePrefab[] prefabMappings;
		}

		[Serializable]
		public struct MaterialTexturePair
		{
			public Material material;

			public Texture2D texture;
		}

		[Serializable]
		public class MaterialGroup
		{
			public Material highPerformance;

			public Material mediumPerformance;

			public Material lowPerformance;
		}

		[SerializeField]
		private BackgroundObject[] _monumentPrefabs = new BackgroundObject[0];

		[SerializeField]
		private BackgroundObject[] _backgroundPrefabs = new BackgroundObject[0];

		[SerializeField]
		private Color _fogColor;

		[SerializeField]
		private Color _fogGradientTop;

		[SerializeField]
		private Color _fogGradientBottom;

		[SerializeField]
		private Color _fogSilhouetteColor;

		[SerializeField]
		private float _fogGradientOffset = 0.5f;

		[SerializeField]
		private Color _glowGoldColor;

		[SerializeField]
		private Material _glowGold;

		[SerializeField]
		private float _glowGoldFalloff = 200f;

		[SerializeField]
		private GameObject[] _environmentModels;

		[SerializeField]
		private GameObject[] _characterModels;

		[SerializeField]
		private Material[] _dynamicChangingMaterials;

		[SerializeField]
		private MaterialTexturePair[] _materialReplacementTextures;

		[SerializeField]
		private MaterialGroup[] _devicePerformanceRelativeMaterials;

		[SerializeField]
		private GenericAndThemePrefabGroup[] _prefabMappingsGeneric;

		[NonSerialized]
		private bool _initialized;

		private Dictionary<GameObject, GameObject> _genericToThemePrefabDict = new Dictionary<GameObject, GameObject>();

		private Dictionary<Material, Material> _devicePerformanceRelativeMaterialsDict = new Dictionary<Material, Material>();

		public Dictionary<Material, Texture> MaterialReplacementTextureMap = new Dictionary<Material, Texture>();

		public Dictionary<string, Mesh> EnvironmentModelMeshes = new Dictionary<string, Mesh>();

		public Dictionary<string, Mesh> CharacterModelMeshes = new Dictionary<string, Mesh>();

		public ReadOnlyCollection<BackgroundObject> MonumentPrefabs { get; private set; }

		public ReadOnlyCollection<BackgroundObject> BackgroundPrefabs { get; private set; }

		public Color FogColor
		{
			get
			{
				return _fogColor;
			}
		}

		public Color FogGradientTop
		{
			get
			{
				return _fogGradientTop;
			}
		}

		public Color FogGradientBottom
		{
			get
			{
				return _fogGradientBottom;
			}
		}

		public Color FogSilhouetteColor
		{
			get
			{
				return _fogSilhouetteColor;
			}
		}

		public float FogGradientOffset
		{
			get
			{
				return _fogGradientOffset;
			}
		}

		public Material GlowGoldMatrial
		{
			get
			{
				return _glowGold;
			}
		}

		public Color GlowGoldColor
		{
			get
			{
				return _glowGoldColor;
			}
		}

		public float GlowGoldFalloff
		{
			get
			{
				return _glowGoldFalloff;
			}
		}

		public Material[] DynamicChangingMaterials
		{
			get
			{
				return (Material[])_dynamicChangingMaterials.Clone();
			}
		}

		public GenericAndThemePrefabGroup[] GenericToThemePrefabMappingGroups
		{
			get
			{
				return _prefabMappingsGeneric;
			}
		}

		private void OnEnable()
		{
			_initialized = false;
			Initialize();
		}

		private void EditorInitData()
		{
			EnvironmentModelMeshes.Clear();
			CharacterModelMeshes.Clear();
			_genericToThemePrefabDict.Clear();
			if (_prefabMappingsGeneric == null || _prefabMappingsGeneric.Length == 0)
			{
				_prefabMappingsGeneric = new GenericAndThemePrefabGroup[6]
				{
					new GenericAndThemePrefabGroup
					{
						name = "Fillers",
						prefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						name = "Obstacles",
						prefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						name = "Specials",
						prefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						name = "Tracks",
						prefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						name = "Tunnels",
						prefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						name = "Trains",
						prefabMappings = new GenericAndThemePrefab[0]
					}
				};
			}
		}

		public void Initialize()
		{
			if (_initialized)
			{
				return;
			}
			if (Application.isEditor)
			{
				EditorInitData();
			}
			if (_prefabMappingsGeneric != null)
			{
				for (int i = 0; i < _prefabMappingsGeneric.Length; i++)
				{
					GenericAndThemePrefabGroup genericAndThemePrefabGroup = _prefabMappingsGeneric[i];
					if (genericAndThemePrefabGroup.prefabMappings == null)
					{
						continue;
					}
					for (int j = 0; j < genericAndThemePrefabGroup.prefabMappings.Length; j++)
					{
						GenericAndThemePrefab genericAndThemePrefab = genericAndThemePrefabGroup.prefabMappings[j];
						if (genericAndThemePrefab.genericPrefab != null && genericAndThemePrefab.themePrefab != null)
						{
							_genericToThemePrefabDict.Add(genericAndThemePrefab.genericPrefab, genericAndThemePrefab.themePrefab);
						}
					}
				}
			}
			DeviceInfo.PerformanceLevel performanceLevel = DeviceInfo.Instance.performanceLevel;
			if (_devicePerformanceRelativeMaterials != null && performanceLevel != DeviceInfo.PerformanceLevel.High)
			{
				for (int k = 0; k < _devicePerformanceRelativeMaterials.Length; k++)
				{
					MaterialGroup materialGroup = _devicePerformanceRelativeMaterials[k];
					if (!(materialGroup.highPerformance == null))
					{
						Material material = null;
						switch (performanceLevel)
						{
						case DeviceInfo.PerformanceLevel.Low:
							material = materialGroup.lowPerformance;
							break;
						case DeviceInfo.PerformanceLevel.Medium:
							material = materialGroup.mediumPerformance;
							break;
						}
						if (!(material == null))
						{
							_devicePerformanceRelativeMaterialsDict[materialGroup.highPerformance] = material;
						}
					}
				}
			}
			MonumentPrefabs = new ReadOnlyCollection<BackgroundObject>(_monumentPrefabs);
			BackgroundPrefabs = new ReadOnlyCollection<BackgroundObject>(_backgroundPrefabs);
			if (_materialReplacementTextures != null)
			{
				for (int l = 0; l < _materialReplacementTextures.Length; l++)
				{
					MaterialTexturePair materialTexturePair = _materialReplacementTextures[l];
					MaterialReplacementTextureMap[materialTexturePair.material] = materialTexturePair.texture;
				}
			}
			GetMeshes();
			_initialized = true;
		}

		private void GetMeshes()
		{
			if (_environmentModels != null)
			{
				GameObject[] environmentModels = _environmentModels;
				foreach (GameObject gameObject in environmentModels)
				{
					if (gameObject == null)
					{
						continue;
					}
					MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>(true);
					MeshFilter[] array = componentsInChildren;
					foreach (MeshFilter meshFilter in array)
					{
						if (EnvironmentModelMeshes.ContainsKey(meshFilter.sharedMesh.name))
						{
							Debug.Log(meshFilter.sharedMesh.name + " already exists at " + Utils.GetLongName(meshFilter.transform));
						}
						else
						{
							EnvironmentModelMeshes.Add(meshFilter.sharedMesh.name, meshFilter.sharedMesh);
						}
					}
				}
			}
			if (_characterModels == null)
			{
				return;
			}
			GameObject[] characterModels = _characterModels;
			foreach (GameObject gameObject2 in characterModels)
			{
				if (gameObject2 == null)
				{
					continue;
				}
				SkinnedMeshRenderer[] componentsInChildren2 = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				SkinnedMeshRenderer[] array2 = componentsInChildren2;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
				{
					if (CharacterModelMeshes.ContainsKey(skinnedMeshRenderer.sharedMesh.name))
					{
						Debug.Log(skinnedMeshRenderer.sharedMesh.name + " already exists at " + Utils.GetLongName(skinnedMeshRenderer.transform));
					}
					else
					{
						CharacterModelMeshes.Add(skinnedMeshRenderer.sharedMesh.name, skinnedMeshRenderer.sharedMesh);
					}
				}
			}
		}

		public bool TryGetDevicePerformanceRelativeMaterial(Material material, out Material performanceRelative)
		{
			Initialize();
			return _devicePerformanceRelativeMaterialsDict.TryGetValue(material, out performanceRelative);
		}

		public bool TryGetThemePrefabFromGeneric(GameObject genericPrefab, out GameObject themePrefab)
		{
			Initialize();
			if (_genericToThemePrefabDict.ContainsKey(genericPrefab))
			{
				themePrefab = _genericToThemePrefabDict[genericPrefab];
				if (themePrefab == null)
				{
					Debug.LogError("Theme.TryGetThemePrefabFromGeneric(): generic prefab is set, but theme specific missing. Setting theme prefab to generic " + genericPrefab.name);
					themePrefab = genericPrefab;
				}
				return true;
			}
			themePrefab = null;
			return false;
		}
	}
}
