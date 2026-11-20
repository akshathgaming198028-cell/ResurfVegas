using System;
using UnityEngine;

[Serializable]
public class MovementMotor : MonoBehaviour
{
	[HideInInspector]
	public Vector3 movementDirection;

	[HideInInspector]
	public Vector3 movementTarget;

	[HideInInspector]
	public Vector3 facingDirection;

	public virtual void Main()
	{
	}
}
