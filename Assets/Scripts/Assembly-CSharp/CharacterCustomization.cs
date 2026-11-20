using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
	private Texture2D gradient;

	private SkinnedMeshRenderer outfitRenderer;

	private string currentCharacterName = string.Empty;

	private int currentCharacterVersion = -1;

	private List<GameObject> specialAssets = new List<GameObject>();

	private List<Material> customMaterials = new List<Material>();

	private List<Material> headMaterials = new List<Material>();

	private List<Material> cachedMaterials = new List<Material>();

	private GameObject _bundledAsset;

	private CharacterCustomizationAssetsFactory assetsFactory;

	public SkinnedMeshRenderer OutfitRenderer
	{
		get
		{
			return outfitRenderer;
		}
	}

	public void Awake()
	{
		assetsFactory = GetComponent<CharacterCustomizationAssetsFactory>();
	}

	private void DeleteSpecialAssets()
	{
		foreach (GameObject specialAsset in specialAssets)
		{
			Object.Destroy(specialAsset);
		}
		specialAssets.Clear();
		if (_bundledAsset != null)
		{
			Object.Destroy(_bundledAsset);
			_bundledAsset = null;
		}
	}

	public void Customize(string name, int customizedVersion, SkinnedMeshRenderer model, SkinnedMeshRenderer outfitMesh, SkinnedMeshRenderer headMesh, CharacterRig characterRig, bool useSpecialAsset, bool deleteSpecialAsset, bool forceLOD0)
	{
		if (name == currentCharacterName)
		{
			if (customizedVersion != currentCharacterVersion)
			{
				DeleteSpecialAssets();
			}
		}
		else if (useSpecialAsset)
		{
			DeleteSpecialAssets();
		}
		currentCharacterName = name;
		currentCharacterVersion = customizedVersion;
		outfitRenderer = outfitMesh;
		outfitRenderer.enabled = false;
		CharacterCustomizationAssetsFactory.GetAssetPrefab(name, customizedVersion, delegate(CharacterCustomizationBundle bundledAsset)
		{
			if (bundledAsset != null)
			{
				_bundledAsset = bundledAsset.gameObject;
				CharacterCustomizationBundle.OutfitInfo newCustomSet = bundledAsset.Outfit;
				DestroyMaterials();
				CharacterMaterial.Request(newCustomSet.MainMaterialInfo.TextureName, newCustomSet.MainMaterialInfo.ShaderType, forceLOD0, DeviceInfo.PerformanceLevel.Low, delegate(Material material)
				{
					model.material = material;
					if (headMesh.sharedMesh != null)
					{
						cachedMaterials.Add(material);
						for (int i = 0; i < headMesh.sharedMesh.subMeshCount; i++)
						{
							headMaterials.Add(material);
						}
						headMesh.materials = headMaterials.ToArray();
						headMaterials.AddRange(headMesh.materials);
					}
				});
				if (newCustomSet.MainMesh != null)
				{
					model.sharedMesh = newCustomSet.MainMesh;
				}
				if (newCustomSet.ExtraMesh != null)
				{
					outfitRenderer.sharedMesh = newCustomSet.ExtraMesh;
					outfitRenderer.enabled = true;
					if (0 < newCustomSet.ExtraMaterialInfo.Length)
					{
						CharacterMaterial.Request(newCustomSet.ExtraMaterialInfo[0].TextureName, newCustomSet.ExtraMaterialInfo[0].ShaderType, forceLOD0, DeviceInfo.PerformanceLevel.Low, delegate(Material customMaterial)
						{
							cachedMaterials.Add(customMaterial);
							if (1 < newCustomSet.ExtraMaterialInfo.Length)
							{
								CharacterMaterial.Request(newCustomSet.ExtraMaterialInfo[1].TextureName, newCustomSet.ExtraMaterialInfo[1].ShaderType, forceLOD0, DeviceInfo.PerformanceLevel.Low, delegate(Material customExtraMaterial)
								{
									customMaterials.Add(customMaterial);
									customMaterials.Add(customExtraMaterial);
								});
							}
							else
							{
								customMaterials.Add(customMaterial);
							}
							outfitRenderer.materials = customMaterials.ToArray();
							customMaterials.AddRange(outfitRenderer.materials);
						});
					}
				}
				if ((newCustomSet.SpecialAssetsName != string.Empty && useSpecialAsset) || (newCustomSet.SpecialAsset != null && useSpecialAsset))
				{
					string currentPopupName = UIScreenController.Instance.GetCurrentPopupName();
					GameObject gameObject = null;
					if (newCustomSet.SpecialAssetsName != string.Empty)
					{
						GameObject gameObject2 = (GameObject)Resources.Load("Prefabs/CharacterSpecialProps/" + newCustomSet.SpecialAssetsName);
						if (gameObject2 != null)
						{
							gameObject = Object.Instantiate(gameObject2);
						}
						else
						{
							Debug.LogWarning(newCustomSet.SpecialAssetsName + " prefab is not present");
						}
					}
					else if (newCustomSet.SpecialAsset != null)
					{
						gameObject = Object.Instantiate(newCustomSet.SpecialAsset);
					}
					Utility.SetLayerRecursively(gameObject.transform, characterRig.gameObject.layer);
					CharacterCustomizationProp[] componentsInChildren = gameObject.GetComponentsInChildren<CharacterCustomizationProp>();
					characterRig.AttachPoint1.transform.localScale = Vector3.one;
					bool flag = false;
					CharacterCustomizationProp[] array = componentsInChildren;
					foreach (CharacterCustomizationProp characterCustomizationProp in array)
					{
						if (deleteSpecialAsset)
						{
							specialAssets.Add(characterCustomizationProp.gameObject);
						}
						characterCustomizationProp.Initialize(characterRig, currentPopupName == "CharacterNameEventPopup");
						flag = flag || gameObject == characterCustomizationProp.gameObject;
					}
					if (!flag)
					{
						gameObject.gameObject.transform.parent = characterRig.transform;
						specialAssets.Add(gameObject);
					}
				}
			}
			else
			{
				Debug.LogWarning(string.Format("Could not find customization for character by the name:- {0}", name));
			}
		});
	}

	private void DestroyMaterials()
	{
		if (cachedMaterials.Count > 0)
		{
			for (int i = 0; i < cachedMaterials.Count; i++)
			{
				Texture mainTexture = cachedMaterials[i].mainTexture;
				if (mainTexture != null)
				{
					Object.Destroy(mainTexture);
				}
				Object.Destroy(cachedMaterials[i]);
			}
			cachedMaterials.Clear();
		}
		if (headMaterials != null)
		{
			for (int j = 0; j < headMaterials.Count; j++)
			{
				Texture mainTexture2 = headMaterials[j].mainTexture;
				if (mainTexture2 != null)
				{
					Object.Destroy(mainTexture2);
				}
				Object.Destroy(headMaterials[j]);
			}
			headMaterials.Clear();
		}
		if (customMaterials == null)
		{
			return;
		}
		for (int k = 0; k < customMaterials.Count; k++)
		{
			Texture mainTexture3 = customMaterials[k].mainTexture;
			if (mainTexture3 != null)
			{
				Object.Destroy(mainTexture3);
			}
			Object.Destroy(customMaterials[k]);
		}
		customMaterials.Clear();
	}

	private void OnDestroy()
	{
		DestroyMaterials();
		if (_bundledAsset != null)
		{
			Object.Destroy(_bundledAsset);
		}
	}
}
