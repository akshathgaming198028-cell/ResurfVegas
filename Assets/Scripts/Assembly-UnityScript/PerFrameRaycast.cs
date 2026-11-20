using System;
using UnityEngine;

[Serializable]
public class PerFrameRaycast : MonoBehaviour
{
	private RaycastHit hitInfo;

	private Transform tr;

	public virtual void Awake()
	{
		tr = transform;
	}

	public virtual void Update()
	{
		hitInfo = default(RaycastHit);
		Physics.Raycast(tr.position, tr.forward, out hitInfo);
	}

	public virtual RaycastHit GetHitInfo()
	{
		return hitInfo;
	}

	public virtual void Main()
	{
	}
}
