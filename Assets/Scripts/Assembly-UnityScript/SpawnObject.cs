using System;
using UnityEngine;

[Serializable]
public class SpawnObject : MonoBehaviour
{
	public GameObject objectToSpawn;

	public SignalSender onDestroyedSignals;

	private GameObject spawned;

	public virtual void OnSignal()
	{
		spawned = Spawner.Spawn(objectToSpawn, transform.position, transform.rotation);
		if (onDestroyedSignals.receivers.Length > 0)
		{
			enabled = true;
		}
	}

	public virtual void Update()
	{
		if (spawned == null || !spawned.activeInHierarchy)
		{
			onDestroyedSignals.SendSignals(this);
			enabled = false;
		}
	}

	public virtual void Main()
	{
		enabled = false;
	}
}
