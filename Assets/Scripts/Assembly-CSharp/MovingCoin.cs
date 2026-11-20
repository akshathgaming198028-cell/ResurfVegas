using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using UnityEngine;

[RequireComponent(typeof(TrackObject))]
public class MovingCoin : MonoBehaviour
{
	public float speed = 1f;

	private Transform coin;

	private Game game;

	private static CharacterController characterController;

	private static List<MovingCoin> activecoins = new List<MovingCoin>();

	private bool autoPilot;

	public static float autoPilotActivationDistance = 200f;

	public void Awake()
	{
		game = Game.Instance;
		if (!(game == null))
		{
			if (characterController == null)
			{
				characterController = Character.Instance.characterController;
			}
			if (base.transform.childCount == 0)
			{
				Debug.Log("No coin child");
			}
			coin = base.transform.GetChild(0);
			coin.localPosition = -Vector3.up * 200f;
			base.enabled = false;
			TrackObject component = GetComponent<TrackObject>();
			component.OnActivate += OnActivate;
			component.OnDeactivate += OnDeactivate;
		}
	}

	public void OnActivate()
	{
		if (characterController == null)
		{
			characterController = Character.Instance.characterController;
		}
		SingletonMonobehavior<CoinSingleManager>.Instance.AddCoin(GetComponent<Coin>());
		activecoins.Add(this);
		base.enabled = true;
		autoPilot = false;
		coin.localPosition = new Vector3(0f, 0f, (base.transform.position.z - characterController.transform.position.z) * speed);
		CoinStats.Old.NotifySpawnPointActivated(1);
		if (Globals.DEBUG_LOG_COINS)
		{
			CoinSystemSpawnLog.Old.LogSpawnPoint(base.gameObject, 1);
		}
	}

	public void Update()
	{
		if (!(game == null))
		{
			if (autoPilot)
			{
				coin.position -= Vector3.forward * Time.deltaTime * game.currentSpeed * speed;
				return;
			}
			Vector3 position = new Vector3(0f, 0f, (base.transform.position.z - characterController.transform.position.z) * speed);
			Vector3 position2 = base.transform.TransformPoint(position);
			coin.position = position2;
		}
	}

	public void OnDeactivate()
	{
		SingletonMonobehavior<CoinSingleManager>.Instance.RemoveCoin(GetComponent<Coin>());
		activecoins.Remove(this);
		base.enabled = false;
		CoinStats.Old.NotifySpawnPointDeactivated(1);
	}

	public void OnDrawGizmos()
	{
		if (coin != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(coin.position, base.transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 5f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(coin.position, 5f);
		}
	}

	public static void ActivateAutoPilot()
	{
		foreach (MovingCoin activecoin in activecoins)
		{
			if (activecoin.GetComponent<Collider>().transform.position.z - characterController.transform.position.z < autoPilotActivationDistance)
			{
				activecoin.autoPilot = true;
			}
		}
	}
}
