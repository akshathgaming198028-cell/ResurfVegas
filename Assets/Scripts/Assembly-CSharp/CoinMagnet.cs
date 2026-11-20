using System.Collections;
using SYBO.Core;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using UnityEngine;

public class CoinMagnet : CharacterModifier
{
	private bool isActive;

	public float pullSpeed = 150f;

	private CharacterController characterController;

	private Character character;

	private CollisionNotifier coinMagnetCollider;

	private CharacterRendering characterRendering;

	private Transform coinEFX;

	private Game game;

	public AudioStateLoop audioStateLoop;

	public AudioClipInfo powerDownSound;

	public ActivePowerup Powerup;

	private static CoinMagnet instance;

	public bool IsActive
	{
		get
		{
			return isActive;
		}
	}

	public static CoinMagnet Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(CoinMagnet)) as CoinMagnet);
		}
	}

	public void Awake()
	{
		instance = this;
	}

	public void Initialize()
	{
		character = Character.Instance;
		characterController = character.characterController;
		coinEFX = character.CharacterPickupParticleSystem.CoinEFX.transform;
		characterRendering = CharacterRendering.Instance;
		coinMagnetCollider = character.coinMagnetCollider;
		coinMagnetCollider.TriggerEnter += CoinHit;
		SetUpHoldAnimation();
		game = Game.Instance;
		game.OnCharacterModelRecreated += Game_OnCharacterModelRecreated;
	}

	private void SetUpHoldAnimation()
	{
		characterRendering.CharacterAnimation["hold_magnet"].AddMixingTransform(characterRendering.CharacterModel.ShoulderTransform);
		characterRendering.CharacterAnimation["hold_magnet"].layer = 3;
		characterRendering.CharacterAnimation["hold_magnet"].weight = 0.9f;
		characterRendering.CharacterAnimation["hold_magnet"].enabled = false;
	}

	private void OnDestroy()
	{
		if (coinMagnetCollider != null)
		{
			coinMagnetCollider.TriggerEnter -= CoinHit;
		}
	}

	private void Game_OnCharacterModelRecreated()
	{
		SetUpHoldAnimation();
	}

	public override void Reset()
	{
		base.Paused = false;
	}

	public override IEnumerator Begin()
	{
		if (character == null)
		{
			Initialize();
		}
		MeshRenderer coinMesh = characterRendering.CharacterModel.MeshCoinMagnet;
		coinMesh.enabled = true;
		GameStats.Instance.pickedUpPowerups++;
		base.Paused = false;
		audioStateLoop.ChangeLoop(AudioState.Magnet);
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		characterRendering.CharacterAnimation["hold_magnet"].enabled = true;
		characterRendering.CharacterAnimation.Play("hold_magnet");
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.coinmagnet);
		coinMagnetCollider.GetComponent<Collider>().enabled = true;
		isActive = true;
		base.enabled = true;
		stop = StopSignal.DONT_STOP;
		while (Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			coinEFX.position = characterRendering.CharacterModel.MeshCoinMagnet.transform.position;
			yield return null;
		}
		coinMagnetCollider.GetComponent<Collider>().enabled = false;
		isActive = false;
		base.enabled = false;
		coinMesh.enabled = false;
		coinEFX.localPosition = CharacterPickupParticles.coinEfxOffset;
		characterRendering.CharacterAnimation["hold_magnet"].enabled = false;
		audioStateLoop.ChangeLoop(AudioState.MagnetStop);
		if (Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public void Update()
	{
	}

	public void CoinHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		Glow componentInChildren = collider.GetComponentInChildren<Glow>();
		if (component != null)
		{
			float num = 70f;
			if ((character.transform.position.y >= num && component.transform.position.y >= num) || (character.transform.position.y < num && component.transform.position.y < num))
			{
				component.GetComponent<Collider>().enabled = false;
				StartCoroutine(Pull(component, componentInChildren));
			}
			return;
		}
		Coin2 component2 = collider.GetComponent<Coin2>();
		if (component2 != null)
		{
			bool flag = character.transform.position.y >= 70f;
			bool flag2 = component2.transform.position.y >= 70f;
			if (component2 != null && flag == flag2)
			{
				component2.MoveToMagnet(pullSpeed);
			}
		}
	}

	private IEnumerator Pull(Coin coin, Glow glow)
	{
		Transform pivot = coin.PivotTransform;
		Vector3 position = pivot.position;
		float distance = (position - characterController.transform.position).magnitude;
		if (glow != null)
		{
			Transform glowPivot = glow.transform;
			Vector3 glowPosition = glowPivot.position;
			yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
			{
				pivot.position = Vector3.Lerp(position, characterRendering.CharacterModel.MeshCoinMagnet.transform.position, t * t);
				glowPivot.position = Vector3.Lerp(glowPosition, characterRendering.CharacterModel.MeshCoinMagnet.transform.position, t * t);
			}));
		}
		else
		{
			yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
			{
				pivot.position = Vector3.Lerp(position, characterRendering.CharacterModel.MeshCoinMagnet.transform.position, t * t);
			}));
		}
		Pickup pickup = coin.GetComponent<Pickup>();
		character.NotifyPickup(pickup, null);
		GameStats.Instance.coinsCoinMagnet++;
	}
}
