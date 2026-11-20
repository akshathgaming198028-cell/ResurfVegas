using System;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using UnityEngine;

public class CharacterMaterial : MonoBehaviour
{
	public enum CharacterMaterialShaderType
	{
		Default = 0,
		Additive = 1
	}

	private class CharacterMaterialPathInfo
	{
		public CharacterMaterialShaderType ShaderType;

		public DeviceInfo.PerformanceLevel PerformanceLevel;

		public string MaterialPath;

		public CharacterMaterialPathInfo(CharacterMaterialShaderType shaderType, DeviceInfo.PerformanceLevel performanceLevel, string materialPath)
		{
			ShaderType = shaderType;
			PerformanceLevel = performanceLevel;
			MaterialPath = materialPath;
		}
	}

	private static List<CharacterMaterialPathInfo> _materialPaths = new List<CharacterMaterialPathInfo>
	{
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Default, DeviceInfo.PerformanceLevel.Low, "Materials/avatar_default_LOD0"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Default, DeviceInfo.PerformanceLevel.Medium, "Materials/avatar_default_LOD1"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Default, DeviceInfo.PerformanceLevel.High, "Materials/avatar_default_LOD2"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Additive, DeviceInfo.PerformanceLevel.Low, "Materials/avatar_additive_LOD0"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Additive, DeviceInfo.PerformanceLevel.Medium, "Materials/avatar_additive_LOD1"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Additive, DeviceInfo.PerformanceLevel.High, "Materials/avatar_additive_LOD2")
	};

	public static void Request(string textureName, CharacterMaterialShaderType shaderType, bool forcePerformanceLevel, DeviceInfo.PerformanceLevel forcedLevel, Action<Material> onSucces)
	{
		DeviceInfo.PerformanceLevel performanceLevel = DeviceInfo.Instance.performanceLevel;
		if (forcePerformanceLevel)
		{
			performanceLevel = forcedLevel;
		}
		string materialPath = GetMaterialPath(shaderType, performanceLevel);
		RequestMaterial(textureName, materialPath, onSucces);
	}

	private static string GetMaterialPath(CharacterMaterialShaderType shaderType, DeviceInfo.PerformanceLevel performanceLevel)
	{
		CharacterMaterialPathInfo characterMaterialPathInfo = _materialPaths.Find((CharacterMaterialPathInfo mp) => mp.ShaderType == shaderType && mp.PerformanceLevel == performanceLevel);
		return (characterMaterialPathInfo == null) ? null : characterMaterialPathInfo.MaterialPath;
	}

	private static void RequestMaterial(string textureName, string materialPath, Action<Material> onSucces)
	{
		Material mat = Resources.Load(materialPath, typeof(Material)) as Material;
		SetTexture(mat, textureName, onSucces);
	}

	private static void SetTexture(Material mat, string textureName, Action<Material> onSucces)
	{
		Material material = new Material(mat);
		SingletonMonobehavior<Factories>.Instance.TextureFactory.RequestItemInstance(textureName + "_low", delegate(Texture2D tex)
		{
			if (tex == null)
			{
				SingletonMonobehavior<Factories>.Instance.TextureFactory.RequestItemInstance(textureName + "_low", delegate(Texture2D texLow)
				{
					material.SetTexture("_MainTex", texLow);
					if (onSucces != null)
					{
						onSucces(material);
					}
				});
			}
			else
			{
				material.SetTexture("_MainTex", tex);
				if (onSucces != null)
				{
					onSucces(material);
				}
			}
		});
	}
}
