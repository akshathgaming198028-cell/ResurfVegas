using System;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class PlayerAnimation : MonoBehaviour
{
	public Rigidbody rigid;

	public Transform rootBone;

	public Transform upperBodyBone;

	public float maxIdleSpeed;

	public float minWalkSpeed;

	public AnimationClip idle;

	public AnimationClip turn;

	public AnimationClip shootAdditive;

	public MoveAnimation[] moveAnimations;

	public SignalSender footstepSignals;

	private Transform tr;

	private Vector3 lastPosition;

	private Vector3 velocity;

	private Vector3 localVelocity;

	private float speed;

	private float angle;

	private float lowerBodyDeltaAngle;

	private float idleWeight;

	private Vector3 lowerBodyForwardTarget;

	private Vector3 lowerBodyForward;

	private MoveAnimation bestAnimation;

	private float lastFootstepTime;

	private float lastAnimTime;

	public Animation animationComponent;

	public PlayerAnimation()
	{
		maxIdleSpeed = 0.5f;
		minWalkSpeed = 2f;
		lastPosition = Vector3.zero;
		velocity = Vector3.zero;
		localVelocity = Vector3.zero;
		lowerBodyForwardTarget = Vector3.forward;
		lowerBodyForward = Vector3.forward;
	}

	public virtual void Awake()
	{
		tr = rigid.transform;
		lastPosition = tr.position;
		int i = 0;
		MoveAnimation[] array = moveAnimations;
		for (int length = array.Length; i < length; i++)
		{
			array[i].Init();
			animationComponent[array[i].clip.name].layer = 1;
			animationComponent[array[i].clip.name].enabled = true;
		}
		animationComponent.SyncLayer(1);
		animationComponent[idle.name].layer = 2;
		animationComponent[turn.name].layer = 3;
		animationComponent[idle.name].enabled = true;
		animationComponent[shootAdditive.name].layer = 4;
		animationComponent[shootAdditive.name].weight = 1f;
		animationComponent[shootAdditive.name].speed = 0.6f;
		animationComponent[shootAdditive.name].blendMode = AnimationBlendMode.Additive;
	}

	public virtual void OnStartFire()
	{
		if (Time.timeScale != 0f)
		{
			animationComponent[shootAdditive.name].enabled = true;
		}
	}

	public virtual void OnStopFire()
	{
		animationComponent[shootAdditive.name].enabled = false;
	}

	public virtual void FixedUpdate()
	{
		velocity = (tr.position - lastPosition) / Time.deltaTime;
		localVelocity = tr.InverseTransformDirection(velocity);
		localVelocity.y = 0f;
		speed = localVelocity.magnitude;
		angle = HorizontalAngle(localVelocity);
		lastPosition = tr.position;
	}

	public virtual void Update()
	{
		idleWeight = Mathf.Lerp(idleWeight, Mathf.InverseLerp(minWalkSpeed, maxIdleSpeed, speed), Time.deltaTime * 10f);
		animationComponent[idle.name].weight = idleWeight;
		if (!(speed <= 0f))
		{
			float num = float.PositiveInfinity;
			int i = 0;
			MoveAnimation[] array = moveAnimations;
			for (int length = array.Length; i < length; i++)
			{
				float num2 = Mathf.Abs(Mathf.DeltaAngle(angle, array[i].angle));
				float num3 = Mathf.Abs(speed - array[i].speed);
				float num4 = num2 + num3;
				if (RuntimeServices.EqualityOperator(array[i], bestAnimation))
				{
					num4 *= 0.9f;
				}
				if (!(num4 >= num))
				{
					bestAnimation = array[i];
					num = num4;
				}
			}
			animationComponent.CrossFade(bestAnimation.clip.name);
		}
		else
		{
			bestAnimation = null;
		}
		if (lowerBodyForward != lowerBodyForwardTarget && !(idleWeight < 0.9f))
		{
			animationComponent.CrossFade(turn.name, 0.05f);
		}
		if (bestAnimation != null && !(idleWeight >= 0.9f))
		{
			float num5 = Mathf.Repeat(animationComponent[bestAnimation.clip.name].normalizedTime * 2f + 0.1f, 1f);
			if (!(num5 >= lastAnimTime) && !(Time.time <= lastFootstepTime + 0.1f))
			{
				footstepSignals.SendSignals(this);
				lastFootstepTime = Time.time;
			}
			lastAnimTime = num5;
		}
	}

	public virtual void LateUpdate()
	{
		float num = Mathf.InverseLerp(minWalkSpeed, maxIdleSpeed, speed);
		if (!(num >= 1f))
		{
			Vector3 zero = Vector3.zero;
			int i = 0;
			MoveAnimation[] array = moveAnimations;
			for (int length = array.Length; i < length; i++)
			{
				if (animationComponent[array[i].clip.name].weight != 0f && Vector3.Dot(array[i].velocity, localVelocity) > 0f)
				{
					zero += array[i].velocity * animationComponent[array[i].clip.name].weight;
				}
			}
			float b = Mathf.DeltaAngle(HorizontalAngle(tr.rotation * zero), HorizontalAngle(velocity));
			lowerBodyDeltaAngle = Mathf.LerpAngle(lowerBodyDeltaAngle, b, Time.deltaTime * 10f);
			lowerBodyForwardTarget = tr.forward;
			lowerBodyForward = Quaternion.Euler(0f, lowerBodyDeltaAngle, 0f) * lowerBodyForwardTarget;
		}
		else
		{
			lowerBodyForward = Vector3.RotateTowards(lowerBodyForward, lowerBodyForwardTarget, Time.deltaTime * 520f * ((float)Math.PI / 180f), 1f);
			lowerBodyDeltaAngle = Mathf.DeltaAngle(HorizontalAngle(tr.forward), HorizontalAngle(lowerBodyForward));
			if (!(Mathf.Abs(lowerBodyDeltaAngle) <= 80f))
			{
				lowerBodyForwardTarget = tr.forward;
			}
		}
		Quaternion quaternion = Quaternion.Euler(0f, lowerBodyDeltaAngle, 0f);
		rootBone.rotation = quaternion * rootBone.rotation;
		upperBodyBone.rotation = Quaternion.Inverse(quaternion) * upperBodyBone.rotation;
	}

	public static float HorizontalAngle(Vector3 direction)
	{
		return Mathf.Atan2(direction.x, direction.z) * 57.29578f;
	}

	public virtual void Main()
	{
	}
}
