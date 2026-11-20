using System;
using UnityEngine;

[Serializable]
public class AI : MonoBehaviour
{
	public MonoBehaviour behaviourOnSpotted;

	public AudioClip soundOnSpotted;

	public MonoBehaviour behaviourOnLostTrack;

	private Transform character;

	private Transform player;

	private bool insideInterestArea;

	public AI()
	{
		insideInterestArea = true;
	}

	public virtual void Awake()
	{
		character = transform;
		player = GameObject.FindWithTag("Player").transform;
	}

	public virtual void OnEnable()
	{
		behaviourOnLostTrack.enabled = true;
		behaviourOnSpotted.enabled = false;
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if (other.transform == player && CanSeePlayer())
		{
			OnSpotted();
		}
	}

	public virtual void OnEnterInterestArea()
	{
		insideInterestArea = true;
	}

	public virtual void OnExitInterestArea()
	{
		insideInterestArea = false;
		OnLostTrack();
	}

	public virtual void OnSpotted()
	{
		if (insideInterestArea && !behaviourOnSpotted.enabled)
		{
			behaviourOnSpotted.enabled = true;
			behaviourOnLostTrack.enabled = false;
			if ((bool)GetComponent<AudioSource>() && (bool)soundOnSpotted)
			{
				GetComponent<AudioSource>().clip = soundOnSpotted;
				GetComponent<AudioSource>().Play();
			}
		}
	}

	public virtual void OnLostTrack()
	{
		if (!behaviourOnLostTrack.enabled)
		{
			behaviourOnLostTrack.enabled = true;
			behaviourOnSpotted.enabled = false;
		}
	}

	public virtual bool CanSeePlayer()
	{
		Vector3 direction = player.position - character.position;
		RaycastHit hitInfo = default(RaycastHit);
		Physics.Raycast(character.position, direction, out hitInfo, direction.magnitude);
		return ((bool)hitInfo.collider && hitInfo.collider.transform == player) ? true : false;
	}

	public virtual void Main()
	{
	}
}
