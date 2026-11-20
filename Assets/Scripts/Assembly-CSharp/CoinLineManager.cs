using System.Collections.Generic;
using UnityEngine;

public class CoinLineManager : MonoBehaviour
{
	private List<CoinLine> topLevelPlaced = new List<CoinLine>();

	private static CoinLineManager instance;

	public static CoinLineManager Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(CoinLineManager)) as CoinLineManager);
		}
	}

	public int GetCoinCount()
	{
		int num = 0;
		foreach (CoinLine item in topLevelPlaced)
		{
			num += item.GetCoinCount();
		}
		return num;
	}

	public int GetActiveCoinCount()
	{
		int num = 0;
		foreach (CoinLine item in topLevelPlaced)
		{
			num += item.GetActiveCoinCount();
		}
		return num;
	}

	public void Reset()
	{
		topLevelPlaced.Clear();
	}

	public void AddLine(CoinLine line)
	{
		line.ToggleCoinVisibility(false);
		topLevelPlaced.Add(line);
	}

	public void ToggleLines(bool active)
	{
		foreach (CoinLine item in topLevelPlaced)
		{
			item.ToggleCoinVisibility(active);
		}
	}

	public void RemoveLine(CoinLine line)
	{
		line.ToggleCoinVisibility(true);
		topLevelPlaced.Remove(line);
	}

	public void ClearTopLevelLines()
	{
		foreach (CoinLine item in topLevelPlaced)
		{
			item.ToggleCoinVisibility(true);
			item.RemoveCoins();
		}
		Reset();
	}

	public void DebugReleaseCoins()
	{
		int coinCount = GetCoinCount();
		int activeCoinCount = GetActiveCoinCount();
		ClearTopLevelLines();
		Debug.Log(string.Format("CLM.DebugReleaseCoins() Count (Before:{0}, After:{1}), Active (Before:{2}, After:{3})", coinCount, GetCoinCount(), activeCoinCount, GetActiveCoinCount()));
	}
}
