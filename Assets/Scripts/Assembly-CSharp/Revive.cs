using System.Collections;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

public class Revive : MonoBehaviour
{
	public delegate void OnSwitchToRunningDelegate();

	public delegate void OnReviveDelegate();

	private Game game;

	private Character character;

	[OptionalField]
	public AudioStateLoop audioStateLoop;

	public float WaitForParticlesDelay = 0.144f;

	public float RemoveObstaclesDistance = 250f;

	[SerializeField]
	[OptionalField]
	private ParticleSystem reviveParticle;

	[OptionalField]
	[SerializeField]
	private GameObject ReviveFX;

	[SerializeField]
	[OptionalField]
	private Animation ReviveAnimation;

	[SerializeField]
	private AnimationClip ReviveAnimationClip;

	private Track track;

	private static Revive instance;

	public static Revive Instance
	{
		get
		{
			return instance ?? (instance = Utils.FindObject<Revive>());
		}
	}

	public event OnSwitchToRunningDelegate OnSwitchToRunning;

	public event OnReviveDelegate OnRevive;

	private void Awake()
	{
		game = Game.Instance;
		character = Character.Instance;
		track = Track.Instance;
		ReviveFX.SetActive(false);
	}

	public void SendRevive()
	{
		StartCoroutine(ReviveNow());
	}

	public void SendSkipRevive()
	{
		StartCoroutine(game.SkipRevive());
	}

	private IEnumerator ReviveNow()
	{
		reviveParticle.gameObject.SetActive(true);
		reviveParticle.Play();
		audioStateLoop.PlayReviveSound();
		float timeLeft = WaitForParticlesDelay;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		track.LayEmptyChunks(character.z, RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
		if (this.OnRevive != null)
		{
			this.OnRevive();
		}
		game.Revive();
		if (this.OnSwitchToRunning != null)
		{
			this.OnSwitchToRunning();
		}
		ReviveFX.SetActive(true);
		ReviveAnimation.Play();
		float timeLeftFX = ReviveAnimationClip.length;
		while (timeLeftFX > 0f)
		{
			timeLeftFX -= Time.deltaTime;
			yield return null;
		}
		ReviveFX.SetActive(false);
		reviveParticle.gameObject.SetActive(false);
	}
}
