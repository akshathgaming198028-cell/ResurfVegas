using System;
using System.Collections.Generic;
using PathologicalGames;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Characters.States;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Coins
{
	[PrefabPath("Prefabs/Managers/CoinManager2")]
	public class CoinManager2 : SingletonMonobehavior<CoinManager2>
	{
		[NonSerialized]
		public bool QA_SpawnNewDynamicCoins = true;

		[NonSerialized]
		public bool QA_SpawnOldDynamicCoins;

		[SerializeField]
		private Coin2 _coinPrefab;

		private SpawnPool _coinPool;

		private HashSet<Coin2> _activeCoins = new HashSet<Coin2>();

		private GameStats _gameStats;

		private Character _character;

		public event EventHandler OnCoinCollected;

		protected void Start()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			_coinPool = SingletonMonobehavior<SpawnPoolManager>.Instance.GetSpawnPool(SpawnPoolType.Coins);
			_character = Character.Instance;
			_gameStats = GameStats.Instance;
			if (Globals.DEBUG_LOG_COINS)
			{
				CoinSystemSpawnLog.CreateLogs();
			}
		}

		public void NotifyCoinActivated(Coin2 coin)
		{
			if (_activeCoins.Contains(coin))
			{
				Debug.LogWarning("Already activated: " + coin, coin.gameObject);
				return;
			}
			_activeCoins.Add(coin);
			coin.OnPickedUp += Coin_OnCoinPickedUp;
		}

		public void NotifyCoinDeactivated(Coin2 coin)
		{
			_activeCoins.Remove(coin);
			coin.OnPickedUp -= Coin_OnCoinPickedUp;
			_coinPool.Despawn(coin.gameObject.transform);
		}

		public void NotifyCoinDestroyed(Coin2 coin)
		{
			_activeCoins.Remove(coin);
		}

		public Coin2 SpawnCoin(Vector3 position, Transform parentTransform = null)
		{
			Coin2 coin = null;
			Transform transform = _coinPool.Spawn(_coinPrefab.gameObject.transform);
			coin = transform.GetComponent<Coin2>();
			transform.position = position;
			if (parentTransform != null)
			{
				transform.parent = parentTransform;
			}
			coin.Initialize();
			coin.Activate();
			return coin;
		}

		private void RewardCoin()
		{
			if (PlayerInfo.Instance.hasDoubleCoins)
			{
				if (DoubleCoins.mysteryDoubleCoinsEnabled)
				{
					_gameStats.coins += 4;
				}
				else
				{
					_gameStats.coins += 2;
				}
			}
			else if (DoubleCoins.mysteryDoubleCoinsEnabled)
			{
				_gameStats.coins += 2;
			}
			else
			{
				_gameStats.coins++;
			}
			if (_character.IsAboveGround)
			{
				_gameStats.coinsInAir++;
			}
			if (JetpackState.Instance.isActive)
			{
				_gameStats.coinsWithJetpack++;
			}
			if (PogoStickState.Instance.isActive)
			{
				_gameStats.coinsWithPogoStick++;
			}
			if (_character.TrackIndex == 0)
			{
				_gameStats.coinsCollectedOnLeftTrack++;
			}
			else if (_character.TrackIndex == 1)
			{
				_gameStats.coinsCollectedOnCenterTrack++;
			}
			else if (_character.TrackIndex == 2)
			{
				_gameStats.coinsCollectedOnRightTrack++;
			}
		}

		private void RaiseOnCoinCollected()
		{
			if (this.OnCoinCollected != null)
			{
				this.OnCoinCollected(this, EventArgs.Empty);
			}
		}

		private void Coin_OnCoinPickedUp(object sender, EventArgs args)
		{
			RewardCoin();
			RaiseOnCoinCollected();
		}
	}
}
