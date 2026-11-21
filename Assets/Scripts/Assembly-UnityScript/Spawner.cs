using System;
using System.Collections;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class Spawner : MonoBehaviour
{
	[NonSerialized]
	public static Spawner spawner;

	public ObjectCache[] caches;

	public Hashtable activeCachedObjects;

	public virtual void Awake()
	{
		spawner = this;
		int num = 0;
		for (int i = 0; i < Extensions.get_length((System.Array)caches); i++)
		{
			caches[i].Initialize();
			num += caches[i].cacheSize;
		}
		activeCachedObjects = new Hashtable(num);
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		ObjectCache objectCache = null;
		if ((bool)spawner)
		{
			for (int i = 0; i < Extensions.get_length((System.Array)spawner.caches); i++)
			{
				if (spawner.caches[i].prefab == prefab)
				{
					objectCache = spawner.caches[i];
				}
			}
		}
		object result;
		if (RuntimeServices.EqualityOperator(objectCache, null))
		{
			result = ((GameObject)UnityEngine.Object.Instantiate(prefab, position, rotation)) as GameObject;
		}
		else
		{
			GameObject nextObjectInCache = objectCache.GetNextObjectInCache();
			nextObjectInCache.transform.position = position;
			nextObjectInCache.transform.rotation = rotation;
			nextObjectInCache.SetActive(true);
			spawner.activeCachedObjects[nextObjectInCache] = true;
			result = nextObjectInCache;
		}
		return (GameObject)result;
	}

	public static void Destroy(GameObject objectToDestroy)
	{
		if ((bool)spawner && spawner.activeCachedObjects.ContainsKey(objectToDestroy))
		{
			objectToDestroy.SetActive(false);
			spawner.activeCachedObjects[objectToDestroy] = false;
		}
		else
		{
			UnityEngine.Object.Destroy(objectToDestroy);
		}
	}

	public virtual void Main()
	{
	}
}
