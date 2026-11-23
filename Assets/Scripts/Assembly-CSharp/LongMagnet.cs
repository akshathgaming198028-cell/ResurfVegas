using System;
using System.Collections;
using SYBO.Core;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class LongMagnet : MonoBehaviour
{
	private Game game;

	private CharacterController characterController;

	private CharacterRendering characterRendering;

	private Character character;

	private CollisionNotifier coinMagnetCollider;

	public float pullSpeed = 200f;

	private VariableBool longMagnetSuction = new VariableBool();

	public bool ShouldBeActive
	{
		get
		{
			return (Hoverboard.Instance.isActive && PlayerInfo.Instance.currentHoverboard == Hoverboards.BoardType.gliderboard) || SuperSneakers.Instance.isActive;
		}
	}

	public VariableBool LongMagnetSuction
	{
		get
		{
			return longMagnetSuction;
		}
	}

	public void Start()
	{
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		coinMagnetCollider = character.coinMagnetLongCollider;
		characterController = character.characterController;
		game = Game.Instance;
		Variable<bool> isInGame = game.IsInGame;
		isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool value)
		{
			if (value)
			{
				longMagnetSuction.Clear();
			}
		});
		VariableBool variableBool = longMagnetSuction;
		variableBool.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(variableBool.OnChange, (VariableBool.OnChangeDelegate)delegate(bool value)
		{
			if (value)
			{
				coinMagnetCollider.TriggerEnter += CoinHit;
				coinMagnetCollider.GetComponent<Collider>().enabled = true;
			}
			else
			{
				coinMagnetCollider.GetComponent<Collider>().enabled = false;
				coinMagnetCollider.TriggerEnter -= CoinHit;
			}
		});
	}

	public void CoinHit(Collider collider)
	{
		if (game.IsInJetpackMode)
		{
			return;
		}
		Coin component = collider.GetComponent<Coin>();
		if (component != null)
		{
			component.GetComponent<Collider>().enabled = false;
			StartCoroutine(PullCoin(component));
			return;
		}
		Coin2 component2 = collider.GetComponent<Coin2>();
		if (component2 != null)
		{
			component2.MoveToSuperSneakers(pullSpeed);
			return;
		}
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
			Pickup componentInChildren = collider.GetComponentInChildren<Pickup>();
			if (componentInChildren != null)
			{
				componentInChildren.NotifyPickup(character.CharacterPickupParticleSystem);
				return;
			}
			PickupEvent componentInChildren2 = collider.GetComponentInChildren<PickupEvent>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.NotifyPickup(character.CharacterPickupParticleSystem);
				return;
			}
		}
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2015)
		{
			PickupSpawnPoint2 component3 = collider.GetComponent<PickupSpawnPoint2>();
			if (component3 != null && component3.SpawnPointState == SpawnPointState.Spawned)
			{
				StartCoroutine(PullPickupSpawnPoint(component3));
			}
		}
	}

	private IEnumerator PullCoin(Coin coin)
	{
		Transform pivot = coin.PivotTransform;
		Vector3 position = pivot.position;
		float distance = (position - characterController.transform.position).magnitude;
		Vector3 offsetCoinHitPosition = new Vector3(0f, -6f, 0f);
		yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
		{
			pivot.position = Vector3.Lerp(position, characterRendering.CharacterModel.MeshSuperSneaker.transform.position + offsetCoinHitPosition, t * t);
		}));
		Pickup pickup = coin.GetComponent<Pickup>();
		if ((bool)pickup)
		{
			character.NotifyPickup(pickup, null);
		}
		PickupEvent pickupEvent = coin.GetComponent<PickupEvent>();
		if ((bool)pickupEvent)
		{
			character.NotifyPickup(null, pickupEvent);
		}
	}

	private IEnumerator PullPickupSpawnPoint(PickupSpawnPoint2 pickupSpawnPoint)
	{
		Transform pivot = pickupSpawnPoint.gameObject.transform;
		Vector3 position = pivot.position;
		float distance = (position - characterController.transform.position).magnitude;
		Vector3 pickupSpawnPointPositionOffset = new Vector3(0f, 0f, 0f);
		yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
		{
			pickupSpawnPoint.SetWorldPosition(Vector3.Lerp(position, characterRendering.CharacterModel.MeshSuperSneaker.transform.position + pickupSpawnPointPositionOffset, t * t));
		}));
	}

	public void Activate()
	{
		longMagnetSuction.Add(this);
	}

	public void Deactivate()
	{
		longMagnetSuction.Remove(this);
	}
}
