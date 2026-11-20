using System;
using UnityEngine;

[Serializable]
public class SpiderAnimation : MonoBehaviour
{
	public MovementMotor motor;

	public AnimationClip activateAnim;

	public AnimationClip forwardAnim;

	public AnimationClip backAnim;

	public AnimationClip leftAnim;

	public AnimationClip rightAnim;

	public AudioSource audioSource;

	public SignalSender footstepSignals;

	public bool skiddingSounds;

	public bool footstepSounds;

	private Transform tr;

	private float lastFootstepTime;

	private float lastAnimTime;

	public virtual void OnEnable()
	{
		tr = motor.transform;
		GetComponent<Animation>()[activateAnim.name].enabled = true;
		GetComponent<Animation>()[activateAnim.name].weight = 1f;
		GetComponent<Animation>()[activateAnim.name].time = 0f;
		GetComponent<Animation>()[activateAnim.name].speed = 1f;
		GetComponent<Animation>()[forwardAnim.name].layer = 1;
		GetComponent<Animation>()[forwardAnim.name].enabled = true;
		GetComponent<Animation>()[forwardAnim.name].weight = 0f;
		GetComponent<Animation>()[backAnim.name].layer = 1;
		GetComponent<Animation>()[backAnim.name].enabled = true;
		GetComponent<Animation>()[backAnim.name].weight = 0f;
		GetComponent<Animation>()[leftAnim.name].layer = 1;
		GetComponent<Animation>()[leftAnim.name].enabled = true;
		GetComponent<Animation>()[leftAnim.name].weight = 0f;
		GetComponent<Animation>()[rightAnim.name].layer = 1;
		GetComponent<Animation>()[rightAnim.name].enabled = true;
		GetComponent<Animation>()[rightAnim.name].weight = 0f;
	}

	public virtual void OnDisable()
	{
		GetComponent<Animation>()[activateAnim.name].enabled = true;
		GetComponent<Animation>()[activateAnim.name].weight = 1f;
		GetComponent<Animation>()[activateAnim.name].normalizedTime = 1f;
		GetComponent<Animation>()[activateAnim.name].speed = -1f;
		GetComponent<Animation>().CrossFade(activateAnim.name, 0.3f, PlayMode.StopAll);
	}

	public virtual void Update()
	{
		Vector3 movementDirection = motor.movementDirection;
		movementDirection.y = 0f;
		float magnitude = movementDirection.magnitude;
		GetComponent<Animation>()[forwardAnim.name].speed = magnitude;
		GetComponent<Animation>()[rightAnim.name].speed = magnitude;
		GetComponent<Animation>()[backAnim.name].speed = magnitude;
		GetComponent<Animation>()[leftAnim.name].speed = magnitude;
		float num = Mathf.DeltaAngle(HorizontalAngle(tr.forward), HorizontalAngle(movementDirection));
		if (!(magnitude <= 0.01f))
		{
			float num2 = default(float);
			if (!(num >= -90f))
			{
				num2 = Mathf.InverseLerp(-180f, -90f, num);
				GetComponent<Animation>()[forwardAnim.name].weight = 0f;
				GetComponent<Animation>()[rightAnim.name].weight = 0f;
				GetComponent<Animation>()[backAnim.name].weight = 1f - num2;
				GetComponent<Animation>()[leftAnim.name].weight = 1f;
			}
			else if (!(num >= 0f))
			{
				num2 = Mathf.InverseLerp(-90f, 0f, num);
				GetComponent<Animation>()[forwardAnim.name].weight = num2;
				GetComponent<Animation>()[rightAnim.name].weight = 0f;
				GetComponent<Animation>()[backAnim.name].weight = 0f;
				GetComponent<Animation>()[leftAnim.name].weight = 1f - num2;
			}
			else if (!(num >= 90f))
			{
				num2 = Mathf.InverseLerp(0f, 90f, num);
				GetComponent<Animation>()[forwardAnim.name].weight = 1f - num2;
				GetComponent<Animation>()[rightAnim.name].weight = num2;
				GetComponent<Animation>()[backAnim.name].weight = 0f;
				GetComponent<Animation>()[leftAnim.name].weight = 0f;
			}
			else
			{
				num2 = Mathf.InverseLerp(90f, 180f, num);
				GetComponent<Animation>()[forwardAnim.name].weight = 0f;
				GetComponent<Animation>()[rightAnim.name].weight = 1f - num2;
				GetComponent<Animation>()[backAnim.name].weight = num2;
				GetComponent<Animation>()[leftAnim.name].weight = 0f;
			}
		}
		if (skiddingSounds)
		{
			if (!(magnitude <= 0.2f) && !audioSource.isPlaying)
			{
				audioSource.Play();
			}
			else if (!(magnitude >= 0.2f) && audioSource.isPlaying)
			{
				audioSource.Pause();
			}
		}
		if (footstepSounds && !(magnitude <= 0.2f))
		{
			float num3 = Mathf.Repeat(GetComponent<Animation>()[forwardAnim.name].normalizedTime * 4f + 0.1f, 1f);
			if (!(num3 >= lastAnimTime) && !(Time.time <= lastFootstepTime + 0.1f))
			{
				footstepSignals.SendSignals(this);
				lastFootstepTime = Time.time;
			}
			lastAnimTime = num3;
		}
	}

	public static float HorizontalAngle(Vector3 direction)
	{
		return Mathf.Atan2(direction.x, direction.z) * 57.29578f;
	}

	public virtual void Main()
	{
	}
}
