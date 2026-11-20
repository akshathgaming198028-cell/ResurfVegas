using System;
using UnityEngine;

[Serializable]
public class DestroyObject : MonoBehaviour
{
	public GameObject objectToDestroy;

	public virtual void OnSignal()
	{
		Spawner.Destroy(objectToDestroy);
	}

	public virtual void Main()
	{
	}
}
