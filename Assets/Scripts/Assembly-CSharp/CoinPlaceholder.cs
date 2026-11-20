using SYBO.Core.Components.Singleton;
using SYBO.Subway.Coins;
using UnityEngine;

public class CoinPlaceholder : MonoBehaviour
{
	private CoinPool coinPool;

	private Transform coin;

	private void Awake()
	{
		coinPool = CoinPool.Instance;
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		trackObject.OnActivate += OnActivate;
		trackObject.OnDeactivate += OnDeactivate;
	}

	public void OnActivate()
	{
		if (Globals.DEBUG_LOG_COINS)
		{
			CoinSystemSpawnLog.Old.LogSpawnPoint(base.gameObject, 1);
		}
		coin = coinPool.GetCoin();
		SingletonMonobehavior<CoinSingleManager>.Instance.AddCoin(coin.GetComponent<Coin>());
		coin.parent = base.transform;
		coin.position = base.transform.position;
		TrackObject component = coin.GetComponent<TrackObject>();
		component.Activate();
		CoinStats.Old.NotifySpawnPointActivated(1);
	}

	public void OnDeactivate()
	{
		if (coin != null)
		{
			TrackObject component = coin.GetComponent<TrackObject>();
			component.Deactivate();
			SingletonMonobehavior<CoinSingleManager>.Instance.RemoveCoin(coin.GetComponent<Coin>());
			coinPool.Put(coin);
			coin = null;
		}
		CoinStats.Old.NotifySpawnPointDeactivated(1);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
