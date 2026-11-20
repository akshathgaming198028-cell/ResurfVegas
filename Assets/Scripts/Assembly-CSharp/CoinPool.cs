using System.Collections;
using System.Collections.Generic;
using SYBO.Subway.Pickups;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
	private static CoinPool _instance;

	[SerializeField]
	private GameObject coinPrefab;

	[SerializeField]
	private int numDeletedPerCleanup = 4;

	[SerializeField]
	private int numPrepool = 200;

	[SerializeField]
	private float cleanupIntervalInSeconds = 5f;

	private Vector3 _spawnPoint = -1000f * Vector3.up;

	private Vector3 _spawnSpacing = -20f * Vector3.right;

	public List<Transform> _coinsInPool = new List<Transform>();

	private int _numberOfActiveCoins;

	private int _numberOfActiveCoins_high;

	private List<PickupRotate> _activeRotatePickups = new List<PickupRotate>();

	private Transform _transform;

	private WaitForSeconds _cleanupIntervalWaitPeriod;

	public List<PickupRotate> ActiveRotatingPickups
	{
		get
		{
			return _activeRotatePickups;
		}
	}

	public static CoinPool Instance
	{
		get
		{
			return _instance ?? (_instance = Object.FindObjectOfType(typeof(CoinPool)) as CoinPool);
		}
	}

	public static bool IsInstantiated()
	{
		return _instance != null;
	}

	private void Awake()
	{
		_instance = this;
		_transform = GetComponent<Transform>();
		_cleanupIntervalWaitPeriod = new WaitForSeconds(cleanupIntervalInSeconds);
		for (int i = 0; i < numPrepool; i++)
		{
			MakeNewCoinInPool(i);
		}
	}

	private void Update()
	{
		for (int i = 0; i < _activeRotatePickups.Count; i++)
		{
			if (_activeRotatePickups[i].WillRotate)
			{
				_activeRotatePickups[i].PhasedRotate();
			}
		}
	}

	private IEnumerator CleanUpCoins()
	{
		while (base.enabled)
		{
			int numDeletes = ((numDeletedPerCleanup >= _coinsInPool.Count) ? _coinsInPool.Count : numDeletedPerCleanup);
			int lastIndex = Mathf.Max(0, _coinsInPool.Count - (numDeletes + 1));
			for (int i = _coinsInPool.Count - 1; i >= lastIndex; i--)
			{
				Object.Destroy(_coinsInPool[i].gameObject);
				_coinsInPool.RemoveAt(i);
			}
			yield return _cleanupIntervalWaitPeriod;
		}
	}

	private Transform MakeNewCoinInPool(int coinIndex)
	{
		Transform transform = MakeNewCoin(coinIndex);
		_coinsInPool.Add(transform);
		return transform;
	}

	private Transform MakeNewCoin(int coinIndex)
	{
		Vector3 position = _spawnPoint + _spawnSpacing * coinIndex;
		GameObject gameObject = Object.Instantiate(coinPrefab, position, Quaternion.identity) as GameObject;
		Transform transform = gameObject.transform;
		transform.parent = _transform;
		return transform;
	}

	public Transform GetCoin()
	{
		Transform transform;
		if (0 < _coinsInPool.Count)
		{
			int index = _coinsInPool.Count - 1;
			transform = _coinsInPool[index];
			_coinsInPool.RemoveAt(index);
		}
		else
		{
			transform = MakeNewCoin(_coinsInPool.Count);
		}
		GameObject gameObject = transform.gameObject;
		if (!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
		}
		_numberOfActiveCoins++;
		_numberOfActiveCoins_high = Mathf.Max(_numberOfActiveCoins_high, _numberOfActiveCoins);
		if (Globals.DEBUG_PRINT_SPAWNED_POWER_UPS)
		{
			SpawnedObjects.countUP("coin");
		}
		return transform;
	}

	public void Put(Transform coin)
	{
		coin.parent = _transform;
		Vector3 position = coin.position;
		position.y = -1000f;
		coin.position = position;
		_coinsInPool.Add(coin);
		_numberOfActiveCoins--;
	}

	public void Put(IList<Transform> coins)
	{
		for (int i = 0; i < coins.Count; i++)
		{
			Put(coins[i]);
		}
	}

	public void DebugReleaseCoins()
	{
		Debug.Log("1 ReleaseCoins() count=" + _coinsInPool.Count);
		for (int num = _coinsInPool.Count - 1; num >= 0; num--)
		{
			_coinsInPool[num].GetComponent<Coin>().Deactivate();
			Debug.Log("\tDeactivate" + _coinsInPool[num].GetComponent<Coin>().CanPickup);
		}
	}
}
