using System;
using UnityEngine;

[Serializable]
public class MechAnimation : MonoBehaviour
{
	public Rigidbody rigid;

	public AnimationClip idle;

	public AnimationClip walk;

	public AnimationClip turnLeft;

	public AnimationClip turnRight;

	public SignalSender footstepSignals;

	private Transform tr;

	private float lastFootstepTime;

	private float lastAnimTime;

	public virtual void OnEnable()
	{
		tr = rigid.transform;
		GetComponent<Animation>()[idle.name].layer = 0;
		GetComponent<Animation>()[idle.name].weight = 1f;
		GetComponent<Animation>()[idle.name].enabled = true;
		GetComponent<Animation>()[walk.name].layer = 1;
		GetComponent<Animation>()[turnLeft.name].layer = 1;
		GetComponent<Animation>()[turnRight.name].layer = 1;
		GetComponent<Animation>()[walk.name].weight = 1f;
		GetComponent<Animation>()[turnLeft.name].weight = 0f;
		GetComponent<Animation>()[turnRight.name].weight = 0f;
		GetComponent<Animation>()[walk.name].enabled = true;
		GetComponent<Animation>()[turnLeft.name].enabled = true;
		GetComponent<Animation>()[turnRight.name].enabled = true;
	}

	public virtual void FixedUpdate()
	{
		float num = Mathf.Abs(rigid.angularVelocity.y) * 57.29578f / 100f;
		float num2 = rigid.velocity.magnitude / 2.5f;
		float num3 = Mathf.Sign(rigid.angularVelocity.y);
		GetComponent<Animation>()[walk.name].speed = Mathf.Lerp(1f, GetComponent<Animation>()[walk.name].length / GetComponent<Animation>()[turnLeft.name].length * 1.33f, num);
		GetComponent<Animation>()[turnLeft.name].time = GetComponent<Animation>()[walk.name].time;
		GetComponent<Animation>()[turnRight.name].time = GetComponent<Animation>()[walk.name].time;
		GetComponent<Animation>()[turnLeft.name].weight = Mathf.Clamp01((0f - num) * num3);
		GetComponent<Animation>()[turnRight.name].weight = Mathf.Clamp01(num * num3);
		GetComponent<Animation>()[walk.name].weight = Mathf.Clamp01(num2);
		if (!(num2 + num <= 0.1f))
		{
			float num4 = Mathf.Repeat(GetComponent<Animation>()[walk.name].normalizedTime * 2f + 0.1f, 1f);
			if (!(num4 >= lastAnimTime) && !(Time.time <= lastFootstepTime + 0.1f))
			{
				footstepSignals.SendSignals(this);
				lastFootstepTime = Time.time;
			}
			lastAnimTime = num4;
		}
	}

	public virtual void Main()
	{
	}
}
