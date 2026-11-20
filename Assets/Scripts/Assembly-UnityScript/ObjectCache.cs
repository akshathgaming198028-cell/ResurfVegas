using System;
using UnityEngine;

[Serializable]
public class ObjectCache
{
	public GameObject prefab;

	public int cacheSize;

	private GameObject[] objects;

	private int cacheIndex;

	public ObjectCache()
	{
		cacheSize = 10;
	}

	public virtual void Initialize()
	{
		objects = new GameObject[cacheSize];
		for (int i = 0; i < cacheSize; i++)
		{
			objects[i] = UnityEngine.Object.Instantiate(prefab) as GameObject;
			objects[i].SetActive(false);
			objects[i].name = objects[i].name + i;
		}
	}

	public virtual GameObject GetNextObjectInCache()
	{
		GameObject gameObject = null;
		for (int i = 0; i < cacheSize; i++)
		{
			gameObject = objects[cacheIndex];
			if (!gameObject.activeSelf)
			{
				break;
			}
			cacheIndex = (cacheIndex + 1) % cacheSize;
		}
		if (gameObject.activeSelf)
		{
			Debug.LogWarning("Spawn of " + prefab.name + " exceeds cache size of " + cacheSize + "! Reusing already active object.", gameObject);
			Spawner.Destroy(gameObject);
		}
		cacheIndex = (cacheIndex + 1) % cacheSize;
		return gameObject;
	}
}
