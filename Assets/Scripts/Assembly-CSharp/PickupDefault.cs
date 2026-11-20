using System;
using SYBO.Subway;
using SYBO.Subway.Pickups;
using UnityEngine;

public class PickupDefault : MonoBehaviour
{
	public delegate void ActivateDelegate();

	public MeshRenderer meshRenderer;

	public Glow glow;

	public MeshRenderer[] rings;

	public bool ShouldSpawnParticles;

	private Collider parentCollider;

	public ActivateDelegate ActivatePickup;

	public bool IsVisible
	{
		get
		{
			if (meshRenderer != null)
			{
				return meshRenderer.enabled;
			}
			return false;
		}
	}

	private void Awake()
	{
		Pickup component = GetComponent<Pickup>();
		component.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(component.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		trackObject.OnActivate += OnActivate;
		trackObject.OnDeactivate += OnDeactivate;
		parentCollider = FindParentCollider(base.transform);
		if (parentCollider == null && Game.IsInstantiated())
		{
			Debug.Log("Error: No collider for PickupDefault. Fix Immediately", base.gameObject);
		}
		SetVisible(false);
	}

	public static PickupType ConvertSubclassInstanceToPickupType(PickupDefault pickupDefault)
	{
		if (pickupDefault.ToString().Contains("jetpackPickup"))
		{
			return PickupType.JetPack;
		}
		if (pickupDefault.ToString().Contains("jumpBooster"))
		{
			return PickupType.SuperSneakers;
		}
		if (pickupDefault.ToString().Contains("doubleScoreMultiplier"))
		{
			return PickupType.MultiplierScore2x;
		}
		if (pickupDefault.ToString().Contains("magnetBooster"))
		{
			return PickupType.CoinMagnet;
		}
		Debug.LogError("Unexpected value here. Fix immediately");
		return PickupType.JetPack;
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

	private void SetVisible(bool isVisible)
	{
		if (this.meshRenderer != null)
		{
			this.meshRenderer.enabled = isVisible;
		}
		if (glow != null)
		{
			glow.SetVisible(isVisible);
		}
		if (rings != null)
		{
			MeshRenderer[] array = rings;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = isVisible;
			}
		}
	}

	private void OnActivate()
	{
		parentCollider.enabled = true;
		if (ActivatePickup != null)
		{
			ActivatePickup();
		}
		SetVisible(true);
	}

	private void OnDeactivate()
	{
		SetVisible(false);
	}

	private void OnValidate()
	{
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>(true);
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			if (meshFilter.sharedMesh == null)
			{
				Debug.LogWarning("Missing mesh for pickup at " + Utils.GetLongName(meshFilter.transform));
			}
		}
	}

	public void Activate()
	{
		base.gameObject.SetActive(true);
		if (ActivatePickup != null)
		{
			ActivatePickup();
		}
		if (parentCollider != null)
		{
			parentCollider.enabled = true;
		}
		SetVisible(true);
	}

	public void Deactivate()
	{
		base.gameObject.SetActive(false);
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
