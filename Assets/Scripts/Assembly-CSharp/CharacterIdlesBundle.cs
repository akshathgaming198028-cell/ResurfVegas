using System.Collections.Generic;
using UnityEngine;

public class CharacterIdlesBundle : MonoBehaviour
{
	public AnimationClip startClip;

	public List<AnimationClip> Idles;

	private void OnValidate()
	{
		for (int i = 0; i < Idles.Count; i++)
		{
			if (Idles[i] == null)
			{
				Debug.LogError("Null idle animation for " + base.name + " (index " + i + ")");
			}
		}
	}
}
