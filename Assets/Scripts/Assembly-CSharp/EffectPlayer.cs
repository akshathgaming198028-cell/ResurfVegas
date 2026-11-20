using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _effectList = new List<GameObject>();

	[SerializeField]
	private float _delay;

	[SerializeField]
	private float _duration;

	[SerializeField]
	private float _maxDuration;

	[SerializeField]
	private float _minimumRotation;

	[SerializeField]
	private float _maximumRotation;

	[SerializeField]
	private bool _doLoop;

	private int _loopFrame = -1;

	[SerializeField]
	private bool _doRandomizeStartingIndex;

	private int _startingIndex = 1;

	private float _selectedDuration;

	public int LoopFrame
	{
		set
		{
			_loopFrame = value;
		}
	}

	public float Duration
	{
		get
		{
			return _selectedDuration;
		}
	}

	private void OnEnable()
	{
		if (_doRandomizeStartingIndex)
		{
			_startingIndex = Random.Range(0, _effectList.Count);
		}
		if (_maxDuration > _duration)
		{
			_selectedDuration = Random.Range(_duration, _maxDuration);
		}
		else
		{
			_selectedDuration = _duration;
		}
		Reset();
		StartCoroutine(DoPlayEffectDelayed());
	}

	private void Reset()
	{
		foreach (GameObject effect in _effectList)
		{
			effect.SetActive(false);
		}
	}

	private IEnumerator DoPlayEffectDelayed()
	{
		yield return new WaitForSeconds(_delay);
		StartCoroutine(DoPlayEffect());
	}

	private IEnumerator DoPlayEffect()
	{
		while (_loopFrame >= 0)
		{
			_effectList[_loopFrame].SetActive(true);
			yield return 0;
		}
		for (int i = _startingIndex; i < _effectList.Count; i++)
		{
			_effectList[i].SetActive(true);
			if (i > 0)
			{
				_effectList[i - 1].SetActive(false);
			}
			else if (i == 0)
			{
				_effectList[_effectList.Count - 1].SetActive(false);
			}
			yield return new WaitForSeconds(_selectedDuration / (float)_effectList.Count);
		}
		foreach (GameObject go in _effectList)
		{
			go.SetActive(false);
		}
		_startingIndex = 0;
		if (_doLoop)
		{
			StartCoroutine(DoPlayEffect());
		}
	}
}
