using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(PerFrameRaycast))]
public class AutoFire : MonoBehaviour
{
	public GameObject bulletPrefab;

	public Transform spawnPoint;

	public float frequency;

	public float coneAngle;

	public bool firing;

	public float damagePerSecond;

	public float forcePerSecond;

	public float hitSoundVolume;

	public GameObject muzzleFlashFront;

	private float lastFireTime;

	private PerFrameRaycast raycast;

	public AutoFire()
	{
		frequency = 10f;
		coneAngle = 1.5f;
		damagePerSecond = 20f;
		forcePerSecond = 20f;
		hitSoundVolume = 0.5f;
		lastFireTime = -1f;
	}

	public virtual void Awake()
	{
		muzzleFlashFront.SetActive(false);
		raycast = GetComponent<PerFrameRaycast>();
		if (spawnPoint == null)
		{
			spawnPoint = transform;
		}
	}

	public virtual void Update()
	{
		if (!firing || Time.time <= lastFireTime + 1f / frequency)
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(UnityEngine.Random.Range(0f - coneAngle, coneAngle), UnityEngine.Random.Range(0f - coneAngle, coneAngle), 0f);
		GameObject gameObject = Spawner.Spawn(bulletPrefab, spawnPoint.position, spawnPoint.rotation * quaternion) as GameObject;
		SimpleBullet component = gameObject.GetComponent<SimpleBullet>();
		lastFireTime = Time.time;
		RaycastHit hitInfo = raycast.GetHitInfo();
		if ((bool)hitInfo.transform)
		{
			Health component2 = hitInfo.transform.GetComponent<Health>();
			if ((bool)component2)
			{
				component2.OnDamage(damagePerSecond / frequency, -spawnPoint.forward);
			}
			if ((bool)hitInfo.rigidbody)
			{
				Vector3 force = transform.forward * (forcePerSecond / frequency);
				hitInfo.rigidbody.AddForceAtPosition(force, hitInfo.point, ForceMode.Impulse);
			}
			component.dist = hitInfo.distance;
		}
		else
		{
			component.dist = 1000f;
		}
	}

	public virtual void OnStartFire()
	{
		if (Time.timeScale != 0f)
		{
			firing = true;
			muzzleFlashFront.SetActive(true);
			if ((bool)GetComponent<AudioSource>())
			{
				GetComponent<AudioSource>().Play();
			}
		}
	}

	public virtual void OnStopFire()
	{
		firing = false;
		muzzleFlashFront.SetActive(false);
		if ((bool)GetComponent<AudioSource>())
		{
			GetComponent<AudioSource>().Stop();
		}
	}

	public virtual void Main()
	{
	}
}
