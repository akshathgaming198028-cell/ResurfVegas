using System;
using SYBO.Core.Components.Singleton;
using UnityEngine;

public class CharacterCustomizationAssetsFactory : MonoBehaviour
{
	public static void GetAssetPrefab(string name, int customizedVersion, Action<CharacterCustomizationBundle> onSucces)
	{
		SingletonMonobehavior<Factories>.Instance.CharacterCustomizationBundle.RequestItemInstance(name + "_" + customizedVersion, delegate(CharacterCustomizationBundle prefab)
		{
			if (onSucces != null)
			{
				onSucces(prefab);
			}
		});
	}
}
