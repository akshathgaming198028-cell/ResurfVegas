using System;
using System.Collections;
using UnityEngine;

public class UISlideIn : MonoBehaviour
{
	protected Vector3 posOff = new Vector3(0f, 65f, 0f);

	protected Vector3 posOn = new Vector3(0f, -5f, 0f);

	private bool _triggerSlideOut;

	private float _slideOutTimer = 3f;

	private bool _triggerReadyForNext;

	private float _readyForNextTimer = 1f;

	private Action _onDidShowCallback;

	protected virtual void Start()
	{
		base.transform.localPosition = posOff;
		base.gameObject.SetActive(false);
	}

	public void SetupSlideIn()
	{
		base.gameObject.SetActive(true);
		SlideIn();
	}

	protected virtual void SlideIn(Action onDidShowCallback = null)
	{
		_onDidShowCallback = onDidShowCallback;
		SpringPosition.Begin(base.gameObject, posOn, 10f).ignoreTimeScale = true;
		_slideOutTimer = 3f;
		_readyForNextTimer = 1f;
		StartCoroutine("InvokeDidShowCallback", 0.5f);
		_triggerSlideOut = true;
	}

	private IEnumerator InvokeDidShowCallback(float waitTime)
	{
		float startPoint = 0f;
		while (startPoint < waitTime)
		{
			startPoint += RealTimeTracker.deltaTime;
			yield return null;
		}
		Action handler = _onDidShowCallback;
		if (handler != null)
		{
			handler();
		}
	}

	protected virtual void SlideOut()
	{
		SpringPosition.Begin(base.gameObject, posOff, 10f).ignoreTimeScale = true;
		_triggerReadyForNext = true;
	}

	protected virtual void ReadyForNewMessage()
	{
		base.gameObject.SetActive(false);
		UIScreenController.Instance.ReadyForNextSlide();
	}

	private void Update()
	{
		if (!_triggerReadyForNext && !_triggerSlideOut)
		{
			return;
		}
		float deltaTime = RealTimeTracker.deltaTime;
		if (_triggerSlideOut)
		{
			_slideOutTimer -= deltaTime;
			if (_slideOutTimer <= 0f)
			{
				SlideOut();
				_triggerSlideOut = false;
			}
		}
		if (_triggerReadyForNext)
		{
			_readyForNextTimer -= deltaTime;
			if (_readyForNextTimer <= 0f)
			{
				_triggerReadyForNext = false;
				ReadyForNewMessage();
			}
		}
	}

	public IEnumerator PreloadSlideIn()
	{
		base.gameObject.SetActive(true);
		yield return null;
		yield return null;
		base.gameObject.SetActive(false);
	}
}
