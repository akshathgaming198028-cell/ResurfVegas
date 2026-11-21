using System;
using System.Collections;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Monetization.Inmobi;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class InmobiVideoAdsProvider : VideoAdProvider
{
	private const string KEY_REWARDED_VIDEO = "rewardedVideo";

	private const string PROVIDER_INMOBI_PRIORITY = "inmobi_video_priority";

	private const int INMOBI_DEFAULT_PRIORITY = 3;

	private const int INMOBI_DEFAULT_WEIGHT = 10;

	private const int MINIMUM_SUPPORTED_IOS_VERSION = 8;

	private bool _initialized;

	private bool _videoWatched;

	private int _priority;

	private int _weight;

	private IEnumerator _checkVideoCoroutine;

	private bool _isCheckAvailabilityCoroutineRunning;

	private static InmobiVideoAdsProvider _instance;

	private float _cachingStartTime = float.MinValue;

	private float _appMinimizedTime;

	private float _appResumedTime;

	private float _amountOfTimePaused;

	public static InmobiVideoAdsProvider instance
	{
		get
		{
			return _instance ?? (_instance = new InmobiVideoAdsProvider());
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			bool flag = Inmobi.IsSupportedOnPlatform();
			bool flag2 = !string.IsNullOrEmpty(AdConstants.instance.InmobiAppIdRewardedVideo);
			bool flag3 = true;
			bool flag4 = true;
			return flag && flag2 && flag3 && flag4;
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

	private string appId
	{
		get
		{
			return AdConstants.instance.InmobiAppIdRewardedVideo;
		}
	}

	private InmobiVideoAdsProvider()
	{
		VideoAdProvider.SetVideoProviderData("inmobi_video_priority", 3, 10, ref _priority, ref _weight);
	}

	public override void Init()
	{
		if (!_initialized)
		{
			Inmobi.onCacheAd += OnVideoCachingSuccess;
			Inmobi.onFailToLoadAd += OnVideoCachingFailed;
			Inmobi.willShowAd += OnVideoStart;
			Inmobi.onDismissAd += OnVideoFinished;
			Inmobi.onCompleteIncentive += OnCompleteIncentive;
			Inmobi.Initialize("rewardedVideo");
			_videoWatched = false;
			_initialized = true;
		}
	}

	public override bool PlayVideoIfAvailable()
	{
		_videoWatched = false;
		if (_initialized && ProviderHasAdAvailable())
		{
			Inmobi.ShowAd("rewardedVideo", appId);
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
			OnVideoCachingFailed(appId);
		}
		else if (!_isCheckAvailabilityCoroutineRunning)
		{
			Inmobi.CacheAd("rewardedVideo", appId);
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
			OnVideoCachingSuccess("rewardedVideo");
			yield break;
		}
		for (int i = 0; i < 10; i++)
		{
			yield return CoroutineUtil.StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));
			if (ProviderHasAdAvailable())
			{
				OnVideoCachingSuccess("rewardedVideo");
				yield break;
			}
		}
		OnVideoCachingTimedOut("rewardedVideo");
	}

	private bool ProviderHasAdAvailable()
	{
		return Inmobi.HasCachedAd("rewardedVideo", appId);
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

	private void OnVideoCachingSuccess(string key)
	{
		if (key.Equals("rewardedVideo"))
		{
			StopCheckVideoAvailabilityCouroutine();
			CalculateAndReportCachingTime("Success");
			InvokeHandlerIfNotNull(_onVideoAvailable, this);
		}
	}

	private void OnVideoCachingFailed(string key)
	{
		if (key.Equals("rewardedVideo"))
		{
			StopCheckVideoAvailabilityCouroutine();
			CalculateAndReportCachingTime("Failed");
			InvokeHandlerIfNotNull(_onVideoUnavailable, this, string.Empty);
		}
	}

	private void OnVideoCachingTimedOut(string key)
	{
		if (key.Equals("rewardedVideo"))
		{
			StopCheckVideoAvailabilityCouroutine();
			ResetCachingTimeLogging();
			InvokeHandlerIfNotNull(_onCachingTimedOut, this);
		}
	}

	private void StopCheckVideoAvailabilityCouroutine()
	{
		if (_isCheckAvailabilityCoroutineRunning)
		{
			CoroutineUtil.StopCoroutine(_checkVideoCoroutine);
			_isCheckAvailabilityCoroutineRunning = false;
		}
	}

	private void OnVideoStart(string key)
	{
		if (key.Equals("rewardedVideo"))
		{
			InvokeHandlerIfNotNull(_onTakeoverBegan, this);
		}
	}

	private void OnVideoFinished(string key)
	{
		if (key.Equals("rewardedVideo"))
		{
			if (_videoWatched)
			{
				InvokeHandlerIfNotNull(_onVideoWatched, this);
				_videoWatched = false;
			}
			InvokeHandlerIfNotNull(_onTakeoverEnded, this);
		}
	}

	private void OnCompleteIncentive(string key)
	{
		if (key.Equals("rewardedVideo"))
		{
			_videoWatched = true;
		}
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

	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError("[InmobiVideo] " + msg, context);
	}

	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning("[InmobiVideo] " + msg, context);
	}

	[Conditional("INMOBI_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log("[InmobiVideo] " + msg, context);
	}
}
