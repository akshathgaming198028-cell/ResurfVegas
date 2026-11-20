using System;
using UnityEngine;

[Serializable]
public class PatrolPoint : MonoBehaviour
{
	public Vector3 position;

	public virtual void Awake()
	{
		position = transform.position;
	}

	public virtual void Main()
	{
	}
}
