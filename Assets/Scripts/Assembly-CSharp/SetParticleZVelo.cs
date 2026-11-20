using SYBO.Subway;
using UnityEngine;

public class SetParticleZVelo : MonoBehaviour
{
	[SerializeField]
	private float _speedMultiplier = 1f;

	private ParticleSystem _particleSystem;

	private Game _game;

	private void Awake()
	{
		_particleSystem = base.gameObject.GetComponent<ParticleSystem>();
		_game = Game.Instance;
	}

	private void Update()
	{
		base.transform.rotation = Quaternion.identity;
		_particleSystem.startSpeed = _game.currentSpeed * _speedMultiplier;
	}
}
