using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class DailyLetterPickup : MonoBehaviour
{
	public Collider pickupCollider;

	public MeshRenderer meshRenderer;

	public Glow glow;

	public MeshFilter LetterMesh;

	public List<Mesh> Letters;

	public bool shouldSpawnParticles;

	private Pickup pickup;

	private char letter;

	private bool canPickup;

	public char Letter
	{
		set
		{
			letter = value;
			if (HasDailyLetter)
			{
				int num = letter - 65;
				if (num < Letters.Count && num >= 0 && LetterMesh != null)
				{
					LetterMesh.mesh = Letters[num];
				}
			}
			if (base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				SetVisible(HasDailyLetter);
			}
		}
	}

	private bool HasDailyLetter
	{
		get
		{
			return letter != '\0';
		}
	}

	private void Awake()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2015)
		{
			DailyLetterPickupManager.Instance.InitializePickup(this);
		}
		else if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
			pickup = GetComponent<Pickup>();
			Pickup obj = pickup;
			obj.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(obj.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
			DailyLetterPickupManager.Instance.InitializePickup(this);
			TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
			trackObject.OnActivate += TrackObject_OnActivate;
			trackObject.OnDeactivate += TrackObject_OnDeactivate;
		}
	}

	private void TrackObject_OnActivate()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
			SetVisible(HasDailyLetter);
			canPickup = true;
		}
	}

	private void TrackObject_OnDeactivate()
	{
		SetVisible(false);
	}

	private void SetVisible(bool visible)
	{
		if (pickupCollider != null)
		{
			pickupCollider.enabled = visible;
		}
		meshRenderer.enabled = visible;
		if (glow != null)
		{
			glow.SetVisible(visible);
		}
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		if (canPickup)
		{
			CoroutineUtil.StartCoroutine(PickupCoroutine(particles));
			canPickup = false;
		}
	}

	private IEnumerator PickupCoroutine(CharacterPickupParticles particles)
	{
		SetVisible(false);
		GameStats.Instance.AddScoreForPickup(PowerupType.letters);
		PlayerInfo.Instance.PickedupLetter(letter);
		particles.PickedUpPowerUp();
		GameStats.Instance.letterPickups++;
		yield return new WaitForSeconds(1f);
		DailyLetterPickupManager.Instance.UpdateLetter();
	}

	public static void DoReward()
	{
		CoroutineUtil.StartCoroutine(DoReward_Coroutine());
	}

	public static IEnumerator DoReward_Coroutine()
	{
		if (DailyLetterPickupManager.Instance != null)
		{
			GameStats.Instance.AddScoreForPickup(PowerupType.letters);
			PlayerInfo.Instance.PickedupLetter(DailyLetterPickupManager.Instance.Letter);
			GameStats.Instance.letterPickups++;
			yield return new WaitForSeconds(2f);
			DailyLetterPickupManager.Instance.UpdateLetter();
		}
		else
		{
			Debug.LogError("DailyLetterPickup.DoReward() Missing Required Reference: " + DailyLetterPickupManager.Instance);
		}
	}
}
