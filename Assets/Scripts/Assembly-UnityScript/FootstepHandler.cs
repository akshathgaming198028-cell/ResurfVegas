using System;
using UnityEngine;

[Serializable]
public class FootstepHandler : MonoBehaviour
{
	public AudioSource audioSource;

	public FootType footType;

	private PhysicMaterial physicMaterial;

	public virtual void OnCollisionEnter(Collision collisionInfo)
	{
		physicMaterial = collisionInfo.collider.sharedMaterial;
	}

	public virtual void OnFootstep()
	{
		if (audioSource.enabled)
		{
			AudioClip clip = null;
			switch (footType)
			{
			}
			audioSource.pitch = UnityEngine.Random.Range(0.98f, 1.02f);
			audioSource.PlayOneShot(clip, UnityEngine.Random.Range(0.8f, 1.2f));
		}
	}

	public virtual void Main()
	{
	}
}
