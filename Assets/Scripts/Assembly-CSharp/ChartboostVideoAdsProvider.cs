using System;
using System.Collections;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Plugins.Chartboost;
using UnityEngine;

public class ChartboostVideoAdsProvider : VideoAdProvider
{
	private const string REWARDEDVIDEO_LOCATION = "Main Menu";

	private const string PROVIDER_CHARTBOOST_PRIORITY = "chartboost_video_priority";

	private const int CHARTBOOST_DEFAULT_PRIORITY = 3;

	private const int CHARTBOOST_DEFAULT_WEIGHT = 10;

	private bool _initialized;

	private bool _videoWatched;

	private int _priority;

	private int _weight;

	private IEnumerator _checkVideoCoroutine;

	private bool _isCheckAvailabilityCoroutineRunning;

	private static ChartboostVideoAdsProvider _instance;

	private float _cachingStartTime = float.MinValue;

	private float _appMinimizedTime;

	private float _appResumedTime;

	private float _amountOfTimePaused;

	public static ChartboostVideoAdsProvider instance
	{
		get
		{
			return _instance ?? (_instance = new ChartboostVideoAdsProvider());
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			return false;
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
			return false;
		}
	}

	public override string id { get; set; }

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

	private ChartboostVideoAdsProvider()
	{
		VideoAdProvider.SetVideoProviderData("chartboost_video_priority", 3, 10, ref _priority, ref _weight);
	}

	public override void Init()
	{
		if (!_initialized)
		{
			Chartboost.StartSession(AdConstants.instance.ChartboostAppId, AdConstants.instance.ChartboostAppSignature);
			Chartboost.onCacheRewardedVideo += OnVideoCachingSuccess;
			Chartboost.onFailToLoadRewardedVideo += OnVideoCachingFailed;
			Chartboost.onDismissRewardedVideo += OnDismissVideo;
			Chartboost.onCloseRewardedVideo += OnVideoFinished;
			Chartboost.onClickRewardedVideo += OnClickRewardedVideo;
			Chartboost.onCompleteRewardedVideo += OnCompleteIncentive;
			Chartboost.onDisplayRewardedVideo += OnDisplayRewardedVideo;
			Chartboost.onWillDisplayVideo += OnVideoStart;
			_videoWatched = false;
			_initialized = true;
		}
	}

	public override bool PlayVideoIfAvailable()
	{
		_videoWatched = false;
		if (_initialized && ProviderHasAdAvailable())
		{
			Chartboost.ShowRewardedVideo("Main Menu");
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
		if (!_initialized)
		{
			OnVideoCachingFailed("Main Menu");
		}
		else if (!_isCheckAvailabilityCoroutineRunning)
		{
			Chartboost.CacheRewardedVideo("Main Menu");
			_checkVideoCoroutine = CheckVideoAvailability();
			CoroutineUtil.StartCoroutine(_checkVideoCoroutine);
		}
	}

	private IEnumerator CheckVideoAvailability()
	{
		InitiateCachingTimeLogging();
		_isCheckAvailabilityCoroutineRunning = true;
		if (ProviderHasAdAvailable())
		{
			OnVideoCachingSuccess("Main Menu");
			yield break;
		}
		for (int i = 0; i < 10; i++)
		{
			yield return CoroutineUtil.StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));
			if (ProviderHasAdAvailable())
			{
				OnVideoCachingSuccess("Main Menu");
				yield break;
			}
		}
		OnVideoCachingTimedOut();
	}

	private bool ProviderHasAdAvailable()
	{
		return Chartboost.HasCachedRewardedVideo("Main Menu");
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

	private void OnVideoCachingSuccess(string location)
	{
		StopCheckVideoAvailabilityCouroutine();
		CalculateAndReportCachingTime("Success");
		InvokeHandlerIfNotNull(_onVideoAvailable, this);
	}

	private void OnVideoCachingFailed(string location)
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

	private void OnVideoStart(string location)
	{
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}

	private void OnVideoFinished(string location)
	{
		if (_videoWatched)
		{
			InvokeHandlerIfNotNull(_onVideoWatched, this);
			_videoWatched = false;
		}
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
	}

	private void OnCompleteIncentive(string location)
	{
		_videoWatched = true;
	}

	private void OnDismissVideo(string location)
	{
	}

	private void OnClickRewardedVideo(string location)
	{
	}

	private void OnDisplayRewardedVideo(string location)
	{
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

	[Conditional("ENABLE_CHARTBOOST_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
