using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using UnityEngine;

namespace SYBO.Subway.Coins
{
	public class CoinSingleManager : SingletonMonobehavior<CoinSingleManager>
	{
		private List<Coin> _coins;

		public int GetCoinCount()
		{
			int result = 0;
			if (_coins != null)
			{
				result = _coins.Count;
			}
			return result;
		}

		public int GetActiveCoinCount()
		{
			int num = 0;
			foreach (Coin coin in _coins)
			{
				if (coin.IsActive)
				{
					num++;
				}
			}
			return num;
		}

		protected void Awake()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			_coins = new List<Coin>();
		}

		public void AddCoin(Coin coin)
		{
			_coins.Add(coin);
		}

		public void RemoveCoin(Coin coin)
		{
			_coins.Remove(coin);
		}

		public void DebugReleaseCoins()
		{
			int coinCount = GetCoinCount();
			int activeCoinCount = GetActiveCoinCount();
			ReleaseAllCoins();
			Debug.Log(string.Format("CSM.DebugReleaseCoins() Count (Before:{0}, After:{1}), Active (Before:{2}, After:{3})", coinCount, GetCoinCount(), activeCoinCount, GetActiveCoinCount()));
		}

		private void ReleaseAllCoins()
		{
			foreach (Coin coin in _coins)
			{
				coin.Deactivate();
			}
		}
	}
}
