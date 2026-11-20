using System;
using UnityEngine;

[Serializable]
public class HealthFlash : MonoBehaviour
{
	public Health playerHealth;

	public Material healthMaterial;

	private float healthBlink;

	private float oneOverMaxHealth;

	public HealthFlash()
	{
		healthBlink = 1f;
		oneOverMaxHealth = 0.5f;
	}

	public virtual void Start()
	{
		oneOverMaxHealth = 1f / playerHealth.maxHealth;
	}

	public virtual void Update()
	{
		float num = playerHealth.health * oneOverMaxHealth;
		healthMaterial.SetFloat("_SelfIllumination", num * 2f * healthBlink);
		if (!(num >= 0.45f))
		{
			healthBlink = Mathf.PingPong(Time.time * 6f, 2f);
		}
		else
		{
			healthBlink = 1f;
		}
	}

	public virtual void Main()
	{
	}
}
