using System;
using System.Collections;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Monetization.Tapjoy;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TapJoyVideoProvider : VideoAdProvider
{
	private enum TapJoyProviderConnectionState
	{
		Disconnected = 0,
		Connecting = 1,
		Connected = 2
	}

	private const string PROVIDER_TAPJOYVIDEO_PRIORITY = "tapjoy_video_priority";

	private bool _hasHookedUpEvents;

	private bool _isStopped;

	private bool _isVideoAvailable;

	private bool _isCheckAvailabilityCoroutineRunning;

	private bool _videoCompleted;

	private TapJoyProviderConnectionState _providerConnectionState;

	private static readonly int TAPJOYVIDEO_DEFAULT_PRIORITY = 2;

	private static readonly int TAPJOYVIDEO_DEFAULT_WEIGHT;

	private IEnumerator _checkVideoCoroutine;

	private static TapJoyVideoProvider _instance;

	private string _id;

	private int _weight;

	private int _priority;

	private float _cachingStartTime = float.MinValue;

	private float _appMinimizedTime;

	private float _appResumedTime;

	private float _amountOfTimePaused;

	public static TapJoyVideoProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TapJoyVideoProvider();
			}
			return _instance;
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			return TapJoy.IsSupportedOnPlatform() && !string.IsNullOrEmpty(AdConstants.instance.TapJoyAppId) && !string.IsNullOrEmpty(AdConstants.instance.TapJoyAppKey);
		}
	}

	public override bool isInitialized
	{
		get
		{
			return _providerConnectionState == TapJoyProviderConnectionState.Connected;
		}
	}

	public override bool IsStopped
	{
		get
		{
			return _isStopped;
		}
	}

	public override string id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public override int weight
	{
		get
		{
			return _weight;
		}
		set
		{
			_weight = value;
		}
	}

	public override int priority
	{
		get
		{
			return _priority;
		}
		set
		{
			_priority = value;
		}
	}

	private TapJoyVideoProvider()
	{
		VideoAdProvider.SetVideoProviderData("tapjoy_video_priority", TAPJOYVIDEO_DEFAULT_PRIORITY, TAPJOYVIDEO_DEFAULT_WEIGHT, ref _priority, ref _weight);
	}

	public override void Init()
	{
		if (!isInitialized && _providerConnectionState != TapJoyProviderConnectionState.Connecting && isVideoProviderSupported)
		{
			_providerConnectionState = TapJoyProviderConnectionState.Connecting;
			if (!_hasHookedUpEvents)
			{
				TapJoy.onInitializeFinished += InitializeFinished;
				TapJoy.onFullScreenAdResponse += FullScreenAdResponse;
				TapJoy.onViewDidAppear += ViewAppeared;
				TapJoy.onViewDidDisappear += ViewDisappeared;
				TapJoy.onViewWillAppear += ViewWillAppear;
				TapJoy.onViewWillDisappear += ViewWillDisappear;
				TapJoy.onVideoStart += VideoStarted;
				TapJoy.onVideoError += VideoError;
				TapJoy.onVideoComplete += VideoCompleted;
				_hasHookedUpEvents = true;
			}
			if (TapJoy.isInitialized)
			{
				InitializeFinished("success");
			}
			else
			{
				TapJoy.Initialize(AdConstants.instance.TapJoyAppId, AdConstants.instance.TapJoyAppKey, AdConstants.instance.TapJoyEventName);
			}
		}
	}

	public override bool PlayVideoIfAvailable()
	{
		if (isInitialized && _isVideoAvailable)
		{
			TapJoy.ShowFullScreenAd();
			_isVideoAvailable = false;
			return true;
		}
		return false;
	}

	public override void StopProvider()
	{
	}

	public override void ResumeProvider()
	{
	}

	public override void StartCachingVideo()
	{
		if (isInitialized && !_isCheckAvailabilityCoroutineRunning)
		{
			_checkVideoCoroutine = CheckVideoAvailability();
			CoroutineUtil.StartCoroutine(_checkVideoCoroutine);
		}
	}

	private IEnumerator CheckVideoAvailability()
	{
		InitiateCachingTimeLogging();
		_isCheckAvailabilityCoroutineRunning = true;
		_isVideoAvailable = false;
		TapJoy.GetFullScreenAd();
		if (_isVideoAvailable)
		{
			OnVideoCachingSucceded();
			yield break;
		}
		for (int i = 0; i < 10; i++)
		{
			yield return CoroutineUtil.StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));
			if (_isVideoAvailable)
			{
				OnVideoCachingSucceded();
				yield break;
			}
		}
		OnVideoCachingTimedOut();
	}

	private void OnVideoCachingSucceded()
	{
		StopCheckVideoAvailabilityCouroutine();
		CalculateAndReportCachingTime("Success");
		InvokeHandlerIfNotNull(_onVideoAvailable, this);
	}

	private void OnVideoCachingFailed()
	{
		StopCheckVideoAvailabilityCouroutine();
		CalculateAndReportCachingTime("Failed");
		InvokeHandlerIfNotNull(_onVideoUnavailable, this, string.Empty);
	}

	private void OnVideoCachingTimedOut()
	{
		StopCheckVideoAvailabilityCouroutine();
		ResetCachingTimeLogging();
		InvokeHandlerIfNotNull(_onCachingTimedOut, this);
	}

	private void StopCheckVideoAvailabilityCouroutine()
	{
		if (_isCheckAvailabilityCoroutineRunning)
		{
			CoroutineUtil.StopCoroutine(_checkVideoCoroutine);
			_isCheckAvailabilityCoroutineRunning = false;
		}
	}

	public override void OnPause()
	{
		_appMinimizedTime = Time.realtimeSinceStartup;
	}

	public override void OnResume()
	{
		_appResumedTime = Time.realtimeSinceStartup;
		_amountOfTimePaused += _appResumedTime - _appMinimizedTime;
	}

	private void InitializeFinished(string result)
	{
		if (result == "success")
		{
			_providerConnectionState = TapJoyProviderConnectionState.Connected;
			StartCachingVideo();
		}
		else
		{
			_providerConnectionState = TapJoyProviderConnectionState.Disconnected;
			OnVideoCachingFailed();
		}
	}

	private void FullScreenAdResponse(string result)
	{
		if (result == "success")
		{
			OnVideoCachingSucceded();
			_isVideoAvailable = true;
		}
		else
		{
			_isVideoAvailable = false;
			OnVideoCachingFailed();
		}
	}

	private void ViewAppeared(string viewType)
	{
		_videoCompleted = false;
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}

	private void ViewDisappeared(string viewType)
	{
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
		if (_videoCompleted)
		{
			InvokeHandlerIfNotNull(_onVideoWatched, this);
			_videoCompleted = false;
		}
	}

	private void ViewWillAppear(string viewType)
	{
	}

	private void ViewWillDisappear(string viewType)
	{
	}

	private void VideoStarted()
	{
	}

	private void VideoError(string error)
	{
	}

	private void VideoCompleted()
	{
		_isVideoAvailable = false;
		_videoCompleted = true;
	}

	public void CalculateAndReportCachingTime(string argkeyState)
	{
		if (_cachingStartTime > float.MinValue)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			double num = Math.Round((realtimeSinceStartup - (_cachingStartTime + _amountOfTimePaused)) * 2f) / 2.0;
			Flurry.LogEventWithAParameter("VideoAds Caching Time(2)", instance.id + argkeyState + " Time", num.ToString());
		}
		ResetCachingTimeLogging();
	}

	private void InitiateCachingTimeLogging()
	{
		_cachingStartTime = Time.realtimeSinceStartup;
		_amountOfTimePaused = 0f;
	}

	private void ResetCachingTimeLogging()
	{
		_cachingStartTime = float.MinValue;
		_amountOfTimePaused = 0f;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_TAPJOY_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
