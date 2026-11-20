#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Monetization.Flurry;
using UnityEngine;

public class FlurryVideoAdsProvider : VideoAdProvider
{
	private const string PROVIDER_FLURRYVIDEO_PRIORITY = "flurry_video_priority";

	private bool _initialized;

	private bool _isStopped;

	private bool _isVideoAvailable;

	private bool _isCheckAvailabilityCoroutineRunning;

	private IEnumerator _checkVideoCoroutine;

	private static readonly int FLURRYVIDEO_DEFAULT_PRIORITY = 4;

	private static readonly int FLURRYVIDEO_DEFAULT_WEIGHT = 10;

	private static FlurryVideoAdsProvider _instance;

	private string _id;

	private int _priority;

	private int _weight;

	private float _cachingStartTime = float.MinValue;

	private float _appMinimizedTime;

	private float _appResumedTime;

	private float _amountOfTimePaused;

	public static FlurryVideoAdsProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FlurryVideoAdsProvider();
			}
			return _instance;
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			return FlurryVideoAds.IsSupportedOnPlatform() && !string.IsNullOrEmpty(AdConstants.instance.FlurryAdSpace);
		}
	}

	public override bool isInitialized
	{
		get
		{
			return _initialized;
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

	private FlurryVideoAdsProvider()
	{
		VideoAdProvider.SetVideoProviderData("flurry_video_priority", FLURRYVIDEO_DEFAULT_PRIORITY, FLURRYVIDEO_DEFAULT_WEIGHT, ref _priority, ref _weight);
	}

	public override void Init()
	{
		if (!_initialized)
		{
			FlurryVideoAds.onTakeoverWillDisplay += OnTakeoverWillDisplay;
			FlurryVideoAds.onTakeoverWillClose += OnTakeoverWillClose;
			FlurryVideoAds.onVideoDidFinish += OnVideoDidFinish;
			FlurryVideoAds.onVideoDidNotFinish += OnVideoDidNotFinish;
			FlurryVideoAds.onVideoAvailable += OnVideoAvailable;
			FlurryVideoAds.onVideoUnavailable += OnVideoUnavailable;
			FlurryVideoAds.Initialize(AdConstants.instance.FlurryAdSpace);
			_initialized = true;
		}
	}

	public override void StartCachingVideo()
	{
		if (!_isCheckAvailabilityCoroutineRunning)
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
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			InvokeHandlerIfNotNull(_onVideoUnavailable, this, string.Empty);
		}
		if (!FlurryVideoAds.HasCachedAd())
		{
			FlurryVideoAds.CacheAd();
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
		else
		{
			OnVideoCachingSucceded();
		}
	}

	private void OnVideoCachingSucceded()
	{
		StopCheckVideoAvailabilityCouroutine();
		CalculateAndReportCachingTime("Success");
		InvokeHandlerIfNotNull(_onVideoAvailable, this);
	}

	private void OnVideoCachingFailed(string message = "")
	{
		StopCheckVideoAvailabilityCouroutine();
		CalculateAndReportCachingTime("Failed");
		InvokeHandlerIfNotNull(_onVideoUnavailable, this, message);
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

	public override bool PlayVideoIfAvailable()
	{
		if (_initialized)
		{
			if (FlurryVideoAds.HasCachedAd())
			{
				FlurryVideoAds.ShowAd();
				return true;
			}
			return false;
		}
		LogError("FlurryVideoAdsProvider PlayVideoIfAvailable() called before initialized");
		return false;
	}

	public override void StopProvider()
	{
	}

	public override void ResumeProvider()
	{
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

	private void OnTakeoverWillDisplay(string adSpace)
	{
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}

	private void OnTakeoverWillClose(string adSpace)
	{
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
	}

	private void OnVideoDidFinish(string adSpace)
	{
		InvokeHandlerIfNotNull(_onVideoWatched, this);
	}

	private void OnVideoDidNotFinish(string message)
	{
	}

	private void OnVideoAvailable(string adSpace)
	{
		OnVideoCachingSucceded();
		_isVideoAvailable = true;
	}

	private void OnVideoUnavailable(string message)
	{
		_isVideoAvailable = false;
		if (!message.Contains("100"))
		{
			message = string.Empty;
		}
		OnVideoCachingFailed(message);
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
		Debug.LogError("FlurryVideoAdsProvider: " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning("FlurryVideoAdsProvider: " + msg, context);
	}

	[Conditional("ENABLE_FLURRYADS_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log("FlurryVideoAdsProvider: " + msg, context);
	}
}
