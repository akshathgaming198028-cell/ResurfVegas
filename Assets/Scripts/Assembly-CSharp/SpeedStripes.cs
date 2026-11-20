using System;
using SYBO.Subway;
using SYBO.Subway.Characters.States;
using UnityEngine;

public class SpeedStripes : MonoBehaviour
{
	[SerializeField]
	private float _fadeInTime = 0.5f;

	private Color _originalColor;

	[SerializeField]
	private Renderer[] _renderes;

	[SerializeField]
	private Material _OriginalMaterial;

	private Material _material;

	private Animator _animator;

	private readonly int id_Active = Animator.StringToHash("Active");

	private void Awake()
	{
		JetpackState.Instance.OnDeactivateTurboHeadstart += OnStopTuboHeadstart;
		JetpackState instance = JetpackState.Instance;
		instance.OnStart = (JetpackState.OnStartDelegate)Delegate.Combine(instance.OnStart, new JetpackState.OnStartDelegate(OnStartJetpack));
		JetpackState instance2 = JetpackState.Instance;
		instance2.OnStop = (JetpackState.OnStopDelegate)Delegate.Combine(instance2.OnStop, new JetpackState.OnStopDelegate(OnStopJetpack));
		Hoverboard.Instance.OnSpeedStart += Activate;
		Hoverboard.Instance.OnSpeedEnd += Deactivate;
		PogoStickState instance3 = PogoStickState.Instance;
		instance3.OnStart = (PogoStickState.OnStartDelegate)Delegate.Combine(instance3.OnStart, new PogoStickState.OnStartDelegate(OnPogoStart));
		PogoStickState instance4 = PogoStickState.Instance;
		instance4.OnStop = (PogoStickState.OnStopDelegate)Delegate.Combine(instance4.OnStop, new PogoStickState.OnStopDelegate(OnPogoStop));
		Game instance5 = Game.Instance;
		instance5.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(instance5.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(Reset));
		_material = new Material(_OriginalMaterial);
		_originalColor = _material.GetColor("_MainColor");
		_material.SetColor("_MainColor", Color.black);
		_animator = GetComponent<Animator>();
		Renderer[] renderes = _renderes;
		foreach (Renderer renderer in renderes)
		{
			renderer.material = _material;
			renderer.enabled = false;
		}
	}

	private void OnPogoStart()
	{
		if ((Hoverboard.Instance.isActive && Hoverboard.Instance.IsHighSpeedBoard) || (Hoverboard.Instance.isActive && Hoverboard.Instance.IsSpeedBoard))
		{
			Deactivate(0.1f);
		}
	}

	private void OnPogoStop()
	{
		if ((Hoverboard.Instance.isActive && Hoverboard.Instance.IsHighSpeedBoard) || (Hoverboard.Instance.isActive && Hoverboard.Instance.IsSpeedBoard))
		{
			Activate(0.1f);
		}
	}

	private void Activate()
	{
		Activate(_fadeInTime);
	}

	private void Activate(float time)
	{
		Renderer[] renderes = _renderes;
		foreach (Renderer renderer in renderes)
		{
			renderer.enabled = true;
		}
		_animator.SetBool(id_Active, true);
		StartCoroutine(pTween.To(time, delegate(float t)
		{
			_material.SetColor("_MainColor", Color.Lerp(Color.black, _originalColor, t));
		}));
	}

	private void OnStartJetpack(bool isHeadstart)
	{
		if (!isHeadstart)
		{
			Deactivate(_fadeInTime);
		}
		else
		{
			Activate(_fadeInTime);
		}
	}

	private void OnStopJetpack()
	{
		if ((Hoverboard.Instance.isActive && Hoverboard.Instance.IsHighSpeedBoard) || (Hoverboard.Instance.isActive && Hoverboard.Instance.IsSpeedBoard))
		{
			Activate(_fadeInTime);
		}
		else
		{
			Deactivate(_fadeInTime);
		}
	}

	private void OnStopTuboHeadstart()
	{
		if (!Hoverboard.Instance.isActive || !Hoverboard.Instance.IsHighSpeedBoard)
		{
			Deactivate(_fadeInTime);
		}
	}

	private void Deactivate()
	{
		Deactivate(_fadeInTime);
	}

	private void Deactivate(float time)
	{
		_animator.SetBool(id_Active, false);
		StartCoroutine(pTween.To(time, delegate(float t)
		{
			_material.SetColor("_MainColor", Color.Lerp(_originalColor, Color.black, t));
			if (t == 1f)
			{
				Renderer[] renderes = _renderes;
				foreach (Renderer renderer in renderes)
				{
					renderer.enabled = false;
				}
			}
		}));
	}

	private void Reset()
	{
		_animator.SetBool(id_Active, false);
		StopAllCoroutines();
		_material.SetColor("_MainColor", Color.black);
		Renderer[] renderes = _renderes;
		foreach (Renderer renderer in renderes)
		{
			renderer.enabled = false;
		}
	}
}
