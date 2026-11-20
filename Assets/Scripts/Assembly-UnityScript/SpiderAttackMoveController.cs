using System;
using UnityEngine;

[Serializable]
public class SpiderAttackMoveController : MonoBehaviour
{
	public MovementMotor motor;

	public float targetDistanceMin;

	public float targetDistanceMax;

	public float proximityDistance;

	public float damageRadius;

	public float proximityBuildupTime;

	public float proximityOfNoReturn;

	public float damageAmount;

	public Renderer proximityRenderer;

	public AudioSource audioSource;

	public SelfIlluminationBlink[] blinkComponents;

	public GlowPlane blinkPlane;

	public GameObject intentionalExplosion;

	public MonoBehaviour animationBehaviour;

	private AI ai;

	private Transform character;

	private Transform player;

	private bool inRange;

	private float nextRaycastTime;

	private float lastRaycastSuccessfulTime;

	private float proximityLevel;

	private float lastBlinkTime;

	private float noticeTime;

	public SpiderAttackMoveController()
	{
		targetDistanceMin = 2f;
		targetDistanceMax = 3f;
		proximityDistance = 4f;
		damageRadius = 5f;
		proximityBuildupTime = 2f;
		proximityOfNoReturn = 0.6f;
		damageAmount = 30f;
	}

	public virtual void Awake()
	{
		character = motor.transform;
		player = GameObject.FindWithTag("Player").transform;
		ai = transform.parent.GetComponentInChildren<AI>();
		if (blinkComponents.Length == 0)
		{
			blinkComponents = transform.parent.GetComponentsInChildren<SelfIlluminationBlink>();
		}
	}

	public virtual void OnEnable()
	{
		inRange = false;
		nextRaycastTime = Time.time;
		lastRaycastSuccessfulTime = Time.time;
		noticeTime = Time.time;
		animationBehaviour.enabled = true;
		if ((bool)blinkPlane)
		{
			blinkPlane.GetComponent<Renderer>().enabled = false;
		}
	}

	public virtual void OnDisable()
	{
		if (proximityRenderer == null)
		{
			Debug.LogError("proximityRenderer is null", this);
		}
		else if (proximityRenderer.material == null)
		{
			Debug.LogError("proximityRenderer.material is null", this);
		}
		else
		{
			proximityRenderer.material.color = Color.white;
		}
		if ((bool)blinkPlane)
		{
			blinkPlane.GetComponent<Renderer>().enabled = false;
		}
	}

	public virtual void Update()
	{
		if (!(Time.time >= noticeTime + 0.7f))
		{
			motor.movementDirection = Vector3.zero;
			return;
		}
		Vector3 movementDirection = player.position - character.position;
		movementDirection.y = 0f;
		float magnitude = movementDirection.magnitude;
		movementDirection /= magnitude;
		if (inRange && !(magnitude <= targetDistanceMax))
		{
			inRange = false;
		}
		if (!inRange && !(magnitude >= targetDistanceMin))
		{
			inRange = true;
		}
		if (inRange)
		{
			motor.movementDirection = Vector3.zero;
		}
		else
		{
			motor.movementDirection = movementDirection;
		}
		if ((!(magnitude >= proximityDistance) && Time.time < lastRaycastSuccessfulTime + 1f) || !(proximityLevel <= proximityOfNoReturn))
		{
			proximityLevel += Time.deltaTime / proximityBuildupTime;
		}
		else
		{
			proximityLevel -= Time.deltaTime / proximityBuildupTime;
		}
		proximityLevel = Mathf.Clamp01(proximityLevel);
		if (proximityLevel == 1f)
		{
			Explode();
		}
		if (!(Time.time <= nextRaycastTime))
		{
			nextRaycastTime = Time.time + 1f;
			if (ai.CanSeePlayer())
			{
				lastRaycastSuccessfulTime = Time.time;
			}
			else if (!(Time.time <= lastRaycastSuccessfulTime + 2f))
			{
				ai.OnLostTrack();
			}
		}
		float num = 1f / Mathf.Lerp(2f, 15f, proximityLevel);
		if (!(Time.time <= lastBlinkTime + num))
		{
			lastBlinkTime = Time.time;
			proximityRenderer.material.color = Color.red;
			if (audioSource.enabled)
			{
				audioSource.Play();
			}
			int i = 0;
			SelfIlluminationBlink[] array = blinkComponents;
			for (int length = array.Length; i < length; i++)
			{
				array[i].Blink();
			}
			if ((bool)blinkPlane)
			{
				blinkPlane.GetComponent<Renderer>().enabled = !blinkPlane.GetComponent<Renderer>().enabled;
			}
		}
		if (!(Time.time <= lastBlinkTime + 0.04f))
		{
			proximityRenderer.material.color = Color.white;
		}
	}

	public virtual void Explode()
	{
		float num = 1f - Vector3.Distance(player.position, character.position) / damageRadius;
		Health component = player.GetComponent<Health>();
		if ((bool)component)
		{
			component.OnDamage(damageAmount * num, character.position - player.position);
		}
		player.GetComponent<Rigidbody>().AddExplosionForce(10f, character.position, damageRadius, 0f, ForceMode.Impulse);
		Spawner.Spawn(intentionalExplosion, transform.position, Quaternion.identity);
		Spawner.Destroy(character.gameObject);
	}

	public virtual void Main()
	{
	}
}
