using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Characters.States;
using SYBO.Subway.Coins;
using UnityEngine;

public class CoinJumpCurve : MonoBehaviour
{
	public float speed = 100f;

	public float curveOffset;

	public float coinSpacing = 15f;

	public float beginRatio;

	public float endRatio = 1f;

	public bool superSneakers;

	private Game game;

	private Character character;

	private RunningState running;

	private static CoinPool coinPool;

	private int previewSteps = 10;

	private List<Transform> coins = new List<Transform>();

	private bool Initialiseret;

	private int activation;

	private float JumpHeight
	{
		get
		{
			return (!superSneakers) ? character.jumpHeightNormal : character.jumpHeightSuperSneakers;
		}
	}

	public void Awake()
	{
		game = Game.Instance;
		running = RunningState.Instance;
		character = Character.Instance;
		if (coinPool == null)
		{
			coinPool = CoinPool.Instance;
		}
		TrackObject component = GetComponent<TrackObject>();
		component.OnActivate += OnActivate;
		component.OnDeactivate += OnDeactivate;
	}

	public void OnActivate()
	{
		if (activation == 1)
		{
			Debug.Log("CoinJumpCurve has been activate twice. " + Utils.GetLongName(base.transform));
			Debug.Break();
		}
		activation++;
		float num = character.JumpLength(game.currentLevelSpeed, JumpHeight);
		if (Globals.DEBUG_LOG_COINS)
		{
			int num2 = 0;
			for (float num3 = beginRatio * num; num3 < endRatio * num; num3 += coinSpacing)
			{
				num2++;
			}
			CoinSystemSpawnLog.Old.LogSpawnPoint(base.gameObject, num2);
		}
		for (float num4 = beginRatio * num; num4 < endRatio * num; num4 += coinSpacing)
		{
			Transform coin = coinPool.GetCoin();
			coin.parent = base.transform;
			coin.position = CalcJumpCurve(num4 / num);
			TrackObject component = coin.GetComponent<TrackObject>();
			component.Activate();
			SingletonMonobehavior<CoinJumpCurveManager>.Instance.AddCoin(coin.GetComponent<Coin>());
			coins.Add(coin);
		}
		CoinStats.Old.NotifySpawnPointActivated(coins.Count);
		game.OnSpeedChanged += PositionCoins;
	}

	public void OnDeactivate()
	{
		game.OnSpeedChanged -= PositionCoins;
		foreach (Transform coin in coins)
		{
			coin.GetComponent<TrackObject>().Deactivate();
			SingletonMonobehavior<CoinJumpCurveManager>.Instance.RemoveCoin(coin.GetComponent<Coin>());
		}
		activation--;
		CoinStats.Old.NotifySpawnPointDeactivated(coins.Count);
		coinPool.Put(coins);
		coins.Clear();
	}

	private void PositionCoins(float forSpeed)
	{
		for (int i = 0; i < coins.Count; i++)
		{
			float ratio = (float)i / (float)(coins.Count - 1);
			coins[i].position = CalcJumpCurve(ratio, forSpeed);
		}
	}

	private float NormalizedJumpCurve(float z)
	{
		return 4f * z * (1f - z);
	}

	private float InvertedSpeed(float z)
	{
		return NormalizedJumpCurve(z) / Mathf.Sqrt(1f + Mathf.Pow(-8f * z + 4f, 2f));
	}

	private Vector3 CalcJumpCurve(float ratio)
	{
		return CalcJumpCurve(ratio, game.currentLevelSpeed);
	}

	private Vector3 CalcJumpCurve(float ratio, float speed)
	{
		float num = character.JumpLength(speed, JumpHeight);
		return base.transform.position + base.transform.forward * num * (ratio - curveOffset) + base.transform.up * NormalizedJumpCurve(ratio) * JumpHeight;
	}

	public void OnDrawGizmos()
	{
		if (game == null)
		{
			game = Game.Instance;
		}
		if (character == null)
		{
			character = Character.Instance;
		}
		if (running == null)
		{
			running = RunningState.Instance;
		}
		if (!(game == null))
		{
			float num = character.JumpLength(110f, JumpHeight);
			Gizmos.color = Color.yellow;
			for (float num2 = beginRatio * num; num2 < endRatio * num; num2 += coinSpacing)
			{
				Vector3 center = CalcJumpCurve(num2 / num, 110f);
				Gizmos.DrawSphere(center, 1f);
			}
			DrawCurve(game.speed.min, Color.grey);
			DrawCurve(game.speed.max, Color.grey);
		}
	}

	private void DrawCurve(float speed, Color color)
	{
		Gizmos.color = color;
		Vector3 vector = CalcJumpCurve(beginRatio, speed);
		for (int i = 0; i < previewSteps; i++)
		{
			Vector3 vector2 = CalcJumpCurve((endRatio - beginRatio) * (float)i / (float)(previewSteps - 1) + beginRatio, speed);
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
		}
	}

	public List<Vector3> GetCoinPositions()
	{
		List<Vector3> list = new List<Vector3>();
		character = Character.Instance;
		float jumpHeightNormal = Character.Instance.jumpHeightNormal;
		float num = Character.Instance.JumpLength(110f, jumpHeightNormal);
		Gizmos.color = Color.yellow;
		for (float num2 = beginRatio * num; num2 < endRatio * num; num2 += coinSpacing)
		{
			Vector3 item = CalcJumpCurve(num2 / num, 110f);
			list.Add(item);
		}
		return list;
	}
}
