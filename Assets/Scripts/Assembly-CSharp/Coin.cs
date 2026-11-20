using System;
using SYBO.Subway.Characters;
using SYBO.Subway.Characters.States;
using SYBO.Subway.Coins;
using UnityEngine;

[RequireComponent(typeof(Pickup))]
[RequireComponent(typeof(TrackObject))]
public class Coin : MonoBehaviour
{
	private Character character;

	private GameStats gameStats;

	private bool __canPickup;

	private Transform pivot;

	private Vector3 initialPivotPosition;

	private GameObject glow;

	private Vector3 initialGlowPosition;

	private Pickup _pickup;

	private PickupDefault _pickupDefault;

	public bool CanPickup
	{
		get
		{
			return __canPickup;
		}
		private set
		{
			__canPickup = value;
		}
	}

	public bool IsActive
	{
		get
		{
			return CanPickup;
		}
	}

	public Transform PivotTransform
	{
		get
		{
			return pivot;
		}
	}

	private void Awake()
	{
		character = Character.Instance;
		gameStats = GameStats.Instance;
		pivot = base.transform.GetChild(0);
		initialPivotPosition = pivot.localPosition;
		_pickup = GetComponent<Pickup>();
		Pickup pickup = _pickup;
		pickup.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(pickup.OnPickup, new Pickup.OnPickupDelegate(Pickup_OnPickup));
		_pickupDefault = GetComponent<PickupDefault>();
		TrackObject component = GetComponent<TrackObject>();
		component.OnActivate += TrackObject_OnActivate;
		component.OnDeactivate += TrackObject_OnDeactivate;
		if (base.gameObject.GetComponentInChildren<Glow>() != null)
		{
			glow = base.gameObject.GetComponentInChildren<Glow>().gameObject;
		}
		if (glow != null)
		{
			initialGlowPosition = glow.transform.localPosition;
		}
		CoinStats.Old.NotifyCoinCreated();
	}

	private void Pickup_OnPickup(CharacterPickupParticles pickupParticles)
	{
		if (!CanPickup || !_pickupDefault.IsVisible)
		{
			return;
		}
		if (PlayerInfo.Instance.hasDoubleCoins)
		{
			if (DoubleCoins.mysteryDoubleCoinsEnabled)
			{
				gameStats.coins += 4;
			}
			else
			{
				gameStats.coins += 2;
			}
		}
		else if (DoubleCoins.mysteryDoubleCoinsEnabled)
		{
			gameStats.coins += 2;
		}
		else
		{
			gameStats.coins++;
		}
		if (character.IsAboveGround)
		{
			gameStats.coinsInAir++;
		}
		if (JetpackState.Instance.isActive)
		{
			gameStats.coinsWithJetpack++;
		}
		if (PogoStickState.Instance.isActive)
		{
			gameStats.coinsWithPogoStick++;
		}
		if (character.TrackIndex == 0)
		{
			gameStats.coinsCollectedOnLeftTrack++;
		}
		else if (character.TrackIndex == 1)
		{
			gameStats.coinsCollectedOnCenterTrack++;
		}
		else if (character.TrackIndex == 2)
		{
			gameStats.coinsCollectedOnRightTrack++;
		}
		pickupParticles.PickedUpCoin(_pickup);
		CanPickup = false;
	}

	private void DebugLog(string message)
	{
		if (!(base.gameObject.transform.position.y > 90f))
		{
		}
	}

	private void TrackObject_OnActivate()
	{
		DebugLog("1 TrackObject_OnActivate()");
		CanPickup = true;
		pivot.localPosition = initialPivotPosition;
		if (glow != null)
		{
			glow.transform.localPosition = initialGlowPosition;
		}
		CoinStats.Old.NotifyCoinActivated();
		if (Globals.DEBUG_LOG_COINS)
		{
			CoinSystemSpawnLog.Old.LogCoin(base.gameObject);
		}
	}

	private void TrackObject_OnDeactivate()
	{
		DebugLog("2 TrackObject_OnActivate()");
		Deactivate();
	}

	public void Deactivate()
	{
		DebugLog("3 Deactivate()");
		CanPickup = false;
		_pickupDefault.Deactivate();
		CoinStats.Old.NotifyCoinDeactivated();
	}
}
