using System;
using UnityEngine;

[Serializable]
public class SpiderReturnMoveController : MonoBehaviour
{
	public MovementMotor motor;

	private AI ai;

	private Transform character;

	private Vector3 spawnPos;

	public MonoBehaviour animationBehaviour;

	public virtual void Awake()
	{
		character = motor.transform;
		ai = transform.parent.GetComponentInChildren<AI>();
		spawnPos = character.position;
	}

	public virtual void Update()
	{
		motor.movementDirection = spawnPos - character.position;
		motor.movementDirection.y = 0f;
		if (!(motor.movementDirection.sqrMagnitude <= 1f))
		{
			motor.movementDirection = motor.movementDirection.normalized;
		}
		if (!(motor.movementDirection.sqrMagnitude >= 0.01f))
		{
			character.position = new Vector3(spawnPos.x, character.position.y, spawnPos.z);
			motor.GetComponent<Rigidbody>().velocity = Vector3.zero;
			motor.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			motor.movementDirection = Vector3.zero;
			enabled = false;
			animationBehaviour.enabled = false;
		}
	}

	public virtual void Main()
	{
	}
}
