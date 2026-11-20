using System;
using SYBO.Core.Components.Singleton;
using UnityEngine;

public class CharacterAssetFactory : MonoBehaviour
{
	public static void GetAssetPrefab(string name, Action<CharacterAssetsBundle> onSucces)
	{
		SingletonMonobehavior<Factories>.Instance.CharacterAssetsBundle.RequestItemInstance(name, delegate(CharacterAssetsBundle prefab)
		{
			if (onSucces != null)
			{
				onSucces(prefab);
			}
			else
			{
				onSucces(null);
			}
		});
	}
}
