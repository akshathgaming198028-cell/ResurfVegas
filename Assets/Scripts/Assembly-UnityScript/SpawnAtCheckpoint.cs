using System;
using UnityEngine;

[Serializable]
public class SpawnAtCheckpoint : MonoBehaviour
{
	public Transform checkpoint;

	public virtual void OnSignal()
	{
		transform.position = checkpoint.position;
		transform.rotation = checkpoint.rotation;
		ResetHealthOnAll();
	}

	public static void ResetHealthOnAll()
	{
		Health[] array = (Health[])UnityEngine.Object.FindObjectsOfType(typeof(Health));
		int i = 0;
		Health[] array2 = array;
		for (int length = array2.Length; i < length; i++)
		{
			array2[i].dead = false;
			array2[i].health = array2[i].maxHealth;
		}
	}

	public virtual void Main()
	{
	}
}
