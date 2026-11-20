using System;
using UnityEngine;

[Serializable]
public class PlaySound : MonoBehaviour
{
	public AudioSource audioSource;

	public AudioClip sound;

	public virtual void Awake()
	{
		if (!audioSource && (bool)GetComponent<AudioSource>())
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	public virtual void OnSignal()
	{
		if ((bool)sound)
		{
			audioSource.clip = sound;
		}
		audioSource.Play();
	}

	public virtual void Main()
	{
	}
}
