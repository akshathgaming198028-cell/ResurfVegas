using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Rigidbody))]
public class FreeMovementMotor : MovementMotor
{
	public float walkingSpeed;

	public float walkingSnappyness;

	public float turningSmoothing;

	public FreeMovementMotor()
	{
		walkingSpeed = 5f;
		walkingSnappyness = 50f;
		turningSmoothing = 0.3f;
	}

	public virtual void FixedUpdate()
	{
		Vector3 vector = movementDirection * walkingSpeed;
		Vector3 vector2 = vector - GetComponent<Rigidbody>().velocity;
		if (GetComponent<Rigidbody>().useGravity)
		{
			vector2.y = 0f;
		}
		GetComponent<Rigidbody>().AddForce(vector2 * walkingSnappyness, ForceMode.Acceleration);
		Vector3 vector3 = facingDirection;
		if (vector3 == Vector3.zero)
		{
			vector3 = movementDirection;
		}
		if (vector3 == Vector3.zero)
		{
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			return;
		}
		float num = AngleAroundAxis(transform.forward, vector3, Vector3.up);
		GetComponent<Rigidbody>().angularVelocity = Vector3.up * num * turningSmoothing;
	}

	public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		float num = Vector3.Angle(dirA, dirB);
		return num * (float)((Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) >= 0f) ? 1 : (-1));
	}

	public override void Main()
	{
	}
}
