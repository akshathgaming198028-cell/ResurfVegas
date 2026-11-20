using System.Collections;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using UnityEngine;

public class AvatarAnimations : MonoBehaviour
{
	private SkinnedMeshRenderer _head;

	private Transform _blendshapeController;

	private Animation _target;

	[SerializeField]
	private List<AnimationClip> IdlesPopup;

	public bool Paused;

	public bool PlayIdleAnimations;

	private AnimationClip startClip;

	private List<AnimationClip> Idles;

	private AnimationClip _breath;

	private CharacterAssetsBundle currentCharacterBundle;

	private bool usePopupIdles;

	private bool useStartClip;

	private IEnumerator animationRoutine;

	private float nextAnimationTime;

	private int _blendshapeCount;

	private int MinIdleTimes = 2;

	private int MaxIdleTimes = 3;

	private CharacterIdlesBundle _idlesBundle;

	public SkinnedMeshRenderer Head
	{
		set
		{
			_head = value;
		}
	}

	public Transform BlendshapeController
	{
		set
		{
			_blendshapeController = value;
		}
	}

	public Animation Target
	{
		set
		{
			_target = value;
		}
	}

	public void Initialize(CharacterAssetsBundle characterAssetsBundle)
	{
		_breath = characterAssetsBundle.Breath;
		currentCharacterBundle = characterAssetsBundle;
		if (PlayIdleAnimations)
		{
			if (usePopupIdles)
			{
				StartIdlePopupAnimations();
			}
			else
			{
				StartIdleAnimations();
			}
		}
		if (_head.sharedMesh != null && _head.enabled)
		{
			_blendshapeCount = _head.sharedMesh.blendShapeCount;
		}
		else
		{
			_blendshapeCount = 0;
		}
	}

	private Animation FindAnimationInParent(GameObject current)
	{
		Animation component = current.GetComponent<Animation>();
		if (component != null)
		{
			return component;
		}
		if (current.transform.parent != null)
		{
			return FindAnimationInParent(current.transform.parent.gameObject);
		}
		return null;
	}

	public static float NormalizeBlendShape(float value)
	{
		float num = Mathf.Abs(value);
		if (num < 0.1f)
		{
			return 0f;
		}
		return Mathf.Clamp(num, 0.1f, 1f);
	}

	private void Update()
	{
		if (!PlayIdleAnimations || animationRoutine == null || Paused)
		{
			return;
		}
		animationRoutine.MoveNext();
		if (_blendshapeCount <= 1)
		{
			return;
		}
		_head.SetBlendShapeWeight(1, NormalizeBlendShape(_blendshapeController.localPosition.y) * 100f);
		if (_blendshapeCount <= 2)
		{
			return;
		}
		_head.SetBlendShapeWeight(2, NormalizeBlendShape(_blendshapeController.localPosition.z) * 100f);
		if (_blendshapeCount <= 3)
		{
			return;
		}
		_head.SetBlendShapeWeight(3, NormalizeBlendShape(_blendshapeController.localRotation.x) * 100f);
		if (_blendshapeCount > 4)
		{
			_head.SetBlendShapeWeight(4, NormalizeBlendShape(_blendshapeController.localRotation.y) * 100f);
			if (_blendshapeCount > 5)
			{
				_head.SetBlendShapeWeight(5, NormalizeBlendShape(_blendshapeController.localRotation.z) * 100f);
			}
		}
	}

	private void OnDestroy()
	{
		ClearBundleAnimation();
	}

	private void ClearBundleAnimation()
	{
		if (!(_idlesBundle != null))
		{
			return;
		}
		if (startClip != null && _target.GetClip(startClip.name) != null)
		{
			_target.RemoveClip(startClip.name);
		}
		startClip = null;
		foreach (AnimationClip idle in Idles)
		{
			if (_target.GetClip(idle.name) != null)
			{
				_target.RemoveClip(idle.name);
			}
		}
		Idles = null;
		Object.Destroy(_idlesBundle.gameObject);
	}

