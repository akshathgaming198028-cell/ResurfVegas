using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CharacterPassTrigger : TrackEventTrigger
{
	private Transform _transform;

	private BoxCollider _collider;

	public float EndPosition
	{
		get
		{
			return _transform.position.z + _collider.center.z + 0.5f * _collider.size.z;
		}
	}

	public event EventHandler Triggered;

	private void Awake()
	{
		_transform = base.transform;
		_collider = GetComponent<Collider>() as BoxCollider;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && this.Triggered != null)
		{
			this.Triggered(this, EventArgs.Empty);
		}
	}

	protected void OnDrawGizmosSelected()
	{
		BoxCollider boxCollider = GetComponent<Collider>() as BoxCollider;
		Gizmos.DrawWireCube(base.transform.position + boxCollider.center, boxCollider.size);
	}
}
