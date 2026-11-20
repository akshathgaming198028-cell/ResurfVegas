using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class CharacterLetterPickup : MonoBehaviour
{
	public Collider pickupCollider;

	public MeshRenderer meshRenderer;

	public Glow glow;

	public MeshFilter LetterMesh;

	public List<Mesh> Letters;

	public bool shouldSpawnParticles;

	private Pickup pickup;

	private char _letterChar;

	private bool _canPickup;

	public char Letter
	{
		set
		{
			_letterChar = value;
			if (HasDailyLetter)
			{
				int num = _letterChar - 65;
				if (num < Letters.Count && num >= 0)
				{
					LetterMesh.mesh = Letters[num];
				}
			}
			if (base.gameObject.activeInHierarchy)
			{
				SetVisible(HasDailyLetter);
			}
		}
	}

	private bool HasDailyLetter
	{
		get
		{
			return _letterChar != '\0';
		}
	}

	public static bool IsCharacterNameEventActive()
	{
		return SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterName) != null;
	}

	public static bool IsCharacterLetterPickupSpawnable()
	{
		bool result = false;
		if (IsCharacterNameEventActive())
		{
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager != null)
			{
				result = typeOfActiveEventManager.ShouldSpawnLetters();
			}
		}
		return result;
	}

	private void ReportLetterWasPickedup()
	{
		if (IsCharacterNameEventActive())
		{
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager != null)
			{
				typeOfActiveEventManager.NotifyLetterCollected(_letterChar);
			}
			if (SpawnPointManager.TEMP_WILL_SHOW_EXTRA_LOG_MESSAGES)
			{
				Debug.Log("CLP: _ReportLetterWasPickedup() _letterChar: " + _letterChar);
			}
		}
	}

	private char? GetNextCharacterLetterToSpawn()
	{
		char? result = null;
		if (IsCharacterLetterPickupSpawnable())
		{
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager != null)
			{
				result = typeOfActiveEventManager.NextLetterToSpawn();
			}
		}
		return result;
	}

	private void Awake()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
			pickup = GetComponent<Pickup>();
			Pickup obj = pickup;
			obj.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(obj.OnPickup, new Pickup.OnPickupDelegate(Pickup_OnPickup));
		}
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		trackObject.OnActivate += OnActivate;
		trackObject.OnDeactivate += OnDeactivate;
	}

	private void OnActivate()
	{
		_canPickup = false;
		SetVisible(false);
		if (IsCharacterLetterPickupSpawnable())
		{
			char? nextCharacterLetterToSpawn = GetNextCharacterLetterToSpawn();
			if (nextCharacterLetterToSpawn.HasValue)
			{
				Letter = nextCharacterLetterToSpawn.Value;
			}
			if (SpawnPointManager.TEMP_WILL_SHOW_EXTRA_LOG_MESSAGES)
			{
				Debug.Log("CLP: letter set to : " + _letterChar);
			}
			CharacterNameEventManager.OnLetterCollected += CharacterNameEventManager_OnLetterCollected;
			SSEventsManagerController.OnSSEventChangedHandler += SSEventsManagerController_OnSSEventChangedHandler;
		}
		else
		{
			Debug.LogWarning("CharacterLetterPickup.OnActivated() yet IsCharacterLetterPickupSpawnable()==false. So probably this spawned properly, then the event ended, then this was activated. This should be rare, but acceptable. Does that sound like this situation?");
		}
		SetVisible(HasDailyLetter);
	}

	private void OnDeactivate()
	{
		SetVisible(false);
		CharacterNameEventManager.OnLetterCollected -= CharacterNameEventManager_OnLetterCollected;
		SSEventsManagerController.OnSSEventChangedHandler -= SSEventsManagerController_OnSSEventChangedHandler;
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

	private IEnumerator PickupCoroutine(CharacterPickupParticles particles)
	{
		if (!_canPickup)
		{
			_canPickup = true;
			ReportLetterWasPickedup();
			SetVisible(false);
			particles.PickedUpPowerUp();
			yield return new WaitForSeconds(2f);
		}
	}

	private void DeactivatePickupIfEventHasCompleted()
	{
		CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
		if (typeOfActiveEventManager == null || typeOfActiveEventManager.HasCompletedCollectionForSelectedCharacter())
		{
			OnDeactivate();
		}
	}

	private void Pickup_OnPickup(CharacterPickupParticles particles)
	{
		StartCoroutine(PickupCoroutine(particles));
	}

	private void CharacterNameEventManager_OnLetterCollected(object sender, CharacterNameEventManager.LetterCollectedEventArgs letterCollectedEventArgs)
	{
		DeactivatePickupIfEventHasCompleted();
	}

	private void SSEventsManagerController_OnSSEventChangedHandler()
	{
		DeactivatePickupIfEventHasCompleted();
	}

	public void DoReward()
	{
		CoroutineUtil.StartCoroutine(DoReward_Coroutine());
	}

	public IEnumerator DoReward_Coroutine()
	{
		ReportLetterWasPickedup();
		SetVisible(false);
		yield return null;
	}
}
