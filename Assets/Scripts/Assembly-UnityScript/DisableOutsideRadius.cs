using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(SphereCollider))]
public class DisableOutsideRadius : MonoBehaviour
{
	private GameObject target;

	private SphereCollider sphereCollider;

	private float activeRadius;

	public virtual void Awake()
	{
		target = transform.parent.gameObject;
		sphereCollider = GetComponent<SphereCollider>();
		activeRadius = sphereCollider.radius;
		Disable();
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && target.transform.parent == transform)
		{
			Enable();
		}
	}

	public virtual void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			Disable();
		}
	}

	public virtual void Disable()
	{
		transform.parent = target.transform.parent;
		target.transform.parent = transform;
		target.SetActive(false);
		sphereCollider.radius = activeRadius;
	}

	public virtual void Enable()
	{
		target.transform.parent = transform.parent;
		target.SetActive(true);
		transform.parent = target.transform;
		sphereCollider.radius = activeRadius * 1.1f;
	}

	public virtual void Main()
	{
	}
}
