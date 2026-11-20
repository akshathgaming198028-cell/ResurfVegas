using System;
using UnityEngine;

[Serializable]
public class SimpleBullet : MonoBehaviour
{
	public float speed;

	public float lifeTime;

	public float dist;

	private float spawnTime;

	private Transform tr;

	public SimpleBullet()
	{
		speed = 10f;
		lifeTime = 0.5f;
		dist = 10000f;
	}

	public virtual void OnEnable()
	{
		tr = transform;
		spawnTime = Time.time;
	}

	public virtual void Update()
	{
		tr.position += tr.forward * speed * Time.deltaTime;
		dist -= speed * Time.deltaTime;
		if (Time.time > spawnTime + lifeTime || !(dist >= 0f))
		{
			Spawner.Destroy(gameObject);
		}
	}

	public virtual void Main()
	{
	}
}