	public void StartIdleAnimations()
	{
		ClearBundleAnimation();
		SingletonMonobehavior<Factories>.Instance.CharacterIdlesBundle.RequestItemInstance(currentCharacterBundle.name + "_idles", delegate(CharacterIdlesBundle idlesBundle)
		{
			if (idlesBundle != null)
			{
				_idlesBundle = idlesBundle;
				startClip = _idlesBundle.startClip;
				Idles = _idlesBundle.Idles;
				PlayIdleAnimations = true;
				Paused = false;
				usePopupIdles = false;
				if (startClip != null)
				{
					useStartClip = true;
					_target.AddClip(startClip, startClip.name);
				}
				else
				{
					useStartClip = false;
				}
				foreach (AnimationClip idle in Idles)
				{
					_target.AddClip(idle, idle.name);
				}
			}
			else
			{
				_idlesBundle = null;
				startClip = null;
				Idles = null;
				PlayIdleAnimations = true;
				Paused = false;
				usePopupIdles = false;
				useStartClip = false;
			}
			_target.AddClip(_breath, _breath.name);
			animationRoutine = Play();
			animationRoutine.MoveNext();
		});
	}

	public void StartIdlePopupAnimations()
	{
		PlayIdleAnimations = true;
		Paused = false;
		usePopupIdles = true;
		useStartClip = false;
		_target.AddClip(_breath, _breath.name);
		foreach (AnimationClip item in IdlesPopup)
		{
			_target.AddClip(item, item.name);
		}
		animationRoutine = Play();
		animationRoutine.MoveNext();
	}

	public void StopIdleAnimations()
	{
		animationRoutine = null;
		PlayIdleAnimations = false;
		List<AnimationClip> list = Idles;
		if (usePopupIdles)
		{
			list = IdlesPopup;
		}
		if (list == null)
		{
			return;
		}
		foreach (AnimationClip item in list)
		{
			foreach (AnimationState item2 in _target)
			{
				if (item2.clip == item)
				{
					_target.RemoveClip(item);
				}
			}
		}
	}

	public void PauseIdleAnimations()
	{
		Paused = true;
		foreach (AnimationState item in _target)
		{
			item.speed = 0f;
		}
	}

	public void ResumeIdleAnimations()
	{
		Paused = false;
		foreach (AnimationState item in _target)
		{
			item.speed = 1f;
		}
	}

	private IEnumerator Play()
	{
		AnimationClip selectedClip = null;
		if (useStartClip)
		{
			_target.Play(startClip.name);
			nextAnimationTime = startClip.length;
			while (nextAnimationTime > 0f)
			{
				nextAnimationTime -= Time.deltaTime;
				yield return null;
			}
			useStartClip = false;
		}
		while (PlayIdleAnimations)
		{
			List<AnimationClip> possibleClips = (usePopupIdles ? IdlesPopup.FindAll((AnimationClip a) => a != selectedClip) : ((Idles == null) ? new List<AnimationClip>() : Idles.FindAll((AnimationClip a) => a != selectedClip)));
			if (possibleClips.Count > 0)
			{
				selectedClip = possibleClips[Random.Range(0, possibleClips.Count)];
			}
			if (selectedClip != null)
			{
				_target.Play(selectedClip.name);
				nextAnimationTime = selectedClip.length;
				while (nextAnimationTime > 0f)
				{
					nextAnimationTime -= Time.deltaTime;
					yield return null;
				}
			}
			if (usePopupIdles)
			{
				continue;
			}
			int count = Random.Range(MinIdleTimes, MaxIdleTimes);
			for (int i = 0; i < count; i++)
			{
				if (_target != null)
				{
					_target.Play(_breath.name);
				}
				else
				{
					Debug.LogWarning("AvatarAnimations.Play(). Fix immediately. Null not acceptable for _target: " + _target);
				}
				nextAnimationTime = _breath.length;
				while (nextAnimationTime > 0f)
				{
					nextAnimationTime -= Time.deltaTime;
					yield return 0;
				}
			}
		}
		animationRoutine = null;
	}
}
