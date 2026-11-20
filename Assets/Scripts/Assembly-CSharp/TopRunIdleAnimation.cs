using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopRunIdleAnimation : MonoBehaviour
{
	[Serializable]
	public class AnimationElement
	{
		public AnimationClip clip;

		public int propability = 1;
	}

	public delegate void AnimationFinishDelegate();

	private Animation Target;

	public bool PlayIdleAnimations;

	public bool paused;

	private IEnumerator animationRoutine;

	private float nextAnimationTime;

	public AnimationClip topRunStart;

	public List<AnimationClip> topRunIdles;

	public List<AnimationClip> topRunHandMiss;

	public List<AnimationElement> topRunHandHit;

	private List<AnimationClip> topRunHandHitWeighted = new List<AnimationClip>();

	public event AnimationFinishDelegate AnimationFinished;

	private void Awake()
	{
		Target = FindAnimationInParent(base.gameObject);
		if (Target == null)
		{
			Debug.LogError("AvatarAnimations: No animation component for avatar animations", this);
		}
	}

	private Animation FindAnimationInParent(GameObject current)
	{
		Animation componentInChildren = current.GetComponentInChildren<Animation>();
		if (componentInChildren != null)
		{
			return componentInChildren;
		}
		if (current.transform.parent != null)
		{
			return FindAnimationInParent(current.transform.parent.gameObject);
		}
		return null;
	}

	private void Update()
	{
		if (PlayIdleAnimations && animationRoutine != null && !paused)
		{
			animationRoutine.MoveNext();
		}
	}

	public void TR_ActivateIdle()
	{
		PlayIdleAnimations = true;
		Target.AddClip(topRunStart, topRunStart.name);
		foreach (AnimationClip topRunIdle in topRunIdles)
		{
			Target.AddClip(topRunIdle, topRunIdle.name);
		}
		animationRoutine = PlayTRIdles();
		animationRoutine.MoveNext();
	}

	public void TR_PlayMiss()
	{
		foreach (AnimationClip item in topRunHandMiss)
		{
			Target.AddClip(item, item.name);
		}
		animationRoutine = PlayTRmiss();
		animationRoutine.MoveNext();
	}

	public void TR_PlayHit()
	{
		foreach (AnimationElement item in topRunHandHit)
		{
			for (int i = 0; i < item.propability; i++)
			{
				topRunHandHitWeighted.Add(item.clip);
			}
		}
		foreach (AnimationClip item2 in topRunHandHitWeighted)
		{
			Target.AddClip(item2, item2.name);
		}
		animationRoutine = PlayTRhit();
		animationRoutine.MoveNext();
	}

	private IEnumerator PlayTRIdles()
	{
		AnimationClip selectedClip = null;
		Target.Play(topRunStart.name);
		nextAnimationTime = topRunStart.length;
		while (nextAnimationTime > 0f)
		{
			nextAnimationTime -= Time.deltaTime;
			yield return null;
		}
		while (PlayIdleAnimations)
		{
			List<AnimationClip> possibleClips = topRunIdles.FindAll((AnimationClip a) => a != selectedClip);
			if (possibleClips.Count > 0)
			{
				selectedClip = possibleClips[UnityEngine.Random.Range(0, possibleClips.Count)];
			}
			Target.Play(selectedClip.name);
			nextAnimationTime = selectedClip.length;
			while (nextAnimationTime > 0f)
			{
				nextAnimationTime -= Time.deltaTime;
				yield return null;
			}
		}
	}

	private IEnumerator PlayTRmiss()
	{
		AnimationClip selectedClip = null;
		selectedClip = topRunHandMiss[UnityEngine.Random.Range(0, topRunHandMiss.Count)];
		Target.CrossFade(selectedClip.name, 0.1f);
		nextAnimationTime = selectedClip.length;
		while (nextAnimationTime > 0.25f)
		{
			nextAnimationTime -= Time.deltaTime;
			yield return null;
		}
		PlayIdleAnimations = false;
		if (this.AnimationFinished != null)
		{
			this.AnimationFinished();
		}
	}

	private IEnumerator PlayTRhit()
	{
		AnimationClip selectedClip = null;
		selectedClip = topRunHandHitWeighted[UnityEngine.Random.Range(0, topRunHandHitWeighted.Count)];
		Target.CrossFade(selectedClip.name, 0.1f);
		nextAnimationTime = selectedClip.length;
		while (nextAnimationTime > 0.25f)
		{
			nextAnimationTime -= Time.deltaTime;
			yield return null;
		}
		PlayIdleAnimations = false;
		if (this.AnimationFinished != null)
		{
			this.AnimationFinished();
		}
	}
}
