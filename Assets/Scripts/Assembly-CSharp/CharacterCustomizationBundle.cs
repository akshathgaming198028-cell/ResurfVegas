using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterCustomizationBundle : MonoBehaviour
{
	[Serializable]
	public struct CharacterMaterialInfo
	{
		[SerializeField]
		public string TextureName;

		[SerializeField]
		public CharacterMaterial.CharacterMaterialShaderType ShaderType;

		public CharacterMaterialInfo(string textureName, CharacterMaterial.CharacterMaterialShaderType shaderType)
		{
			TextureName = textureName;
			ShaderType = shaderType;
		}
	}

	[Serializable]
	public class OutfitInfo
	{
		[SerializeField]
		[FormerlySerializedAs("materialTextureName")]
		[HideInInspector]
		private string MaterialTextureName;

		[HideInInspector]
		[FormerlySerializedAs("customSkinnedMaterialTextureName")]
		[SerializeField]
		private string CustomSkinnedMaterialTextureName;

		[SerializeField]
		[FormerlySerializedAs("customSkinnedMaterialExtraTextureName")]
		[HideInInspector]
		private string CustomSkinnedMaterialExtraTextureName;

		[SerializeField]
		public CharacterMaterialInfo MainMaterialInfo;

		[SerializeField]
		public CharacterMaterialInfo[] ExtraMaterialInfo;

		public Mesh MainMesh;

		[FormerlySerializedAs("OutfitMesh")]
		public Mesh ExtraMesh;

		[FormerlySerializedAs("specialAsset")]
		public GameObject SpecialAsset;

		[FormerlySerializedAs("specialAssetsName")]
		public string SpecialAssetsName;
	}

	[FormerlySerializedAs("_customSet")]
	[SerializeField]
	private OutfitInfo _outfit;

	public OutfitInfo Outfit
	{
		get
		{
			return _outfit;
		}
	}

	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_outfit.MainMaterialInfo.TextureName))
		{
			Debug.LogError("Un-defined material texture name for " + base.name);
		}
	}
}
