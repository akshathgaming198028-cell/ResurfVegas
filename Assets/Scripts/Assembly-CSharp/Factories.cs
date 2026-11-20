using System;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Subway.Info;
using UnityEngine;

public class Factories : SingletonMonobehavior<Factories>
{
	[Serializable]
	private class AssetBundleMapping
	{
		public Characters.CharacterType CharacterType;

		public string[] AssetBundleNames;
	}

	[SerializeField]
	private AssetBundleMapping[] _assetBundleMappings = new AssetBundleMapping[0];

	private Dictionary<byte, string[]> _runTimeAssetBundleMapping = new Dictionary<byte, string[]>();

	public Action OnLateUpdate;

	public AssetBundleFactory<CharacterCustomizationBundle> CharacterCustomizationBundle { get; private set; }

	public AssetBundleFactory<CharacterIdlesBundle> CharacterIdlesBundle { get; private set; }

	public AssetBundleFactory<CharacterAssetsBundle> CharacterAssetsBundle { get; private set; }

	public AssetBundleFactory<Material> MaterialFactory { get; private set; }

	public AssetBundleFactory<Texture2D> TextureFactory { get; private set; }

	private void Awake()
	{
		CharacterCustomizationBundle = new AssetBundleFactory<CharacterCustomizationBundle>();
		CharacterIdlesBundle = new AssetBundleFactory<CharacterIdlesBundle>();
		CharacterAssetsBundle = new AssetBundleFactory<CharacterAssetsBundle>();
		MaterialFactory = new AssetBundleFactory<Material>();
		TextureFactory = new AssetBundleFactory<Texture2D>();
		for (int i = 0; i < _assetBundleMappings.Length; i++)
		{
			_runTimeAssetBundleMapping.Add((byte)_assetBundleMappings[i].CharacterType, _assetBundleMappings[i].AssetBundleNames);
		}
	}

	private void LateUpdate()
	{
		if (OnLateUpdate != null)
		{
			OnLateUpdate();
		}
	}

	public string[] GetReferencedAssetBundleNames(Characters.CharacterType characterType)
	{
		if (_runTimeAssetBundleMapping.ContainsKey((byte)characterType))
		{
			return _runTimeAssetBundleMapping[(byte)characterType];
		}
		for (int i = 0; i < _assetBundleMappings.Length; i++)
		{
			if (_assetBundleMappings[i].CharacterType == characterType)
			{
				return _assetBundleMappings[i].AssetBundleNames;
			}
		}
		Debug.LogError("Missing asset bundle mapping for " + characterType);
		return new string[0];
	}
}
