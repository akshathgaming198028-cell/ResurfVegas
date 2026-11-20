using System.Collections.Generic;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using UnityEngine;

public class CoinLine : MonoBehaviour
{
	private const float _TOP_LEVEL_HEIGHT = 70f;

	public float length = 100f;

	public float coinSpacing = 15f;

	private CoinPool coinPool;

	private CoinLineManager coinLineManager;

	private List<Transform> _coins = new List<Transform>();

	public int GetCoinCount()
	{
		if (_coins != null)
		{
			return _coins.Count;
		}
		return 0;
	}

	public int GetActiveCoinCount()
	{
		int num = 0;
		Coin coin = null;
		foreach (Transform coin2 in _coins)
		{
			coin = coin2.GetComponent<Coin>();
			if (coin.IsActive)
			{
				num++;
			}
		}
		return num;
	}

	private void Awake()
	{
		TrackObject component = GetComponent<TrackObject>();
		component.OnActivate += OnActivate;
		component.OnDeactivate += OnDeactivate;
		coinPool = CoinPool.Instance;
		coinLineManager = CoinLineManager.Instance;
	}

	private void OnActivate()
	{
		if (Globals.DEBUG_LOG_COINS)
		{
			int num = 0;
			for (float num2 = 0f; num2 < length; num2 += coinSpacing)
			{
				num++;
			}
			CoinSystemSpawnLog.Old.LogSpawnPoint(base.gameObject, num);
		}
		for (float num3 = 0f; num3 < length; num3 += coinSpacing)
		{
			Transform coin = coinPool.GetCoin();
			coin.parent = base.transform;
			coin.position = base.transform.position + base.transform.forward * num3;
			TrackObject component = coin.GetComponent<TrackObject>();
			if (component != null)
			{
				component.Activate();
			}
			_coins.Add(coin);
		}
		if (base.transform.position.y > 70f)
		{
			coinLineManager.AddLine(this);
			if (Character.Instance.transform.position.y > 70f && !Game.Instance.IsInJetpackMode)
			{
				ToggleCoinVisibility(true);
			}
		}
		CoinStats.Old.NotifySpawnPointActivated(_coins.Count);
	}

	public void ToggleCoinVisibility(bool isVisible)
	{
		foreach (Transform coin in _coins)
		{
			Renderer[] componentsInChildren = coin.gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = isVisible;
			}
		}
	}

	private void OnDeactivate()
	{
		CoinStats.Old.NotifySpawnPointDeactivated(_coins.Count);
		RemoveCoins();
		if (base.transform.position.y > 70f)
		{
			coinLineManager.RemoveLine(this);
		}
	}

	public void RemoveCoins()
	{
		foreach (Transform coin in _coins)
		{
			coin.GetComponent<TrackObject>().Deactivate();
		}
		for (int i = 0; i < _coins.Count; i++)
		{
			_coins[i].GetComponent<Coin>().Deactivate();
		}
		for (int j = 0; j < _coins.Count; j++)
		{
			_coins[j].GetComponent<TrackObject>().Deactivate();
		}
		coinPool.Put(_coins);
		_coins.Clear();
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * length);
		for (float num = 0f; num < length; num += coinSpacing)
		{
			Vector3 center = base.transform.position + base.transform.forward * num;
			Gizmos.DrawSphere(center, 1f);
		}
	}

	public List<Vector3> GetCoinPositions()
	{
		List<Vector3> list = new List<Vector3>();
		for (float num = 0f; num < length; num += coinSpacing)
		{
			Vector3 item = base.transform.position + base.transform.forward * num;
			list.Add(item);
		}
		return list;
	}
}
