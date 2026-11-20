using System;
using UnityEngine;

public class PickupController : MonoBehaviour
{
	public delegate void OnActivateDelegate();

	public delegate void OnDeactivateDelegate();

	public OnActivateDelegate OnActivate;

	public OnDeactivateDelegate OnDeactivate;

	public MeshRenderer meshRenderer;

	public Glow glow;

	public bool ShouldSpawnParticles;

	private Collider parentCollider;

	private void Awake()
	{
		PickupEvent component = GetComponent<PickupEvent>();
		component.OnPickup = (PickupEvent.OnPickupDelegate)Delegate.Combine(component.OnPickup, new PickupEvent.OnPickupDelegate(OnPickup));
		parentCollider = FindParentCollider(base.transform);
		if (parentCollider == null)
		{
			Debug.Log("Error: No collider for PickupDefault.");
		}
		SetVisible(false);
	}

	private Collider FindParentCollider(Transform current)
	{
		if (current.GetComponent<Collider>() != null)
		{
			return current.GetComponent<Collider>();
		}
		if (current.parent != null)
		{
			return FindParentCollider(current.parent);
		}
		return null;
	}

	private void SetVisible(bool visible)
	{
		meshRenderer.enabled = visible;
		if (glow != null)
		{
			glow.SetVisible(visible);
		}
	}

	public void Activate()
	{
		parentCollider.enabled = true;
		SetVisible(true);
		if (OnActivate != null)
		{
			OnActivate();
		}
	}

	public void Deactivate()
	{
		SetVisible(false);
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		if (base.gameObject != null)
		{
			parentCollider.enabled = false;
		}
		SetVisible(false);
		if (ShouldSpawnParticles)
		{
			particles.PickedUpDefaultPowerUp();
		}
	}
}
