using System.Collections.Generic;
using UnityEngine;

public class PogostickCoinsManager : MonoBehaviour
{
	public GameObject coinPrefab;

	private List<Transform> coins = new List<Transform>();

	private AnimationCurve curve;

	private Track track;

	private CoinPool coinPool;

	public void Awake()
	{
		track = Track.Instance;
		coinPool = CoinPool.Instance;
	}

	public void placeRow(float z, float height)
	{
		MoveCoin(track.GetPosition(0f, z) + Vector3.up * height);
		MoveCoin(track.GetPosition(20f, z) + Vector3.up * height);
		MoveCoin(track.GetPosition(40f, z) + Vector3.up * height);
	}

	private void MoveCoin(Vector3 position)
	{
		Transform coin = coinPool.GetCoin();
		coin.position = position;
		coin.GetComponent<TrackObject>().Activate();
		coins.Add(coin);
	}

	public void ReleaseCoins()
	{
		for (int i = 0; i < coins.Count; i++)
		{
			coins[i].GetComponent<TrackObject>().Deactivate();
		}
		coinPool.Put(coins);
		coins.Clear();
	}
}
