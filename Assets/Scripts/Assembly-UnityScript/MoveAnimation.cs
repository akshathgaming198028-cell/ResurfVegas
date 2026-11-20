using System;
using UnityEngine;

[Serializable]
public class MoveAnimation
{
	public AnimationClip clip;

	public Vector3 velocity;

	[HideInInspector]
	public float weight;

	[HideInInspector]
	public bool currentBest;

	[HideInInspector]
	public float speed;

	[HideInInspector]
	public float angle;

	public virtual void Init()
	{
		velocity.y = 0f;
		speed = velocity.magnitude;
		angle = PlayerAnimation.HorizontalAngle(velocity);
	}
}
