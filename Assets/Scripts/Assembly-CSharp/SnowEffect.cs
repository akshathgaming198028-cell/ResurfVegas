using System;
using SYBO.Subway;
using UnityEngine;

public class SnowEffect : MonoBehaviour
{
	[SerializeField]
	private ParticleAnimator particleAnimator;

	[SerializeField]
	private ParticleRenderer particleRenderer;

	private Vector3 currentAccel = new Vector3(0f, -1f, 0f);

	private void Start()
	{
		Game instance = Game.Instance;
		instance.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(instance.OnIntroRun, (Game.OnIntroRunDelegate)delegate
		{
			GetComponent<ParticleEmitter>().enabled = false;
			particleRenderer.enabled = false;
			base.enabled = false;
		});
		instance.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(instance.OnStageMenuSequence, (Game.OnStageMenuSequenceDelegate)delegate
		{
			GetComponent<ParticleEmitter>().enabled = true;
			particleRenderer.enabled = true;
			base.enabled = true;
		});
	}

	private void Update()
	{
		Vector3 acceleration = Input.acceleration;
		acceleration.y = -1f;
		currentAccel = Vector3.Slerp(currentAccel, acceleration, Time.deltaTime * 100f);
		particleAnimator.force = currentAccel * 0.3f;
	}
}
